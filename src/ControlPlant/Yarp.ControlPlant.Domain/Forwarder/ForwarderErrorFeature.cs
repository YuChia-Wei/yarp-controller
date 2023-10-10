// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Forwarder;

internal sealed class ForwarderErrorFeature : IForwarderErrorFeature
{
    internal ForwarderErrorFeature(ForwarderError error, Exception? ex)
    {
        this.Error = error;
        this.Exception = ex;
    }

    /// <summary>
    /// The specified ForwarderError.
    /// </summary>
    public ForwarderError Error { get; }

    /// <summary>
    /// The error, if any.
    /// </summary>
    public Exception? Exception { get; }
}