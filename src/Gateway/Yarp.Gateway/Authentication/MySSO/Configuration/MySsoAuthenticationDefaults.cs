namespace Yarp.Gateway.Authentication.MySSO.Configuration;

/// <summary>
/// MySSO remote authentication flow 使用的預設 scheme、路徑與 Cookie 名稱。
/// </summary>
public static class MySsoAuthenticationDefaults
{
    /// <summary>
    /// MySSO remote authentication scheme 名稱。
    /// </summary>
    public const string RemoteScheme = "MySSO";

    /// <summary>
    /// MySSO 登入成功後保存工作階段的 Cookie authentication scheme 名稱。
    /// </summary>
    public const string SessionScheme = "MySSOSession";

    /// <summary>
    /// MySSO 工作階段 Cookie 名稱。
    /// </summary>
    public const string SessionCookieName = "MySSO.Session";

    /// <summary>
    /// MySSO 以 form POST 回傳登入結果的預設 callback 路徑。
    /// </summary>
    public const string CallbackPath = "/signin-mysso";

    /// <summary>
    /// Gateway 提供給前端啟動 MySSO 登入流程的預設路徑。
    /// </summary>
    public const string LoginPath = "/auth/mysso/login";

    /// <summary>
    /// Gateway 依要求內容選擇驗證機制時使用的 policy scheme 名稱。
    /// </summary>
    public const string PolicyScheme = "GatewayAuthentication";
}