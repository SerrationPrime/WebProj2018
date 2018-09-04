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
        static Dictionary<string, string> ActiveSessions = new Dictionary<string, string>();

        [HttpPost]
        public IHttpActionResult Post(UserPass credentials)
        {
            Random rnd = new Random();
            var RandVal = rnd.Next(0, int.MaxValue).ToString();

            switch (TekstSkladiste.Autentikacija(credentials.Username, credentials.Password)){
                case ("Musterija"):
                    ActiveSessions.Add(RandVal, "Musterija");
                    break;
                case ("Vozac"):
                    ActiveSessions.Add(RandVal, "Vozac");
                    break;
                case ("Dispecer"):
                    ActiveSessions.Add(RandVal, "Dispecer");
                    break;
                default:
                    TekstSkladiste.LogUpisi("Pokusan login sa nepravilnim podacima, username: " + credentials.Username + ".");
                    return Unauthorized();
            }
            var Resp = new HttpResponseMessage();
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
                return ActiveSessions[cookie];
            }
            else
            {
                return "Nepoznat";
            }
        }
    }
}