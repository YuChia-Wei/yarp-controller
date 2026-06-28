namespace Yarp.Gateway.Authentication.ExternalToken.Contracts;

/// <summary>
/// 向外部服務驗證既有 token 的要求內容。
/// </summary>
/// <param name="AppId">呼叫外部服務的應用程式識別碼。</param>
/// <param name="Token">由 Authorization header 帶入、需要驗證的 token。</param>
public sealed record ExternalTokenValidationRequest(string? AppId, string Token);