// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

internal class AtomicCounter
{
    private int _value;

    /// <summary>
    /// Gets the current value of the counter.
    /// </summary>
    public int Value
    {
        get => Volatile.Read(ref this._value);
        set => Volatile.Write(ref this._value, value);
    }

    /// <summary>
    /// Atomically decrements the counter value by 1.
    /// </summary>
    public int Decrement()
    {
        return Interlocked.Decrement(ref this._value);
    }

    /// <summary>
    /// Atomically increments the counter value by 1.
    /// </summary>
    public int Increment()
    {
        return Interlocked.Increment(ref this._value);
    }

    /// <summary>
    /// Atomically resets the counter value to 0.
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref this._value, 0);
    }
}