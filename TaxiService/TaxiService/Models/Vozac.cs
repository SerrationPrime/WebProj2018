using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Vozac : Korisnik
    {
        public Vozac(Korisnik korisnik): base(korisnik)
        {
            Lokacija = new Lokacija();
        }

        public Lokacija Lokacija { get; set; }
        public string AutomobilBroj { get; set; }
    }
}