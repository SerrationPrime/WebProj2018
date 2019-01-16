using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class VozacSkracena
    {
        public string username { get; set; }
        public double distanca { get; set; }
        public string vozilo { get; set; }

        public VozacSkracena(string username, double distanca, AutoTip vozilo)
        {
            this.username = username;
            this.distanca = distanca;
            this.vozilo = vozilo.ToString();
        }
    }
}