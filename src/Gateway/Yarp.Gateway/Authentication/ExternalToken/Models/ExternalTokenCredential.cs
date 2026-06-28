namespace Yarp.Gateway.Authentication.ExternalToken.Models;

/// <summary>
/// 從要求中解析出的外部驗證憑證。
/// </summary>
/// <param name="Kind">外部驗證憑證的型態。</param>
/// <param name="Value">實際要交給外部服務驗證或交換的值。</param>
internal sealed record ExternalTokenCredential(ExternalTokenCredentialKind Kind, string Value);