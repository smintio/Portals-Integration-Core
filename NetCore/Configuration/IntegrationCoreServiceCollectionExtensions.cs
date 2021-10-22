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

using Client.Options;
using Microsoft.Extensions.Configuration;
using SmintIo.Portals.Integration.Core.Authenticator;
using SmintIo.Portals.Integration.Core.Authenticator.Impl;
using SmintIo.Portals.Integration.Core.Database;
using SmintIo.Portals.Integration.Core.Database.Impl;
using SmintIo.Portals.Integration.Core.Providers;
using SmintIo.Portals.Integration.Core.Providers.Impl;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IntegrationCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddSmintIoPortalsIntegrationCore(this IServiceCollection services, IConfiguration configuration)
        {            
            Console.WriteLine("Initializing Portals Integration Core...");

            var smintIoSection = configuration.GetSection("SmintIo");
            services.AddSingleton(smintIoSection.GetSection("App").Get<SmintIoAppOptions>());
            services.AddSingleton(smintIoSection.GetSection("Auth").Get<SmintIoAuthOptions>());

            services.AddSingleton<ISmintIoCLPortalAdminApiClientProvider, SmintIoCLPortalAdminApiClientProviderImpl>();
            services.AddSingleton<ISmintIoPortalsBackendApiClientProvider, SmintIoPortalsBackendApiClientProviderImpl>();
            services.AddSingleton<ISmintIoPortalsFrontendApiClientProvider, SmintIoPortalsFrontendApiClientProviderImpl>();

            services.AddSingleton<ISmintIoAuthenticationRefresher, SmintIoAuthenticationRefresherImpl>();
            services.AddSingleton<ISmintIoAuthenticator, SmintIoSystemBrowserAuthenticatorImpl>();

            services.AddSingleton<ISmintIoSettingsDatabaseProvider, SmintIoSettingsMemoryDatabase>();
            services.AddSingleton<ISmintIoTokenDatabaseProvider, SmintIoTokenMemoryDatabase>();
            
            Console.WriteLine("Portals Integration Core initialized successfully");

            return services;
        }
    }
}
