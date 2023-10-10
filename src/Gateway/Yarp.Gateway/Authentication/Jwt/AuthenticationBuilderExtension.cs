using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication.Jwt;

/// <summary>
/// Auth 設定
/// </summary>
public static class AuthenticationBuilderExtension
{
    /// <summary>
    /// 純 api 站台使用，加入 JWT 認證
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="jwtAuthOptions"></param>
    public static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder builder, JwtAuthOptions jwtAuthOptions)
    {
        // .net 7 之後預設使用的 jwt 套件已移除 name 這個 claim key，可以使用以下方式加入預設解析的 key mapping，其他名稱對應也可用相同的方式處理
        // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtRegisteredClaimNames.Name, ClaimTypes.Name);
        builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = jwtAuthOptions.Authority;
            options.RequireHttpsMetadata = jwtAuthOptions.RequireHttpsMetadata;
            options.Audience = jwtAuthOptions.Audience;
        });

        return builder;
    }
}