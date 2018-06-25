using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Musterija : Korisnik
    {
        public Lokacija Lokacija { get; set; }
        //Lokacija već sadrzi adresu
    }
}