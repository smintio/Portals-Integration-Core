﻿#region copyright
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

using System.Threading.Tasks;

namespace SmintIo.Portals.Integration.Core.Authenticator
{
    /// <summary>
    /// An authenticator authenticates the user with a remote system and stores all required data to access resources on
    /// the remote system subsequently.
    /// </summary>
    /// <typeparam name="T">Is passed to <see cref="IAuthenticationRefresher{T}"/> and defines the type of the
    /// authentication storage that is used with the authenticator.</typeparam>
    public interface IAuthenticator<T> : IAuthenticationRefresher<T>
    {
        /// <summary>
        /// Performs an authentication with the remote system.
        /// <remarks>All required data to perform the authentication must be provided elsewhere as it is implementation
        /// dependent, what is needed.</remarks>
        /// </summary>
        /// <returns>A task to wait for finishing the process or not.</returns>
        Task InitializeAuthenticationAsync();
    }
}
