// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

// TODO: update or remove this once AspNetCore provides a mechanism to validate the RateLimiter policies https://github.com/dotnet/aspnetcore/issues/45684

internal interface IYarpRateLimiterPolicyProvider
{
    ValueTask<object?> GetPolicyAsync(string policyName);
}

internal class YarpRateLimiterPolicyProvider : IYarpRateLimiterPolicyProvider
{
#if NET7_0_OR_GREATER
    private readonly RateLimiterOptions _rateLimiterOptions;

    private readonly IDictionary _policyMap, _unactivatedPolicyMap;

    public YarpRateLimiterPolicyProvider(IOptions<RateLimiterOptions> rateLimiterOptions)
    {
        this._rateLimiterOptions = rateLimiterOptions?.Value ?? throw new ArgumentNullException(nameof(rateLimiterOptions));

        var type = typeof(RateLimiterOptions);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        this._policyMap = type.GetProperty("PolicyMap", flags)?.GetValue(this._rateLimiterOptions, null) as IDictionary
                          ?? throw new NotSupportedException("This version of YARP is incompatible with the current version of ASP.NET Core.");
        this._unactivatedPolicyMap = type.GetProperty("UnactivatedPolicyMap", flags)?.GetValue(this._rateLimiterOptions, null) as IDictionary
                                     ?? throw new NotSupportedException("This version of YARP is incompatible with the current version of ASP.NET Core.");
    }

    public ValueTask<object?> GetPolicyAsync(string policyName)
    {
        return ValueTask.FromResult(this._policyMap[policyName] ?? this._unactivatedPolicyMap[policyName]);
    }
#else
    public ValueTask<object?> GetPolicyAsync(string policyName)
    {
        return default;
    }
#endif
}