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
        public static string[] GetHtml(City c1, City c2)
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
                string[] result = new string[4];
                result[0] = c1.Name; result[1] = c2.Name; result[2] = distance; result[3] = time;
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
                string[] result = new string[4];
                result[0] = c1.Name; result[1] = c2.Name; result[2] = distance; result[3] = time;
                return result;
            }
        }
    }
}