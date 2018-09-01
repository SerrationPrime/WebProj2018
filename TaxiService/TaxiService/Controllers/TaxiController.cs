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
    public class TaxiController : ApiController
    {

        public async Task<IHttpActionResult> Post(Korisnik korisnik)
        {

            if (!TekstSkladiste.Upisi(korisnik))
            {
                return this.InternalServerError();
            }
            else
            {
                return this.Ok();
            }
        }
    }
}
