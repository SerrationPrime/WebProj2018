using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class VoznjaController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Voznja voznja)
        {
            string sessionId;
            var cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookie != null)
            {
                sessionId = cookie["session-id"].Value;
            }
            else
            {
                return Unauthorized();
            }

            CookiePomoc miniCookie;
            if (LoginController.ActiveSessions.ContainsKey(sessionId))
            {
                miniCookie = LoginController.ActiveSessions[sessionId];
            }
            else
            {
                return Unauthorized();
            }

            switch (miniCookie.Uloga)
            {
                case ("Dispecer"):
                    voznja.DispecerUsername = miniCookie.Username;
                    voznja.Status = StatusVoznje.Obradjena;
                    break;
                default:
                    voznja.MusterijaUsername = miniCookie.Username;
                    voznja.Status = StatusVoznje.Formirana;
                    break;
            }

            if (TekstSkladiste.UpisiVoznju(voznja) != "ok")
            {
                return InternalServerError();
            }
            else return Ok();
        }

        [HttpGet]
        public List<Voznja> Get()
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
                return retList;
            }
            CookiePomoc miniCookie;
            if (LoginController.ActiveSessions.ContainsKey(sessionId))
            {
                miniCookie = LoginController.ActiveSessions[sessionId];
            }
            else
            {
                return retList;
            }

            switch (miniCookie.Uloga)
            {
                case ("Dispecer"):
                    retList = TekstSkladiste.PokupiVoznje("Dispecer", miniCookie.Username);
                    break;
                case ("Vozac"):
                    retList = TekstSkladiste.PokupiVoznje("Vozac", miniCookie.Username);
                    break;
                default:
                    retList = TekstSkladiste.PokupiVoznje("Musterija", miniCookie.Username);
                    break;
            }

            return retList;
        }
    }
}