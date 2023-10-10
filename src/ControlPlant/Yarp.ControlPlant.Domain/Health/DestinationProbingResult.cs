// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Model;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Health;

/// <summary>
/// Result of a destination's active health probing.
/// </summary>
public readonly struct DestinationProbingResult
{
    public DestinationProbingResult(DestinationState destination, HttpResponseMessage? response, Exception? exception)
    {
        this.Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        this.Response = response;
        this.Exception = exception;
    }

    /// <summary>
    /// Probed destination.
    /// </summary>
    public DestinationState Destination { get; }

    /// <summary>
    /// Response recieved.
    /// It can be null in case of a failure.
    /// </summary>
    public HttpResponseMessage? Response { get; }

    /// <summary>
    /// Exception thrown during probing.
    /// It is null in case of a success.
    /// </summary>
    public Exception? Exception { get; }
}