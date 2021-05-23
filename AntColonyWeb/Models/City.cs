using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntColonyWeb.Models
{
    public class City
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? Cost { get; set; }
        public int? Days { get; set; }
        public int? Hours { get; set; }
        public string ImageLink { get; set; }
        public int? Value { get; set; }
        public Pricing Pricing { get; set; }
    }
}