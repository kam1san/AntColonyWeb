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
using System.Data.Entity;

namespace AntColonyWeb.Controllers
{
    public class HomeController : Controller
    {
        CitiesContext cities_db = new CitiesContext();

        public ActionResult CitiesChoice()
        {
            var cities = cities_db.Cities.ToList();
            return View(cities);
        }

        [HttpPost]
        public ActionResult GetChosenCities(int[] selected, int fuel_cons, string fuel_t, int TotalMoney, int TotalTimeDays, int TotalTimeHours, int TotalTimeMinutes)
        {
            List<City> selected_cities_list = new List<City>();

            for (int i = 0; i < selected.Length; i++)
            {
                int cityID = selected[i];
                City city = cities_db.Cities.Where(x => x.ID == cityID).FirstOrDefault();

                city.Pricing = cities_db.Pricings.Where(x => x.ID == cityID).FirstOrDefault();

                selected_cities_list.Add(city);
            }
            HttpContext.Session["Selected_Cities"] = selected_cities_list;

            HttpContext.Session["Fuel_Consumption"] = fuel_cons;
            HttpContext.Session["Fuel_Type"] = fuel_t;
            HttpContext.Session["Fuel_Price"] = HtmlParser.GetFuelPrice((string)HttpContext.Session["Fuel_Type"]);

            HttpContext.Session["Total_Money"] = TotalMoney;
            HttpContext.Session["Total_Time"] = TotalTimeDays*60*24 + TotalTimeHours*60 + TotalTimeMinutes;

            return RedirectToAction("EditCities", "Home");
        }

        public ActionResult EditCities()
        {
            return View((List<City>)HttpContext.Session["Selected_Cities"]);
        }

        [HttpPost]
        public ActionResult EditCities(string startup_city, List<City> edited_cities)
        {
            foreach (var city in edited_cities)
                city.Cost = city.Days*(city.Pricing.Events + city.Pricing.Food + city.Pricing.Museums + city.Pricing.Shopping + city.Pricing.Staying + city.Pricing.Tours);
            
            HttpContext.Session["Selected_Cities"] = edited_cities;
            HttpContext.Session["StartUp_City_Position"] = Convert.ToInt32(startup_city);

            return RedirectToAction("GetResult", "Home");
        }

        public ActionResult GetResult()
        {
            var cities = (List<City>)HttpContext.Session["Selected_Cities"];
            int[][] distances = new int[cities.Count][];
            for (int i = 0; i < cities.Count; i++)
                for (int j = 0; j < cities.Count; j++)
                {
                    distances[i] = new int[cities.Count];
                    distances[i][j] = 0;
                }

            string[][] time = new string[cities.Count][];
            for (int i = 0; i < cities.Count; i++)
                for (int j = 0; j < cities.Count; j++)
                {
                    time[i] = new string[cities.Count];
                    time[i][j] = "";
                }

            for (int i=0; i<cities.Count; i++)
            {
                for (int j = 0; j < cities.Count; j++)
                    if (i == j) 
                        continue;
                    else
                    {
                        string[] vals = HtmlParser.GetRoutesInfo(cities[i], cities[j]);
                        distances[i][j] = Convert.ToInt32((double)HttpContext.Session["Fuel_Price"] * (Convert.ToInt32(vals[0]) * (int)HttpContext.Session["Fuel_Consumption"]) / 100 + 1);
                        time[i][j] = vals[1];
                    }
            }

            HttpContext.Session["Distances"] = distances;
            HttpContext.Session["Time_Non_Convert"] = TimeConverter.ConvertToStringUkr(time, cities.Count);
            HttpContext.Session["Time"] = TimeConverter.ConvertToInt(time, cities.Count);

            var distances_time = TimeConverter.ConvertToInt(time, cities.Count);

            int[] cities_cost = new int[cities.Count];
            int[] cities_time = new int[cities.Count];
            int[] cities_value = new int[cities.Count];
            for(int i=0; i<cities.Count; i++)
            {
                cities_cost[i] = Convert.ToInt32(cities[i].Cost);
                cities_time[i] = Convert.ToInt32(cities[i].Days * 60 * 24 + cities[i].Hours * 60);
                cities_value[i] = Convert.ToInt32(cities[i].Value);
            }

            AntColonyAlgorithmSetup setup = new AntColonyAlgorithmSetup("test1",
                0.4, 0.6, 0.01, 2.0,
                cities.Count,
                cities.Count,
                10000,
                (int)HttpContext.Session["Total_Money"],
                (int)HttpContext.Session["Total_Time"],
                cities_cost,
                cities_time,
                distances,
                distances_time,
                cities_value,
                (int)HttpContext.Session["StartUp_City_Position"]);

            int min = 0;
            AntColonyResult AntColonyResult = new AntColonyResult();
            AntColonyResult test = new AntColonyResult();
            for (int i = 0; i < 10; i++)
            {
                test = AntColonyMain.Execute(setup);
                if (test.Value > min)
                {
                    min = Convert.ToInt32(test.Value);
                    AntColonyResult = test;
                }
            }

            var DT = (string[][])HttpContext.Session["Time_Non_Convert"];
            var DC = (int[][])HttpContext.Session["Distances"];

            for (int i = 0; i < AntColonyResult.Path.Count; i++)
                for (int j = 0; j < cities.Count; j++)
                    if (AntColonyResult.Path[i] == j)
                    {
                        if (i < AntColonyResult.Path.Count - 1)
                        {
                            AntColonyResult.distances_cost[i] = DC[j][AntColonyResult.Path[i + 1]];
                            AntColonyResult.distances_time[i] = DT[j][AntColonyResult.Path[i + 1]];
                        }
                        AntColonyResult.Path_Cities.Add(cities[j]);
                    }

            ViewBag.AntColonyResult = AntColonyResult;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            cities_db.Dispose();
            base.Dispose(disposing);
        }

    }
}