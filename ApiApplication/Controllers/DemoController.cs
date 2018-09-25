using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiApplication.Common.Helper;
using ApiApplication.IService;
using ApiApplication.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        IAdvertisementService _advertisementService;

        public DemoController(IAdvertisementService advertisementService)
        {
            this._advertisementService = advertisementService;
        }

        [HttpGet("sum")]
        public int Sum()
        {
            //IAdvertisementService advertisementServices = new AdvertisementService();
            return _advertisementService.Sum(2,5);
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var res = await _advertisementService.Query(r => true);
            return Ok(res);
        }

        [HttpGet("list2")]
        public async Task<IActionResult> List2()
        {
            var res = await _advertisementService.GetAdvertisements();
            var strConnection = Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" });
            return Ok(strConnection);
        }
    }
}