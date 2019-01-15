using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class VozacController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Vozac vozac)
        {
            //Nije pouzdano oslanjati se iskljucivo na klijentsku validaciju
            if (!vozac.Valid())
            {
                return this.BadRequest();
            }

            vozac.Uloga = "Vozac";
            string Rezultat = TekstSkladiste.Upisi(vozac);
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

        [HttpGet]
        public List<VozacSkracena> Get(double lat, double lng)
        {
            return TekstSkladiste.PronadjiNajblizeVozace(lat, lng);
        }
    }
}
