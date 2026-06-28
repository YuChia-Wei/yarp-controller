namespace Yarp.Gateway.Authentication.ExternalToken.Configuration;

/// <summary>
/// 外部 Token 驗證使用的預設 scheme 與 HttpClient 名稱。
/// </summary>
public static class ExternalTokenAuthenticationDefaults
{
    /// <summary>
    /// 外部 Token 驗證使用的 ASP.NET Core authentication scheme 名稱。
    /// </summary>
    public const string AuthenticationScheme = "ExternalToken";

    /// <summary>
    /// 以 Authorization header 帶入外部 key 時使用的 scheme 名稱。
    /// </summary>
    public const string KeyScheme = "ExternalKey";

    /// <summary>
    /// 呼叫外部驗證服務時使用的具名 HttpClient 名稱。
    /// </summary>
    public const string BackchannelHttpClientName = "ExternalTokenAuthentication";
}