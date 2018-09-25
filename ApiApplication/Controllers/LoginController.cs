using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiApplication.AuthHelper;
using ApiApplication.AuthHelper.OverWrite;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        [Route("Token")]
        public IActionResult GetJWTStr(long id = 1, string sub = "Admin", int expiresSliding = 30, int expiresAbsoulute = 30)
        {
            TokenModel tokenModel = new TokenModel();
            tokenModel.Uid = id;
            tokenModel.Sub = sub;
            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMinutes(expiresSliding);
            DateTime d3 = d1.AddDays(expiresAbsoulute);

            TimeSpan sliding = d2 - d1;
            TimeSpan absoulute = d3 - d1;

            string jwtStr = MyToken.IssueJWT(tokenModel, sliding, absoulute);

            return Ok(jwtStr);
        }

        /// <summary>
        /// 获取JWT的重写方法，推荐这种，注意在文件夹OverWrite下
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="sub">角色</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Token2")]
        public IActionResult GetJWTStr2(long id = 1, string sub = "Admin")
        {
            //这里就是用户登陆以后，通过数据库去调取数据，分配权限的操作
            TokenModelJWT tokenModel = new TokenModelJWT();
            tokenModel.Uid = id;
            tokenModel.Role = sub;

            string jwtStr = JwtHelper.IssueJWT(tokenModel);
            return Ok(jwtStr);
        }
    }
}