namespace Yarp.Gateway.Authentication.ExternalToken.Models;

/// <summary>
/// 外部驗證憑證的型態。
/// </summary>
internal enum ExternalTokenCredentialKind
{
    /// <summary>
    /// 代表憑證值是可直接送往外部服務驗證的 token。
    /// </summary>
    Token,

    /// <summary>
    /// 代表憑證值是要送往外部服務交換驗證資料的 key。
    /// </summary>
    Key
}