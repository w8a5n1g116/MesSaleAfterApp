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
            //string body = "{\"button\":[{\"name\":\"售后\",\"sub_button\":[{\"type\": \"view\",\"name\": \"客户报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/Index&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"员工报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/Dispatch&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"售后计划\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/WorkList&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"评价统计\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/AssessmentCount&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"说明书查询\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/GetPDF&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"}]}" + "]}";

            string body =
                "{\"button\":["
                + "{\"name\":\"关于我们\",\"sub_button\":["
                    + "{\"type\":\"view\",\"name\":\"公司概况\",\"url\":\"http://kocelkma.com/\"},"
                    + "{\"type\":\"view\",\"name\":\"公司新闻\",\"url\":\"http://kocelkma.com/h-col-127.html\"}"
                    + "]},"
                + "{\"name\":\"售后\",\"sub_button\":[{\"type\": \"view\",\"name\": \"客户报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/Index&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"员工报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/Dispatch&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"售后计划\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/WorkList&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"评价统计\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/AssessmentCount&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
                + "{\"type\": \"view\",\"name\": \"说明书查询\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc15f96471ac8980b&redirect_uri=http://kocel.stopno.net/Home/GetPDF&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"}]},"
                + "{\"type\":\"view\",\"name\":\"联系我们\",\"url\":\"http://kocelkma.com/h-col-106.html\"}"
                + "]}";

            //string body = 
            //    "{\"button\":["
            //    + "{\"name\":\"关于我们\",\"sub_button\":["
            //        + "{\"type\":\"view\",\"name\":\"公司概况\",\"url\":\"http://kocelkma.com/\"},"
            //        + "{\"type\":\"view\",\"name\":\"公司新闻\",\"url\":\"http://kocelkma.com/h-col-127.html\"}"
            //        + "]},"
            //    + "{\"name\":\"售后\",\"sub_button\":[{\"type\": \"view\",\"name\": \"客户报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx913c75d440c96c98&redirect_uri=http://stopno.free.idcfengye.com/Home/Index&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"员工报修\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx913c75d440c96c98&redirect_uri=http://stopno.free.idcfengye.com/Home/Dispatch&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"售后计划\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx913c75d440c96c98&redirect_uri=http://stopno.free.idcfengye.com/Home/WorkList&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"评价统计\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx913c75d440c96c98&redirect_uri=http://stopno.free.idcfengye.com/Home/AssessmentCount&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"},"
            //    + "{\"type\": \"view\",\"name\": \"说明书查询\",\"url\": \"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx913c75d440c96c98&redirect_uri=http://stopno.free.idcfengye.com/Home/GetPDF&response_type=code&scope=snsapi_base&state=0#wechat_redirect\"}]},"
            //    + "{\"type\":\"view\",\"name\":\"联系我们\",\"url\":\"http://kocelkma.com/h-col-106.html\"}"
            //    + "]}";


            WeChatCommon wcc = new WeChatCommon();
            wcc.CreateCustomerMenu(body);
        }
    }
}
