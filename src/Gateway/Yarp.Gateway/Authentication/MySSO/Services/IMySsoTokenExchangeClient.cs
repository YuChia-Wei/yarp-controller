using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;

namespace Yarp.Gateway.Authentication.MySSO.Services;

/// <summary>
/// 將 MySSO 單次 token 送往平台 Auth Server 並取得平台權杖的用戶端。
/// </summary>
public interface IMySsoTokenExchangeClient
{
    /// <summary>
    /// 交換 MySSO 單次 token。
    /// </summary>
    /// <param name="token">MySSO form POST callback 帶回的單次 token。</param>
    /// <param name="options">MySSO remote authentication 執行期設定。</param>
    /// <param name="cancellationToken">取消 backchannel request 的權杖。</param>
    /// <returns>平台 Auth Server 回傳的 token exchange 結果。</returns>
    Task<MySsoTokenExchangeResult> ExchangeAsync(
        string token,
        MySsoAuthenticationOptions options,
        CancellationToken cancellationToken);

    /// <summary>
    /// 以 refresh token 向平台 Auth Server 換發新的平台權杖。
    /// </summary>
    /// <param name="refreshToken">先前保存在 session 中的 refresh token。</param>
    /// <param name="options">MySSO remote authentication 執行期設定。</param>
    /// <param name="cancellationToken">取消 backchannel request 的權杖。</param>
    /// <returns>平台 Auth Server 回傳的續期結果。</returns>
    Task<MySsoTokenExchangeResult> RefreshAsync(
        string refreshToken,
        MySsoAuthenticationOptions options,
        CancellationToken cancellationToken);
}