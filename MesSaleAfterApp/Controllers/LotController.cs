using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MesSaleAfterApp.Models;
using MesSaleAfterApp.Models.OrbitModel;
using Newtonsoft.Json;

namespace MesSaleAfterApp.Controllers
{
    public class LotController : ApiController
    {
        OrbitMOMEntity entity = new OrbitMOMEntity();
        public string Get(string LotSN)
        {
            ReturnModel returnModel = null;

            Lot lot = entity.Lot.Where(p => p.LotSN == LotSN).FirstOrDefault();
            MO mo = entity.MO.Where(p => p.MOName == LotSN).FirstOrDefault();

            if (mo != null || lot != null)
            {
                if(lot != null)
                {
                    mo = entity.MO.Where(p => p.MOId == lot.MOId).FirstOrDefault();
                }

                Product product = entity.Product.Where(p => p.ProductId == mo.ProductId).FirstOrDefault();

                ProductRoot pr = entity.ProductRoot.Where(p => p.ProductRootId == product.ProductRootId).FirstOrDefault();


                SO so = entity.SO.Where(p => p.SOName == mo.SOName).FirstOrDefault();

                Customer cus = new Customer();

                if (so != null)
                {
                    cus = entity.Customer.Where(p => p.CustomerId == so.CustomerId).FirstOrDefault();
                }

                if (mo != null)
                {
                    returnModel = new ReturnModel(new { mo, product, pr, so, cus ,lot}, true);
                }
                else
                {
                    returnModel = new ReturnModel(false, "批号不存在");
                }

                
            }
            else
            {
                returnModel = new ReturnModel(false, "订单号不存在");
            }

            string json = JsonConvert.SerializeObject(returnModel);

            return json;
        }
    }
}
