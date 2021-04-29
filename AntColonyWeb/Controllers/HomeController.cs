using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AntColonyWeb.AntColony;
using AntColonyWeb.Models;
using AntColonyWeb.Models.Context;
using AntColonyWeb.Parser;
using Google.Apis.CustomSearchAPI;

namespace AntColonyWeb.Controllers
{
    public class HomeController : Controller
    {
        CitiesContext cities_db = new CitiesContext();
        public ActionResult Index()
        {
            AntColonyAlgorithmSetup setup = new AntColonyAlgorithmSetup("test1", 3, 2, 0.01, 2.0, 5, 5, 10000, 50, 50, new int[] { 3, 2, 5, 7, 6 }, new int[] { 5, 3, 1, 4, 6 }, new int[5][] { new int[5] { 0, 3, 4, 8, 0 }, new int[5] { 3, 0, 3, 9, 6 }, new int[5] { 4, 3, 0, 0, 7 }, new int[5] { 8, 9, 0, 0, 6 }, new int[5] { 0, 6, 7, 6, 0 } }, new int[5][] { new int[5] { 0, 2, 3, 10, 0 }, new int[5] { 2, 0, 3, 7, 5 }, new int[5] { 3, 3, 0, 0, 2 }, new int[5] { 10, 7, 0, 0, 7 }, new int[5] { 0, 5, 2, 7, 0 } }, new int[] { 4, 3, 2, 5, 7 });
            ViewBag.Path = AntColonyMain.Execute(setup);
            return View();
        }

        public ActionResult GetHtml()
        {
            City Kiev = cities_db.Cities.Where(x=>x.Name=="Київ").FirstOrDefault();
            //ViewBag.city = Kiev;
            List<string[]> distances = new List<string[]>();
            foreach (City city in cities_db.Cities.ToList())
            {
                if (city.Name == "Київ")
                    continue;
                distances.Add(HtmlParser.GetHtml(Kiev, city));
            }
            ViewBag.Distances = distances;
            return View();
        }

        public ActionResult GetCities()
        {
            var cities = cities_db.Cities;
            return View(cities.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            cities_db.Dispose();
            base.Dispose(disposing);
        }

    }
}