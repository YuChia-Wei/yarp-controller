using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.MySSO.Services;

namespace Yarp.Gateway.Authentication.MySSO.Handlers;

/// <summary>
/// 處理 MySSO challenge 與 form-post callback 的 remote authentication handler。
/// </summary>
/// <param name="options">MySSO remote authentication 執行期設定。</param>
/// <param name="logger">驗證處理器使用的記錄器工廠。</param>
/// <param name="encoder">產生 redirect URL 時使用的 URL 編碼器。</param>
/// <param name="tokenExchangeClient">負責向平台 Auth Server 交換 MySSO 單次 token 的用戶端。</param>
internal sealed class MySsoAuthenticationHandler(
    IOptionsMonitor<MySsoAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IMySsoTokenExchangeClient tokenExchangeClient)
    : RemoteAuthenticationHandler<MySsoAuthenticationOptions>(options, logger, encoder)
{
    /// <summary>
    /// 將瀏覽器導向 MySSO 登入頁，並建立受保護的 correlation state。
    /// </summary>
    /// <param name="properties">登入流程使用的驗證屬性與完成後導向位置。</param>
    /// <returns>已完成 challenge response 的工作。</returns>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (string.IsNullOrWhiteSpace(properties.RedirectUri))
        {
            properties.RedirectUri = this.Request.PathBase + this.Request.Path + this.Request.QueryString;
        }

        this.GenerateCorrelationId(properties);

        var state = this.Options.StateDataFormat!.Protect(properties);
        var callbackUri = this.BuildRedirectUri(this.Options.CallbackPath);
        var parameters = this.Options.AdditionalAuthorizationParameters
                             .ToDictionary(item => item.Key, item => (string?)item.Value);
        parameters[this.Options.RedirectUriParameterName] = callbackUri;
        parameters[this.Options.StateParameterName] = state;

        if (!string.IsNullOrWhiteSpace(this.Options.AppId))
        {
            parameters[this.Options.AppIdParameterName] = this.Options.AppId;
        }

        var authorizationUri = QueryHelpers.AddQueryString(
            this.Options.AuthorizationEndpoint!.AbsoluteUri,
            parameters);

        this.Response.Redirect(authorizationUri);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 驗證 MySSO form-post callback，並向平台 Auth Server 交換平台權杖。
    /// </summary>
    /// <returns>MySSO remote authentication 的處理結果。</returns>
    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        string? protectedState;
        string? remoteError;
        string? errorDescription;
        string? token;

        if (HttpMethods.IsPost(this.Request.Method))
        {
            if (!this.Request.HasFormContentType)
            {
                return HandleRequestResult.Fail("MySSO callback content type must be form data.");
            }

            var form = await this.Request.ReadFormAsync(this.Context.RequestAborted).ConfigureAwait(false);
            protectedState = form[this.Options.StateParameterName].FirstOrDefault();
            remoteError = form["error"].FirstOrDefault();
            errorDescription = form["error_description"].FirstOrDefault();
            token = form[this.Options.TokenParameterName].FirstOrDefault();
        }
        else if (HttpMethods.IsGet(this.Request.Method) && this.Options.AllowQueryStringCallback)
        {
            protectedState = this.Request.Query[this.Options.StateParameterName].FirstOrDefault();
            remoteError = this.Request.Query["error"].FirstOrDefault();
            errorDescription = this.Request.Query["error_description"].FirstOrDefault();
            token = this.Request.Query[this.Options.TokenParameterName].FirstOrDefault();
        }
        else
        {
            return HandleRequestResult.Fail("MySSO callback request method is not allowed.");
        }

        if (string.IsNullOrWhiteSpace(protectedState))
        {
            return HandleRequestResult.Fail("MySSO callback does not contain state.");
        }

        var properties = this.Options.StateDataFormat!.Unprotect(protectedState);
        if (properties is null)
        {
            return HandleRequestResult.Fail("MySSO callback state is invalid.");
        }

        if (!this.ValidateCorrelationId(properties))
        {
            return HandleRequestResult.Fail("MySSO correlation validation failed.");
        }

        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            return HandleRequestResult.Fail(errorDescription ?? remoteError);
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return HandleRequestResult.Fail("MySSO callback does not contain a token.");
        }

        MySsoTokenExchangeResult exchangeResult;
        try
        {
            exchangeResult = await tokenExchangeClient
                                   .ExchangeAsync(token, this.Options, this.Context.RequestAborted)
                                   .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (this.Context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "MySSO token exchange failed.");
            return HandleRequestResult.Fail("MySSO token exchange failed.");
        }

        if (!exchangeResult.IsSuccess)
        {
            return HandleRequestResult.Fail(exchangeResult.ErrorMessage ?? "MySSO token exchange failed.");
        }

        ClaimsPrincipal principal;
        try
        {
            principal = exchangeResult.CreatePrincipal(this.Scheme.Name);
        }
        catch (InvalidOperationException ex)
        {
            this.Logger.LogWarning(ex, "MySSO token exchange response is invalid.");
            return HandleRequestResult.Fail(ex);
        }

        if (this.Options.SaveTokens)
        {
            exchangeResult.StoreTokens(properties);
        }

        return HandleRequestResult.Success(new AuthenticationTicket(principal, properties, this.Scheme.Name));
    }
}