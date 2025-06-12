// <copyright file="UnauthenticatedMarainOperationsControl.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Client.OperationsControl;

using System;
using System.Net.Http;

/// <summary>
/// Operations Control API client for use in scenarios where authentication is not required.
/// </summary>
/// <remarks>
/// <para>
/// In scenarios in which inter-service communication is secured at a networking level, it
/// might be unnecessary to authenticate requests. The base proxy type supports this but only
/// through protected constructors. This type makes a suitable constructor available publicly.
/// </para>
/// </remarks>
public class UnauthenticatedMarainOperationsControl : MarainOperationsControl
{
    /// <summary>
    /// Create an <see cref="UnauthenticatedMarainOperationsControl"/>.
    /// </summary>
    /// <param name="baseUri">The base URI of the Operations control service.</param>
    /// <param name="handlers">Optional request processing handlers.</param>
    public UnauthenticatedMarainOperationsControl(Uri baseUri, params DelegatingHandler[] handlers)
        : base(baseUri, handlers)
    {
    }
}