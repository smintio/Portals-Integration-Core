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

using System;
using Autofac;
using Client.Options;
using Microsoft.Extensions.Configuration;

namespace SmintIo.Portals.Integration.Core.Service
{
    /// <summary>
    /// Implementing classes create new services of specific types.
    /// </summary>
    ///
    /// <remarks>Services need to be created with a bunch of configuration and may be their own
    /// dependency injection stack. Factory instances take all the necessary data, create service tailored for the specific
    /// client and will pass them to the caller to execute these in a scheduled way.
    /// </remarks>
    public interface IServiceFactory
    {
        /// <summary>
        /// Creates a new tenant based service instance.
        /// </summary>
        ///
        /// <remarks>The provided data is used to configure the service.
        /// </remarks>
        /// <param name="scope">A Scoped service provider to deliver configuration and other instances
        /// to the service. The scope has been configured previously with <see cref="ConfigureContainerBuilder"/>.
        /// Proper instances of <see cref="SmintIoAppOptions"/>, <see cref="SmintIoAuthOptions"/>,
        /// </param>
        /// <returns>A worker task to execute the service.</returns>
        IService CreateService(ILifetimeScope scope) => scope.Resolve<IService>();

        /// <summary>
        /// Configures the Autofac dependency injector to contain all necessary dependencies to create a service.
        /// </summary>
        ///
        /// <remarks>To separate services from each other, a lifetime scope of Autofac is used. This lifetime
        /// scope needs to be configured to contain all necessary dependency definitions, prior to creating the
        /// service. It will be pre-configured with necessary references to <see cref="SmintIoAppOptions"/>,
        /// <see cref="SmintIoAuthOptions"/>, and <see cref="IConfiguration"/>.
        /// See <see href="https://autofac.org/apidoc/html/7021277C.htm">Autofac API documentation</see>.
        /// </remarks>
        /// <param name="containerBuilder">The container builder that can be used to configure the DI container.</param>
        /// <returns>A worker task to execute the service.</returns>
        IServiceFactory ConfigureContainerBuilder(ContainerBuilder containerBuilder);

        /// <summary>
        /// The name of the service that is covered by the implementation of the interface.
        /// </summary>
        ///
        /// <remarks>The name must be unique as it will be used with some sort of mapping or service locator.</remarks>
        public string ServiceName { get; }
    }
}