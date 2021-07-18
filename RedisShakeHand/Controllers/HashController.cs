using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisShakeHand.Services.Interfaces;

namespace RedisShakeHand.Controllers
{
    /// <summary>
    /// Hash類型操作
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase"/>
    [ApiController]
    [Route("[controller]/[action]")]
    public class HashController : ControllerBase
    {
        private readonly IRedisHashService _redisHashService;

        public HashController(IRedisHashService redisHashService)
        {
            _redisHashService = redisHashService;
        }

        /// <summary>
        /// Sets the set.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetSet([FromBody] string key)
        {
            _redisHashService.HashSet(key, TimeSpan.FromMinutes(5));

            return Ok();
        }

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete([FromBody] string key)
        {
            _redisHashService.HashDelete(key);

            return Ok();
        }

        /// <summary>
        /// Hashes the exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult HashExists(string key)
        {
            var res = _redisHashService.HashExists(key);

            return Ok(res);
        }

        /// <summary>
        /// 清除所有Hash資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FlushDB()
        {
            _redisHashService.FlushDatabase();
            return Ok();
        }
    }
}