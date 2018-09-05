using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxiService.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Osnovna strana";

            string sessionId;

            try
            {
                sessionId = Request.Cookies["session-id"].Value;
            }
            catch
            {
                return View();
            }
            switch (LoginController.Autorizacija(sessionId))
            {
                case ("Musterija"):
                    return Redirect("/Musterija");
                default:
                    break;
            }
            return View();
        }
        public ActionResult HomeTemplate()
        {
            return View();
        }

        public ActionResult Musterija()
        {
            //Kontrola potrebna u slucaju da neko ukuca URL direktno u adresu
            string sessionId;
            try
            {
                sessionId = Request.Cookies["session-id"].Value;
            }
            catch
            {
                return Redirect("/");
            }
            if (LoginController.Autorizacija(sessionId) != "Musterija")
            {
                return Redirect("/");
            }
            return View("Index");
        }

        public ActionResult MusterijaTemplate()
        {
            return View();
        }
    }
}
