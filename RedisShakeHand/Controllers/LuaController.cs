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
    /// Lua腳本
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase"/>
    [ApiController]
    [Route("[controller]/[action]")]
    public class LuaController : ControllerBase
    {
        /// <summary>
        /// Runs the lua script.
        /// </summary>
        /// <param name="redis">The redis.</param>
        /// <param name="db">The database.</param>
        /// <param name="defaultServer">The default server.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RunLuaScript()
        {
            //Lua debug可以參考 https://jed1978.github.io/2018/05/13/Redis-Programming-CSharp-Basic-2.html

            RedisConnection.Init("127.0.0.1:6379");
            var redis = RedisConnection.Instance.ConnectionMultiplexer;
            var db = redis.GetDatabase();

            var result = (string[])RunLuaScript(redis, db, "127.0.0.1:6379").Result;
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }

            return Ok();
        }

        private async Task<RedisResult> RunLuaScript(ConnectionMultiplexer redis, IDatabase db, string defaultServer)
        {
            //參數要跟lua的同名
            //型別是RedisKey的話，會幫你轉成KEYS[]，否則的話轉成ARGV[]
            string script = System.IO.File.ReadAllText("Lua\\script.lua");
            return await LuaScript
                .Prepare(script)
                .Load(redis.GetServer(defaultServer))
                .EvaluateAsync(db, new { key = (RedisKey)"test:*", value = 5 });
        }
    }
}