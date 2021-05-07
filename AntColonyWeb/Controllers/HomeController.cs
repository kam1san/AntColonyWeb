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
            //AntColonyAlgorithmSetup setup = new AntColonyAlgorithmSetup("test1", 3, 2, 0.01, 2.0, 5, 5, 10000, 50, 50, new int[] { 3, 2, 5, 7, 6 }, new int[] { 5, 3, 1, 4, 6 }, new int[5][] { new int[5] { 0, 3, 4, 8, 0 }, new int[5] { 3, 0, 3, 9, 6 }, new int[5] { 4, 3, 0, 0, 7 }, new int[5] { 8, 9, 0, 0, 6 }, new int[5] { 0, 6, 7, 6, 0 } }, new int[5][] { new int[5] { 0, 2, 3, 10, 0 }, new int[5] { 2, 0, 3, 7, 5 }, new int[5] { 3, 3, 0, 0, 2 }, new int[5] { 10, 7, 0, 0, 7 }, new int[5] { 0, 5, 2, 7, 0 } }, new int[] { 4, 3, 2, 5, 7 });
            //ViewBag.Path = AntColonyMain.Execute(setup); використання алгоритму

            return View(cities_db.Cities);
        }

        [HttpPost]
        public ActionResult GetChosenCities(int[] selected, int fuel_cons, string fuel_t, int TotalMoney, int TotalTimeDays, int TotalTimeHours, int TotalTimeMinutes)
        {
            List<City> selected_cities_list = new List<City>();

            for (int i = 0; i < selected.Length; i++)
            {
                int city = selected[i];
                selected_cities_list.Add(cities_db.Cities.Where(x => x.ID == city).FirstOrDefault());
            }
            HttpContext.Session["Selected_Cities"] = selected_cities_list;

            HttpContext.Session["Fuel_Consumption"] = fuel_cons;
            HttpContext.Session["Fuel_Type"] = fuel_t;
            HttpContext.Session["Total_Money"] = TotalMoney;
            HttpContext.Session["Total_Time"] = TotalTimeDays*360 + TotalTimeHours*60 + TotalTimeMinutes;
            return RedirectToAction("EditCities", "Home");
        }

        public ActionResult EditCities()
        {
            ViewBag.FuelPrice = HtmlParser.GetFuelPrice((string)HttpContext.Session["Fuel_Type"]);
            ViewBag.FuelType = (string)HttpContext.Session["Fuel_Type"];
            return View((List<City>)HttpContext.Session["Selected_Cities"]);
        }

        [HttpPost]
        public ActionResult EditCities(List<City> edited_cities)
        {
            HttpContext.Session["Selected_Cities"] = edited_cities;
            //foreach (var city in edited_cities)
            //    cities_db.Entry(city).State = EntityState.Modified;
            //cities_db.SaveChanges();

            return RedirectToAction("GetCities", "Home");
        }

        public ActionResult GetCities()
        {
            return View((List<City>)HttpContext.Session["Selected_Cities"]);
        }

        public ActionResult GetAllCities()
        {
            return View(cities_db.Cities.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            cities_db.Dispose();
            base.Dispose(disposing);
        }

    }
}