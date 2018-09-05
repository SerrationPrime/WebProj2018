using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Vozac : Korisnik
    {
        public Vozac()
        {
            Lokacija = new Lokacija();
            Status = "Slobodan";
        }
        public Vozac(Korisnik korisnik) : base(korisnik)
        {
            Lokacija = new Lokacija();
            Status = "Slobodan";
            AutomobilBroj = "Bez vozila";
        }

        public Lokacija Lokacija { get; set; }
        public string AutomobilBroj { get; set; }
        public string Status { get; set; }
    }
}