using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesSaleAfterApp.Models
{
    public class ReturnModel
    {
        public ReturnModel(object data, bool success)
        {
            this.data = data;
            this.success = true;
        }

        public ReturnModel(bool success, string error)
        {
            this.success = false;
            this.error = error;
        }

        public object data { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
    }
}