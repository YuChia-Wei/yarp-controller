using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.MySSO.Services;
using Yarp.Gateway.Authentication.Options;
using Yarp.Gateway.Configuration;

namespace Yarp.Gateway.Authentication.MySSO;

/// <summary>
/// 映射 MySSO 登入、session 查詢與權杖更新端點的擴充方法。
/// </summary>
public static class MySsoEndpointRouteBuilderExtensions
{
    /// <summary>
    /// MySSO 啟用時，映射登入、session 查詢與權杖更新端點。
    /// </summary>
    /// <param name="endpoints">Gateway endpoint route builder。</param>
    /// <param name="gatewayAuthConfiguration">Gateway authentication 組態；可為 <see langword="null"/>。</param>
    /// <returns>原始 Gateway endpoint route builder。</returns>
    public static IEndpointRouteBuilder MapMySsoEndpoints(
        this IEndpointRouteBuilder endpoints,
        GatewayAuthConfiguration? gatewayAuthConfiguration)
    {
        if (gatewayAuthConfiguration?.MySSO is not { } mySso)
        {
            return endpoints;
        }

        endpoints.MapGet(
                     mySso.LoginPath.Value!,
                     (string? returnUrl) =>
                     {
                         var redirectUri = IsLocalReturnUrl(returnUrl)
                                               ? returnUrl!
                                               : "/";
                         return Results.Challenge(
                             new AuthenticationProperties
                             {
                                 RedirectUri = redirectUri
                             },
                             [MySsoAuthenticationDefaults.RemoteScheme]);
                     })
                 .RequireCors(GatewayCorsPolicyNames.AllowMySso);

        // 前端查詢登入狀態與 session 到期時間；同時觸發 sliding 續期並帶上到期 header。
        endpoints.MapGet(
                     mySso.SessionPath.Value!,
                     async (HttpContext context) =>
                     {
                         var result = await context.AuthenticateAsync(MySsoAuthenticationDefaults.SessionScheme);
                         if (!result.Succeeded)
                         {
                             return Results.Unauthorized();
                         }

                         var expiresAt =
                             context.Items.TryGetValue(MySsoAuthenticationDefaults.SessionExpiresItemKey, out var value) &&
                             value is DateTimeOffset itemExpiresAt
                                 ? itemExpiresAt
                                 : result.Properties?.ExpiresUtc;

                         return Results.Ok(new
                         {
                             authenticated = true,
                             expiresAt,
                             idleTimeoutSeconds = (int)TimeSpan.FromMinutes(mySso.SessionIdleTimeoutMinutes).TotalSeconds
                         });
                     })
                 .RequireCors(GatewayCorsPolicyNames.AllowMySso);

        // 前端主動以 refresh token 換發新平台權杖並延長 session；失敗則清除 session 由前端重新登入。
        endpoints.MapPost(
                     mySso.RefreshPath.Value!,
                     async (
                         HttpContext context,
                         IMySsoTokenExchangeClient tokenExchangeClient,
                         IOptionsMonitor<MySsoAuthenticationOptions> optionsMonitor) =>
                     {
                         var result = await context.AuthenticateAsync(MySsoAuthenticationDefaults.SessionScheme);
                         if (!result.Succeeded || result.Principal is null || result.Properties is null)
                         {
                             await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                             return Results.Unauthorized();
                         }

                         var refreshToken = result.Properties.GetTokenValue(OpenIdConnectParameterNames.RefreshToken);
                         if (string.IsNullOrWhiteSpace(refreshToken))
                         {
                             await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                             return Results.Unauthorized();
                         }

                         var remoteOptions = optionsMonitor.Get(MySsoAuthenticationDefaults.RemoteScheme);

                         MySsoTokenExchangeResult refreshResult;
                         try
                         {
                             refreshResult = await tokenExchangeClient
                                                   .RefreshAsync(refreshToken, remoteOptions, context.RequestAborted)
                                                   .ConfigureAwait(false);
                         }
                         catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
                         {
                             throw;
                         }
                         catch (Exception)
                         {
                             await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                             return Results.Unauthorized();
                         }

                         if (!refreshResult.IsSuccess)
                         {
                             await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                             return Results.Unauthorized();
                         }

                         // 以新權杖覆寫保存的 token，並清除既有到期資訊讓 cookie handler 重新計算到期時間以延長 session。
                         refreshResult.StoreTokens(result.Properties);
                         result.Properties.IssuedUtc = null;
                         result.Properties.ExpiresUtc = null;
                         await context.SignInAsync(
                             MySsoAuthenticationDefaults.SessionScheme,
                             result.Principal,
                             result.Properties);

                         if (result.Properties.ExpiresUtc is { } refreshedExpiresAt)
                         {
                             context.Items[MySsoAuthenticationDefaults.SessionExpiresItemKey] = refreshedExpiresAt;
                             context.Response.Headers[mySso.SessionExpiresHeaderName] = refreshedExpiresAt.ToString("O");
                         }

                         return Results.Ok(new
                         {
                             expiresAt = result.Properties.ExpiresUtc
                         });
                     })
                 .RequireCors(GatewayCorsPolicyNames.AllowMySso);

        return endpoints;
    }

    /// <summary>
    /// 判斷登入完成後的 return URL 是否為本機相對路徑。
    /// </summary>
    /// <param name="returnUrl">登入完成後預計導向的 URL。</param>
    /// <returns>URL 為本機相對路徑時回傳 <see langword="true"/>，否則回傳 <see langword="false"/>。</returns>
    private static bool IsLocalReturnUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) &&
               returnUrl[0] == '/' &&
               (returnUrl.Length == 1 || returnUrl[1] is not '/' and not '\\');
    }
}