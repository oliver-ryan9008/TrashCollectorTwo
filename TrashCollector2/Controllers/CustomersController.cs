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
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        public ActionResult CustomerHome()
        {
            var userId = User.Identity.GetUserId();
            var customer = (from c in db.Customers where c.UserId == userId select c).FirstOrDefault();
            return View(customer);
        }

        public ActionResult MoneyOwed()
        {
            var userId = User.Identity.GetUserId();
            var customer = (from c in db.Customers where c.UserId == userId select c).FirstOrDefault();

            return View(customer);
        }

        // GET: Customers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmailAddress,UserName,Password,FullName,StreetAddress,ZipCode,WeeklyPickupDay")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.MoneyOwed = 0;
                customer.IsOnHold = false;
                customer.UserId = User.Identity.GetUserId();
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("CustomerHome");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StreetAddress,ZipCode,WeeklyPickupDay")] Customer customer, string id)
        {
            
            if (ModelState.IsValid)
            {
                Customer updatedCustomer = db.Customers.Find(id);
                if (updatedCustomer == null)
                {
                    return RedirectToAction("DisplayError", "Employees");
                }

                updatedCustomer.StreetAddress = customer.StreetAddress;
                updatedCustomer.ZipCode = customer.ZipCode;
                updatedCustomer.WeeklyPickupDay = customer.WeeklyPickupDay;
                
                db.Entry(updatedCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CustomerHome");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Customer customer = db.Customers.Find(id);

            return RedirectToAction("SorryToSeeYouGo", "Home");
        }



        public ActionResult DelayPickupsForTime()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DelayPickupsForTime([Bind(Include = "StartOfDelayedPickup, EndOfDelayedPickup")] Customer customer)
        {
            var userId = User.Identity.GetUserId();
            var today = DateTime.Now.Date;
            var currentCustomer = (from c in db.Customers where userId == c.UserId select c).FirstOrDefault();
            if (currentCustomer != null)
            {
                currentCustomer.StartOfDelayedPickup = customer.StartOfDelayedPickup;
                currentCustomer.EndOfDelayedPickup = customer.EndOfDelayedPickup;

                if (today >= currentCustomer.StartOfDelayedPickup &&
                    today <= currentCustomer.EndOfDelayedPickup)
                {
                    currentCustomer.IsOnHold = true;
                }
                else
                {
                    currentCustomer.IsOnHold = false;
                }
                db.Entry(currentCustomer).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("CustomerHome");
        }

        public ActionResult ChooseNewOneTimePickup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseNewOneTimePickup([Bind(Include = "OneTimePickupDate")] Customer customer)
        {
            var userId = User.Identity.GetUserId();
            var identityToInt = userId;
            var currentCustomer = (from c in db.Customers where userId == c.UserId select c).First();
            currentCustomer.OneTimePickupDate = customer.OneTimePickupDate;

            db.Entry(currentCustomer).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("CustomerHome");
        }

        public ActionResult PayForPickups()
        {
            var userId = User.Identity.GetUserId();
            Customer currentCustomer = (from c in db.Customers where userId == c.UserId select c).First();

            return View(currentCustomer);
        }

        
        public ActionResult PayForPickupsConfirmed(string id)
        {
            Customer currentCustomer = (from c in db.Customers where c.UserId == id select c).First();

            currentCustomer.IsConfirmed = false;
            currentCustomer.MoneyOwed = 0;

            db.Entry(currentCustomer).State = EntityState.Modified;
            db.SaveChanges();

            return View(currentCustomer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
