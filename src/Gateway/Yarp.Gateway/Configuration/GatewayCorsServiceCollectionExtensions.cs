using Microsoft.AspNetCore.Cors.Infrastructure;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Configuration;

/// <summary>
/// 註冊 Gateway CORS policies 的擴充方法。
/// </summary>
public static class GatewayCorsServiceCollectionExtensions
{
    /// <summary>
    /// 註冊一般 Gateway 路由與 MySSO 前端端點使用的 CORS policies。
    /// </summary>
    /// <param name="services">應用程式服務集合。</param>
    /// <param name="gatewayAuthConfiguration">Gateway authentication 組態；可為 <see langword="null"/>。</param>
    /// <returns>原始應用程式服務集合。</returns>
    public static IServiceCollection AddGatewayCors(
        this IServiceCollection services,
        GatewayAuthConfiguration? gatewayAuthConfiguration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                GatewayCorsPolicyNames.AllowAll,
                ConfigureAllowAll);

            options.AddPolicy(
                GatewayCorsPolicyNames.AllowMySso,
                corsPolicyBuilder =>
                {
                    ConfigureAllowAll(corsPolicyBuilder);

                    var sessionExpiresHeader = gatewayAuthConfiguration?.MySSO?.SessionExpiresHeaderName;
                    if (!string.IsNullOrWhiteSpace(sessionExpiresHeader))
                    {
                        corsPolicyBuilder.WithExposedHeaders(sessionExpiresHeader);
                    }
                });
        });

        return services;
    }

    /// <summary>
    /// 設定允許任意來源、HTTP header 與 HTTP method 的 CORS 規則。
    /// </summary>
    private static void ConfigureAllowAll(CorsPolicyBuilder corsPolicyBuilder)
    {
        corsPolicyBuilder.AllowAnyOrigin()
                         .AllowAnyHeader()
                         .AllowAnyMethod();
    }
}