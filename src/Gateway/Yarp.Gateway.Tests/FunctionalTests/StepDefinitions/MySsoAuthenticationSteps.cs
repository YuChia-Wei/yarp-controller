using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Reqnroll;
using Xunit;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Tests.FunctionalTests.Support;

namespace Yarp.Gateway.Tests.FunctionalTests.StepDefinitions;

[Binding]
[Scope(Feature = "MySSO authentication")]
public sealed class MySsoAuthenticationSteps
{
    private readonly FakeMySsoTokenExchangeClient _tokenExchangeClient = new();
    private GatewayTestApplication? _application;
    private HttpResponseMessage? _response;
    private string? _correlationCookie;
    private string? _state;
    private string? _sessionCookie;

    [Given("Gateway 已啟用 MySSO authentication")]
    public async Task GivenGatewayHasEnabledMySsoAuthentication()
    {
        this._application = await GatewayTestApplication.StartMySsoAsync(this._tokenExchangeClient);
    }

    [When("未登入使用者要求一般受保護路由")]
    public async Task WhenAnonymousUserRequestsProtectedRoute()
    {
        await this.SetResponseAsync(this.Application.Client.GetAsync("/protected"));
    }

    [When("使用者以無效 MySSO session Cookie 要求一般受保護路由")]
    public async Task WhenUserRequestsProtectedRouteWithInvalidSessionCookie()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/protected");
        request.Headers.TryAddWithoutValidation("Cookie", $"{MySsoAuthenticationDefaults.SessionCookieName}=invalid-ticket");
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
    }

    [When("未登入使用者要求互動式路由")]
    public async Task WhenAnonymousUserRequestsInteractiveRoute()
    {
        await this.RequestInteractiveRouteAsync();
    }

    [When("瀏覽器以有效單次 token 提交 MySSO form-post callback")]
    public async Task WhenBrowserPostsValidMySsoCallback()
    {
        await this.SubmitValidCallbackAsync();
    }

    [Given("使用者已完成 MySSO 登入")]
    public async Task GivenUserHasCompletedMySsoLogin()
    {
        await this.RequestInteractiveRouteAsync();
        await this.SubmitValidCallbackAsync();
        this.CaptureSessionCookie();
    }

    [When("瀏覽器使用 MySSO session Cookie 要求一般受保護路由")]
    public async Task WhenBrowserRequestsProtectedRouteWithSessionCookie()
    {
        using var request = this.CreateSessionRequest(HttpMethod.Get, "/protected");
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
    }

    [When("瀏覽器查詢 MySSO session")]
    public async Task WhenBrowserQueriesMySsoSession()
    {
        using var request = this.CreateSessionRequest(HttpMethod.Get, MySsoAuthenticationDefaults.SessionPath);
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
    }

    [When(@"來源 ""(.*)"" 的瀏覽器查詢 MySSO session")]
    public async Task WhenBrowserFromOriginQueriesMySsoSession(string origin)
    {
        using var request = this.CreateSessionRequest(HttpMethod.Get, MySsoAuthenticationDefaults.SessionPath);
        request.Headers.TryAddWithoutValidation("Origin", origin);
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
    }

    [When("瀏覽器要求更新 MySSO session")]
    public async Task WhenBrowserRefreshesMySsoSession()
    {
        using var request = this.CreateSessionRequest(HttpMethod.Post, MySsoAuthenticationDefaults.RefreshPath);
        request.Content = new StringContent(string.Empty);
        await this.SetResponseAsync(this.Application.Client.SendAsync(request));

        if (this.Response.IsSuccessStatusCode)
        {
            this.CaptureSessionCookie();
        }
    }

    [Given("Auth Server 將拒絕 refresh token")]
    public void GivenAuthServerRejectsRefreshToken()
    {
        this._tokenExchangeClient.RefreshResult = MySsoTokenExchangeResult.Fail("Refresh token is invalid.");
    }

    [Then(@"HTTP 狀態碼應為 (\d+)")]
    public void ThenStatusCodeShouldBe(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, this.Response.StatusCode);
    }

    [Then("回應不應包含 MySSO 轉導位置")]
    public void ThenResponseShouldNotContainMySsoRedirect()
    {
        Assert.Null(this.Response.Headers.Location);
    }

    [Then("回應應刪除 MySSO session Cookie")]
    public void ThenResponseShouldDeleteMySsoSessionCookie()
    {
        var setCookie = GetSetCookie(this.Response, MySsoAuthenticationDefaults.SessionCookieName);
        Assert.Contains("expires=Thu, 01 Jan 1970", setCookie, StringComparison.OrdinalIgnoreCase);
    }

    [Then("回應應轉導至 MySSO 並包含 callback 與 state")]
    public void ThenResponseShouldRedirectToMySsoWithCallbackAndState()
    {
        this.CaptureChallenge();

        var location = this.Response.Headers.Location!;
        Assert.Equal("mysso.test", location.Host);

        var query = QueryHelpers.ParseQuery(location.Query);
        Assert.Equal("gateway-tests", query["app_id"]);
        Assert.Equal("https://gateway.test/signin-mysso", query["redirect_uri"]);
        Assert.False(string.IsNullOrWhiteSpace(query["state"]));
    }

    [Then("回應應建立 correlation Cookie")]
    public void ThenResponseShouldCreateCorrelationCookie()
    {
        this.CaptureChallenge();
        Assert.StartsWith(".AspNetCore.Correlation.", this._correlationCookie, StringComparison.Ordinal);
    }

    [Then("回應應建立 MySSO session Cookie")]
    public void ThenResponseShouldCreateMySsoSessionCookie()
    {
        this.CaptureSessionCookie();
        Assert.StartsWith($"{MySsoAuthenticationDefaults.SessionCookieName}=", this._sessionCookie, StringComparison.Ordinal);
    }

    [Then(@"回應應包含 customer id ""(.*)""")]
    public async Task ThenResponseShouldContainCustomerId(string expectedCustomerId)
    {
        using var json = await this.ReadJsonResponseAsync();
        Assert.Equal(expectedCustomerId, json.RootElement.GetProperty("customerId").GetString());
    }

    [Then(@"回應應包含 access token ""(.*)""")]
    public async Task ThenResponseShouldContainAccessToken(string expectedAccessToken)
    {
        using var json = await this.ReadJsonResponseAsync();
        Assert.Equal(expectedAccessToken, json.RootElement.GetProperty("accessToken").GetString());
    }

    [Then(@"session 回應應顯示已驗證及 (\d+) 秒閒置時間")]
    public async Task ThenSessionResponseShouldContainAuthenticatedState(int expectedIdleTimeoutSeconds)
    {
        using var json = await this.ReadJsonResponseAsync();
        Assert.True(json.RootElement.GetProperty("authenticated").GetBoolean());
        Assert.Equal(expectedIdleTimeoutSeconds, json.RootElement.GetProperty("idleTimeoutSeconds").GetInt32());
        Assert.NotEqual(JsonValueKind.Null, json.RootElement.GetProperty("expiresAt").ValueKind);
    }

    [Then("回應應包含 session 到期 header")]
    public void ThenResponseShouldContainSessionExpirationHeader()
    {
        Assert.True(this.Response.Headers.TryGetValues(MySsoAuthenticationDefaults.SessionExpiresHeaderName, out var values));
        Assert.True(DateTimeOffset.TryParse(values.Single(), out _));
    }

    [Then("CORS 回應應允許任意來源")]
    public void ThenCorsResponseShouldAllowAnyOrigin()
    {
        Assert.Equal("*", this.Response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    [Then("CORS 回應應公開 session 到期 header")]
    public void ThenCorsResponseShouldExposeSessionExpirationHeader()
    {
        var exposedHeaders = this.Response.Headers.GetValues("Access-Control-Expose-Headers");
        Assert.Contains(
            MySsoAuthenticationDefaults.SessionExpiresHeaderName,
            exposedHeaders.SelectMany(value => value.Split(',', StringSplitOptions.TrimEntries)));
    }

    [Then(@"Auth Server 應收到既有 refresh token ""(.*)""")]
    public void ThenAuthServerShouldReceiveRefreshToken(string expectedRefreshToken)
    {
        Assert.Equal(1, this._tokenExchangeClient.RefreshCallCount);
        Assert.Equal(expectedRefreshToken, this._tokenExchangeClient.LastRefreshToken);
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

    private async Task RequestInteractiveRouteAsync()
    {
        await this.SetResponseAsync(this.Application.Client.GetAsync("/interactive"));
        this.CaptureChallenge();
    }

    private async Task SubmitValidCallbackAsync()
    {
        this.CaptureChallenge();

        using var request = new HttpRequestMessage(HttpMethod.Post, MySsoAuthenticationDefaults.CallbackPath)
        {
            Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["state"] = this._state!,
                    ["token"] = "one-time-token"
                })
        };
        request.Headers.TryAddWithoutValidation("Cookie", this._correlationCookie);

        await this.SetResponseAsync(this.Application.Client.SendAsync(request));
        Assert.Equal("one-time-token", this._tokenExchangeClient.LastExchangeToken);
    }

    private void CaptureChallenge()
    {
        if (this._state is not null && this._correlationCookie is not null)
        {
            return;
        }

        var location = this.Response.Headers.Location ??
                       throw new InvalidOperationException("MySSO challenge response does not contain a redirect location.");
        var query = QueryHelpers.ParseQuery(location.Query);

        this._state = query["state"].SingleOrDefault();
        this._correlationCookie = GetCookiePair(this.Response, ".AspNetCore.Correlation.");

        Assert.False(string.IsNullOrWhiteSpace(this._state));
    }

    private void CaptureSessionCookie()
    {
        this._sessionCookie = GetCookiePair(this.Response, MySsoAuthenticationDefaults.SessionCookieName);
    }

    private HttpRequestMessage CreateSessionRequest(HttpMethod method, string requestUri)
    {
        if (string.IsNullOrWhiteSpace(this._sessionCookie))
        {
            throw new InvalidOperationException("MySSO session Cookie is not available.");
        }

        var request = new HttpRequestMessage(method, requestUri);
        request.Headers.TryAddWithoutValidation("Cookie", this._sessionCookie);
        return request;
    }

    private async Task<JsonDocument> ReadJsonResponseAsync()
    {
        var content = await this.Response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    private async Task SetResponseAsync(Task<HttpResponseMessage> responseTask)
    {
        var response = await responseTask;
        this._response?.Dispose();
        this._response = response;
    }

    private static string GetCookiePair(HttpResponseMessage response, string cookieNamePrefix)
    {
        var setCookie = response.Headers.TryGetValues("Set-Cookie", out var values)
                            ? values.FirstOrDefault(value => value.StartsWith(cookieNamePrefix, StringComparison.Ordinal))
                            : null;
        if (setCookie is null)
        {
            throw new InvalidOperationException($"Response does not contain Cookie '{cookieNamePrefix}'.");
        }

        return setCookie.Split(';', 2)[0];
    }

    private static string GetSetCookie(HttpResponseMessage response, string cookieName)
    {
        return response.Headers.TryGetValues("Set-Cookie", out var values)
                   ? values.First(value => value.StartsWith($"{cookieName}=", StringComparison.Ordinal))
                   : throw new InvalidOperationException($"Response does not contain Cookie '{cookieName}'.");
    }
}