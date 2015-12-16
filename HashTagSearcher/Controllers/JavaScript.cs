using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace HashTagSearcher.Controllers
{
    public static class JavaScript
    {
        public static string Serialize(object o)
        {
            var js = new JavaScriptSerializer();
            return js.Serialize(o);
        }
    }
}