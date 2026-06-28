using Microsoft.AspNetCore.Authentication;
using Yarp.Gateway.Authentication.ExternalToken.Configuration;

namespace Yarp.Gateway.Authentication.Options;

/// <summary>
/// 外部 Token 驗證設定，適用於每次 request 都需要呼叫外部服務驗證憑證的 authentication scheme。
/// </summary>
public sealed class ExternalTokenAuthenticationConfiguration : AuthenticationSchemeOptions
{
    /// <summary>
    /// 呼叫外部驗證服務時帶入的應用程式識別碼。
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// 讀取外部憑證時使用的 HTTP header 名稱。
    /// </summary>
    public string AuthorizationHeaderName { get; set; } = "Authorization";

    /// <summary>
    /// 外部 token 使用的 Authorization scheme。
    /// </summary>
    public string TokenScheme { get; set; } = ExternalTokenAuthenticationDefaults.AuthenticationScheme;

    /// <summary>
    /// 外部 key 使用的 Authorization scheme。
    /// </summary>
    public string KeyScheme { get; set; } = ExternalTokenAuthenticationDefaults.KeyScheme;

    /// <summary>
    /// 是否允許以 Bearer scheme 帶入外部 token。
    /// </summary>
    public bool AcceptBearerToken { get; set; }

    /// <summary>
    /// 當允許 Bearer scheme 時，是否略過看起來像 JWT 的 token。
    /// </summary>
    public bool IgnoreJwtBearerToken { get; set; } = true;

    /// <summary>
    /// 外部 token 驗證端點。
    /// </summary>
    public Uri? TokenValidationEndpoint { get; set; }

    /// <summary>
    /// 外部 key 交換驗證資料的端點。
    /// </summary>
    public Uri? KeyExchangeEndpoint { get; set; }

    /// <summary>
    /// 呼叫外部驗證服務時 backchannel HTTP 要求的逾時秒數。
    /// </summary>
    public int BackchannelTimeoutSeconds { get; set; } = 30;
}