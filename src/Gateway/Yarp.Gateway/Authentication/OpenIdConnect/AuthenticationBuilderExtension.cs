using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication.OpenIdConnect;

/// <summary>
/// Auth 設定
/// </summary>
public static class AuthenticationBuilderExtension
{
    /// <summary>
    /// MVC / Gateway 或其他需要 opid (OAuth2) 認證時使用
    /// </summary>
    /// <param name="builder">IServiceCollection</param>
    /// <param name="authOptions">OpidAuthOptions</param>
    /// <param name="cookieTicketStoreRedisServerUrl"></param>
    public static AuthenticationBuilder AddOpenIdConnectWithCookie(
        this AuthenticationBuilder builder,
        OpidAuthOptions authOptions,
        string cookieTicketStoreRedisServerUrl)
    {
        var applicationName = Environment.GetEnvironmentVariable("ApplicationName") ?? AppDomain.CurrentDomain.FriendlyName;

        // cookies auth via.https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-7.0
        builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.Cookie.Name = authOptions.CookieLoginName;
                   // options.Cookie.SameSite = SameSiteMode.Lax;
                   options.Cookie.SameSite = authOptions.CookieSameSiteMode;

                   // options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                   options.Cookie.SecurePolicy = authOptions.CookieSecurePolicy;
                   options.Cookie.Domain = authOptions.CookieLoginDomain;

                   options.SessionStore = new RedisCacheTicketStore($"{applicationName}:LoginSession:", cookieTicketStoreRedisServerUrl);

                   options.Events = new CookieAuthenticationEvents
                   {
                       OnValidatePrincipal = async cookieContext =>
                       {
                           // Console.WriteLine("CookieAuthenticationEvents - OnValidatePrincipal");

                           /*
                            * cookieContext.Properties.GetTokenValue(key)
                            * cookieContext.Properties.UpdateTokenValue(key)
                            * key 可使用的參數值有
                            * 1. access_token
                            * 2. id_token       = openId connection 驗證身分所需的 token，預設 5 分鐘過期
                            * 3. refresh_token
                            * 4. token_type
                            * 5. expires_at     = access_token 的到期日
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
                               // Console.WriteLine("CookieAuthenticationEvents - OnValidatePrincipal - refresh");

                               var refreshToken = cookieContext.Properties.GetTokenValue("refresh_token");

                               var httpClient = cookieContext.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
                               var response = await httpClient.RequestRefreshTokenAsync(
                                                  new RefreshTokenRequest
                                                  {
                                                      Address = $"{authOptions.Authority}/connect/token",
                                                      ClientId = authOptions.ClientId,
                                                      ClientSecret = authOptions.ClientSecret,
                                                      RefreshToken = refreshToken
                                                  });

                               if (!response.IsError)
                               {
                                   var expiresInSeconds = response.ExpiresIn;
                                   var updatedExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);

                                   cookieContext.Properties.UpdateTokenValue("expires_at", updatedExpiresAt.ToString());

                                   cookieContext.Properties.UpdateTokenValue("access_token", response.AccessToken);
                                   cookieContext.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);
                                   cookieContext.Properties.UpdateTokenValue("id_token", response.IdentityToken);

                                   // Indicate to the cookie middleware that the cookie should be
                                   // remade (since we have updated it)
                                   cookieContext.ShouldRenew = true;
                               }
                               else
                               {
                                   cookieContext.RejectPrincipal();
                                   await cookieContext.HttpContext.SignOutAsync();
                               }
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

                   options.RequireHttpsMetadata = true;
                   options.ResponseType = OpenIdConnectResponseType.Code;
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
                       var roleElement = context.User.RootElement.GetProperty(JwtClaimTypes.Role);

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

                       return Task.CompletedTask;
                   };

                   // OpenIdConnect 套件預設 PKCE = True
                   // options.UsePkce = false; 
               });
        return builder;
    }
}