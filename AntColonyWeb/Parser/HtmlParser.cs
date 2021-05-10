using AntColonyWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace AntColonyWeb.Parser
{
    public class HtmlParser
    {
        public static string[] GetRoutesInfo(City c1, City c2)
        {
            string url = $"https://degruz.ua/calculation_of_distance/route={c1.ID}:{c2.ID}";
            HttpWebRequest myHttwebrequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //myHttwebrequest.Timeout = 10;
            HttpWebResponse myHttpWebresponse = (HttpWebResponse)myHttwebrequest.GetResponse();
            StreamReader strm = new StreamReader(myHttpWebresponse.GetResponseStream());
            string HtmlText = strm.ReadToEnd();
            try
            {
                var searchFor = "Відстань: <b>";
                int firstIndex = HtmlText.IndexOf(searchFor);
                int secondIndex = HtmlText.IndexOf(" km</b>", firstIndex);
                string distance = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                searchFor = "Час в дорозі: <b>";
                firstIndex = HtmlText.IndexOf(searchFor);
                secondIndex = HtmlText.IndexOf("</b></span>", firstIndex);
                string time = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                string[] result = new string[2];
                 result[0] = distance; result[1] = time;
                return result;
            }
            catch
            {
                var searchFor = "Короткий маршрут: <b>";
                int firstIndex = HtmlText.IndexOf(searchFor);
                int secondIndex = HtmlText.IndexOf(" km</b>", firstIndex);
                string distance = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                searchFor = "Час в дорозі: <b>";
                firstIndex = HtmlText.IndexOf(searchFor, firstIndex);
                secondIndex = HtmlText.IndexOf("</b></span>", firstIndex);
                string time = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                string[] result = new string[2];
                result[0] = distance; result[1] = time;
                return result;
            }
        }

        public static double GetFuelPrice(string fuel_type)
        {
            string url = "https://index.minfin.com.ua/ua/markets/fuel/";
            HttpWebRequest myHttwebrequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //myHttwebrequest.Timeout = 10;
            HttpWebResponse myHttpWebresponse = (HttpWebResponse)myHttwebrequest.GetResponse();
            StreamReader strm = new StreamReader(myHttpWebresponse.GetResponseStream());
            string HtmlText = strm.ReadToEnd();
            if (fuel_type == "Пальне А-95")
            {
                var searchFor = "Бензин А-95</a></td><td align='center'><br></td><td align='right'><big>";
                int firstIndex = HtmlText.IndexOf(searchFor);
                int secondIndex = HtmlText.IndexOf("</big>", firstIndex);
                string fuel_price = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                return Math.Round(Convert.ToDouble(fuel_price), 2);
            }
            else if (fuel_type == "Пальне А-92")
            {
                var searchFor = "Бензин А-92</a></td><td align='center'><br></td><td align='right'><big>";
                int firstIndex = HtmlText.IndexOf(searchFor);
                int secondIndex = HtmlText.IndexOf("</big>", firstIndex);
                string fuel_price = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                return Math.Round(Convert.ToDouble(fuel_price), 2);
            }
            else if (fuel_type == "Дизельне пальне")
            {
                var searchFor = "Дизельне паливо</a></td><td align='center'><br></td><td align='right'><big>";
                int firstIndex = HtmlText.IndexOf(searchFor);
                int secondIndex = HtmlText.IndexOf("</big>", firstIndex);
                string fuel_price = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                return Math.Round(Convert.ToDouble(fuel_price), 2);
            }
            else if (fuel_type == "Газ")
            {
                var searchFor = "Газ авто&shy;мобільний</a></td><td align='center'><br></td><td align='right'><big>";
                int firstIndex = HtmlText.IndexOf(searchFor);
                int secondIndex = HtmlText.IndexOf("</big>", firstIndex);
                string fuel_price = HtmlText.Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length);
                return Math.Round(Convert.ToDouble(fuel_price), 2);
            }
            else
                return 0;
        } 

    }
}