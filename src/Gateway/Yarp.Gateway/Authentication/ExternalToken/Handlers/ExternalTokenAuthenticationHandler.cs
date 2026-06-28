using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Yarp.Gateway.Authentication.ExternalToken.Models;
using Yarp.Gateway.Authentication.ExternalToken.Services;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Authentication.ExternalToken.Handlers;

/// <summary>
/// 從 Authorization header 解析外部 token 或 key，並交由外部服務驗證的處理器。
/// </summary>
internal sealed class ExternalTokenAuthenticationHandler(
    IOptionsMonitor<ExternalTokenAuthenticationConfiguration> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IExternalTokenAuthenticationClient authClient)
    : AuthenticationHandler<ExternalTokenAuthenticationConfiguration>(options, logger, encoder)
{
    /// <summary>
    /// 判斷 Bearer token 是否符合 JWT 三段式格式，避免 fallback 誤處理 JWT。
    /// </summary>
    private static readonly Regex JwtShapeRegex = new(
        "^[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+$",
        RegexOptions.Compiled);

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!this.TryGetCredential(out var credential))
        {
            return AuthenticateResult.NoResult();
        }

        ExternalTokenAuthenticationResult authResult;
        try
        {
            authResult = credential.Kind switch
            {
                ExternalTokenCredentialKind.Token => await authClient.ValidateTokenAsync(
                                                          credential.Value,
                                                          this.Options,
                                                          this.Context.RequestAborted).ConfigureAwait(false),
                ExternalTokenCredentialKind.Key => await authClient.ExchangeKeyAsync(
                                                        credential.Value,
                                                        this.Options,
                                                        this.Context.RequestAborted).ConfigureAwait(false),
                _ => ExternalTokenAuthenticationResult.Fail("Unsupported external authentication credential.")
            };
        }
        catch (OperationCanceledException) when (this.Context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "External token authentication failed.");
            return AuthenticateResult.Fail("External token authentication failed.");
        }

        if (!authResult.IsSuccess)
        {
            return AuthenticateResult.Fail(authResult.ErrorMessage ?? "External token is invalid.");
        }

        return AuthenticateResult.Success(
            new AuthenticationTicket(
                authResult.CreatePrincipal(this.Scheme.Name),
                authResult.CreateAuthenticationProperties(),
                this.Scheme.Name));
    }

    /// <inheritdoc />
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        this.Response.StatusCode = StatusCodes.Status401Unauthorized;
        this.Response.Headers.WWWAuthenticate = this.Options.TokenScheme;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 從 Authorization header 取出指定 scheme 後方的參數值。
    /// </summary>
    private static string? GetAuthorizationParameter(string authorization, string scheme)
    {
        if (!AuthenticationHeaderValue.TryParse(authorization, out var parsedHeader))
        {
            return null;
        }

        return string.Equals(parsedHeader.Scheme, scheme, StringComparison.OrdinalIgnoreCase)
                   ? parsedHeader.Parameter?.Trim()
                   : null;
    }

    /// <summary>
    /// 從目前 HTTP 要求解析可交由外部服務驗證的憑證。
    /// </summary>
    private bool TryGetCredential(out ExternalTokenCredential credential)
    {
        credential = default!;

        if (!this.Request.Headers.TryGetValue(this.Options.AuthorizationHeaderName, out var headerValues))
        {
            return false;
        }

        var authorization = headerValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            return false;
        }

        var token = GetAuthorizationParameter(authorization, this.Options.TokenScheme);
        if (!string.IsNullOrWhiteSpace(token))
        {
            credential = new ExternalTokenCredential(ExternalTokenCredentialKind.Token, token);
            return true;
        }

        var key = GetAuthorizationParameter(authorization, this.Options.KeyScheme);
        if (!string.IsNullOrWhiteSpace(key))
        {
            credential = new ExternalTokenCredential(ExternalTokenCredentialKind.Key, key);
            return true;
        }

        var bearerToken = GetAuthorizationParameter(authorization, "Bearer");
        if (string.IsNullOrWhiteSpace(bearerToken) || !this.Options.AcceptBearerToken)
        {
            return false;
        }

        if (this.Options.IgnoreJwtBearerToken && JwtShapeRegex.IsMatch(bearerToken))
        {
            return false;
        }

        credential = new ExternalTokenCredential(ExternalTokenCredentialKind.Token, bearerToken);
        return true;
    }
}