using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using StackExchange.Redis;
using Yarp.Gateway.Authentication.ExternalToken.Configuration;
using Yarp.Gateway.Authentication.ExternalToken.Handlers;
using Yarp.Gateway.Authentication.ExternalToken.Services;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Handlers;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.MySSO.Services;
using Yarp.Gateway.Authentication.OpenIdConnect;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication;

public static class AuthenticationBuilderExtension
{
    /// <summary>
    /// 純 api 站台使用，加入 JWT 認證
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="jwtAuthConfiguration"></param>
    public static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder builder, JwtAuthConfiguration jwtAuthConfiguration)
    {
        // .net 7 之後預設使用的 jwt 套件已移除 name 這個 claim key，可以使用以下方式加入預設解析的 key mapping，其他名稱對應也可用相同的方式處理
        // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtRegisteredClaimNames.Name, ClaimTypes.Name);
        // .net 8 之後要使用這個
        // JsonWebTokenHandler.DefaultInboundClaimTypeMap.Add(JwtRegisteredClaimNames.Name, ClaimTypes.Name);
        builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = jwtAuthConfiguration.Authority;
            options.RequireHttpsMetadata = jwtAuthConfiguration.RequireHttpsMetadata;
            options.Audience = jwtAuthConfiguration.Audience;
        });

        return builder;
    }

    /// <summary>
    /// 加入每次 request 都會呼叫外部服務驗證 token 或 key 的 authentication scheme。
    /// </summary>
    /// <param name="builder">ASP.NET Core authentication builder。</param>
    /// <param name="authConfiguration">外部 Token 驗證設定。</param>
    public static AuthenticationBuilder AddExternalTokenAuthentication(
        this AuthenticationBuilder builder,
        ExternalTokenAuthenticationConfiguration authConfiguration)
    {
        builder.Services.TryAddSingleton<IExternalTokenAuthenticationClient, ExternalTokenAuthenticationHttpClient>();
        builder.Services.AddHttpClient(ExternalTokenAuthenticationDefaults.BackchannelHttpClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(authConfiguration.BackchannelTimeoutSeconds);
        });

        builder.AddScheme<ExternalTokenAuthenticationConfiguration, ExternalTokenAuthenticationHandler>(
            ExternalTokenAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.AppId = authConfiguration.AppId;
                options.AuthorizationHeaderName = authConfiguration.AuthorizationHeaderName;
                options.TokenScheme = authConfiguration.TokenScheme;
                options.KeyScheme = authConfiguration.KeyScheme;
                options.AcceptBearerToken = authConfiguration.AcceptBearerToken;
                options.IgnoreJwtBearerToken = authConfiguration.IgnoreJwtBearerToken;
                options.TokenValidationEndpoint = authConfiguration.TokenValidationEndpoint;
                options.KeyExchangeEndpoint = authConfiguration.KeyExchangeEndpoint;
                options.BackchannelTimeoutSeconds = authConfiguration.BackchannelTimeoutSeconds;
            });

        return builder;
    }

    /// <summary>
    /// 加入 MySSO form-post remote authentication flow 與獨立 session Cookie。
    /// </summary>
    /// <param name="builder">ASP.NET Core authentication builder。</param>
    /// <param name="authConfiguration">MySSO remote authentication 設定。</param>
    public static AuthenticationBuilder AddMySsoAuthentication(
        this AuthenticationBuilder builder,
        MySsoAuthenticationConfiguration authConfiguration)
    {
        // MySSO session cookie 只存放 ticket store key，access／refresh token 一律保存在 Redis ticket store，因此 Redis 連線為必填。
        if (string.IsNullOrWhiteSpace(authConfiguration.TicketStoreRedisServer))
        {
            throw new InvalidOperationException("MySSO ticket store Redis server is not configured.");
        }

        builder.Services.TryAddSingleton<IMySsoTokenExchangeClient, MySsoTokenExchangeHttpClient>();
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IPostConfigureOptions<MySsoAuthenticationOptions>,
                MySsoAuthenticationPostConfigureOptions>());

        var idleTimeout = TimeSpan.FromMinutes(authConfiguration.SessionIdleTimeoutMinutes);
        var renewalInterval = TimeSpan.FromSeconds(authConfiguration.SessionRenewalIntervalSeconds);

        builder.AddCookie(
            MySsoAuthenticationDefaults.SessionScheme,
            options =>
            {
                options.Cookie.Name = authConfiguration.SessionCookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = authConfiguration.SessionCookieSameSite;
                options.Cookie.SecurePolicy = authConfiguration.SessionCookieSecurePolicy;
                options.ExpireTimeSpan = idleTimeout;
                options.SlidingExpiration = true;

                if (!string.IsNullOrWhiteSpace(authConfiguration.SessionCookieDomain))
                {
                    options.Cookie.Domain = authConfiguration.SessionCookieDomain;
                }

                options.SessionStore = new RedisCacheTicketStore(
                    authConfiguration.SessionStoreKeyPrefix,
                    authConfiguration.TicketStoreRedisServer);

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        var cookieOptions = context.Options.Cookie.Build(context.HttpContext);
                        context.Response.Cookies.Delete(authConfiguration.SessionCookieName, cookieOptions);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    },
                    OnCheckSlidingExpiration = context =>
                    {
                        // 有操作就重置閒置時間：Always 每個請求都延長；Periodic 距上次續期超過間隔才延長以降低 Redis 寫入。
                        var shouldRenew = authConfiguration.SessionRenewalMode == MySsoSessionRenewalMode.Always ||
                                          context.ElapsedTime >= renewalInterval;
                        context.ShouldRenew = shouldRenew;

                        var now = (context.Options.TimeProvider ?? TimeProvider.System).GetUtcNow();
                        var expiresAt = shouldRenew ? now.Add(idleTimeout) : context.Properties.ExpiresUtc;
                        if (expiresAt is not null)
                        {
                            context.HttpContext.Items[MySsoAuthenticationDefaults.SessionExpiresItemKey] = expiresAt;
                            context.HttpContext.Response.Headers[authConfiguration.SessionExpiresHeaderName] =
                                expiresAt.Value.ToString("O");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        builder.AddRemoteScheme<MySsoAuthenticationOptions, MySsoAuthenticationHandler>(
            MySsoAuthenticationDefaults.RemoteScheme,
            null,
            options =>
            {
                options.AuthorizationEndpoint = authConfiguration.AuthorizationEndpoint;
                options.TokenExchangeEndpoint = authConfiguration.TokenExchangeEndpoint;
                options.RefreshTokenEndpoint = authConfiguration.RefreshTokenEndpoint;
                options.AppId = authConfiguration.AppId;
                options.CallbackPath = authConfiguration.CallbackPath;
                options.TokenParameterName = authConfiguration.TokenParameterName;
                options.StateParameterName = authConfiguration.StateParameterName;
                options.AppIdParameterName = authConfiguration.AppIdParameterName;
                options.RedirectUriParameterName = authConfiguration.RedirectUriParameterName;
                options.AllowQueryStringCallback = authConfiguration.AllowQueryStringCallback;
                options.AdditionalAuthorizationParameters = authConfiguration.AdditionalAuthorizationParameters;
                options.SignInScheme = MySsoAuthenticationDefaults.SessionScheme;
                options.BackchannelTimeout = TimeSpan.FromSeconds(authConfiguration.BackchannelTimeoutSeconds);
                options.SaveTokens = true;
            });

        builder.AddPolicyScheme(
            MySsoAuthenticationDefaults.InteractiveScheme,
            null,
            options =>
            {
                options.ForwardAuthenticate = MySsoAuthenticationDefaults.SessionScheme;
                options.ForwardChallenge = MySsoAuthenticationDefaults.RemoteScheme;
                options.ForwardForbid = MySsoAuthenticationDefaults.SessionScheme;
                options.ForwardSignIn = MySsoAuthenticationDefaults.SessionScheme;
                options.ForwardSignOut = MySsoAuthenticationDefaults.SessionScheme;
            });

        builder.Services.AddAuthorizationBuilder()
               .AddPolicy(
                   MySsoAuthenticationDefaults.InteractivePolicy,
                   policy =>
                   {
                       policy.AddAuthenticationSchemes(MySsoAuthenticationDefaults.InteractiveScheme);
                       policy.RequireAuthenticatedUser();
                   });

        return builder;
    }

    /// <summary>
    /// MVC / Gateway 或其他需要 opid (OAuth2) 認證時使用
    /// </summary>
    /// <param name="builder">IServiceCollection</param>
    /// <param name="authConfiguration">Opid</param>
    public static AuthenticationBuilder AddOpenIdConnectWithCookie(
        this AuthenticationBuilder builder,
        OpidAuthConfiguration authConfiguration)
    {
        // Cookie OAuth 會需要設定 HA Server 時的外部資料儲存來源
        // 在 Cookie Auth 區塊有設定 SessionStore 的狀況下，這個設定無效 (via. https://github.com/dotnet/AspNetCore.Docs/issues/21163 )
        // ref: https://learn.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-7.0#share-authentication-cookies-with-aspnet-core-identity
        builder.Services
               .AddDataProtection()
               .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(authConfiguration.TicketStoreRedisServer),
                                                $"{authConfiguration.LoginApplicationName}:LoginCookies:")
               .SetApplicationName(authConfiguration.LoginApplicationName);

        // cookies auth via.https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-7.0
        builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.Cookie.Name = authConfiguration.LoginCookieName;
                   // options.Cookie.SameSite = SameSiteMode.Lax;
                   options.Cookie.SameSite = authConfiguration.CookieSameSiteMode;

                   // options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                   options.Cookie.SecurePolicy = authConfiguration.CookieSecurePolicy;

                   //如果有設定 cookie domain (要共用登入資訊) 的話，再指定 domain，不然哪個網址進入就存哪邊
                   if (!string.IsNullOrEmpty(authConfiguration.LoginCookieDomain))
                   {
                       options.Cookie.Domain = authConfiguration.LoginCookieDomain;
                   }

                   options.SessionStore =
                       new RedisCacheTicketStore($"{authConfiguration.LoginApplicationName}:LoginSession:", authConfiguration.TicketStoreRedisServer);

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
                           if (string.IsNullOrWhiteSpace(expiresAt))
                           {
                               logger.LogWarning("OnValidatePrincipal - Access Token Expiration Not Found!");

                               cookieContext.RejectPrincipal();
                               await cookieContext.HttpContext.SignOutAsync().ConfigureAwait(false);
                               return;
                           }

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
                                                      Address = authConfiguration.RefreshTokenAddress,
                                                      ClientId = authConfiguration.ClientId,
                                                      ClientSecret = authConfiguration.ClientSecret,
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
                   options.Authority = authConfiguration.Authority;
                   options.ClientId = authConfiguration.ClientId;
                   options.ClientSecret = authConfiguration.ClientSecret;

                   // 如果要改 redirect url 的時候要用這個
                   // 預設是 /signin-oidc
                   // options.CallbackPath = "/auth-redirect-url";

                   options.RequireHttpsMetadata = authConfiguration.RequireHttpsMetadata;
                   options.ResponseType = authConfiguration.ResponseType;
                   // options.ResponseType = OpenIdConnectResponseType.Code;
                   // 預設就是 form post，目前沒有碰到需要改變的情境 (identity server 4 / keycloak / microsoft entra id 都用 form post)
                   // options.ResponseMode = OpenIdConnectResponseMode.FormPost;

                   // 沒有清除的話，預設 scope 裡面有一個 profile 的項目，因為預期所有的 scope 都統一由設定檔給予，所以這邊要先清掉
                   options.Scope.Clear();

                   //要讓 .net 預設的 openid 認證成功需要這個 Scope
                   // 應該全部的 scope 都由設定檔控制
                   // options.Scope.Add(OpenIdConnectScope.OpenId);

                   // 從設定檔中取得 OAuth Scope
                   foreach (var item in authConfiguration.WebApiAudience)
                   {
                       options.Scope.Add(item);
                   }

                   // 跟 OAuth Server 要 Refresh Token
                   // 2025-04-22: 應該全部的 scope 都由設定檔控制
                   // options.Scope.Add(OpenIdConnectScope.OfflineAccess);

                   // if true, cookies ExpiresUtc will be use id_token expires time
                   // options.UseTokenLifetime = true;

                   options.SaveTokens = true;

                   // 如果採用 microsoft entra id 這邊就必須設定為 false，不然會因為另外使用 api 去跟 MS Graph 調用使用者資料
                   // 導致 scope 需要增加 "User.Read" 才能正常通過 auth，並且拿到的 access token 中的 audience 會變成 MS Graph 的 audience
                   options.GetClaimsFromUserInfoEndpoint = false;

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
    /// <param name="gatewayAuthConfiguration"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddYarpAuthentication(this IServiceCollection serviceCollection, GatewayAuthConfiguration? gatewayAuthConfiguration)
    {
        ArgumentNullException.ThrowIfNull(gatewayAuthConfiguration);

        var authenticationSchemeCount =
            (gatewayAuthConfiguration.Jwt is null ? 0 : 1) +
            (gatewayAuthConfiguration.MySSO is null ? 0 : 1) +
            (gatewayAuthConfiguration.ExternalToken is null ? 0 : 1);
        var usePolicyScheme = authenticationSchemeCount > 1;

        AuthenticationBuilder authenticationBuilder;
        switch (gatewayAuthConfiguration.Default)
        {
            case DefaultAuthMethod.Anonymous:
                authenticationBuilder = serviceCollection.AddAuthentication();
                break;

            case DefaultAuthMethod.Opid:
                ArgumentNullException.ThrowIfNull(gatewayAuthConfiguration.Opid);

                authenticationBuilder = serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                });
                break;
            case DefaultAuthMethod.Jwt:
                ArgumentNullException.ThrowIfNull(gatewayAuthConfiguration.Jwt);

                authenticationBuilder = serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultScheme = usePolicyScheme
                                                ? MySsoAuthenticationDefaults.PolicyScheme
                                                : JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = options.DefaultScheme;
                });
                break;
            case DefaultAuthMethod.MySSO:
                ArgumentNullException.ThrowIfNull(gatewayAuthConfiguration.MySSO);

                authenticationBuilder = serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultScheme = usePolicyScheme
                                                ? MySsoAuthenticationDefaults.PolicyScheme
                                                : MySsoAuthenticationDefaults.SessionScheme;
                    options.DefaultChallengeScheme = options.DefaultScheme;
                });
                break;
            case DefaultAuthMethod.ExternalToken:
                ArgumentNullException.ThrowIfNull(gatewayAuthConfiguration.ExternalToken);

                authenticationBuilder = serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultScheme = usePolicyScheme
                                                ? MySsoAuthenticationDefaults.PolicyScheme
                                                : ExternalTokenAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = options.DefaultScheme;
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (gatewayAuthConfiguration.Opid is not null)
        {
            authenticationBuilder.AddOpenIdConnectWithCookie(gatewayAuthConfiguration.Opid);
        }

        if (gatewayAuthConfiguration.Jwt is not null)
        {
            authenticationBuilder.AddJwtAuthentication(gatewayAuthConfiguration.Jwt);
        }

        if (gatewayAuthConfiguration.MySSO is not null)
        {
            authenticationBuilder.AddMySsoAuthentication(gatewayAuthConfiguration.MySSO);
        }

        if (gatewayAuthConfiguration.ExternalToken is not null)
        {
            authenticationBuilder.AddExternalTokenAuthentication(gatewayAuthConfiguration.ExternalToken);
        }

        if (usePolicyScheme &&
            gatewayAuthConfiguration.Default is DefaultAuthMethod.Jwt or DefaultAuthMethod.MySSO or DefaultAuthMethod.ExternalToken)
        {
            authenticationBuilder.AddPolicyScheme(
                MySsoAuthenticationDefaults.PolicyScheme,
                null,
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                        SelectAuthenticationScheme(
                            context,
                            gatewayAuthConfiguration.Default,
                            gatewayAuthConfiguration);
                });
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

    private static string DefaultScheme(DefaultAuthMethod defaultAuthMethod)
    {
        return defaultAuthMethod switch
        {
            DefaultAuthMethod.Jwt => JwtBearerDefaults.AuthenticationScheme,
            DefaultAuthMethod.MySSO => MySsoAuthenticationDefaults.SessionScheme,
            DefaultAuthMethod.ExternalToken => ExternalTokenAuthenticationDefaults.AuthenticationScheme,
            _ => throw new ArgumentOutOfRangeException(nameof(defaultAuthMethod), defaultAuthMethod, null)
        };
    }

    private static string SelectAuthenticationScheme(
        HttpContext context,
        DefaultAuthMethod defaultAuthMethod,
        GatewayAuthConfiguration gatewayAuthConfiguration)
    {
        var authorization = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authorization))
        {
            var externalToken = gatewayAuthConfiguration.ExternalToken;
            if (externalToken is not null &&
                (authorization.StartsWith($"{externalToken.TokenScheme} ", StringComparison.OrdinalIgnoreCase) ||
                 authorization.StartsWith($"{externalToken.KeyScheme} ", StringComparison.OrdinalIgnoreCase)))
            {
                return ExternalTokenAuthenticationDefaults.AuthenticationScheme;
            }

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                if (gatewayAuthConfiguration.Jwt is not null)
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                if (externalToken?.AcceptBearerToken == true)
                {
                    return ExternalTokenAuthenticationDefaults.AuthenticationScheme;
                }
            }
        }

        if (gatewayAuthConfiguration.MySSO is not null &&
            context.Request.Cookies.ContainsKey(gatewayAuthConfiguration.MySSO.SessionCookieName))
        {
            return MySsoAuthenticationDefaults.SessionScheme;
        }

        return DefaultScheme(defaultAuthMethod);
    }
}