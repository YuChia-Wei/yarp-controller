using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Yarp.Gateway.Authentication.MySSO.Configuration;

namespace Yarp.Gateway.Authentication.MySSO.Options;

/// <summary>
/// MySSO remote authentication handler 的執行期設定。
/// </summary>
public sealed class MySsoAuthenticationOptions : RemoteAuthenticationOptions
{
    /// <summary>
    /// 初始化 MySSO remote authentication 的預設設定。
    /// </summary>
    public MySsoAuthenticationOptions()
    {
        this.CallbackPath = MySsoAuthenticationDefaults.CallbackPath;
        this.SaveTokens = true;
        this.CorrelationCookie.SameSite = SameSiteMode.None;
        this.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    }

    /// <summary>
    /// MySSO 登入頁端點。
    /// </summary>
    public Uri? AuthorizationEndpoint { get; set; }

    /// <summary>
    /// 平台 Auth Server 接收 MySSO 單次 token 並簽發平台權杖的端點。
    /// </summary>
    public Uri? TokenExchangeEndpoint { get; set; }

    /// <summary>
    /// 平台 Auth Server 以 refresh token 換發新平台權杖的端點；供 /auth/refresh 使用，未設定時不提供主動續期。
    /// </summary>
    public Uri? RefreshTokenEndpoint { get; set; }

    /// <summary>
    /// Gateway 在 MySSO 平台使用的應用程式識別碼。
    /// </summary>
    public string? AppId { get; set; }

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
    /// 是否允許 MySSO 以 query string 回傳單次 token。
    /// </summary>
    public bool AllowQueryStringCallback { get; set; }

    /// <summary>
    /// MySSO 登入要求中額外附加的固定 query string 參數。
    /// </summary>
    public Dictionary<string, string> AdditionalAuthorizationParameters { get; set; } = [];

    /// <summary>
    /// 保護與還原登入 correlation state 的資料格式。
    /// </summary>
    public ISecureDataFormat<AuthenticationProperties>? StateDataFormat { get; set; }

    /// <inheritdoc />
    public override void Validate()
    {
        base.Validate();

        if (this.AuthorizationEndpoint is null)
        {
            throw new InvalidOperationException("MySSO authorization endpoint is not configured.");
        }

        if (this.TokenExchangeEndpoint is null)
        {
            throw new InvalidOperationException("MySSO token exchange endpoint is not configured.");
        }

        if (string.IsNullOrWhiteSpace(this.TokenParameterName))
        {
            throw new InvalidOperationException("MySSO token parameter name is not configured.");
        }

        if (string.IsNullOrWhiteSpace(this.StateParameterName))
        {
            throw new InvalidOperationException("MySSO state parameter name is not configured.");
        }
    }
}