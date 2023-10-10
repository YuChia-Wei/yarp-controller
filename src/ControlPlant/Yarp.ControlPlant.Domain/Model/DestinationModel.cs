// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Model;

/// <summary>
/// Immutable representation of the portions of a destination
/// that only change in reaction to configuration changes
/// (e.g. address).
/// </summary>
/// <remarks>
/// All members must remain immutable to avoid thread safety issues.
/// Instead, instances of <see cref="DestinationModel" /> are replaced
/// in their entirety when values need to change.
/// </remarks>
public sealed class DestinationModel
{
    /// <summary>
    /// Creates a new instance. This constructor is for tests and infrastructure, this type is normally constructed by
    /// the configuration loading infrastructure.
    /// </summary>
    public DestinationModel(DestinationConfig destination)
    {
        this.Config = destination ?? throw new ArgumentNullException(nameof(destination));
    }

    /// <summary>
    /// This destination's configuration.
    /// </summary>
    public DestinationConfig Config { get; }

    internal bool HasChanged(DestinationConfig destination)
    {
        return this.Config != destination;
    }
}