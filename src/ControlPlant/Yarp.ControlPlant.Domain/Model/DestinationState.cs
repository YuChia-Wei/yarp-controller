// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Model;

/// <summary>
/// Representation of a cluster's destination for use at runtime.
/// </summary>
public sealed class DestinationState : IReadOnlyList<DestinationState>
{
    private volatile DestinationModel _model = default!;

    /// <summary>
    /// Creates a new instance. This constructor is for tests and infrastructure, this type is normally constructed by
    /// the configuration loading infrastructure.
    /// </summary>
    public DestinationState(string destinationId)
    {
        if (string.IsNullOrEmpty(destinationId))
        {
            throw new ArgumentNullException(nameof(destinationId));
        }

        this.DestinationId = destinationId;
    }

    /// <summary>
    /// The destination's unique id.
    /// </summary>
    public string DestinationId { get; }

    /// <summary>
    /// A snapshot of the current configuration
    /// </summary>
    public DestinationModel Model
    {
        get => this._model;
        internal set => this._model = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Mutable health state for this destination.
    /// </summary>
    public DestinationHealthState Health { get; } = new DestinationHealthState();

    /// <summary>
    /// Keeps track of the total number of concurrent requests on this endpoint.
    /// The setter should only be used for testing purposes.
    /// </summary>
    public int ConcurrentRequestCount
    {
        get => this.ConcurrencyCounter.Value;
        set => this.ConcurrencyCounter.Value = value;
    }

    internal AtomicCounter ConcurrencyCounter { get; } = new AtomicCounter();

    DestinationState IReadOnlyList<DestinationState>.this[int index] => index == 0 ? this : throw new IndexOutOfRangeException();

    int IReadOnlyCollection<DestinationState>.Count => 1;

    IEnumerator<DestinationState> IEnumerable<DestinationState>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    private struct Enumerator : IEnumerator<DestinationState>
    {
        private bool _read;

        internal Enumerator(DestinationState instance)
        {
            this.Current = instance;
            this._read = false;
        }

        public DestinationState Current { get; }

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            if (!this._read)
            {
                this._read = true;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }
    }
}