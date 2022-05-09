using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using RedisShakeHand.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisShakeHand.Services
{
    public class SetService : ISetService
    {
        private IConnectionMultiplexer _connection;

        private IDatabase Database => _connection.GetDatabase();

        private RedisCacheOptions _redisCacheOptions;

        public SetService(IConnectionMultiplexer connectionMultiplexer,
            IOptions<RedisCacheOptions> options)
        {
            _connection = connectionMultiplexer;
            _redisCacheOptions = options.Value;
        }

        public void AddSet(string key, string value, TimeSpan timeSpan)
        {
            var redisCacheKey = BuildCachekey(key);
            Database.SetAdd(
                key: redisCacheKey,
                value: value,
                CommandFlags.FireAndForget);
        }

        public void DeleteSet(string key, string value)
        {
            var redisCacheKey = BuildCachekey(key);
            Database.SetRemove(redisCacheKey, value);
        }

        public long GetSetCount(string key)
        {
            var redisCacheKey = BuildCachekey(key);
            return Database.SetLength(redisCacheKey);
        }

        public IEnumerable<string> GetValues(string key)
        {
            var redisCacheKey = BuildCachekey(key);
            var result = Database.SetMembers(redisCacheKey);

            var list = new List<string>();
            foreach (var item in result)
            {
                list.Add((string)item);
            }

            return list;
        }

        private string BuildCachekey(string cachekey)
        {
            if (string.IsNullOrWhiteSpace(cachekey))
            {
                throw new ArgumentNullException(nameof(cachekey), $"The value '{nameof(cachekey)}' cannot be null or Empty.");
            }

            var cachekeyPrefix = $"{_redisCacheOptions.InstanceName}::";

            string result;

            if (cachekey.StartsWith(cachekeyPrefix).Equals(true))
            {
                result = cachekey;
                return result;
            }

            if (cachekey.StartsWith("::"))
            {
                result = $"{_redisCacheOptions.InstanceName}{cachekey}";
                return result;
            }

            result = $"{cachekeyPrefix}{cachekey}";

            return result;
        }
    }
}