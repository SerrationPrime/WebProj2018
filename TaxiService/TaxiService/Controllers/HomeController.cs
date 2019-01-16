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
                case ("Dispecer"):
                    return Redirect("/Dispecer");
                case ("Vozac"):
                    return Redirect("/Vozac");
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
            if (ValidAuth("Musterija"))
            {
                return View("Index");
            }
            else
            {
                return Redirect("/");
            }
        }

        public ActionResult Vozac()
        {
            if (ValidAuth("Vozac"))
            {
                return View("Index");
            }
            else
            {
                return Redirect("/");
            }
        }

        public ActionResult Dispecer()
        {
            if (ValidAuth("Dispecer"))
            {
                return View("Index");
            }
            else
            {
                return Redirect("/");
            }
        }

        public ActionResult MusterijaTemplate()
        {
            return View();
        }

        public ActionResult DispecerTemplate()
        {
            return View();
        }

        public ActionResult VozacTemplate()
        {
            return View();
        }

        public bool ValidAuth(string uloga)
        {
            //Kontrola potrebna u slucaju da neko ukuca URL direktno u adresu
            string sessionId;
            try
            {
                sessionId = Request.Cookies["session-id"].Value;
            }
            catch
            {
                return false;
            }
            if (LoginController.Autorizacija(sessionId) != uloga)
            {
                return false;
            }
            return true;
    }
    }
}
