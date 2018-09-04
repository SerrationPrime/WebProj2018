using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxiService.Controllers
{
    public class KontaktController : Controller
    {
        // GET: Kontakt
        public ActionResult Index()
        {
            ViewBag.Title = "Kontakt";

            return View("~/Views/Home/Index.cshtml");
        }
        public ActionResult KontaktTemplate()
        {
            return View();
        }
    }
}