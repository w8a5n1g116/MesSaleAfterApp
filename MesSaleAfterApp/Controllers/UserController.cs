using MesSaleAfterApp.Models;
using MesSaleAfterApp.Models.OrbitModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MesSaleAfterApp.Controllers
{
    public class UserController : ApiController
    {
        OrbitMOMEntity entity = new OrbitMOMEntity();
        public string Get(string userId)
        {
            ReturnModel returnModel = null;

            try
            {
                SysUser user = entity.SysUser.Where(p => p.UserName == userId).FirstOrDefault();

                if (user != null)
                {
                    returnModel = new ReturnModel(user, true);
                }
                else
                {
                    returnModel = new ReturnModel(false, "用户不存在");
                }
            }
            catch (Exception ex)
            {
                returnModel = new ReturnModel(false, "获取用户异常！：" + ex.ToString());
            }

            return JsonConvert.SerializeObject(returnModel);
        }
    }
}
