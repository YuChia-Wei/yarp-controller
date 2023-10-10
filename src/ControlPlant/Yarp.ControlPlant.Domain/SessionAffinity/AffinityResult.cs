// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Model;

namespace Yarp.ReverseProxy.ControlPlant.Entity.SessionAffinity;

/// <summary>
/// Affinity resolution result.
/// </summary>
public readonly struct AffinityResult
{
    public IReadOnlyList<DestinationState>? Destinations { get; }

    public AffinityStatus Status { get; }

    public AffinityResult(IReadOnlyList<DestinationState>? destinations, AffinityStatus status)
    {
        this.Destinations = destinations;
        this.Status = status;
    }
}