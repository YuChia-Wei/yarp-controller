using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using Yarp.Gateway.Authentication.ExternalToken.Configuration;
using Yarp.Gateway.Authentication.MySSO.Configuration;

namespace Yarp.Gateway.YarpComponents.TransformProviders;

/// <summary>
/// 解析目前驗證身分可轉送至下游服務的 access token。
/// </summary>
internal static class DownstreamAccessTokenResolver
{
    /// <summary>
    /// 依目前使用的 authentication scheme 解析下游 access token 與 Authorization header 處理方式。
    /// </summary>
    /// <param name="httpContext">目前 Gateway HTTP context。</param>
    /// <returns>下游 Authorization header 的解析結果。</returns>
    internal static async Task<DownstreamAccessTokenResolution> ResolveAsync(HttpContext httpContext)
    {
        if (IsAuthenticatedWith(httpContext, MySsoAuthenticationDefaults.RemoteScheme))
        {
            var accessToken = await httpContext.GetTokenAsync(
                                  MySsoAuthenticationDefaults.SessionScheme,
                                  OpenIdConnectParameterNames.AccessToken)
                              .ConfigureAwait(false);
            return DownstreamAccessTokenResolution.Handled(accessToken);
        }

        if (IsAuthenticatedWith(httpContext, ExternalTokenAuthenticationDefaults.AuthenticationScheme))
        {
            var authenticateResult = await httpContext
                                           .AuthenticateAsync(ExternalTokenAuthenticationDefaults.AuthenticationScheme)
                                           .ConfigureAwait(false);
            var credentialKind =
                authenticateResult.Properties is { } properties &&
                properties.Items.TryGetValue(
                    ExternalTokenAuthenticationDefaults.CredentialKindProperty,
                    out var storedCredentialKind)
                    ? storedCredentialKind
                    : null;

            if (string.Equals(
                    credentialKind,
                    ExternalTokenAuthenticationDefaults.KeyCredentialKind,
                    StringComparison.Ordinal))
            {
                var accessToken = authenticateResult.Properties?
                                                    .GetTokenValue(OpenIdConnectParameterNames.AccessToken);
                return DownstreamAccessTokenResolution.Handled(accessToken);
            }

            return DownstreamAccessTokenResolution.Handled(null);
        }

        var savedAccessToken = await httpContext
                                     .GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
                                     .ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(savedAccessToken))
        {
            return DownstreamAccessTokenResolution.Handled(savedAccessToken);
        }

        var authorization = httpContext.Request.Headers[HeaderNames.Authorization].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authorization) &&
            AuthenticationHeaderValue.TryParse(authorization, out var authorizationHeader) &&
            string.Equals(authorizationHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(authorizationHeader.Parameter))
        {
            return DownstreamAccessTokenResolution.Handled(authorizationHeader.Parameter);
        }

        return DownstreamAccessTokenResolution.NotHandled;
    }

    /// <summary>
    /// 判斷目前 principal 是否包含指定 authentication type 的已驗證身分。
    /// </summary>
    private static bool IsAuthenticatedWith(HttpContext httpContext, string authenticationType)
    {
        return httpContext.User.Identities.Any(
            identity =>
                identity.IsAuthenticated &&
                string.Equals(identity.AuthenticationType, authenticationType, StringComparison.Ordinal));
    }
}

/// <summary>
/// 表示下游 Authorization header 是否由 Gateway 接管，以及要轉送的 access token。
/// </summary>
/// <param name="IsHandled">Gateway 是否應覆寫或移除原始 Authorization header。</param>
/// <param name="AccessToken">要以 Bearer scheme 轉送的 access token；無權杖時為 <see langword="null"/>。</param>
internal readonly record struct DownstreamAccessTokenResolution(bool IsHandled, string? AccessToken)
{
    /// <summary>
    /// 表示 Gateway 不處理目前 Authorization header。
    /// </summary>
    internal static DownstreamAccessTokenResolution NotHandled { get; } = new(false, null);

    /// <summary>
    /// 建立由 Gateway 接管 Authorization header 的解析結果。
    /// </summary>
    /// <param name="accessToken">要轉送的 access token；無權杖時為 <see langword="null"/>。</param>
    /// <returns>由 Gateway 接管的解析結果。</returns>
    internal static DownstreamAccessTokenResolution Handled(string? accessToken)
    {
        return new DownstreamAccessTokenResolution(
            true,
            string.IsNullOrWhiteSpace(accessToken) ? null : accessToken);
    }
}