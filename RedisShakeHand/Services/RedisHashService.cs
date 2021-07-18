using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using RedisShakeHand.Services.Interfaces;
using StackExchange.Redis;

namespace RedisShakeHand.Services
{
    /// <summary>
    /// Redis Hash的用法
    /// </summary>
    public class RedisHashService : IRedisHashService
    {
        private IConnectionMultiplexer _connection;

        private RedisCacheOptions _redisCacheOptions;

        private IDatabase Database => _connection.GetDatabase();

        public RedisHashService(IConnectionMultiplexer connection, IOptions<RedisCacheOptions> options)
        {
            _connection = connection;

            _redisCacheOptions = options.Value;
        }

        /// <summary>
        /// 將 Cachekey 存放到 Hash 裡
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="timeSpan">The time span.</param>
        public void HashSet(string key, TimeSpan timeSpan)
        {
            var redisCacheKey = BuildCachekey(key);

            Database.HashSet(
                key: _redisCacheOptions.InstanceName,
                hashField: redisCacheKey,
                value: timeSpan.Ticks,
                when: When.Always,
                flags: CommandFlags.FireAndForget
                );
        }

        /// <summary>
        /// 刪除Hash裡的key
        /// </summary>
        /// <param name="caccheKey">The cacche key.</param>
        public void HashDelete(string caccheKey)
        {
            var redisCachekey = BuildCachekey(caccheKey);

            Database.HashDelete(_redisCacheOptions.InstanceName,
                redisCachekey,
                CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 移除指定快取資料
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool RemoveCacheItem(string key)
        {
            var redisCachekey = this.BuildCachekey(key);

            var result = false;

            var keyExists = this.Database.KeyExists(redisCachekey);

            if (keyExists.Equals(true))
            {
                result = this.Database.KeyDelete(redisCachekey);
            }

            this.HashDelete(redisCachekey);

            return result;
        }

        /// <summary>
        /// Hashes the exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool HashExists(string key)
        {
            var redisCachekey = this.BuildCachekey(key);

            var result = Database.HashExists(
                key: _redisCacheOptions.InstanceName,
                hashField: redisCachekey,
                CommandFlags.None);

            return result;
        }

        /// <summary>
        /// 取得 Hash 內的 Cachekey 總數
        /// </summary>
        /// <returns></returns>
        public long HashLength()
        {
            return Database.HashLength(_redisCacheOptions.InstanceName);
        }

        /// <summary>
        /// 清除 Hash 裡以超過存續時間的 Cachekey.
        /// </summary>
        /// <returns></returns>
        public int HashClean()
        {
            var hashEntries = this.Database.HashGetAll(_redisCacheOptions.InstanceName);
            var cleanCount = 0;
            var currentTicks = DateTime.UtcNow.Ticks;

            foreach (var item in hashEntries)
            {
                if (long.TryParse(item.Value, out var value).Equals(false))
                {
                    continue;
                }

                if (value >= currentTicks)
                {
                    continue;
                }

                var cachekey = $"{item.Name}";
                if (string.IsNullOrWhiteSpace(cachekey))
                {
                    continue;
                }

                var keyExists = this.Database.KeyExists(cachekey);
                if (keyExists.Equals(true))
                {
                    continue;
                }

                this.Database.HashDelete(this._redisCacheOptions.InstanceName, cachekey, CommandFlags.FireAndForget);
                cleanCount++;
            }

            return cleanCount;
        }

        /// <summary>
        ///清除所有的 Cache 資料
        /// </summary>
        public void FlushDatabase()
        {
            var hashEntries = Database.HashGetAll(_redisCacheOptions.InstanceName);
            foreach (var entry in hashEntries)
            {
                RemoveCacheItem(entry.Name);
            }
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