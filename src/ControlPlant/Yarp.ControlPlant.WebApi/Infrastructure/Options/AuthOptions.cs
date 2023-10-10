namespace Yarp.ControlPlant.WebApi.Infrastructure.Options;

/// <summary>
/// openId connection Auth 設定
/// </summary>
/// <remarks>
/// 因為此物件會依據各 Application 的需求而導致屬性欄位有所差異，因此放置於 Application 專案而非 infra 專案
/// </remarks>
public class AuthOptions
{
    /// <summary>
    /// Config 常數
    /// </summary>
    public const string Auth = "Auth";

    /// <summary>
    /// Authority Url
    /// </summary>
    public string Authority { get; set; }

    /// <summary>
    /// ClientId
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>
    /// The client secret.
    /// </value>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>
    /// The audience.
    /// </value>
    public string Audience { get; set; }

    /// <summary>
    /// 從 Configuration 中取得認證物件
    /// </summary>
    /// <param name="configurationSection"></param>
    /// <returns></returns>
    public static AuthOptions CreateInstance(IConfiguration configurationSection)
    {
        return configurationSection.GetSection(Auth).Get<AuthOptions>();
    }
}