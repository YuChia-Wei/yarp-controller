namespace Yarp.Gateway.Authentication.Options;

public static class ConfigurationManagerExtenstion
{
    /// <summary>
    /// 建立 JwtAuthOptions 實體
    /// </summary>
    /// <param name="configurationSection"></param>
    /// <returns></returns>
    public static JwtAuthOptions CreateJwtAuthOptions(this ConfigurationManager configurationSection)
    {
        return configurationSection.GetSection("JwtAuth").Get<JwtAuthOptions>();
    }

    /// <summary>
    /// 建立 OpidAuthOptions 實體
    /// </summary>
    /// <param name="configurationSection"></param>
    /// <returns></returns>
    public static OpidAuthOptions CreateOpidAuthOptions(this ConfigurationManager configurationSection)
    {
        return configurationSection.GetSection("OpidAuth").Get<OpidAuthOptions>();
    }
}