// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Model;

/// <summary>
/// Tracks destination passive and active health states.
/// </summary>
public class DestinationHealthState
{
    private volatile DestinationHealth _active;
    private volatile DestinationHealth _passive;

    /// <summary>
    /// Passive health state.
    /// </summary>
    public DestinationHealth Passive
    {
        get => this._passive;
        set => this._passive = value;
    }

    /// <summary>
    /// Active health state.
    /// </summary>
    public DestinationHealth Active
    {
        get => this._active;
        set => this._active = value;
    }
}