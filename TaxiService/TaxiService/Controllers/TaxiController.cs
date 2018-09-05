using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    //Kontroler za registraciju musterija
    public class TaxiController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Korisnik korisnik)
        {
            //Nije pouzdano oslanjati se iskljucivo na klijentsku validaciju
            if (!korisnik.Valid())
            {
                return this.BadRequest();
            }
            //dodati korisnik moze biti musterija ili vozac, potrebno je prebaciti korisnika u specificnu klasu
            
            string Rezultat = TekstSkladiste.Upisi(korisnik);
            if (Rezultat == "ok")
            {
                return this.Ok();
            }
            else if (Rezultat == "postoji")
            {
                return this.Conflict();
            }
            else
            {
                return this.InternalServerError();
            }
            
        }
    }
}
