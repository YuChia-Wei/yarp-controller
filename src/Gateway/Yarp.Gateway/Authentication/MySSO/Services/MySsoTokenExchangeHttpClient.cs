using System.Text.Json;
using Yarp.Gateway.Authentication.MySSO.Contracts;
using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;

namespace Yarp.Gateway.Authentication.MySSO.Services;

/// <summary>
/// 使用 MySSO remote handler 的 backchannel 呼叫平台 Auth Server。
/// </summary>
/// <param name="logger">記錄平台 Auth Server 回應狀態的記錄器。</param>
internal sealed class MySsoTokenExchangeHttpClient(ILogger<MySsoTokenExchangeHttpClient> logger)
    : IMySsoTokenExchangeClient
{
    /// <summary>
    /// MySSO token exchange 使用的 JSON 設定。
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
    public async Task<MySsoTokenExchangeResult> ExchangeAsync(
        string token,
        MySsoAuthenticationOptions options,
        CancellationToken cancellationToken)
    {
        if (options.TokenExchangeEndpoint is null)
        {
            throw new InvalidOperationException("MySSO token exchange endpoint is not configured.");
        }

        using var response = await options.Backchannel
                                          .PostAsJsonAsync(
                                              options.TokenExchangeEndpoint,
                                              new MySsoTokenExchangeRequest(options.AppId, token),
                                              SerializerOptions,
                                              cancellationToken)
                                          .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Platform auth server returned HTTP {StatusCode} for MySSO token exchange.", (int)response.StatusCode);
            return MySsoTokenExchangeResult.Fail($"HTTP {(int)response.StatusCode}");
        }

        var result = await response.Content
                                   .ReadFromJsonAsync<MySsoTokenExchangeResult>(SerializerOptions, cancellationToken)
                                   .ConfigureAwait(false);
        return result ?? MySsoTokenExchangeResult.Fail("Empty MySSO token exchange response.");
    }
}