using System;

namespace TaxiService.Models
{
    public enum StatusVoznje { Kreirana, Formirana, Obradjena, Prihvacena, Otkazana, Neuspesna, Uspesna}

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
        public Komentar Komentar { get; set; }
        public StatusVoznje Status { get; set; }

        public Voznja()
        {
            Vreme = DateTime.Now;
        }
    }
}