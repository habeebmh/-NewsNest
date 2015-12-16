using System.Diagnostics;
using System.Web.Mvc;

namespace HashTagSearcher.Models
{
    public class IndexModel
    {
        private string _query = "";
        public string Query
        {
            get { return _query; }
            set
            {
                Trace.WriteLine(value);
                _query = value;
            } 
        }

        public string[] TrendingTags { get; set; }
    }
}