using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WeChatTools;

namespace MesSaleAfterApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //启动程序时创建公众号自定义菜单
            string body = "{\"button\":[{\"name\":\"售后\",\"sub_button\":[{\"type\": \"view\",\"name\": \"客户报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/Index&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"员工报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/Dispatch&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"售后计划\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/WorkList&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"评价统计\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/AssessmentCount&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"说明书查询\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/GetPDF&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"}]}" + "]}";

            WeChatCommon wcc = new WeChatCommon();
            wcc.CreateCustomerMenu(body);
        }
    }
}
