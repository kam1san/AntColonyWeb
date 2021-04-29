using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AntColonyWeb.Models.Context
{
    public class CitiesContext:DbContext
    {
        public DbSet<City> Cities { get; set; }
    }
}