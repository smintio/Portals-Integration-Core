#region copyright
// MIT License
//
// Copyright (c) 2019 Smint.io GmbH
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

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using SmintIo.Portals.Integration.Core.Authenticator;
using SmintIo.Portals.Integration.Core.Service;

namespace SmintIo.Portals.Integration.Core.Services
{
    public class SimpleService : IHostedService, IDisposable
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly ILifetimeScope _dependencyScope;
        private readonly ILogger<SimpleService> _logger;

        private IService _service;
        private ILifetimeScope _clientScope;

        public SimpleService(
            IServiceFactory serviceFactory,
            ILifetimeScope dependencyScope,
            ILogger<SimpleService> logger)
        {
            _serviceFactory = serviceFactory;
            _dependencyScope = dependencyScope;
            _logger = logger;
        }

        public void Dispose()
        {
            if (_serviceFactory is IDisposable disposableServiceFactory)
            {
                disposableServiceFactory.Dispose();
            }

            if (_service is IDisposable disposableService)
            {
                disposableService.Dispose();
            }
            _clientScope?.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting service...");

            if (_service == null && _clientScope == null)
            {
                _clientScope = _dependencyScope.BeginLifetimeScope(
                     containerConfig => _serviceFactory.ConfigureContainerBuilder(containerConfig));

                // The scope is disposed right after creation of the service. This should not be a problem as
                // the service holds references to all required instances after creation.
                _service = _serviceFactory.CreateService(_clientScope);

                var authenticator = _clientScope.Resolve<ISmintIoAuthenticator>();
                Debug.Assert(authenticator != null, nameof(authenticator) + " != null");

                // we have a system browser based authenticator here, which will work synchronously
                await authenticator.InitializeAuthenticationAsync();
            }

            await _service.StartAsync(cancellationToken);

            _logger.LogInformation("Started service");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_service != null)
            {
                _logger.LogInformation("Stopping service...");
                await _service.StopAsync(cancellationToken);
                _clientScope.Dispose();
                _clientScope = null;
                _logger.LogInformation("Stopped service");
            }
            else
            {
                _logger.LogInformation("Service has not been started yet");
            }
        }
    }
}
