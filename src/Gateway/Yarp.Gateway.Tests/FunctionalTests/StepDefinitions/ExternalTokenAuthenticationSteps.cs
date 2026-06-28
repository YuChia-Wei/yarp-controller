using System.Net;
using System.Text.Json;
using Reqnroll;
using Xunit;
using Yarp.Gateway.Tests.FunctionalTests.Support;

namespace Yarp.Gateway.Tests.FunctionalTests.StepDefinitions;

[Binding]
[Scope(Feature = "External token authentication")]
public sealed class ExternalTokenAuthenticationSteps
{
    private readonly FakeExternalTokenAuthenticationClient _authenticationClient = new();
    private GatewayTestApplication? _application;
    private HttpResponseMessage? _response;

    [Given("Gateway 已啟用 ExternalToken authentication")]
    public async Task GivenGatewayHasEnabledExternalTokenAuthentication()
    {
        this._application = await GatewayTestApplication.StartExternalTokenAsync(this._authenticationClient);
    }

    [When(@"使用者以 ExternalToken ""(.*)"" 要求外部驗證路由")]
    public async Task WhenUserRequestsRouteWithExternalToken(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/external-protected");
        request.Headers.TryAddWithoutValidation("Authorization", $"ExternalToken {token}");
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
    }

    [When("使用者以 JWT-shaped Bearer token 要求外部驗證路由")]
    public async Task WhenUserRequestsRouteWithJwtShapedBearerToken()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/external-protected");
        request.Headers.TryAddWithoutValidation("Authorization", "Bearer header.payload.signature");
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
    }

    [Then(@"HTTP 狀態碼應為 (\d+)")]
    public void ThenStatusCodeShouldBe(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, this.Response.StatusCode);
    }

    [Then(@"回應應包含外部使用者 id ""(.*)""")]
    public async Task ThenResponseShouldContainExternalUserId(string expectedUserId)
    {
        var content = await this.Response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        Assert.Equal(expectedUserId, json.RootElement.GetProperty("userId").GetString());
    }

    [Then(@"外部驗證服務應收到 token ""(.*)""")]
    public void ThenExternalServiceShouldReceiveToken(string expectedToken)
    {
        Assert.Equal(1, this._authenticationClient.ValidationCallCount);
        Assert.Equal(expectedToken, this._authenticationClient.LastValidatedToken);
    }

    [Then("外部驗證服務不應被呼叫")]
    public void ThenExternalServiceShouldNotBeCalled()
    {
        Assert.Equal(0, this._authenticationClient.ValidationCallCount);
    }

    [AfterScenario]
    public async Task DisposeApplicationAsync()
    {
        this._response?.Dispose();
        if (this._application is not null)
        {
            await this._application.DisposeAsync();
        }
    }

    private GatewayTestApplication Application =>
        this._application ?? throw new InvalidOperationException("Gateway test application has not been started.");

    private HttpResponseMessage Response =>
        this._response ?? throw new InvalidOperationException("No HTTP response is available.");

    private async Task SetResponseAsync(Task<HttpResponseMessage> responseTask)
    {
        var response = await responseTask;
        this._response?.Dispose();
        this._response = response;
    }
}