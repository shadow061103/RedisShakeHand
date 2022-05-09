using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisShakeHand.Models;
using StackExchange.Redis;

namespace RedisShakeHand.Controllers
{
    /// <summary>
    /// 用DI注入取得Redis實體
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase"/>
    [ApiController]
    [Route("[controller]/[action]")]
    public class RedisController : ControllerBase
    {
        private IConnectionMultiplexer _redis;

        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        #region DI

        /// <summary>
        /// 設定string key(DI模式)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetStringDI()
        {
            var db = _redis.GetDatabase();
            db.StringSet("foo", "success");

            return Ok();
        }

        #endregion DI

        #region Singletion

        //redis連線不能用using，是使用Multiplex處理
        /// <summary>
        /// 設定string key
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetString()
        {
            //singleton模式
            RedisConnection.Init("127.0.0.1:6379");
            var redis = RedisConnection.Instance.ConnectionMultiplexer;

            //一般模式
            //var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");

            //GetDatabase可以設定是否要用非同步模式
            var db = redis.GetDatabase();
            db.StringSet("foo3", 168, TimeSpan.FromSeconds(10), When.NotExists, CommandFlags.DemandMaster);

            return Ok();
        }

        /// <summary>
        /// 測試發佈訂閱
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Publish()
        {
            RedisConnection.Init("127.0.0.1:6379");
            var redis = RedisConnection.Instance.ConnectionMultiplexer;
            var db = redis.GetDatabase();

            var sub = redis.GetSubscriber();

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Subscribed channel topic.test ");

            sub.Subscribe("topic.test", (channel, message) =>
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Received message {message}");
            });

            redis.GetDatabase().Publish("topic.test", "Hello World!");

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Published message to channel topic.test");

            return Ok();
        }

        /// <summary>
        /// 非同步的發佈訂閱
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PublishMsgAsync()
        {
            RedisConnection.Init("127.0.0.1:6379");
            var redis = RedisConnection.Instance.ConnectionMultiplexer;
            var db = redis.GetDatabase();

            var sub = redis.GetSubscriber();

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Subscribed channel topic.test ");

            sub.Subscribe($"topic.test", (channel, message) =>
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Received message {message}");
            });

            //非同物順序問題 預設還是會照同樣順序 但測試目前版本已不會
            //過時redis.PreserveAsyncOrder = false;

            for (int i = 0; i < 10; i++)
            {
                redis.GetDatabase().PublishAsync("topic.test", $"{i} Hello World!");
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} - Published message to channel topic.test");
            }

            return Ok();
        }

        #endregion Singletion
    }
}