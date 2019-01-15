using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class ControlController : ApiController
    {
        // Post: Control
        [HttpPost]
        public IHttpActionResult Post(string username)
        {
            if (username != "")
            {
                if (TekstSkladiste.BlokDeblok(username))
                    return this.Ok();
                else
                    return this.BadRequest();
            }
            else
                return this.BadRequest();
        }
    }
}