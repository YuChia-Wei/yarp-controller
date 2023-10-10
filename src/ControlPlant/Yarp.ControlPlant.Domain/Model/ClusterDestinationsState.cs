// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Model;

public sealed class ClusterDestinationsState
{
    public ClusterDestinationsState(
        IReadOnlyList<DestinationState> allDestinations,
        IReadOnlyList<DestinationState> availableDestinations)
    {
        this.AllDestinations = allDestinations ?? throw new ArgumentNullException(nameof(allDestinations));
        this.AvailableDestinations = availableDestinations ?? throw new ArgumentNullException(nameof(availableDestinations));
    }

    public IReadOnlyList<DestinationState> AllDestinations { get; }

    public IReadOnlyList<DestinationState> AvailableDestinations { get; }
}