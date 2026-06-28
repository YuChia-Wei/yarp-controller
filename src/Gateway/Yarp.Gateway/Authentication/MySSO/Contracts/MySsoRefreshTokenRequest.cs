namespace Yarp.Gateway.Authentication.MySSO.Contracts;

/// <summary>
/// Gateway 向平台 Auth Server 以 refresh token 換發新平台權杖的續期要求。
/// </summary>
/// <param name="AppId">Gateway 在 MySSO 平台使用的應用程式識別碼。</param>
/// <param name="RefreshToken">先前由平台 Auth Server 簽發並保存在 session 中的 refresh token。</param>
public sealed record MySsoRefreshTokenRequest(string? AppId, string RefreshToken);
