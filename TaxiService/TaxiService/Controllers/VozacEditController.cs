using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class VozacEditController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(double lat, double lng)
        {
            string sessionId;
            var cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookie != null)
            {
                sessionId = cookie["session-id"].Value;
            }
            else
            {
                return this.Unauthorized();
            }
            CookiePomoc miniCookie;
            if (LoginController.ActiveSessions.ContainsKey(sessionId))
            {
                miniCookie = LoginController.ActiveSessions[sessionId];
            }
            else
            {
                return this.Unauthorized();
            }

            switch (miniCookie.Uloga)
            {
                case ("Vozac"):
                    if (TekstSkladiste.ObnoviLokaciju(miniCookie.Username, lat, lng))
                        return this.Ok();
                    else return this.InternalServerError();
                default:
                    return this.Unauthorized();
            }

        }
    }
}
