using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisShakeHand.Services.Interfaces
{
    public interface IRedisHashService
    {
        /// <summary>
        /// 將 Cachekey 存放到 Hash 裡
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="timeSpan">The time span.</param>
        void HashSet(string key, TimeSpan timeSpan);

        /// <summary>
        /// 將 Cachekey 從 Hash 裡移除
        /// </summary>
        /// <param name="caccheKey">The cacche key.</param>
        void HashDelete(string caccheKey);

        /// <summary>
        /// 移除指定快取資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveCacheItem(string key);

        /// <summary>
        /// 取得 Hash 內的 Cachekey 總數
        /// </summary>
        /// <returns></returns>
        long HashLength();

        /// <summary>
        ///清除 Hash 裡以超過存續時間的 Cachekey.
        /// </summary>
        /// <returns></returns>
        int HashClean();

        /// <summary>
        /// 清除所有的 Cache 資料
        /// </summary>
        void FlushDatabase();

        /// <summary>
        /// Hashes the exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool HashExists(string key);
    }
}