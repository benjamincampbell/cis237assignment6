using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageBCampbellEntities db = new BeverageBCampbellEntities();

        // GET: /Beverages/
        public ActionResult Index()
        {
            //return View(db.Beverages.ToList());

            DbSet<Beverage> CarsToSearch = db.Beverages;

            string filterName = "";
            string filterMin = "";
            string filterMax = "";

            decimal min = 0;
            decimal max = 1000;

            //Check to see there is a value in the session, and if there is, assign it to the
            //variable that we setup to hold the value.
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }
            //same as above but for min, and we are parsing the string
            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = decimal.Parse(filterMin);
            }
            //same as above but for max, and we are parsing the string
            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = decimal.Parse(filterMax);
            }

            //Do the filter on the CarsToSearch Dataset. User the Where that we used before
            //when doing EF work, only this time send in more lambda expressions to narrow it
            //down further. Since we setup default values for each of the filter parameters,
            //min, max, and filterMake, we can count on this always running with no errors.
            IEnumerable<Beverage> filtered = CarsToSearch.Where(beverage => beverage.price >= min &&
                                                                  beverage.price <= max &&
                                                                  beverage.name.Contains(filterName));

            //Convert the database set to a list now that the query work is done on it.
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            //Place the string representation of the values in the session into the
            //ViewBag so that they can be retrived and displayed on the view.
            ViewBag.filterName = filterName;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            //Return the view with a filtered selection of the cars. 
            return View(finalFiltered);

            //This is what used to be returned before a filter was setup.
            //return View(db.Cars.ToList());
        }

        // GET: /Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: /Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: /Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: /Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: /Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: /Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost, ActionName("Filter")]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Get the form data that was sent out of the Request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control.
            String name = Request.Form.Get("name");
            String min = Request.Form.Get("min");
            String max = Request.Form.Get("max");

            //Store the form data into the session so that it can be retrived later
            //on to filter the data.
            Session["name"] = name;
            Session["min"] = min;
            Session["max"] = max;

            //Redirect the user to the index page. We will do the work of actually
            //fiiltering the list in the index method.
            return RedirectToAction("Index");
        }
    }
}
