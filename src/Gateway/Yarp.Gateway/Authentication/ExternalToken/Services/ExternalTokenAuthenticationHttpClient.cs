using System.Text.Json;
using Yarp.Gateway.Authentication.ExternalToken.Configuration;
using Yarp.Gateway.Authentication.ExternalToken.Contracts;
using Yarp.Gateway.Authentication.ExternalToken.Models;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication.ExternalToken.Services;

/// <summary>
/// 使用 HTTP JSON 呼叫外部驗證服務的預設用戶端實作。
/// </summary>
internal sealed class ExternalTokenAuthenticationHttpClient(
    IHttpClientFactory httpClientFactory,
    ILogger<ExternalTokenAuthenticationHttpClient> logger)
    : IExternalTokenAuthenticationClient
{
    /// <summary>
    /// 外部驗證服務要求與回應使用的 JSON 設定。
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
    public Task<ExternalTokenAuthenticationResult> ValidateTokenAsync(
        string token,
        ExternalTokenAuthenticationConfiguration options,
        CancellationToken cancellationToken)
    {
        if (options.TokenValidationEndpoint is null)
        {
            throw new InvalidOperationException("External token validation endpoint is not configured.");
        }

        return this.PostAsync(
            options.TokenValidationEndpoint,
            new ExternalTokenValidationRequest(options.AppId, token),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<ExternalTokenAuthenticationResult> ExchangeKeyAsync(
        string key,
        ExternalTokenAuthenticationConfiguration options,
        CancellationToken cancellationToken)
    {
        if (options.KeyExchangeEndpoint is null)
        {
            throw new InvalidOperationException("External key exchange endpoint is not configured.");
        }

        return this.PostAsync(
            options.KeyExchangeEndpoint,
            new ExternalKeyExchangeRequest(options.AppId, key),
            cancellationToken);
    }

    /// <summary>
    /// 將要求內容以 JSON POST 到指定的外部驗證端點。
    /// </summary>
    private async Task<ExternalTokenAuthenticationResult> PostAsync<TRequest>(
        Uri endpoint,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(ExternalTokenAuthenticationDefaults.BackchannelHttpClientName);
        using var response = await httpClient.PostAsJsonAsync(endpoint, request, SerializerOptions, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("External authentication server returned HTTP {StatusCode}.", (int)response.StatusCode);
            return ExternalTokenAuthenticationResult.Fail($"HTTP {(int)response.StatusCode}");
        }

        var result = await response.Content
                                   .ReadFromJsonAsync<ExternalTokenAuthenticationResult>(SerializerOptions, cancellationToken)
                                   .ConfigureAwait(false);
        return result ?? ExternalTokenAuthenticationResult.Fail("Empty external authentication response.");
    }
}