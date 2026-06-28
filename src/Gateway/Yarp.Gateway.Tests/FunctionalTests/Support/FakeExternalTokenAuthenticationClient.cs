using Yarp.Gateway.Authentication.ExternalToken.Models;
using Yarp.Gateway.Authentication.ExternalToken.Services;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests.FunctionalTests.Support;

internal sealed class FakeExternalTokenAuthenticationClient : IExternalTokenAuthenticationClient
{
    public int ValidationCallCount { get; private set; }

    public string? LastValidatedToken { get; private set; }

    public Task<ExternalTokenAuthenticationResult> ExchangeKeyAsync(
        string key,
        ExternalTokenAuthenticationConfiguration options,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(ExternalTokenAuthenticationResult.Fail("External key is invalid."));
    }

    public Task<ExternalTokenAuthenticationResult> ValidateTokenAsync(
        string token,
        ExternalTokenAuthenticationConfiguration options,
        CancellationToken cancellationToken)
    {
        this.ValidationCallCount++;
        this.LastValidatedToken = token;

        return Task.FromResult(
            token == "valid-external-token"
                ? new ExternalTokenAuthenticationResult
                {
                    Success = true,
                    Id = "external-user-001",
                    Name = "外部驗證使用者",
                    Roles = ["Gateway-User"]
                }
                : ExternalTokenAuthenticationResult.Fail("External token is invalid."));
    }
}