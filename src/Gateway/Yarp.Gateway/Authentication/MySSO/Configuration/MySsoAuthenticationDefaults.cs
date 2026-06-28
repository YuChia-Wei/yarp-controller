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
    /// 僅供指定互動式路由使用，驗證時轉派至 session，challenge 時轉派至 MySSO 的 policy scheme 名稱。
    /// </summary>
    public const string InteractiveScheme = "MySSOInteractive";

    /// <summary>
    /// 驗證失敗時允許轉導至 MySSO 的指定路由 authorization policy 名稱。
    /// </summary>
    public const string InteractivePolicy = "MySSOInteractive";

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
    /// 前端查詢目前 MySSO session 狀態與到期時間的預設路徑。
    /// </summary>
    public const string SessionPath = "/auth/session";

    /// <summary>
    /// 前端主動續期 access／refresh token 與 session 到期時間的預設路徑。
    /// </summary>
    public const string RefreshPath = "/auth/refresh";

    /// <summary>
    /// 回應中承載 MySSO session 到期時間（UTC, ISO-8601）的 header 名稱。
    /// </summary>
    public const string SessionExpiresHeaderName = "X-Session-Expires-At";

    /// <summary>
    /// 在 HttpContext.Items 暫存目前 session 有效到期時間（供回應 header 中介軟體讀取）的鍵值。
    /// </summary>
    public const string SessionExpiresItemKey = "MySSO:Session:ExpiresAt";

    /// <summary>
    /// Gateway 依要求內容選擇驗證機制時使用的 policy scheme 名稱。
    /// </summary>
    public const string PolicyScheme = "GatewayAuthentication";
}