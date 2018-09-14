using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TrashCollector2.Models;

namespace TrashCollector2.Controllers
{
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Password,FullName,ZipCode,UserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var currentUser = (from u in db.Users where u.Id == userId select u).First();
                employee.UserId = userId;
                employee.EmailAddress = currentUser.Email;
                employee.Password = currentUser.PasswordHash;
                
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("EmployeeTodayPickups");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmailAddress,UserName,Password,FullName,ZipCode,UserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EmployeeTodayPickups");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("EmployeeTodayPickups");
        }


        public ActionResult DisplayError()
        {
            return View();
        }

        // GET: Pickups
        public ActionResult EmployeeTodayPickups()
        {
            var userId = User.Identity.GetUserId();
            var currentEmployee = (from e in db.Employees where e.UserId == userId select e).FirstOrDefault();
            var todayDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var todayDate = DateTime.Now.Date;
            var customersMatchingZip = (from c in db.Customers where c.ZipCode == currentEmployee.ZipCode select c).ToList();

            if (!customersMatchingZip.Any())
            {
                return View();
            }
            else
            {
                var checkTodayPickups = db.Customers.Where(c => (c.OneTimePickupDate == todayDate || c.WeeklyPickupDay == todayDayOfWeek) && (c.IsOnHold != true) && (c.IsConfirmed != true) && c.ZipCode == currentEmployee.ZipCode).ToList();
                if (!checkTodayPickups.Any())
                {
                    return View();
                }
                else
                {
                    return View(checkTodayPickups);
                }
            }
        }
        
        public ActionResult ConfirmPickup(string id)
        {
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        [HttpPost, ActionName("ConfirmPickup")]
        public ActionResult ConfirmedPickup(string id)
        {
            var currentCustomer = (from c in db.Customers where c.UserId == id select c).FirstOrDefault();
            var moneyOwed = ChargeCustomer(currentCustomer);

            if (currentCustomer != null) currentCustomer.MoneyOwed = moneyOwed;
            if (currentCustomer != null) currentCustomer.IsConfirmed = true;
            if (currentCustomer != null && currentCustomer.OneTimePickupDate == DateTime.Now.Date)
            {
                currentCustomer.OneTimePickupDate = null;
            }

            db.Entry(currentCustomer).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EmployeeTodayPickups");
        }

        public ActionResult FilterPickupsByWeekday()
        {
            return View();
        }

        [HttpPost, ActionName("FilterPickupsByWeekday")]
        public ActionResult FilteredPickupsByWeekday(string chosenDay)
        {
            return RedirectToAction("ViewPickupsChosenByWeekday", new { day = chosenDay });
        }

        public ActionResult ViewPickupsChosenByWeekday(string day)
        {
            var userId = User.Identity.GetUserId();
            var currentEmployee = (from e in db.Employees where e.UserId == userId select e).FirstOrDefault();
            var customersMatchingZip = (from c in db.Customers where c.ZipCode == currentEmployee.ZipCode select c).ToList();
            List<Customer> specificDayCustomers = new List<Customer>();

            if (customersMatchingZip.Any())
            {

                foreach (var cust in customersMatchingZip)
                {
                    var pickupDateString = cust.OneTimePickupDate.ToString();
                    var specificDatePickup = DateTime.Parse(pickupDateString).DayOfWeek.ToString();

                    if ((cust.WeeklyPickupDay == day || specificDatePickup == day) && !cust.IsOnHold && cust.IsConfirmed != true)
                    {
                        specificDayCustomers.Add(cust);
                    }
                }
            }

            return View(specificDayCustomers);
        }

        public ActionResult SeeCustomersOnMap(string id)
        {
            var userId = User.Identity.GetUserId();
            var currentEmployee = (from e in db.Employees where e.UserId == userId select e).FirstOrDefault();
            var employeeZip = currentEmployee.ZipCode;
            ViewBag.zip = employeeZip;
            var todayDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var todayDate = DateTime.Now.Date;
            var customersMatchingZip = (from c in db.Customers where c.ZipCode == currentEmployee.ZipCode select c).ToList();
            if (id != null)
            {
                var customer = (from c in db.Customers where c.UserId == id select c).ToList();

                return View(customer);
            }

            if (!customersMatchingZip.Any())
            {
                return View();
            }
            else
            {
                var checkTodayPickups = db.Customers.Where(c => (c.OneTimePickupDate == todayDate || c.WeeklyPickupDay == todayDayOfWeek) && (c.IsOnHold != true) && (c.IsConfirmed != true) && c.ZipCode == currentEmployee.ZipCode).ToList();
                if (!checkTodayPickups.Any())
                {
                    return View();
                }
                else
                {
                    return View(checkTodayPickups);
                }
            }

            
        }

        public ActionResult ViewCustomersOnMap(string customer)
        {
            // DO NOT USE!!!!!!!!
            return View(db.Customers.ToList());
        }

        public int? ChargeCustomer(Customer customer)
        {

            customer.MoneyOwed = customer.MoneyOwed + 100;
            var money = customer.MoneyOwed;

            return money;
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
