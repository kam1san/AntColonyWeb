using AntColonyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntColonyWeb.AntColony
{
    public class AntColonyResult
    {
        public List<int> Path { get; set; }
        public double Money { get; set; }
        public double Time { get; set; }
        public double Value { get; set; }
        public string Time_Non_Convert { get; set; }
        public List<City> Path_Cities { get; set; }
        public int[] distances_cost { get; set; }
        public string[] distances_time { get; set; }
        public AntColonyResult(List<int> p, double m, double t, double v)
        {
            Path = p;
            Money = m;
            Time = t;
            Value = v;
            Time_Non_Convert = this.ConvertTimeToString(t);
            Path_Cities = new List<City>();
            distances_cost = new int[p.Count-1];
            distances_time = new string[p.Count - 1];
        }

        public AntColonyResult()
        { }

        public string ConvertTimeToString(double time)
        {
            int days = Convert.ToInt32(Math.Truncate(time / 1440));
            int hours = Convert.ToInt32(Convert.ToInt32(time - days * 1440) /60);
            return $"Приблизно {days} днів, {hours} годин.";
        }
    }
}