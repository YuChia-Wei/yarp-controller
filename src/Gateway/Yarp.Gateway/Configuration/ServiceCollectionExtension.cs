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
    public static ConfigurationManager AddAuthenticationConfigurationJsons(this ConfigurationManager configurationManager)
    {
        configurationManager.AddJwtAuthSetting(Path.Combine("Configuration", "Authentication", "JwtSetting.json"));
        configurationManager.AddOpidAuthSetting(Path.Combine("Configuration", "Authentication", "OpidSetting.json"));
        return configurationManager;
    }

    /// <summary>
    /// 加入 Yarp 設定控制檔
    /// </summary>
    /// <param name="configurationManager"></param>
    public static ConfigurationManager AddYarpConfigurationJsons(this ConfigurationManager configurationManager)
    {
        configurationManager.AddYarpClusterJson(Path.Combine("Configuration", "ReverseProxy", "ClustersSetting.json"));
        configurationManager.AddYarpRouteJson(Path.Combine("Configuration", "ReverseProxy", "RoutesSetting.json"));

        return configurationManager;
    }

    private static void AddJwtAuthSetting(this ConfigurationManager configurationManager, string filePath)
    {
        configurationManager.AddJsonFile(filePath, true, false);
    }

    private static void AddOpidAuthSetting(this ConfigurationManager configurationManager, string filePath)
    {
        configurationManager.AddJsonFile(filePath, true, false);
    }

    private static void AddYarpClusterJson(this ConfigurationManager configurationManager, string jsonPath)
    {
        configurationManager.AddJsonFile(jsonPath, true, true);
    }

    private static void AddYarpRouteJson(this ConfigurationManager configurationManager, string jsonPath)
    {
        configurationManager.AddJsonFile(jsonPath, true, true);
    }
}