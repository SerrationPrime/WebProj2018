using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    //C# ne dopusta da su clanovi enuma samo brojevi, ali zbog nacina kojim jezik tretira enume, Nula=0, Jedinica=1...
    public enum Ocena { Nula, Jedinica, Dvojka, Trojka, Cetvorka, Petica }
    
    public class Komentar
    {
        public string Opis { get; set; }
        public DateTime Datum { get; set; }
        public string KorisnikUsername { get; set; }
        public string VoznjaID { get; set; }
        public Ocena Ocena { get; set; }
    }
}