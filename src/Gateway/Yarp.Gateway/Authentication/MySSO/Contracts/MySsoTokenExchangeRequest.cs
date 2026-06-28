namespace Yarp.Gateway.Authentication.MySSO.Contracts;

/// <summary>
/// Gateway 向平台 Auth Server 送出 MySSO 單次 token 的交換要求。
/// </summary>
/// <param name="AppId">Gateway 在 MySSO 平台使用的應用程式識別碼。</param>
/// <param name="Token">MySSO form POST callback 帶回的單次 token。</param>
public sealed record MySsoTokenExchangeRequest(string? AppId, string Token);