using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AntColonyWeb.Models
{
    public class Pricing
    {
        [Key]
        [ForeignKey("City")]
        public int ID { get; set; }
        public int? Staying { get; set; }
        public int? Food { get; set; }
        public int? Tours { get; set; }
        public int? Museums { get; set; }
        public int? Shopping { get; set; }
        public int? Events { get; set; }
        public City City { get; set; }
    }
}