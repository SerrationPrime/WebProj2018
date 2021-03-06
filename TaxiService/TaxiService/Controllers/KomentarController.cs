﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class KomentarController : ApiController
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

            voznja.Komentar.Username = miniCookie.Username;

            if (TekstSkladiste.UpisiKomentar(voznja) != "ok")
            {
                return InternalServerError();
            }
            else return Ok();
        }
    }
}
