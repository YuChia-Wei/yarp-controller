using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Yarp.Gateway.Authentication.ExternalToken.Models;

/// <summary>
/// 外部驗證服務回傳的結果，並負責轉換為 ASP.NET Core 驗證資料。
/// </summary>
public sealed class ExternalTokenAuthenticationResult
{
    /// <summary>
    /// 外部服務明確回傳的成功狀態；未提供時會改以狀態代碼判斷。
    /// </summary>
    public bool? Success { get; init; }

    /// <summary>
    /// 外部服務回傳的狀態代碼；預設 "0" 表示成功。
    /// </summary>
    public string? StatusCode { get; init; }

    /// <summary>
    /// 外部服務回傳的狀態訊息。
    /// </summary>
    public string? StatusMessage { get; init; }

    /// <summary>
    /// 外部服務舊版契約使用的狀態訊息欄位。
    /// </summary>
    public string? StatusMsg { get; init; }

    /// <summary>
    /// 驗證成功後代表使用者或應用程式的識別碼。
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// 驗證成功後顯示用的名稱。
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// 若外部服務有代理或模擬登入情境，代表原始使用者識別碼。
    /// </summary>
    public string? OriginalId { get; init; }

    /// <summary>
    /// 驗證來源裝置的識別碼。
    /// </summary>
    public string? DeviceId { get; init; }

    /// <summary>
    /// 外部服務回傳的角色清單。
    /// </summary>
    public string[] Roles { get; init; } = [];

    /// <summary>
    /// 外部服務回傳的額外 claims。
    /// </summary>
    public Dictionary<string, string[]> Claims { get; init; } = [];

    /// <summary>
    /// 外部服務回傳、可轉送給下游服務的存取權杖。
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// 外部服務回傳、可用來更新存取權杖的重新整理權杖。
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// 外部服務回傳的權杖類型。
    /// </summary>
    public string? TokenType { get; init; }

    /// <summary>
    /// 存取權杖剩餘有效秒數。
    /// </summary>
    public int? ExpiresIn { get; init; }

    /// <summary>
    /// 保留外部服務回應中目前模型未明確定義的欄位。
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; } = [];

    /// <summary>
    /// 表示外部驗證是否成功。
    /// </summary>
    public bool IsSuccess => this.Success ?? FirstNonEmpty(this.StatusCode, this.GetString("status_code")) == "0";

    /// <summary>
    /// 外部服務回傳的錯誤訊息。
    /// </summary>
    public string? ErrorMessage => FirstNonEmpty(this.StatusMessage, this.StatusMsg, this.GetString("status_message"), this.GetString("status_msg"));

    /// <summary>
    /// 建立失敗的外部驗證結果。
    /// </summary>
    /// <param name="message">失敗原因或錯誤訊息。</param>
    /// <returns>失敗的外部驗證結果。</returns>
    public static ExternalTokenAuthenticationResult Fail(string message)
    {
        return new ExternalTokenAuthenticationResult
        {
            Success = false,
            StatusMessage = message
        };
    }

    /// <summary>
    /// 建立包含外部服務權杖資訊的 ASP.NET Core 驗證屬性。
    /// </summary>
    /// <returns>外部服務回傳的驗證屬性。</returns>
    internal AuthenticationProperties CreateAuthenticationProperties()
    {
        var properties = new AuthenticationProperties();
        this.StoreTokens(properties);
        return properties;
    }

    /// <summary>
    /// 將外部服務回傳的權杖資訊保存至指定驗證屬性。
    /// </summary>
    /// <param name="properties">要保存權杖資訊的驗證屬性。</param>
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

        if (tokens.Count > 0)
        {
            properties.StoreTokens(tokens);
        }
    }

    /// <summary>
    /// 將外部服務回傳的使用者資料與角色轉換為 claims principal。
    /// </summary>
    /// <param name="authenticationType">建立 claims identity 時使用的驗證型態。</param>
    /// <returns>外部驗證成功後的 claims principal。</returns>
    internal ClaimsPrincipal CreatePrincipal(string authenticationType)
    {
        var claims = new List<Claim>();

        if (!string.IsNullOrWhiteSpace(this.Id))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, this.Id));
            claims.Add(new Claim(ClaimTypes.Name, this.Id));
        }

        if (!string.IsNullOrWhiteSpace(this.Name) && this.Name != this.Id)
        {
            claims.Add(new Claim(ClaimTypes.Name, this.Name));
        }

        if (!string.IsNullOrWhiteSpace(this.OriginalId))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, this.OriginalId));
        }

        if (!string.IsNullOrWhiteSpace(this.DeviceId))
        {
            claims.Add(new Claim("device_id", this.DeviceId));
        }

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
    /// 將非空白權杖值加入驗證權杖清單。
    /// </summary>
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
    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    /// <summary>
    /// 從延伸欄位讀取整數值。
    /// </summary>
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
    private string? GetString(string propertyName)
    {
        return this.ExtensionData.TryGetValue(propertyName, out var value) && value.ValueKind == JsonValueKind.String
                   ? value.GetString()
                   : null;
    }
}