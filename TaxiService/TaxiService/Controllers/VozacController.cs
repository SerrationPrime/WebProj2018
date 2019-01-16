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
        [HttpGet]
        public Vozac Get()
        {
            var retList = new List<Voznja>();

            string sessionId;
            var cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookie != null)
            {
                sessionId = cookie["session-id"].Value;
            }
            else
            {
                return null;
            }
            CookiePomoc miniCookie;
            if (LoginController.ActiveSessions.ContainsKey(sessionId))
            {
                miniCookie = LoginController.ActiveSessions[sessionId];
            }
            else
            {
                return null;
            }

            switch (miniCookie.Uloga)
            {
                case ("Dispecer"):
                    return TekstSkladiste.PronadjiVozaca(miniCookie.Username);
                    break;
                case ("Vozac"):
                    return TekstSkladiste.PronadjiVozaca(miniCookie.Username);
                    break;
                default:
                    return null;
            }

        }
    }
}
