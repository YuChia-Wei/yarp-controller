namespace Yarp.ReverseProxy.ControlPlant.Entity.Enums.Router;

/// <summary>
/// 因為原生 HttpMethod 型別主要用於 HttpClient 等地方使用，而非資料配對，這邊建立專用 enum 來處理 api 傳入資料的配對。
/// MEMO: 因為 Yarp 在處理 HttpMethod 字串時使用 CaseInsensitive 做法，所以這邊的命名可使用大駝峰
/// </summary>
public enum HttpRequestMethod
{
    Connect,
    Delete,
    Get,
    Head,
    Method,
    Options,
    Patch,
    Post,
    Put,
    Trace
}