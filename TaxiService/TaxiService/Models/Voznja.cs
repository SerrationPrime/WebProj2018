using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public enum StatusVoznje { Ceka, Formirana, Obradjena, Prihvacena, Otkazana, Neuspesna, Uspesna}

    public class Voznja
    {

        public DateTime Vreme { get; set; }
        public Lokacija Lokacija { get; set; }
        public string ZeljeniTip { get; set; }
        public string MusterijaUsername { get; set; }
        public Lokacija Odrediste { get; set; }
        public string DispecerUsername { get; set;}
        public string VozacUsername { get; set; }
        public double Iznos { get; set; }
        public DateTime KomentarVreme { get; set; }
        public StatusVoznje Status { get; set; }

        public Voznja()
        {
            Vreme = DateTime.Now;
        }
    }
}