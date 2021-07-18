using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisShakeHand.Services.Interfaces
{
    public interface ICacheService
    {
        bool SaveSliding<T>(string key, T value, TimeSpan slidingExpiration);

        bool SaveAbsolute<T>(string key, T value, TimeSpan absoluteExpiration);

        bool SaveSlidingCollection<T>(string key, List<T> collection, TimeSpan slidingExpiration);

        bool SaveAbsoluteCollection<T>(string key, List<T> collection, TimeSpan absoluteExpiration);

        IEnumerable<T> GetCollection<T>(string key);

        object Get(string key);

        object Get<T>(string key);

        bool Remove(string key);

        bool Exists(string key);
    }
}