#region copyright
// MIT License
//
// Copyright (c) 2021 Smint.io GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice (including the next paragraph) shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// SPDX-License-Identifier: MIT
#endregion

using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using SmintIo.Portals.Integration.Core.Authenticator;
using SmintIo.Portals.Integration.Core.Database;
using SmintIo.PortalsAPI.Frontend.Client.Generated;

namespace SmintIo.Portals.Integration.Core.Providers.Impl
{
    internal class SmintIoPortalsFrontendApiClientProviderImpl : IDisposable, ISmintIoPortalsFrontendApiClientProvider
    {
        private const int MaxRetryAttempts = 5;

        private readonly ISmintIoSettingsDatabaseProvider _smintIoSettingsDatabaseProvider;
        private readonly ISmintIoTokenDatabaseProvider _smintIoTokenDatabaseProvider;

        private readonly ISmintIoAuthenticationRefresher _smintIoAuthenticationRefresher;

        private readonly HttpClient _http;

        private readonly AsyncRetryPolicy _retryPolicy;

        private bool _disposed;

        private readonly ILogger _logger;

        private readonly PortalsAPIFEOpenApiClient _portalsApiFEOpenApiClient;

        public SmintIoPortalsFrontendApiClientProviderImpl(
            ISmintIoSettingsDatabaseProvider smintIoSettingsDatabaseProvider,
            ISmintIoTokenDatabaseProvider smintIoTokenDatabaseProvider,
            ILogger<SmintIoCLPortalAdminApiClientProviderImpl> logger,
            ISmintIoAuthenticationRefresher smintIoAuthenticationRefresher)
        {
            _smintIoSettingsDatabaseProvider = smintIoSettingsDatabaseProvider;
            _smintIoTokenDatabaseProvider = smintIoTokenDatabaseProvider;

            _smintIoAuthenticationRefresher = smintIoAuthenticationRefresher;
            
            _disposed = false;

            _http = new HttpClient();

            _logger = logger;

            _retryPolicy = GetRetryStrategy();

            _portalsApiFEOpenApiClient = new PortalsAPIFEOpenApiClient(_http);            
        }

        public async Task<TOut> ExecuteWithRetryPolicyAsync<TOut>(Func<PortalsAPIFEOpenApiClient, Task<TOut>> funcAsync)
        {
            await SetupPortalsAdminOpenApiClientAsync();

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                // get a new access token in case it was refreshed
                var tokenDatabaseModel = await _smintIoTokenDatabaseProvider.GetTokenDatabaseModelAsync();
                _portalsApiFEOpenApiClient.AccessToken = tokenDatabaseModel.AccessToken;

                return await funcAsync(_portalsApiFEOpenApiClient).ConfigureAwait(false);
            });

            return result;
        }

        private async Task SetupPortalsAdminOpenApiClientAsync()
        {
            var smintIoSettingsDatabaseModel = await _smintIoSettingsDatabaseProvider.GetSmintIoSettingsDatabaseModelAsync();
            var tokenDatabaseModel = await _smintIoTokenDatabaseProvider.GetTokenDatabaseModelAsync();

            _portalsApiFEOpenApiClient.BaseUrl = $"https://{smintIoSettingsDatabaseModel.TenantId}.portalsapife.smint.io/frontend/v1";
            _portalsApiFEOpenApiClient.AccessToken = tokenDatabaseModel.AccessToken;
        }

        private AsyncRetryPolicy GetRetryStrategy()
        {
            return Policy
                .Handle<ApiException>()
                .Or<Exception>()
                .WaitAndRetryAsync(
                    MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    async (ex, timespan, context) =>
                    {
                        _logger.LogError(ex, "Error communicating to Smint.io");

                        if (ex is ApiException apiEx)
                        {
                            if (apiEx.StatusCode == (int)HttpStatusCode.Forbidden || apiEx.StatusCode == (int)HttpStatusCode.Unauthorized)
                            {
                                await _smintIoAuthenticationRefresher.RefreshAuthenticationAsync();

                                // backoff and try again 

                                return;
                            }
                            else if (apiEx.StatusCode == (int)HttpStatusCode.TooManyRequests)
                            {
                                // backoff and try again

                                return;
                            }

                            // expected error happened server side, most likely our problem, cancel

                            throw ex;
                        }

                        // some server side or communication issue, backoff and try again
                    });
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _http?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
