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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmintIo.Portals.Integration.Core.Service.Impl
{
    /// <summary>
    /// Implements common behaviour for all services
    /// </summary>
    ///
    /// <remarks>Services perform long running tasks. They may schedule execution on their own by
    /// registering itself with some messaging service or timer service. On disposing the service, all those
    /// registrations must be undone, though.
    /// </remarks>
    public abstract class BaseService : IService, IDisposable
    {
        public bool IsRunning { get; private set; }

        public bool IsErrorState { get; private set; }

        public List<string> ErrorMessages { get; private set; }

        private IHostApplicationLifetime _lifeTime;

        protected BaseService(IServiceProvider serviceProvider)
        {
            _lifeTime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
        }

        /// <summary>
        /// Base implementation clear error messages and flags and registers with Pusher service.
        /// </summary>
        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await this.StopAsync(cancellationToken);
                return;
            }

            if (!IsRunning)
            {
                ErrorMessages = new List<string>();
                IsErrorState = false;
                IsRunning = true;

                await TriggerAsync();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task StopAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (IsRunning)
            {
                IsRunning = false;
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task TriggerAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {

        }

        public void Dispose()
        {

        }

        protected void StopApplication()
        {
            _lifeTime.StopApplication();
        }
    }
}