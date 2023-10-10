namespace Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

public enum SessionAffinityPolicy
{
    HashCookie = 1,
    ArrCookie,
    Cookie,
    CustomHeader
}