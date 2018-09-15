using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TrashCollector2.Models;

namespace TrashCollector2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeTodayPickups", "Employees");
            }
            if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("CustomerHome", "Customers");
                }

            return View();
            
        }
        [HttpPost, ActionName("Index")]
        public ActionResult VisitorHome()
        {
            return RedirectToAction("Register", "Account");
        }

        
        public ActionResult SorryToSeeYouGo()
        {            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
    }
}