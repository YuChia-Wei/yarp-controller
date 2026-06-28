using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Yarp.Gateway.Authentication.MySSO.Models;

/// <summary>
/// 平台 Auth Server 完成 MySSO 身分交換後回傳的登入結果。
/// </summary>
public sealed class MySsoTokenExchangeResult
{
    /// <summary>
    /// Auth Server 明確回傳的成功狀態；未提供時會改以狀態代碼判斷。
    /// </summary>
    public bool? Success { get; init; }

    /// <summary>
    /// Auth Server 回傳的狀態代碼；預設 "0" 表示成功。
    /// </summary>
    public string? StatusCode { get; init; }

    /// <summary>
    /// Auth Server 回傳的狀態訊息。
    /// </summary>
    public string? StatusMessage { get; init; }

    /// <summary>
    /// Auth Server 舊版契約使用的狀態訊息欄位。
    /// </summary>
    public string? StatusMsg { get; init; }

    /// <summary>
    /// MySSO 驗證後取得的 customer id。
    /// </summary>
    public string? CustomerId { get; init; }

    /// <summary>
    /// Auth Server 回傳的使用者識別碼；未提供 customer id 時作為相容欄位。
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// 登入成功後顯示用的名稱。
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Auth Server 回傳的角色清單。
    /// </summary>
    public string[] Roles { get; init; } = [];

    /// <summary>
    /// Auth Server 回傳的額外 claims。
    /// </summary>
    public Dictionary<string, string[]> Claims { get; init; } = [];

    /// <summary>
    /// 平台 Auth Server 簽發的存取權杖。
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// 平台 Auth Server 簽發的一次性重新整理權杖。
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// 平台 Auth Server 回傳的權杖類型。
    /// </summary>
    public string? TokenType { get; init; }

    /// <summary>
    /// 存取權杖剩餘有效秒數。
    /// </summary>
    public int? ExpiresIn { get; init; }

    /// <summary>
    /// 重新整理權杖剩餘有效秒數。
    /// </summary>
    public int? RefreshTokenExpiresIn { get; init; }

    /// <summary>
    /// 重新整理權杖的 UTC 到期時間。
    /// </summary>
    public DateTimeOffset? RefreshTokenExpiresAt { get; init; }

    /// <summary>
    /// 保留 Auth Server 回應中目前模型未明確定義的欄位。
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; } = [];

    /// <summary>
    /// 表示 MySSO token exchange 是否成功。
    /// </summary>
    public bool IsSuccess => this.Success ?? FirstNonEmpty(this.StatusCode, this.GetString("status_code")) == "0";

    /// <summary>
    /// Auth Server 回傳的錯誤訊息。
    /// </summary>
    public string? ErrorMessage => FirstNonEmpty(this.StatusMessage, this.StatusMsg, this.GetString("status_message"), this.GetString("status_msg"));

    /// <summary>
    /// 建立失敗的 MySSO token exchange 結果。
    /// </summary>
    /// <param name="message">失敗原因或錯誤訊息。</param>
    /// <returns>失敗的 MySSO token exchange 結果。</returns>
    public static MySsoTokenExchangeResult Fail(string message)
    {
        return new MySsoTokenExchangeResult
        {
            Success = false,
            StatusMessage = message
        };
    }

    /// <summary>
    /// 將 Auth Server 回傳的權杖保存至 ASP.NET Core 驗證屬性。
    /// </summary>
    /// <param name="properties">要保存平台權杖的驗證屬性。</param>
    internal void StoreTokens(AuthenticationProperties properties)
    {
        var tokens = new List<AuthenticationToken>();

        AddToken(tokens, OpenIdConnectParameterNames.AccessToken, FirstNonEmpty(this.AccessToken, this.GetString("access_token")));
        AddToken(tokens, OpenIdConnectParameterNames.RefreshToken, FirstNonEmpty(this.RefreshToken, this.GetString("refresh_token")));
        AddToken(tokens, OpenIdConnectParameterNames.TokenType, FirstNonEmpty(this.TokenType, this.GetString("token_type")));

        var expiresIn = this.ExpiresIn ?? this.GetInt("expires_in");
        if (expiresIn is > 0)
        {
            AddToken(tokens, "expires_at", DateTimeOffset.UtcNow.AddSeconds(expiresIn.Value).ToString("O"));
        }

        var refreshTokenExpiresAt = this.GetRefreshTokenExpiresAt();
        if (refreshTokenExpiresAt is not null)
        {
            AddToken(tokens, "refresh_token_expires_at", refreshTokenExpiresAt.Value.ToString("O"));
        }

        if (tokens.Count > 0)
        {
            properties.StoreTokens(tokens);
        }
    }

    /// <summary>
    /// 將 Auth Server 回傳的 customer 與授權資料轉換為 claims principal。
    /// </summary>
    /// <param name="authenticationType">建立 claims identity 時使用的驗證型態。</param>
    /// <returns>MySSO 登入成功後的 claims principal。</returns>
    internal ClaimsPrincipal CreatePrincipal(string authenticationType)
    {
        var customerId = FirstNonEmpty(this.CustomerId, this.GetString("customer_id"), this.Id, this.GetString("id"));
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new InvalidOperationException("MySSO token exchange response does not contain a customer id.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customerId),
            new(ClaimTypes.Name, FirstNonEmpty(this.Name, customerId)!)
        };

        claims.AddRange(this.Roles.Where(role => !string.IsNullOrWhiteSpace(role))
                            .Select(role => new Claim(ClaimTypes.Role, role)));

        foreach (var (claimType, values) in this.Claims)
        {
            claims.AddRange(values.Where(value => !string.IsNullOrWhiteSpace(value))
                                  .Select(value => new Claim(claimType, value)));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType, ClaimTypes.Name, ClaimTypes.Role));
    }

    /// <summary>
    /// 計算重新整理權杖的 UTC 到期時間。
    /// </summary>
    /// <returns>重新整理權杖的 UTC 到期時間；無資料時回傳 null。</returns>
    private DateTimeOffset? GetRefreshTokenExpiresAt()
    {
        if (this.RefreshTokenExpiresAt is not null)
        {
            return this.RefreshTokenExpiresAt;
        }

        var extensionExpiresAt = this.GetDateTimeOffset("refresh_token_expires_at");
        if (extensionExpiresAt is not null)
        {
            return extensionExpiresAt;
        }

        var expiresIn = this.RefreshTokenExpiresIn ?? this.GetInt("refresh_token_expires_in");
        return expiresIn is > 0
                   ? DateTimeOffset.UtcNow.AddSeconds(expiresIn.Value)
                   : null;
    }

    /// <summary>
    /// 將非空白權杖值加入驗證權杖清單。
    /// </summary>
    /// <param name="tokens">要加入的驗證權杖清單。</param>
    /// <param name="name">權杖名稱。</param>
    /// <param name="value">權杖值。</param>
    private static void AddToken(List<AuthenticationToken> tokens, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            tokens.Add(new AuthenticationToken
            {
                Name = name,
                Value = value
            });
        }
    }

    /// <summary>
    /// 從多個字串中取第一個非空白值。
    /// </summary>
    /// <param name="values">候選字串值。</param>
    /// <returns>第一個非空白值；沒有符合項目時回傳 null。</returns>
    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    /// <summary>
    /// 從延伸欄位讀取 UTC 日期時間。
    /// </summary>
    /// <param name="propertyName">延伸欄位名稱。</param>
    /// <returns>解析成功時回傳 UTC 日期時間；否則回傳 null。</returns>
    private DateTimeOffset? GetDateTimeOffset(string propertyName)
    {
        var value = this.GetString(propertyName);
        return DateTimeOffset.TryParse(value, out var result)
                   ? result
                   : null;
    }

    /// <summary>
    /// 從延伸欄位讀取整數值。
    /// </summary>
    /// <param name="propertyName">延伸欄位名稱。</param>
    /// <returns>讀取成功時回傳整數值；否則回傳 null。</returns>
    private int? GetInt(string propertyName)
    {
        if (!this.ExtensionData.TryGetValue(propertyName, out var value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetInt32(out var number) => number,
            JsonValueKind.String when int.TryParse(value.GetString(), out var number) => number,
            _ => null
        };
    }

    /// <summary>
    /// 從延伸欄位讀取字串值。
    /// </summary>
    /// <param name="propertyName">延伸欄位名稱。</param>
    /// <returns>讀取成功時回傳字串值；否則回傳 null。</returns>
    private string? GetString(string propertyName)
    {
        return this.ExtensionData.TryGetValue(propertyName, out var value) && value.ValueKind == JsonValueKind.String
                   ? value.GetString()
                   : null;
    }
}