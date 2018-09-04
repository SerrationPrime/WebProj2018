using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Dispecer : Korisnik
    {
        public Dispecer(Korisnik korisnik) : base(korisnik)
        {

        }

        public Lokacija Lokacija { get; set; }
    }
}