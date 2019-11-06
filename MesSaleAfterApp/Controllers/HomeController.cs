using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MesSaleAfterApp.Models.OrbitModel;
using MESWebApp.Tool;
using myDDDev;
using Newtonsoft.Json;
using QRCoder;
using WeChatModels;
using WeChatTools;
using ManagementProject;

namespace MesSaleAfterApp.Controllers
{
    public class HomeController : Controller
    {
        OrbitMOMEntity entity = new OrbitMOMEntity();
        WeChatCommon wcc = new WeChatCommon();



        public ActionResult Dispatch()
        {
            string code = Request.QueryString["code"];

            try
            {
                wx_backdata<wx_oauth2token> oauth2token = wcc.GetOauth2AccessToken(code);

                WXUSerBind wxuser = entity.WXUSerBind.Where(p => p.OpenID == oauth2token.ResponseData.openid).FirstOrDefault();

                if (wxuser == null)
                {
                    return new RedirectResult("/Home/UserBind?OpenID=" + oauth2token.ResponseData.openid);
                }
                else
                {
                    SysUser user = entity.SysUser.Where(p => p.UserId == wxuser.UserID).FirstOrDefault();

                    if(user == null)
                    {
                        ViewBag.Content = "用户不存在！";
                        return View("Error");
                    }

                    return new RedirectResult("/Home/Index?UserID=" + user.UserId + "&UserName=" + user.UserName + "&IsCustomer=No");

                    //WXSaleAfterMain main = entity.WXSaleAfterMain.Where(p => p.UserID == user.UserId && p.IsFinish != "是").FirstOrDefault();

                    //if (main != null && !string.IsNullOrEmpty(main.MoID) && main.IsXJR != "是")
                    //{
                    //    //MO mo = entity.MO.Where(p => p.MOId == main.MoID).FirstOrDefault();

                    //    Ter_Customer_Com_Records tccr = entity.Ter_Customer_Com_Records.Where(p => p.MOName == main.MoID).FirstOrDefault();

                    //    if(tccr != null && tccr.ProblemCompletion == "完成")
                    //    {
                    //        return new RedirectResult("/Home/QRCode?MainID=" + main.ID);
                    //    }
                    //    else
                    //    {
                    //        return new RedirectResult("/Home/WorkNote?MainID=" + main.ID);
                    //    }
                        
                    //}
                    //else
                    //{
                    //    return new RedirectResult("/Home/Index?UserID=" + user.UserId + "&UserName=" + user.UserName + "&IsCustomer=No");
                    //}
                }
            }
            catch (Exception e)
            {
                ViewBag.Content = e.ToString();
                return View("Error");
            }

        }

        public ActionResult WorkList()
        {
            string code = Request.QueryString["code"];


            wx_backdata<wx_oauth2token> oauth2token = wcc.GetOauth2AccessToken(code);

            WXUSerBind wxuser = entity.WXUSerBind.Where(p => p.OpenID == oauth2token.ResponseData.openid).FirstOrDefault();
            //WXUSerBind wxuser = entity.WXUSerBind.Where(p => p.ID == 3).FirstOrDefault();

            if (wxuser == null)
            {
                return new RedirectResult("/Home/UserBind?OpenID=" + oauth2token.ResponseData.openid);
            }
            else
            {
                SysUser user = entity.SysUser.Where(p => p.UserId == wxuser.UserID).FirstOrDefault();

                if (user == null)
                {
                    ViewBag.Content = "用户不存在！";
                    return View("Error");
                }



                List<WXSaleAfterMain> mainList = entity.WXSaleAfterMain.Where(p => p.UserID == user.UserId && p.IsFinish != "是").ToList();

                ViewBag.MainList = mainList;
            }

            return View();
        }
        public ActionResult Menu()
        {
            ViewBag.Title = "目录";

            string timeStamp = DingTalkAuth.GetTimeStamp();
            string nonceStr = "kocel";
            string sigurate = null;
            string jsapi_ticket = getjsapi_ticket();
            string url = Request.Url.ToString();
            DingTalkAuth.GenSigurate(nonceStr, timeStamp, jsapi_ticket, url, ref sigurate);
            ViewBag.timeStamp = timeStamp;
            ViewBag.nonceStr = nonceStr;
            ViewBag.signature = sigurate;

            return View();
        }

        public ActionResult Index(string UserID,string UserName,string IsCustomer)
        {
            ViewBag.Title = "售后录入";

            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

            string accessToken = wcc.Accesstoken();
            string jsapt_tickectString = JSAPI.GetTickect(accessToken);
            wx_js_api_ticket jsapt_ticket = JsonConvert.DeserializeObject<wx_js_api_ticket>(jsapt_tickectString);
            long timestamp = JSAPI.GetTimeStamp();
            string noncestr = "nostop";
            string retstring = null;

            string url = null;
            if(string.IsNullOrEmpty(UserID))
                url = "http://1e556377q9.imwork.net/Home/Index" + "?code=" + code + "&state=" + state;
            else
                url = "http://1e556377q9.imwork.net/Home/Index?UserID=" + UserID + "&UserName=" + UserName + "&IsCustomer=No";
            string signture = JSAPI.GetSignature(jsapt_ticket.ticket, noncestr, timestamp, url, out retstring);

            ViewBag.timestamp = timestamp;
            ViewBag.noncestr = noncestr;
            ViewBag.signture = signture;

            if (IsCustomer != "No")
                IsCustomer = "Yes";
            ViewBag.UserID = UserID;
            ViewBag.UserName = UserName;
            ViewBag.IsCustomer = IsCustomer;
           
            return View();
        }

        [HttpPost]
        public ActionResult Index(WXSaleAfterMain main)
        {

            ObjectParameter ftpparameter = new ObjectParameter("PKID", typeof(string));

            entity.SysGetObjectPKId(null, "FtpFile", ftpparameter);


            if (Request.Files.Count > 0 && !string.IsNullOrEmpty(Request.Files[0].FileName))
            {
                Request.Files[0].SaveAs(Server.MapPath("~/App_Data/") + Request.Files[0].FileName);

                int lastindex = Request.Files[0].FileName.LastIndexOf(".");

                string postfix = Request.Files[0].FileName.Substring(lastindex);

                try
                {
                    FTPHelper.FtpUploadBroken(Server.MapPath("~/App_Data/") + Request.Files[0].FileName, "MesSaleAfter", (string)ftpparameter.Value + postfix);

                    main.Filename = (string)ftpparameter.Value + postfix;
                }
                catch(Exception ex)
                {
                    ViewBag.Content = ex.Message;

                    return View("Done");
                }

                
            }

            entity.WXSaleAfterMain.Add(main);

            entity.SaveChanges();

            if (main.ServiceReason == "故障检查维修" || main.ServiceReason == "安装调试" || main.ServiceReason == "日常回访" || main.ServiceReason == "其他")
            {
                if(main.IsXJR == "否")
                {
                    ObjectParameter parameter = new ObjectParameter("PKID", typeof(string));

                    entity.SysGetObjectPKId(null, "Ter_Customer_Com_Records", parameter);

                    Ter_Customer_Com_Records tccr = new Ter_Customer_Com_Records();

                    tccr.Ter_Customer_Com_RecordsId = (string)parameter.Value;

                    tccr.MOName = main.MoID;

                    if (!string.IsNullOrEmpty(main.MoID))
                    {
                        MO mo = entity.MO.Where(p => p.MOName == main.MoID).FirstOrDefault();

                        //tccr.MOName = mo.MOName;
                        if(mo != null)
                            tccr.MachineNo = mo.SAP_CHARG;
                    }



                    int serialNumber = (int)entity.Ter_Customer_Com_Records.Max(p => p.SerialNumber);

                    tccr.SerialNumber = serialNumber + 1;



                    tccr.ComplaintTime = DateTime.Now;

                    tccr.ComplaintContent = main.ProblemDescription;

                    tccr.CustomerAddress = main.CustomerAddress;

                    tccr.CustomerContact = main.CustomerContact + "," + main.CustomerPhone;

                    tccr.CustomerName = main.CustomerName;

                    tccr.DefectType = main.ProblemType;

                    tccr.CustomerId = main.ID.ToString();

                    tccr.ProblemCompletion = "进行中";

                    tccr.CreateDate = DateTime.Now;

                    tccr.WorkflowStepId = "WFS10000033Z";

                    tccr.WorkflowStatus = "1";



                    if (!string.IsNullOrEmpty(main.Filename))
                    {


                        FtpFile ftpfile = new FtpFile();

                        ftpfile.FtpFileId = (string)ftpparameter.Value;

                        ftpfile.FtpFileName = main.Filename;

                        ftpfile.FtpDirectory = "";

                        ftpfile.FtpFileSize = 0;

                        ftpfile.PluginCommand = "TCCR";

                        ftpfile.UploadUser = main.UserName;

                        ftpfile.UploadComputer = "";

                        ftpfile.CreateDate = DateTime.Now;

                        ftpfile.PKId = (string)parameter.Value;

                        entity.FtpFile.Add(ftpfile);

                        tccr.ImageID = (string)ftpparameter.Value;
                    }

                    entity.Ter_Customer_Com_Records.Add(tccr);
                    entity.SaveChanges();
                }
                else
                {
                    ObjectParameter parameter = new ObjectParameter("PKID", typeof(string));

                    entity.SysGetObjectPKId(null, "Q_issue_Rec_ForCustomer", parameter);

                    Q_issue_Rec_ForCustomer qirf = new Q_issue_Rec_ForCustomer();

                    qirf.Q_issue_Rec_ForCustomerId = (string)parameter.Value;

                    qirf.OrderName = main.MoID;

                    if (!string.IsNullOrEmpty(main.MoID))
                    {
                        MO mo = entity.MO.Where(p => p.MOName == main.MoID).FirstOrDefault();

                        ///qirf.OrderName = mo.MOName;
                        if(mo != null)
                            qirf.MachineNo = mo.SAP_CHARG;                        
                    }

                    qirf.CustomerId = "CUS10000027Y";

                    ProductRoot pr = entity.ProductRoot.Where(p => p.ProductName == main.ProductID).FirstOrDefault();

                    if (pr != null)
                        qirf.ProductID = pr.DefaultProductId;

                    qirf.FeedbackDate = DateTime.Now;

                    qirf.IssueDesc = main.ProblemDescription;

                    if (!string.IsNullOrEmpty(main.Filename))
                    {


                        FtpFile ftpfile = new FtpFile();

                        ftpfile.FtpFileId = (string)ftpparameter.Value;

                        ftpfile.FtpFileName = main.Filename;

                        ftpfile.FtpDirectory = "";

                        ftpfile.FtpFileSize = 0;

                        ftpfile.PluginCommand = "QIRF";

                        ftpfile.UploadUser = main.UserName;

                        ftpfile.UploadComputer = "";

                        ftpfile.CreateDate = DateTime.Now;

                        ftpfile.PKId = (string)parameter.Value;

                        entity.FtpFile.Add(ftpfile);

                        qirf.Image4ID = (string)ftpparameter.Value;
                    }

                    qirf.WorkflowStepId = "WFS10000033X";

                    qirf.WorkflowStatus = "1";

                    entity.Q_issue_Rec_ForCustomer.Add(qirf);
                    entity.SaveChanges();
                }
                
            }

            
            

            ViewBag.Content = "添加售后维护单成功！";

            return View("Done");
        }

        public ActionResult UserBind(string OpenID)
        {
            ViewBag.Title = "用户绑定";
            ViewBag.OpenID = OpenID;

            return View();
        }

        public ActionResult GetPDF()
        {
            ViewBag.Title = "产品说明书查询";

            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

            string accessToken = wcc.Accesstoken();
            string jsapt_tickectString = JSAPI.GetTickect(accessToken);
            wx_js_api_ticket jsapt_ticket = JsonConvert.DeserializeObject<wx_js_api_ticket>(jsapt_tickectString);
            long timestamp = JSAPI.GetTimeStamp();
            string noncestr = "nostop";
            string retstring = null;

            string url = "http://1e556377q9.imwork.net/Home/GetPDF" + "?code=" + code + "&state=" + state;

            string signture = JSAPI.GetSignature(jsapt_ticket.ticket, noncestr, timestamp, url, out retstring);

            ViewBag.timestamp = timestamp;
            ViewBag.noncestr = noncestr;
            ViewBag.signture = signture;

            return View();
        }

        public ActionResult PDF(string productID)
        {
            MemoryStream ms = null;
            try
            {
                ms = FTPHelper.FtpDownloadMemoryStream("InstructionBook/" + productID + ".pdf", false, 0);

                Response.ClearContent();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(ms.ToArray());

                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Content = "未找到产品说明书！";

                return View("Done");
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
            }
            
        }

        [HttpPost]
        public ActionResult UserBind(string UserNo,string OpenID)
        {

            SysUser user = entity.SysUser.Where(p => p.UserName == UserNo).FirstOrDefault();

            if(user == null)
            {
                ViewBag.Content = "为找到对应用户，请检查输入是否正确！";

                return View("Done");
            }

            WXUSerBind wxuser = entity.WXUSerBind.Where(p => p.UserID == user.UserId).FirstOrDefault();

            if(wxuser != null)
            {
                ViewBag.Content = "该用户已绑定，请勿重复绑定！";

                return View("Done");
            }
            else
            {
                wxuser = new WXUSerBind();

                wxuser.OpenID = OpenID;
                wxuser.UserID = user.UserId;

                entity.WXUSerBind.Add(wxuser);

                entity.SaveChanges();
            }

            ViewBag.Content = "已完成绑定！";

            return View("Done");
        }

        public ActionResult WorkNote(string MainID)
        {
            ViewBag.Title = "工作记录";

            ViewBag.MainID = MainID;

            return View();
        }

        [HttpPost]
        public ActionResult WorkNote(WXSaleAfterWorkNote note,string IsDone)
        {
            entity.WXSaleAfterWorkNote.Add(note);

            entity.SaveChanges();

            if(IsDone == "是")
            {
                WXSaleAfterMain main = entity.WXSaleAfterMain.Where(p => p.ID == note.MainID).FirstOrDefault();

                //MO mo = entity.MO.Where(p => p.MOId == main.MoID).FirstOrDefault();

                Ter_Customer_Com_Records tccr = entity.Ter_Customer_Com_Records.Where(p => p.MOName == main.MoID).FirstOrDefault();

                tccr.ProblemCompletion = "完成";

                entity.SaveChanges();

                return new RedirectResult("/Home/QRCode?MainID=" + note.MainID);
            }
            else
            {
                ViewBag.Content = "已记录！";

                return View("Done");
            }           
        }

        public ActionResult Assessment(string MainID)
        {
            ViewBag.Title = "客户评价";

            ViewBag.MainID = MainID;

            return View();
        }

        public ActionResult NoteList(string MainID)
        {
            ViewBag.Title = "记录查询";

            int? ID = Convert.ToInt32(MainID);

            var NoteList = entity.WXSaleAfterWorkNote.Where(p => p.MainID == ID).ToList();
            var Main = entity.WXSaleAfterMain.Where(p => p.ID == ID).FirstOrDefault();

            ViewBag.NoteList = NoteList;
            ViewBag.Main = Main;

            return View();
        }


        [HttpPost]
        public ActionResult Assessment(WXSaleAfterAssessment assessment)
        {
            assessment.AssessmentDate = DateTime.Now;
            entity.WXSaleAfterAssessment.Add(assessment);

            entity.SaveChanges();

            WXSaleAfterMain main = entity.WXSaleAfterMain.Where(p => p.ID == assessment.MainID).FirstOrDefault();

            main.IsFinish = "是";

            //MO mo = entity.MO.Where(p => p.MOId == main.MoID).FirstOrDefault();

            Ter_Customer_Com_Records tccr = entity.Ter_Customer_Com_Records.Where(p => p.MOName == main.MoID).FirstOrDefault();

            AfterWorkSchedule aws = entity.AfterWorkSchedule.Where(p => p.Ter_Customer_Com_RecordId == tccr.Ter_Customer_Com_RecordsId).FirstOrDefault();

            aws.AfterSalesEvaluation = assessment.SaleAfterAssessment;

            entity.SaveChanges();

            ViewBag.Content = "已完成评价！";

            return View("Done");
        }

        public ActionResult AssessmentCount(string yearMonth)
        {
            string code = Request.QueryString["code"];

            wx_backdata<wx_oauth2token> oauth2token = wcc.GetOauth2AccessToken(code);

            WXUSerBind wxuser = entity.WXUSerBind.Where(p => p.OpenID == oauth2token.ResponseData.openid).FirstOrDefault();

            if (wxuser == null || (wxuser != null && wxuser.UserID != "SUR1000007BQ" && wxuser.UserID != "SUR1000007HM"))
            {
                ViewBag.Content = "您无权查看此内容！";
                return View("Error");
            }

            int year = 0;
            int month = 0;
            if(string.IsNullOrEmpty(yearMonth))
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;

                yearMonth = year.ToString() + "-" + month.ToString("00");
            }
            else
            {
                string[] array = yearMonth.Split(new char[] { '-' });
                year = int.Parse(array[0]);
                month = int.Parse(array[1]);
            }

            ViewBag.yearMonth = yearMonth;

            List<WXSaleAfterAssessment> asseessmentList = entity.WXSaleAfterAssessment.Where(p => p.AssessmentDate.HasValue && p.AssessmentDate.Value.Year == year && p.AssessmentDate.Value.Month == month).ToList();

            List<ChartData> chardataList = new List<ChartData>();

            foreach(var assessment in asseessmentList)
            {
                ChartData chardata = new ChartData();
                string name = entity.WXSaleAfterMain.Where(p => p.ID == assessment.MainID).FirstOrDefault().CustomerName;
                double value = 0;
                if (assessment.SaleAfterAssessment == "非常满意")
                    value = 10;
                else if (assessment.SaleAfterAssessment == "满意")
                    value = 8;
                else if (assessment.SaleAfterAssessment == "一般")
                    value = 6;
                else if (assessment.SaleAfterAssessment == "较差")
                    value = 3;

                string assessmentDate = assessment.AssessmentDate.Value.ToString("yyyy-MM-dd");

                chardata.name = name;
                chardata.value = value;
                chardata.AssessmentDate = assessmentDate;

                chardataList.Add(chardata);
            }

            List<ChartData> chardataPersonList = new List<ChartData>();

            List<int> mainsId = asseessmentList.Select(p => (int)p.MainID).ToList();

            List<string> usernames = entity.WXSaleAfterMain.Where(p => mainsId.Contains(p.ID)).Select(p => p.UserName).ToList();

            foreach(var username in usernames)
            {
                ChartData chardata = new ChartData();
                string name = username;

                double value = 0;
                int count = 0;
                foreach (var assessment in asseessmentList)
                {
                    if(entity.WXSaleAfterMain.Where(p => p.ID == assessment.MainID).FirstOrDefault().UserName == username)
                    {                       
                        if (assessment.SaleAfterAssessment == "非常满意")
                            value += 10;
                        else if (assessment.SaleAfterAssessment == "满意")
                            value += 8;
                        else if (assessment.SaleAfterAssessment == "一般")
                            value += 6;
                        else if (assessment.SaleAfterAssessment == "较差")
                            value += 3;

                        count ++;
                    }                                    

                    string assessmentDate = assessment.AssessmentDate.Value.ToString("yyyy-MM-dd");
                    chardata.AssessmentDate = assessmentDate;
                }

                chardata.name = name;
                chardata.value = value/count;
                

                chardataPersonList.Add(chardata);
            }

            

            ViewBag.chardataList = chardataList;
            ViewBag.chardataPersonList = chardataPersonList;

            return View();
        }

        public ActionResult QRCode(string MainID)
        {
            ViewBag.Title = "客户评价二维码";

            int ID = int.Parse(MainID);

            WXSaleAfterMain main = entity.WXSaleAfterMain.Where(p => p.ID == ID).FirstOrDefault();

            if(main.IsFinish == "是")
            {
                ViewBag.Content = "已完成评价！";

                return View("Done");
            }

            string url = "http://" + HttpContext.Request.Url.Host + ":" +  "/Home/Assessment?MainID=" + MainID;//HttpContext.Request.Url.Port +

            QRCodeGenerator qrcg = new QRCodeGenerator();
            QRCodeData qrcd = qrcg.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrc = new QRCode(qrcd);

            Bitmap bit = qrc.GetGraphic(10, Color.Black, Color.White, true);

            //Graphics g = Graphics.FromImage(bit);

            MemoryStream ms = new MemoryStream();
            bit.Save(ms, ImageFormat.Png);

            Response.ClearContent();
            Response.ContentType = "image/png";
            Response.BinaryWrite(ms.ToArray());

            return View();
        }

        public ActionResult Webcatch()
        {
            string token = "dispatching";
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            string echoStr = Request.QueryString["echoStr"];//随机字符串 
            string signature = Request.QueryString["signature"];//微信加密签名
            string timestamp = Request.QueryString["timestamp"];//时间戳 
            string nonce = Request.QueryString["nonce"];//随机数 
            string[] ArrTmp = { token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                return Content(echoStr);
            }
            else
            {
                return Content("false");
            }
        }

        public string getjsapi_ticket()
        {

            HttpWebResponse gethttp = HttpHelper.CreateGetHttpResponse("https://oapi.dingtalk.com/gettoken?appkey=dinghuhbjk3pvkudrwbe&appsecret=Jhzm1Do2DcSENK2cjumui-zwz7XkbmGW6OVyBdLcK11GUhlzyVf5dyRoo0ytwTgF", 1000, null, null);

            string access_takenJson = HttpHelper.GetResponseString(gethttp);

            Access_Token token = JsonConvert.DeserializeObject<Access_Token>(access_takenJson);

            HttpWebResponse gethttp2 = HttpHelper.CreateGetHttpResponse("https://oapi.dingtalk.com/get_jsapi_ticket?access_token=" + token.access_token, 1000, null, null);

            string jspai_ticketJson = HttpHelper.GetResponseString(gethttp2);

            Jsapi_Ticket ticket = JsonConvert.DeserializeObject<Jsapi_Ticket>(jspai_ticketJson);

            return ticket.ticket;
        }
    }

    public class ChartData
    {
        public string name { get; set; }
        public double value { get; set; }
        public string AssessmentDate { get; set; }
    }

    public class Access_Token
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string access_token { get; set; }
    }

    public class Jsapi_Ticket
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }
    }

    /// <summary>
    /// http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html
    /// 微信JS-SDK使用权限签名算法
    /// </summary>
    public class JSAPI
    {
        /// <summary>
        /// 获取jsapi_ticket
        /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
        /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
        /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。
        /// </summary>
        /// <param name="access_token">BasicAPI获取的access_token,也可以通过TokenHelper获取</param>
        /// <returns></returns>
        public static string GetTickect(string access_token)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", access_token);
            var client = new HttpClient();
            var result = client.GetAsync(url).Result;
            if (!result.IsSuccessStatusCode) return string.Empty;
            var jsTicket = result.Content.ReadAsStringAsync().Result;
            return jsTicket;
        }


        /// <summary>
        /// 签名算法
        /// </summary>
        /// <param name="jsapi_ticket">jsapi_ticket</param>
        /// <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
        /// <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
        /// <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
        /// <returns></returns>
        public static string GetSignature(string jsapi_ticket, string noncestr, long timestamp, string url, out string string1)
        {
            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
                          .Append("noncestr=").Append(noncestr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            string1 = string1Builder.ToString();
            return Sha1(string1);
        }

        /// <summary>
        /// Sha1
        /// </summary>
        /// <param name="orgStr"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Sha1(string orgStr, string encode = "UTF-8")
        {
            var sha1 = new SHA1Managed();
            var sha1bytes = System.Text.Encoding.GetEncoding(encode).GetBytes(orgStr);
            byte[] resultHash = sha1.ComputeHash(sha1bytes);
            string sha1String = BitConverter.ToString(resultHash).ToLower();
            sha1String = sha1String.Replace("-", "");
            return sha1String;
        }

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

    }
}
