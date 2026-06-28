using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.MySSO.Services;

namespace Yarp.Gateway.Tests.FunctionalTests.Support;

internal sealed class FakeMySsoTokenExchangeClient : IMySsoTokenExchangeClient
{
    public MySsoTokenExchangeResult ExchangeResult { get; set; } = new()
    {
        Success = true,
        CustomerId = "customer-001",
        Name = "測試使用者",
        Roles = ["Gateway-User"],
        AccessToken = "access-token-initial",
        RefreshToken = "refresh-token-initial",
        TokenType = "Bearer",
        ExpiresIn = 300,
        RefreshTokenExpiresIn = 3600
    };

    public MySsoTokenExchangeResult RefreshResult { get; set; } = new()
    {
        Success = true,
        CustomerId = "customer-001",
        Name = "測試使用者",
        Roles = ["Gateway-User"],
        AccessToken = "access-token-refreshed",
        RefreshToken = "refresh-token-refreshed",
        TokenType = "Bearer",
        ExpiresIn = 300,
        RefreshTokenExpiresIn = 3600
    };

    public int ExchangeCallCount { get; private set; }

    public int RefreshCallCount { get; private set; }

    public string? LastExchangeToken { get; private set; }

    public string? LastRefreshToken { get; private set; }

    public Task<MySsoTokenExchangeResult> ExchangeAsync(
        string token,
        MySsoAuthenticationOptions options,
        CancellationToken cancellationToken)
    {
        this.ExchangeCallCount++;
        this.LastExchangeToken = token;
        return Task.FromResult(this.ExchangeResult);
    }

    public Task<MySsoTokenExchangeResult> RefreshAsync(
        string refreshToken,
        MySsoAuthenticationOptions options,
        CancellationToken cancellationToken)
    {
        this.RefreshCallCount++;
        this.LastRefreshToken = refreshToken;
        return Task.FromResult(this.RefreshResult);
    }
}