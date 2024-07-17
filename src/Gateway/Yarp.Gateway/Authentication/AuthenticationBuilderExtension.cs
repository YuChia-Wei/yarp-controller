using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Yarp.Gateway.Authentication.OpenIdConnect;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication;

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
        // .net 8 之後要使用這個
        // JsonWebTokenHandler.DefaultInboundClaimTypeMap.Add(JwtRegisteredClaimNames.Name, ClaimTypes.Name);
        builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = jwtAuthOptions.Authority;
            options.RequireHttpsMetadata = jwtAuthOptions.RequireHttpsMetadata;
            options.Audience = jwtAuthOptions.Audience;
        });

        return builder;
    }

    /// <summary>
    /// MVC / Gateway 或其他需要 opid (OAuth2) 認證時使用
    /// </summary>
    /// <param name="builder">IServiceCollection</param>
    /// <param name="authOptions">Opid</param>
    public static AuthenticationBuilder AddOpenIdConnectWithCookie(this AuthenticationBuilder builder,
                                                                   OpidAuthOptions authOptions)
    {
        // Cookie OAuth 會需要設定 HA Server 時的外部資料儲存來源
        // 在 Cookie Auth 區塊有設定 SessionStore 的狀況下，這個設定無效 (via. https://github.com/dotnet/AspNetCore.Docs/issues/21163 )
        // ref: https://learn.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-7.0#share-authentication-cookies-with-aspnet-core-identity
        builder.Services
               .AddDataProtection()
               .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(authOptions.TicketStoreRedisServer),
                                                $"{authOptions.LoginApplicationName}:LoginCookies:")
               .SetApplicationName(authOptions.LoginApplicationName);

        // cookies auth via.https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-7.0
        builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.Cookie.Name = authOptions.LoginCookieName;
                   // options.Cookie.SameSite = SameSiteMode.Lax;
                   options.Cookie.SameSite = authOptions.CookieSameSiteMode;

                   // options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                   options.Cookie.SecurePolicy = authOptions.CookieSecurePolicy;

                   //如果有設定 cookie domain (要共用登入資訊) 的話，再指定 domain，不然哪個網址進入就存哪邊
                   if (!string.IsNullOrEmpty(authOptions.LoginCookieDomain))
                   {
                       options.Cookie.Domain = authOptions.LoginCookieDomain;
                   }

                   options.SessionStore =
                       new RedisCacheTicketStore($"{authOptions.LoginApplicationName}:LoginSession:", authOptions.TicketStoreRedisServer);

                   options.Events = new CookieAuthenticationEvents
                   {
                       OnValidatePrincipal = async cookieContext =>
                       {
                           var logger = cookieContext.HttpContext.RequestServices.GetRequiredService<ILogger<CookieAuthenticationEvents>>();

                           /*
                            * cookieContext.Properties.GetTokenValue(key)
                            * cookieContext.Properties.UpdateTokenValue(key)
                            * key 可使用的參數值有
                            * 1. access_token
                            * 2. id_token       = openId connection 驗證身分所需的 token，預設 5 分鐘過期
                            * 3. refresh_token
                            * 4. token_type
                            * 5. expires_at     = access_token 的到期日
                            * 對應 OpenIdConnect 的參數物件
                            * OpenIdConnectParameterNames.AccessToken
                            * OpenIdConnectParameterNames.IdToken
                            * OpenIdConnectParameterNames.RefreshToken
                            * OpenIdConnectParameterNames.TokenType
                            *
                            * expires_at 的部分在 OpenIdConnectParameterNames 無對應
                            *
                            * cookieContext.Properties.IssuedUtc = 跟 OAuth Server 進行驗證的時間
                            *
                            * cookieContext.Properties.ExpiresUtc
                            *   * cookies 有效期的時間
                            *   * 如果 AddOpenIdConnect 裡面有設定 UseTokenLifeTime 的話，這個時間會使用 id_token 的過期時間
                            */

                           var now = DateTimeOffset.UtcNow;
                           var expiresAt = cookieContext.Properties.GetTokenValue("expires_at");
                           var accessTokenExpiration = DateTimeOffset.Parse(expiresAt);

                           var timeRemaining = accessTokenExpiration.Subtract(now);

                           // TODO: Get this from configuration with a fallback value.
                           var refreshThresholdMinutes = 5;
                           var refreshThreshold = TimeSpan.FromMinutes(refreshThresholdMinutes);

                           if (timeRemaining < refreshThreshold)
                           {
                               logger.LogTrace("OnValidatePrincipal - should be refresh token");

                               var refreshToken = cookieContext.Properties.GetTokenValue(OpenIdConnectParameterNames.RefreshToken);

                               if (refreshToken is null)
                               {
                                   logger.LogWarning("OnValidatePrincipal - Refresh Token Not Found!");

                                   cookieContext.RejectPrincipal();
                                   await cookieContext.HttpContext.SignOutAsync().ConfigureAwait(false);
                                   //登出後強制中斷事件，讓 .net 回去走登入流程
                                   return;
                               }

                               var httpClient = cookieContext.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
                               var response = await httpClient.RequestRefreshTokenAsync(
                                                  new RefreshTokenRequest
                                                  {
                                                      Address = authOptions.RefreshTokenAddress,
                                                      ClientId = authOptions.ClientId,
                                                      ClientSecret = authOptions.ClientSecret,
                                                      RefreshToken = refreshToken!
                                                  }).ConfigureAwait(false);

                               //如果 refresh 錯誤就登出且強制中斷事件
                               if (response.IsError)
                               {
                                   logger.LogWarning("OnValidatePrincipal - Token Refresh Error!");

                                   cookieContext.RejectPrincipal();
                                   await cookieContext.HttpContext.SignOutAsync().ConfigureAwait(false);
                                   return;
                               }

                               //can ref OpenIdConnectHandler
                               var expiresInSeconds = response.ExpiresIn;
                               var updatedExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);

                               cookieContext.Properties.UpdateTokenValue("expires_at", updatedExpiresAt.ToString());
                               cookieContext.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, response.AccessToken);
                               cookieContext.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, response.RefreshToken);
                               cookieContext.Properties.UpdateTokenValue(OpenIdConnectParameterNames.IdToken, response.IdentityToken);

                               // Indicate to the cookie middleware that the cookie should be
                               // remade (since we have updated it)
                               cookieContext.ShouldRenew = true;
                           }
                       }
                   };
               })
               .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
               {
                   options.Authority = authOptions.Authority;
                   options.ClientId = authOptions.ClientId;
                   options.ClientSecret = authOptions.ClientSecret;

                   // 如果要改 redirect url 的時候要用這個
                   // options.CallbackPath = "/auth-redirect-url";

                   options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
                   options.ResponseType = authOptions.ResponseType;
                   // options.ResponseType = OpenIdConnectResponseType.Code;
                   // options.ResponseMode = OpenIdConnectResponseMode.FormPost;

                   // 沒有清除的話，預設 scope 裡面有一個 profile 的項目
                   options.Scope.Clear();

                   //要讓 .net 預設的 openid 認證成功需要這個 Scope
                   options.Scope.Add(OpenIdConnectScope.OpenId);

                   // 從設定檔中取得 OAuth Scope
                   foreach (var item in authOptions.WebApiAudience)
                   {
                       options.Scope.Add(item);
                   }

                   // 跟 OAuth Server 要 Refresh Token
                   options.Scope.Add(OpenIdConnectScope.OfflineAccess);

                   // if true , cookies ExpiresUtc will be use id_token expires time
                   // options.UseTokenLifetime = true;

                   options.SaveTokens = true;

                   options.GetClaimsFromUserInfoEndpoint = true;

                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       // NameClaimType = "name",
                       NameClaimType = JwtClaimTypes.Name,
                       // RoleClaimType = "role"
                       RoleClaimType = JwtClaimTypes.Role
                   };

                   //ref: https://github.com/skoruba/IdentityServer4.Admin/issues/109
                   //ref: https://stackoverflow.com/a/70279411
                   options.Events.OnUserInformationReceived = context =>
                   {
                       // var roleElement = context.User.RootElement.GetProperty("role");
                       // var roleElement = context.User.RootElement.GetProperty(JwtClaimTypes.Role);
                       if (context.User.RootElement.TryGetProperty(JwtClaimTypes.Role, out var roleElement))
                       {
                           AppendRoleToClaims(context, roleElement);
                       }

                       return Task.CompletedTask;
                   };

                   // OpenIdConnect 套件預設 PKCE = True
                   // options.UsePkce = false;
               });
        return builder;
    }

    /// <summary>
    /// Add Authentication for Yarp
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configurationManager"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddYarpAuthentication(this IServiceCollection serviceCollection, ConfigurationManager configurationManager)
    {
        var gatewayAuthSettingOptions = configurationManager.GetSection("GatewayAuthSetting").Get<GatewayAuthSettingOptions>();

        ArgumentNullException.ThrowIfNull(gatewayAuthSettingOptions);

        var authenticationBuilder = gatewayAuthSettingOptions.Default switch
        {
            DefaultAuthEnum.Opid => serviceCollection.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }),
            DefaultAuthEnum.Jwt => serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme),
            _ => throw new ArgumentOutOfRangeException()
        };

        // if (gatewayAuthSettingOptions.Opid?.IsSettled ?? false)
        // {
        //     authenticationBuilder.AddOpenIdConnectWithCookie(gatewayAuthSettingOptions.Opid);
        // }

        if (gatewayAuthSettingOptions.Jwt?.IsSettled ?? false)
        {
            authenticationBuilder.AddJwtAuthentication(gatewayAuthSettingOptions.Jwt);
        }
    }

    private static void AppendRoleToClaims(UserInformationReceivedContext context, JsonElement roleElement)
    {
        // context.User.RootElement.TryGetProperty("roles", out var rolesElement);

        var claims = new List<Claim>();

        if (roleElement.ValueKind == JsonValueKind.Array)
        {
            claims.AddRange(roleElement.EnumerateArray().Select(r => new Claim(JwtClaimTypes.Role, r.GetString() ?? string.Empty)));
        }
        else
        {
            claims.Add(new Claim(JwtClaimTypes.Role, roleElement.GetString() ?? string.Empty));
        }

        if (context.Principal?.Identity is ClaimsIdentity id)
        {
            id.AddClaims(claims);
        }
    }
}