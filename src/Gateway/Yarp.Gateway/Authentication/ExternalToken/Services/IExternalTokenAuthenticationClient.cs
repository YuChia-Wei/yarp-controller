using Yarp.Gateway.Authentication.ExternalToken.Models;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication.ExternalToken.Services;

/// <summary>
/// 外部驗證服務用戶端，負責驗證 token 或以 key 交換驗證資料。
/// </summary>
public interface IExternalTokenAuthenticationClient
{
    /// <summary>
    /// 使用外部 key 向第三方服務交換驗證資料。
    /// </summary>
    Task<ExternalTokenAuthenticationResult> ExchangeKeyAsync(
        string key,
        ExternalTokenAuthenticationConfiguration options,
        CancellationToken cancellationToken);

    /// <summary>
    /// 使用 token 向第三方服務驗證身分。
    /// </summary>
    Task<ExternalTokenAuthenticationResult> ValidateTokenAsync(
        string token,
        ExternalTokenAuthenticationConfiguration options,
        CancellationToken cancellationToken);
}