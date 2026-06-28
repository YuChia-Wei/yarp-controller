using Microsoft.AspNetCore.Http;
using Yarp.Gateway.Authentication.MySSO.Configuration;

namespace Yarp.Gateway.Authentication.Options;

/// <summary>
/// MySSO form-post remote authentication flow 與獨立 session Cookie 的組態設定。
/// </summary>
public sealed class MySsoAuthenticationConfiguration
{
    /// <summary>
    /// MySSO 登入頁端點。
    /// </summary>
    public Uri? AuthorizationEndpoint { get; set; }

    /// <summary>
    /// 平台 Auth Server 接收 MySSO 單次 token 並簽發 access／refresh token 的端點。
    /// </summary>
    public Uri? TokenExchangeEndpoint { get; set; }

    /// <summary>
    /// Gateway 在 MySSO 平台使用的應用程式識別碼。
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// MySSO form POST callback 路徑。
    /// </summary>
    public PathString CallbackPath { get; set; } = MySsoAuthenticationDefaults.CallbackPath;

    /// <summary>
    /// 前端啟動 MySSO 登入流程的 Gateway 路徑。
    /// </summary>
    public PathString LoginPath { get; set; } = MySsoAuthenticationDefaults.LoginPath;

    /// <summary>
    /// MySSO form POST 中承載單次 token 的欄位名稱。
    /// </summary>
    public string TokenParameterName { get; set; } = "token";

    /// <summary>
    /// MySSO form POST 與登入要求中承載 correlation state 的欄位名稱。
    /// </summary>
    public string StateParameterName { get; set; } = "state";

    /// <summary>
    /// MySSO 登入要求中承載應用程式識別碼的 query string 參數名稱。
    /// </summary>
    public string AppIdParameterName { get; set; } = "app_id";

    /// <summary>
    /// MySSO 登入要求中承載 callback URL 的 query string 參數名稱。
    /// </summary>
    public string RedirectUriParameterName { get; set; } = "redirect_uri";

    /// <summary>
    /// 是否允許 MySSO 以 query string 回傳單次 token；預設關閉以避免 token 出現在 URL 與存取記錄。
    /// </summary>
    public bool AllowQueryStringCallback { get; set; }

    /// <summary>
    /// MySSO 登入要求中額外附加的固定 query string 參數。
    /// </summary>
    public Dictionary<string, string> AdditionalAuthorizationParameters { get; set; } = [];

    /// <summary>
    /// MySSO session Cookie 名稱。
    /// </summary>
    public string SessionCookieName { get; set; } = MySsoAuthenticationDefaults.SessionCookieName;

    /// <summary>
    /// MySSO session Cookie 所屬網域；未設定時使用目前 Gateway host。
    /// </summary>
    public string? SessionCookieDomain { get; set; }

    /// <summary>
    /// MySSO session Cookie 的 SameSite 設定。
    /// </summary>
    public SameSiteMode SessionCookieSameSite { get; set; } = SameSiteMode.Lax;

    /// <summary>
    /// MySSO session Cookie 的 Secure 設定。
    /// </summary>
    public CookieSecurePolicy SessionCookieSecurePolicy { get; set; } = CookieSecurePolicy.SameAsRequest;

    /// <summary>
    /// MySSO session 的閒置逾時分鐘數。
    /// </summary>
    public int SessionIdleTimeoutMinutes { get; set; } = 30;

    /// <summary>
    /// 保存 MySSO authentication ticket 的 Redis 連線字串；未設定時 ticket 會存放在受保護的 Cookie。
    /// </summary>
    public string? TicketStoreRedisServer { get; set; }

    /// <summary>
    /// MySSO authentication ticket 在 Redis 使用的 key 前綴。
    /// </summary>
    public string SessionStoreKeyPrefix { get; set; } = "MySSO:Session:";

    /// <summary>
    /// 呼叫平台 Auth Server 時 backchannel HTTP 要求的逾時秒數。
    /// </summary>
    public int BackchannelTimeoutSeconds { get; set; } = 30;
}