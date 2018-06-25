using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public enum Pol { Musko, Zensko, Nedefinisano }

    public class Korisnik
    {
        public string Username { get; set; }
        //Svestan sam da bi lozinka trebala biti hešovana/soljena, ali to nije zahtevano u okviru zadatka
        public string Password { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Pol Pol { get; set; }
        public string JMBG { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        //Usled upotrebe nasleđivanja, ovo polje se suvišno, ali je zahtevano u specifikaciji
        public string Uloga { get; set; }
        public List<String> IDVoznje;
    }
}