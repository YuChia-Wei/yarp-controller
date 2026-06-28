namespace Yarp.Gateway.Authentication.Options;

public enum DefaultAuthMethod
{
    /// <summary>
    /// 不啟用預設驗證流程。
    /// </summary>
    Anonymous = 0,

    /// <summary>
    /// 使用 OpenID Connect 與 Cookie 作為預設驗證流程。
    /// </summary>
    Opid = 1,

    /// <summary>
    /// 使用 JWT Bearer 作為預設驗證流程。
    /// </summary>
    Jwt = 2,

    /// <summary>
    /// 使用 MySSO 作為預設驗證流程。
    /// </summary>
    MySSO = 3,

    /// <summary>
    /// 使用外部 Token 驗證服務作為預設驗證流程。
    /// </summary>
    ExternalToken = 4
}