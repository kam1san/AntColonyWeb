using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace AntColonyWeb.AntColony
{
    public class AntColonyAlgorithmSetup
    {
        public string name { get; set; }
        public double  a { get; set; }
        public double b { get; set; }
        public double r { get; set; }
        public double Q { get; set; }
        public int n { get; set; }
        public int m { get; set; }
        public int iterations { get; set; }
        public int max_money { get; set; }
        public int max_time { get; set; }
        public int[] CC { get; set; }
        public int[] CT { get; set; }
        public int[][] DC { get; set; }
        public int[][] DT { get; set; }
        public int[] values { get; set; }
        public int startup_city { get; set; }

        public AntColonyAlgorithmSetup(string name, double a, double b, double r, double Q, int n, int m, int iterations, int max_money, int max_time, int[] CC, int[] CT, int[][] DC, int[][] DT, int[] values, int start_city)
        {
            this.name = name;
            this.a = a;
            this.b = b;
            this.r = r;
            this.Q = Q;
            this.n = n;
            this.m = m;
            this.iterations = iterations;
            this.max_money = max_money;
            this.max_time = max_time;
            this.CC = CC;
            this.CT = CT;
            this.DC = DC;
            this.DT = DT;
            this.values = values;
            this.startup_city = start_city;
        }

        public AntColonyAlgorithmSetup(){}
    }
}