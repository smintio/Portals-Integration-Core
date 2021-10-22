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

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmintIo.Portals.Integration.Core.Authenticator;
using SmintIo.Portals.Integration.Core.Authenticator.Impl;

namespace SmintIo.Portals.Integration.Core.Service.Impl
{
    /// <summary>
    /// Implementing classes create new services of specific types.
    /// </summary>
    ///
    /// <remarks>Services need to be created with a bunch of configuration and may be their own
    /// dependency injection stack. Factory instances take all the necessary data, create services tailored for the specific
    /// client and will pass them to the caller to execute these in a scheduled way.
    /// </remarks>
    /// <typeparam name="T">The type of the <see cref="IService"/>, which is used to implement
    /// <see cref="ServiceTargetName"/>.</typeparam>
    public class BaseServiceFactoryImpl<T> : IServiceFactory where T : IService
    {
        public IConfiguration Configuration { get; set; }
        
        public BaseServiceFactoryImpl(IConfiguration appConfig)
        {
            Configuration = appConfig;
        }

        public virtual IServiceFactory ConfigureContainerBuilder(ContainerBuilder containerBuilder)
        {
            var services = new ServiceCollection();

            services.AddTransient<IOAuthAuthenticationRefresher, OAuthAuthenticationRefresherImpl>();
            services.AddTransient<IOAuthAuthenticator, OAuthAuthenticatorImpl>();

            containerBuilder.Populate(services);

            return this;
        }

        /// <summary>
        /// The name of the service that is covered by the implementation of the interface.
        /// </summary>
        ///
        /// <remarks>The name is derived from the class name by removing "service", "client" and "impl"
        /// from the lower case name of <c>T</c>.</remarks>
        public string ServiceName { get; } = nameof(T)
            .ToLower()
            .Replace("service", "")
            .Replace("client", "")
            .Replace("impl", "")
        ;
    }
}