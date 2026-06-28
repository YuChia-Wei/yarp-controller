using System.Net.Http.Headers;
using Xunit;
using Yarp.Gateway.YarpComponents.TransformProviders;

namespace Yarp.Gateway.Tests.YarpComponents;

public sealed class AuthenticationTokenTransformProviderTest
{
    [Fact]
    public void ApplyAuthorization_GivenHandledWithoutAccessToken_RemovesOriginalAuthorization()
    {
        using var proxyRequest = CreateRequest("ExternalToken", "external-token");

        AuthenticationTokenTransformProvider.ApplyAuthorization(
            proxyRequest,
            DownstreamAccessTokenResolution.Handled(null));

        Assert.Null(proxyRequest.Headers.Authorization);
    }

    [Fact]
    public void ApplyAuthorization_GivenHandledAccessToken_UsesBearerAccessToken()
    {
        using var proxyRequest = CreateRequest("ExternalKey", "external-key");

        AuthenticationTokenTransformProvider.ApplyAuthorization(
            proxyRequest,
            DownstreamAccessTokenResolution.Handled("downstream-access-token"));

        Assert.Equal("Bearer", proxyRequest.Headers.Authorization?.Scheme);
        Assert.Equal("downstream-access-token", proxyRequest.Headers.Authorization?.Parameter);
    }

    [Fact]
    public void ApplyAuthorization_GivenNotHandled_PreservesOriginalAuthorization()
    {
        using var proxyRequest = CreateRequest("Custom", "credential");

        AuthenticationTokenTransformProvider.ApplyAuthorization(
            proxyRequest,
            DownstreamAccessTokenResolution.NotHandled);

        Assert.Equal("Custom", proxyRequest.Headers.Authorization?.Scheme);
        Assert.Equal("credential", proxyRequest.Headers.Authorization?.Parameter);
    }

    private static HttpRequestMessage CreateRequest(string scheme, string parameter)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://downstream.test/resource");
        request.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        return request;
    }
}