namespace Yarp.Gateway.Authentication.ExternalToken.Contracts;

/// <summary>
/// 向外部服務以 key 交換驗證資料的要求內容。
/// </summary>
/// <param name="AppId">呼叫外部服務的應用程式識別碼。</param>
/// <param name="Key">由用戶端帶入、要交給外部服務交換的 key。</param>
public sealed record ExternalKeyExchangeRequest(string? AppId, string Key);