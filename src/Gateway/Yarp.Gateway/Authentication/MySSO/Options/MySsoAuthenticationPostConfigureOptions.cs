using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Handlers;

namespace Yarp.Gateway.Authentication.MySSO.Options;

/// <summary>
/// 為 MySSO remote authentication options 建立 state 資料保護格式。
/// </summary>
/// <param name="dataProtectionProvider">用來建立 state protector 的 ASP.NET Core Data Protection provider。</param>
internal sealed class MySsoAuthenticationPostConfigureOptions(IDataProtectionProvider dataProtectionProvider)
    : IPostConfigureOptions<MySsoAuthenticationOptions>
{
    /// <inheritdoc />
    public void PostConfigure(string? name, MySsoAuthenticationOptions options)
    {
        options.DataProtectionProvider ??= dataProtectionProvider;
        options.StateDataFormat ??= new PropertiesDataFormat(
            options.DataProtectionProvider.CreateProtector(
                typeof(MySsoAuthenticationHandler).FullName!,
                name ?? MySsoAuthenticationDefaults.RemoteScheme,
                "v1"));
    }
}