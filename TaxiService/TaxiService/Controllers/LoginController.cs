using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using TaxiService.Models;
using System.Net;
using System.Net.Http.Headers;

namespace TaxiService.Controllers
{
    public class UserPass
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginController : ApiController
    {
        public static Dictionary<string, CookiePomoc> ActiveSessions = new Dictionary<string, CookiePomoc>();

        [HttpPost]
        public IHttpActionResult Post(UserPass credentials)
        {
            Random rnd = new Random();
            var RandVal = rnd.Next(0, int.MaxValue).ToString();
            var Resp = new HttpResponseMessage();

            var Podaci = TekstSkladiste. Autentikacija(credentials.Username, credentials.Password);

            switch (Podaci.Uloga) {
                case ("Blokiran"):
                    TekstSkladiste.LogUpisi("Pokusan login blokiranog korisnika, username: " + credentials.Username + ".");
                    Resp = Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Blokirani ste, i ne mozete korisiti ovaj web sajt.");
                    return ResponseMessage(Resp);
                case("Nepoznat"):
                    TekstSkladiste.LogUpisi("Pokusan login sa nepravilnim podacima, username: " + credentials.Username + ".");
                    return Unauthorized();
                default:
                    ActiveSessions.Add(RandVal, Podaci);
                    break;
            }
            
            Resp.StatusCode = HttpStatusCode.OK;
            var Cookie = new CookieHeaderValue("session-id", RandVal);
            Cookie.Path = "/";

            Resp.Headers.AddCookies(new[] { Cookie });

            TekstSkladiste.LogUpisi("Korisnik " + credentials.Username + " ulogovan.");
            return ResponseMessage(Resp);
        
            
        }

        public static string Autorizacija(string cookie)
        {
            if (ActiveSessions.ContainsKey(cookie))
            {
                return ActiveSessions[cookie].Uloga;
            }
            else
            {
                return "Nepoznat";
            }
        }
    }
}