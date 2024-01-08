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
        configurationManager.AddAuthSetting(GetRealJsonPath(Path.Combine("Configuration", "Authentication", "AuthSetting.json")));

        return configurationManager;
    }

    /// <summary>
    /// 加入 Yarp 設定控制檔
    /// </summary>
    /// <param name="configurationManager"></param>
    public static ConfigurationManager AddYarpConfigurationJsons(this ConfigurationManager configurationManager)
    {
        //因為需要動態處理檔案，所以必須確認檔案的正確位置，才能真的去異動資訊
        configurationManager.AddYarpClusterJson(GetRealJsonPath(Path.Combine("Configuration", "ReverseProxy", "ClustersSetting.json")));
        configurationManager.AddYarpRouteJson(GetRealJsonPath(Path.Combine("Configuration", "ReverseProxy", "RoutesSetting.json")));

        return configurationManager;
    }

    private static void AddAuthSetting(this ConfigurationManager configurationManager, string filePath)
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

    private static string GetRealJsonPath(string jsonPath)
    {
        var resolveLinkTarget = File.ResolveLinkTarget(jsonPath, true);
        return resolveLinkTarget?.FullName ?? jsonPath;
    }
}