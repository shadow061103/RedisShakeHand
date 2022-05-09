using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisShakeHand.Services.Interfaces
{
    public interface ISetService
    {
        void AddSet(string key, string value, TimeSpan timeSpan);

        void DeleteSet(string key, string value);

        long GetSetCount(string key);

        IEnumerable<string> GetValues(string key);
    }
}