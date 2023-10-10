using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Yarp.Gateway.Authentication.OpenIdConnect;

/// <summary>
/// Redis Auth Ticket Store
/// ref: https://mikerussellnz.github.io/.NET-Core-Auth-Ticket-Redis/
/// </summary>
public class RedisCacheTicketStore : ITicketStore
{
    private readonly IDistributedCache _distributedCache;
    private readonly string _redisMainKey;

    public RedisCacheTicketStore(string mainKey, string authOptionsCookieTicketStoreRedisServerUrl)
    {
        this._distributedCache = new RedisCache(new RedisCacheOptions { Configuration = authOptionsCookieTicketStoreRedisServerUrl });
        this._redisMainKey = mainKey;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var guid = Guid.NewGuid();
        var key = this._redisMainKey + guid;
        await this.RenewAsync(key, ticket);
        return key;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var options = new DistributedCacheEntryOptions();
        var expiresUtc = ticket.Properties.ExpiresUtc;
        if (expiresUtc.HasValue)
        {
            options.SetAbsoluteExpiration(expiresUtc.Value);
        }

        var val = SerializeToBytes(ticket);
        this._distributedCache.Set(key, val, options);
        return Task.FromResult(0);
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var bytes = this._distributedCache.Get(key);
        var ticket = DeserializeFromBytes(bytes);
        return Task.FromResult(ticket);
    }

    public Task RemoveAsync(string key)
    {
        this._distributedCache.Remove(key);
        return Task.FromResult(0);
    }

    private static AuthenticationTicket? DeserializeFromBytes(byte[]? source)
    {
        return source is null ? null : TicketSerializer.Default.Deserialize(source);
    }

    private static byte[] SerializeToBytes(AuthenticationTicket source)
    {
        return TicketSerializer.Default.Serialize(source);
    }
}