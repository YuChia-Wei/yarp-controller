namespace Yarp.Gateway.Configuration;

/// <summary>
/// Auth 設定
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// 加入身分認證設定檔
    /// </summary>
    /// <param name="configurationManager"></param>
    public static ConfigurationManager AddAuthenticationConfigurations(this ConfigurationManager configurationManager)
    {
        configurationManager
            .AddJsonFile(Path.Combine("Configuration", "Authentication", "JwtSetting.json"), true, false)
            .AddJsonFile(Path.Combine("Configuration", "Authentication", "OpidSetting.json"), true, false);
        return configurationManager;
    }

    /// <summary>
    /// 加入 Yarp 設定控制檔
    /// </summary>
    /// <param name="configurationManager"></param>
    public static ConfigurationManager AddYarpConfigurations(this ConfigurationManager configurationManager)
    {
        configurationManager
            .AddJsonFile(Path.Combine("Configuration", "ReverseProxy", "ClustersSetting.json"), true, true)
            .AddJsonFile(Path.Combine("Configuration", "ReverseProxy", "RoutesSetting.json"), true, true);
        return configurationManager;
    }
}