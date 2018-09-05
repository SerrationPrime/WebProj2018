using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Lokacija
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Adresa { get; set; }

        public Lokacija()
        {
            Latitude = 0;
            Longitude = 0;
            Adresa = "";
        }
    }
}