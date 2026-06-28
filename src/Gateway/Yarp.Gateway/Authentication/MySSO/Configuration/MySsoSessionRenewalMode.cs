namespace Yarp.Gateway.Authentication.MySSO.Configuration;

/// <summary>
/// MySSO session 在有操作時延長閒置逾時的續期策略。
/// </summary>
public enum MySsoSessionRenewalMode
{
    /// <summary>
    /// 定期延長：距上次續期超過設定的續期間隔秒數才重新簽發，降低 ticket store 寫入。
    /// </summary>
    Periodic = 0,

    /// <summary>
    /// 強制延長：每個已驗證請求都重新簽發並重置閒置逾時。
    /// </summary>
    Always = 1
}
