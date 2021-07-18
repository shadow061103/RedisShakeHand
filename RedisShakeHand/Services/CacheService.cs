using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using RedisShakeHand.Services.Interfaces;

namespace RedisShakeHand.Services
{
    /// <summary>
    /// DistributedCache操作 預設用Hash型別
    /// </summary>
    /// <seealso cref="RedisShakeHand.Services.Interfaces.ICacheService"/>
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        private readonly MessagePackSerializerOptions options;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            options = ContractlessStandardResolver.Options;
        }

        public bool SaveSliding<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            bool result;

            try
            {
                _distributedCache.Set
                (
                    key: key,
                    value: MessagePackSerializer.Serialize(value, options),
                    options: GetCacheSlideEntryOptions(slidingExpiration)
                );

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = false;
            }
            return result;
        }

        public bool SaveAbsolute<T>(string key, T value, TimeSpan absoluteExpiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            bool result;

            try
            {
                _distributedCache.Set
                (
                    key: key,
                    value: MessagePackSerializer.Serialize(value, options),
                    options: GetCacheAbsoluteEntryOptions(absoluteExpiration)
                );

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = false;
            }

            return result;
        }

        public bool SaveSlidingCollection<T>(string key, List<T> collection, TimeSpan slidingExpiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (collection is null || collection.Any().Equals(false))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            bool result;

            try
            {
                _distributedCache.Set
                (
                    key: key,
                    value: MessagePackSerializer.Serialize(collection, options),
                    options: GetCacheSlideEntryOptions(slidingExpiration)
                );

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = false;
            }

            return result;
        }

        public bool SaveAbsoluteCollection<T>(string key, List<T> collection, TimeSpan absoluteExpiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (collection is null || collection.Any().Equals(false))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            bool result;

            try
            {
                _distributedCache.Set
                (
                    key: key,
                    value: MessagePackSerializer.Serialize(collection, options),
                    options: GetCacheAbsoluteEntryOptions(absoluteExpiration)
                );

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = false;
            }

            return result;
        }

        public IEnumerable<T> GetCollection<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _distributedCache.Get(key);

            var cachedValue = value != null
                ? MessagePackSerializer.Deserialize<IEnumerable<T>>(value, options)
                : Enumerable.Empty<T>();

            return cachedValue;
        }

        public bool TryGetValue(string key, out object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var exists = this.Exists(key);
            if (exists.Equals(false))
            {
                value = null;
                return false;
            }

            value = this.Get(key);

            if (value != null)
            {
                return true;
            }

            value = null;
            return false;
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _distributedCache.Get(key);

            var cacheValue = value != null ? MessagePackSerializer.Deserialize<object>(value, options) : null;

            return cacheValue;
        }

        public object Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _distributedCache.Get(key);

            var cacheValue = value != null ? MessagePackSerializer.Deserialize<T>(value, options) : default;

            return cacheValue;
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (Exists(key).Equals(false))
            {
                return false;
            }

            _distributedCache.Remove(key);

            var res = Exists(key).Equals(false);

            return res;
        }

        public bool Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var cacheValue = _distributedCache.Get(key);

            var res = cacheValue != null;

            return res;
        }

        /// <summary>
        /// 設定絕對過期時間
        /// </summary>
        /// <param name="cacheTime">The cache time.</param>
        /// <returns></returns>
        private DistributedCacheEntryOptions GetCacheAbsoluteEntryOptions(TimeSpan cacheTime)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheTime
            };

            return options;
        }

        /// <summary>
        /// 設定相對過期時間
        /// </summary>
        /// <param name="cacheTime">The cache time.</param>
        /// <returns></returns>
        private DistributedCacheEntryOptions GetCacheSlideEntryOptions(TimeSpan cacheTime)
        {
            //每次讀取值後會重新計算過期時間
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = cacheTime
            };

            return options;
        }

        /// <summary>
        /// 取得預設逾時時間
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        private TimeSpan GetDefaultDuration(TimeSpan? timeSpan)
        {
            return timeSpan.HasValue ? timeSpan.Value : TimeSpan.FromMinutes(5);
        }
    }
}