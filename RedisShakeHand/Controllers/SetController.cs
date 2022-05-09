using Microsoft.AspNetCore.Mvc;
using RedisShakeHand.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisShakeHand.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SetController : ControllerBase
    {
        private ISetService _setService;

        private string key = "setkey";

        public SetController(ISetService setService)
        {
            _setService = setService;
        }

        [HttpPost]
        public IActionResult SetAdd([FromBody] string value)
        {
            _setService.AddSet(key, value, TimeSpan.FromMinutes(30));
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteSet([FromBody] string value)
        {
            _setService.DeleteSet(key, value);
            return Ok();
        }

        [HttpPost]
        public IActionResult GetCount()
        {
            return Ok(_setService.GetSetCount(key));
        }

        [HttpPost]
        public IActionResult GetMembers()
        {
            return Ok(_setService.GetValues(key));
        }
    }
}