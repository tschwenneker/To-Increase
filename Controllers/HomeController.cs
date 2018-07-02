using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BisConnectivityServices.Models;

namespace BisConnectivityServices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Business Connectivity";
            ViewBag.Title = "Busines integration solution";

            AxAccessModel access = new AxAccessModel();
            return View(access);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Business Connectivity";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Business Connectivity";

            return View();
        }
    }
}
