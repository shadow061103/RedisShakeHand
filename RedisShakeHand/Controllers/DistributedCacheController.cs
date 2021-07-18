using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisShakeHand.Controllers
{
    /// <summary>
    /// 分散式快取
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase"/>
    [ApiController]
    [Route("[controller]/[action]")]
    public class DistributedCacheController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;

        private readonly IWeatherForecastService _weatherForecastService;

        private readonly ICacheService _cacheService;

        public DistributedCacheController(IDistributedCache distributedCache,
            IWeatherForecastService weatherForecastService,
            ICacheService cacheService)
        {
            _distributedCache = distributedCache;
            _weatherForecastService = weatherForecastService;
            _cacheService = cacheService;
        }

        //distributedCache的實作是用lua script
        //會幫忙建data,逾期時間...
        //https://github.com/aspnet/Caching/blob/master/src/Microsoft.Extensions.Caching.StackExchangeRedis/RedisCache.cs

        /// <summary>
        /// 分散式快取設定值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetJsonCache()
        {
            var cacheKey = "WeatherForecast";

            var cacheValues = _distributedCache.Get(cacheKey);

            //轉成json儲存
            if (cacheValues != null)
            {
                string json = Encoding.UTF8.GetString(cacheValues);
                var weather = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
                return Ok(weather);
            }
            else
            {
                //set cache
                var weather = _weatherForecastService.Get();
                var json = JsonSerializer.Serialize(weather);
                var redisModel = Encoding.UTF8.GetBytes(json);
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _distributedCache.Set(cacheKey, redisModel, options);

                return Ok(weather);
            }
        }

        /// <summary>
        /// 分散式快取設定值(MsgPack)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetMsgPackCache()
        {
            var cacheKey = "WeatherForecast:msgpack";

            var exists = _cacheService.Exists(cacheKey);

            if (exists)
            {
                var weather = _cacheService.GetCollection<WeatherForecast>(cacheKey);
                return Ok(weather);
            }
            else
            {
                var weather = _weatherForecastService.Get();
                _cacheService.SaveAbsoluteCollection(cacheKey, weather.ToList(), TimeSpan.FromMinutes(5));
                return Ok(weather);
            }
        }
    }
}