using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.WebApi.Models.Cluster;

/// <summary>
/// Session affinity options.
/// </summary>
public class SessionAffinity
{
    /// <summary>
    /// Indicates whether session affinity is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// The session affinity policy to use.
    /// </summary>
    public SessionAffinityPolicy? Policy { get; set; }

    /// <summary>
    /// Strategy handling missing destination for an affinitized request.
    /// </summary>
    public SessionAffinityFailurePolicy? FailurePolicy { get; set; } = SessionAffinityFailurePolicy.Redistribute;

    /// <summary>
    /// Identifies the name of the field where the affinity value is stored.
    /// For the cookie affinity policy this will be the cookie name.
    /// For the header affinity policy this will be the header name.
    /// The policy will give its own default if no value is set.
    /// This value should be unique across clusters to avoid affinity conflicts.
    /// https://github.com/microsoft/reverse-proxy/issues/976
    /// This field is required.
    /// </summary>
    public string? AffinityKeyName { get; set; }

    /// <summary>
    /// Configuration of a cookie storing the session affinity key in case
    /// the <see cref="Policy" /> is set to 'Cookie'.
    /// </summary>
    public CookieConfig? Cookie { get; set; }

    internal SessionAffinityDto MapToDto()
    {
        return new SessionAffinityDto
        {
            Enabled = this.Enabled ?? null,
            Policy = this.Policy,
            FailurePolicy = this.FailurePolicy,
            AffinityKeyName = this.AffinityKeyName!,
            Cookie = this.Cookie != null
                         ? new CookieDto
                         {
                             Path = this.Cookie.Path,
                             Domain = this.Cookie.Domain,
                             HttpOnly = this.Cookie.HttpOnly,
                             SecurePolicy = this.Cookie.SecurePolicy,
                             SameSite = this.Cookie.SameSite,
                             Expiration = this.Cookie.Expiration,
                             MaxAge = this.Cookie.MaxAge,
                             IsEssential = this.Cookie.IsEssential
                         }
                         : null
        };
    }
}