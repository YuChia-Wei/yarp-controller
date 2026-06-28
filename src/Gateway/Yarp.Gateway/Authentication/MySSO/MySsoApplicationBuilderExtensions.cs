using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.MySSO.Services;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication.MySSO;

/// <summary>
/// 註冊 MySSO 登入、session 查詢與權杖更新 HTTP pipeline 的擴充方法。
/// </summary>
public static class MySsoApplicationBuilderExtensions
{
    /// <summary>
    /// 加入 MySSO session 到期 header middleware，以及登入、session 查詢與權杖更新端點。
    /// </summary>
    /// <param name="app">Gateway WebApplication。</param>
    /// <param name="mySso">MySSO authentication 組態。</param>
    /// <returns>原始 Gateway WebApplication。</returns>
    public static WebApplication UseMySsoAuthentication(
        this WebApplication app,
        MySsoAuthenticationConfiguration mySso)
    {
        // 將 sliding 續期後的 session 到期時間，透過回應 header 回傳給前端校正倒數。
        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                if (context.Items.TryGetValue(MySsoAuthenticationDefaults.SessionExpiresItemKey, out var value) &&
                    value is DateTimeOffset expiresAt)
                {
                    context.Response.Headers[mySso.SessionExpiresHeaderName] = expiresAt.ToString("O");
                }

                return Task.CompletedTask;
            });

            await next(context);
        });

        app.MapGet(
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
            });

        // 前端查詢登入狀態與 session 到期時間；同時觸發 sliding 續期並帶上到期 header。
        app.MapGet(
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
            });

        // 前端主動以 refresh token 換發新平台權杖並延長 session；失敗則清除 session 由前端重新登入。
        app.MapPost(
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
                await context.SignInAsync(MySsoAuthenticationDefaults.SessionScheme, result.Principal, result.Properties);

                if (result.Properties.ExpiresUtc is { } refreshedExpiresAt)
                {
                    context.Items[MySsoAuthenticationDefaults.SessionExpiresItemKey] = refreshedExpiresAt;
                }

                return Results.Ok(new
                {
                    expiresAt = result.Properties.ExpiresUtc
                });
            });

        return app;
    }

    /// <summary>
    /// 判斷登入完成後的 return URL 是否為本機相對路徑。
    /// </summary>
    private static bool IsLocalReturnUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) &&
               returnUrl[0] == '/' &&
               (returnUrl.Length == 1 || returnUrl[1] is not '/' and not '\\');
    }
}