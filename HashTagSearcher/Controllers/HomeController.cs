using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Gapi.Search;
using HashTagSearcher.Models;
using Tweetinvi.Core.Extensions;
using TweetSharp;
using SearchOptions = TweetSharp.SearchOptions;

namespace HashTagSearcher.Controllers
{
    public class HomeController : Controller
    {
        private TwitterService _service;

        public ActionResult Index(string query)
        {
            if (_service == null) AuthApi();


//          var trends = _service.ListLocalTrendsFor(new ListLocalTrendsForOptions {Id = 23424977});
            var tagsList = new List<string> {"hashtag", "happy", "blacked"};
//          if (trends != null)
//          {
//              tagsList =
//                  (from trend in trends
//                      where trend?.Query != null
//                      select trend.Query.Replace("#", "").Replace("%23", "")).ToList();
//          }
//          else
//          {
//              tagsList.Add("hashtag");
//              tagsList.Add("happy");
//              tagsList.Add("blacked");
//          }
//
//          Trace.WriteLine(tagsList.ToString());
//
//          var tostring = tagsList.Aggregate("", (current, tag) => current + (tag + ", "));
//          Trace.WriteLine(tostring);

            var model = new IndexModel {Query = query, TrendingTags = tagsList.ToArray()};

            return View(model);
        }


        public ActionResult _SearchResultsPartial(string query)
        {
            if (query.IsNullOrEmpty()) return View();

            var tweetsSearch = Search(query);
            ViewBag.Tweets = tweetsSearch;

            return View();
        }

        public ActionResult _MapPartial(string query)
        {
            if (query.IsNullOrEmpty()) return View();

            var tweetsSearch = Search(query);

            var points = (from tweet in tweetsSearch
                where tweet.Location != null
                select tweet.Location.Coordinates.Latitude + "," + tweet.Location.Coordinates.Longitude).ToArray();

            var model = new MapModel {Points = points};

            return View(model);
        }

        public ActionResult _NewsPartial(string query)
        {
            if (query.IsNullOrEmpty()) return View();

            var resutList = new List<SearchResult>();

            for (var i = 0; i < 6; i++)
            {
                 var search = Searcher.Search(SearchType.News, query, i);
                 resutList.AddRange(search.Items);
            }

            ViewBag.Results = resutList.ToArray();

            return View();
        }

        public void AuthApi()
        {
            _service = new TwitterService("Nd7J1InJaXN3QRIZLo9Hi3Vcg",
                "p5AtH77gdWDRQyfUYUNqhFNPcxaADlzyUjq9b1s0CPooOFT2by");
            _service.AuthenticateWith("1559190498-lfTKYXPu6g83X7aeWp2XkEU0dni3ARuHqZ59ayF",
                "3uwnzRv5eHAAhEtDnmvydT13y4BV2gojFnShBJBeGvzj8");
        }

        public IEnumerable<TwitterStatus> Search(string query)
        {
            var tweets = new List<TwitterStatus>();
            var dayBefore = DateTime.Today.AddDays(-1);

            if (_service == null) AuthApi();
            if (!query.StartsWith("#")) query = "#" + query.ToLower();

            var code = new TwitterGeoLocationSearch(39.50, -98.35, 1500, TwitterGeoLocationSearch.RadiusType.Mi);
            var geoSearch = _service.Search(new SearchOptions {Q = query, Geocode = code, Count = Convert.ToInt32(100000)}).Statuses;

            var geoStatuses = geoSearch as TwitterStatus[] ?? geoSearch.ToArray();
            tweets.AddRange(
                geoStatuses.Where(
                    tweet => DateTime.Compare(tweet.CreatedDate, dayBefore) > -1));

            var fullSearch = _service.Search(new SearchOptions {Q = query, Count = Convert.ToInt32(100000)});
            var fullStatuses = fullSearch.Statuses as IList<TwitterStatus> ?? fullSearch.Statuses.ToList();
            tweets.AddRange(
                fullStatuses.Where(
                    tweet => // !geoStatuses.Contains(tweet) &&
                             DateTime.Compare(tweet.CreatedDate, dayBefore) > -1));


            tweets.Sort((t1, t2) => DateTime.Compare(t1.CreatedDate, t2.CreatedDate));
            tweets.Reverse();

            return tweets.AsEnumerable();
        }
    }
}