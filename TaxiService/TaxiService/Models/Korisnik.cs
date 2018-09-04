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

        public Korisnik()
        {

        }

        protected Korisnik(Korisnik korisnik)
        {
            Username = korisnik.Username;
            Password = korisnik.Password;
            Ime = korisnik.Ime;
            Prezime = korisnik.Prezime;
            Pol = korisnik.Pol;
            JMBG = korisnik.JMBG;
            Telefon = korisnik.Telefon;
            Email = korisnik.Email;
            Uloga = korisnik.Uloga;
            IDVoznje = korisnik.IDVoznje;
        }

        public bool Valid()
        {
            if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(Ime) || String.IsNullOrEmpty(Prezime)
                || String.IsNullOrEmpty(JMBG) || String.IsNullOrEmpty(Telefon) || String.IsNullOrEmpty(Email)
                || String.IsNullOrEmpty(Uloga))
            {
                return false;
            }
            if (JMBG.Length != 13 || !System.Text.RegularExpressions.Regex.IsMatch(JMBG, @"\d"))
            {
                return false;
            }
            return true;
        }
    }
}