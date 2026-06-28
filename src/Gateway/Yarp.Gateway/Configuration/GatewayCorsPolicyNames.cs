namespace Yarp.Gateway.Configuration;

/// <summary>
/// 定義 Gateway 使用的 CORS policy 名稱。
/// </summary>
public static class GatewayCorsPolicyNames
{
    /// <summary>
    /// 允許任意來源、HTTP header 與 HTTP method 的 policy。
    /// </summary>
    public const string AllowAll = "AllowAllPolicy";

    /// <summary>
    /// 提供 MySSO 前端端點使用，並公開 session 到期 header 的 policy。
    /// </summary>
    public const string AllowMySso = "AllowMySsoPolicy";
}