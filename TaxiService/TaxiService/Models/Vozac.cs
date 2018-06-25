using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Vozac : Korisnik
    {
        public Lokacija Lokacija { get; set; }
        public string AutomobilBroj { get; set; }
    }
}