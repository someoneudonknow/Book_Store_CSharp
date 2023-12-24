﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
	[Authorize(Roles = "Customer,Shipper,Staff")]

	public class CUSTOMER_ORDERController : Controller
    {
        private BookStoreManagerEntities db = new BookStoreManagerEntities();

        // GET: CUSTOMER_ORDER
        //public ActionResult Index()
        //{

        //    //var cUSTOMER_ORDER = db.CUSTOMER_ORDER.Include(c => c.Person).Include(c => c.Person1).Include(c => c.Person2);
        //    var id = User.Identity.GetUserId();
        //    var currentCus = db.People.FirstOrDefault(c => c.AccountID == id).PersonID;
        //    var currentRole = db.AspNetUsers.FirstOrDefault(p => p.Id == id).AspNetRoles.FirstOrDefault().Id;
        //    var cUSTOMER_ORDER = db.CUSTOMER_ORDER.Include(c => c.Person).Include(c => c.Person1).Include(c => c.Person2);
            

        //    if (Convert.ToInt32(currentRole) == 3 /*|| Convert.ToInt32(currentRole) == 2*/)
        //    {
        //        var currentStatus = from CUSTOMER_ORDER_STATUS in db.CUSTOMER_ORDER_STATUS where CUSTOMER_ORDER_STATUS.StatusID == 6 select CUSTOMER_ORDER_STATUS.OrderID;
        //        int[] status = currentStatus.ToArray();
        //        cUSTOMER_ORDER = from CUSTOMER_ORDER in db.CUSTOMER_ORDER where status.Contains(CUSTOMER_ORDER.OrderID) select CUSTOMER_ORDER;
        //    }
        //    ViewBag.currentRole = db.AspNetUsers.FirstOrDefault(p => p.Id == id).AspNetRoles.FirstOrDefault().Id;
        //    ViewBag.cusGiven = currentCus;
        //    return View(cUSTOMER_ORDER.ToList());
        //}

        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var currentCus = db.People.FirstOrDefault(c => c.AccountID.Equals(id)).PersonID;
            var currentRole = db.AspNetUsers.FirstOrDefault(p => p.Id == id).AspNetRoles.FirstOrDefault().Id;
            var cUSTOMER_ORDER = db.CUSTOMER_ORDER.Where(c => c.CustomerID == currentCus);
            if(Convert.ToInt32(currentRole) == 3)
            {
                int[] cusID = db.V_cus_order_status.Select(v => v.OrderID).ToArray();
                cUSTOMER_ORDER = from CUSTOMER_ORDER in db.CUSTOMER_ORDER where cusID.Contains(CUSTOMER_ORDER.OrderID) select CUSTOMER_ORDER;
                var shipConfirm = from SHIP_CONFIRMATION in db.SHIP_CONFIRMATION select SHIP_CONFIRMATION.OrderID;
                int[] confirm = shipConfirm.ToArray();
                ViewBag.confirm = confirm;
            }
            else if (Convert.ToInt32(currentRole) == 2)
            {
                int[] cusID = db.V_cus_order_status_not_success.Select(v => v.OrderID).ToArray();
                cUSTOMER_ORDER = from CUSTOMER_ORDER in db.CUSTOMER_ORDER where cusID.Contains(CUSTOMER_ORDER.OrderID) select CUSTOMER_ORDER;
            }
            ViewBag.currentRole = currentRole;
            ViewBag.cusGiven = currentCus;
            if(User.Identity.IsAuthenticated && User.IsInRole("Manager"))
            {
                cUSTOMER_ORDER = db.CUSTOMER_ORDER;
                return View("IndexForManager", cUSTOMER_ORDER.ToList());
            }

            if (TempData["ErrorMessage"] != null)
            {
                string errorMessage = TempData["ErrorMessage"].ToString();
                TempData.Remove("ErrorMessage");
                ViewBag.ErrorMessage = errorMessage;
            }

            if (TempData["SuccessMessage"] != null)
            {
                string successMessage = TempData["SuccessMessage"].ToString();
                TempData.Remove("SuccessMessage");
                ViewBag.SuccessMessage = successMessage;
            }

            if (TempData["WarningMessage"] != null)
            {
                string warningMessage = TempData["WarningMessage"].ToString();
                TempData.Remove("WarningMessage");
                ViewBag.WarningMessage = warningMessage;
            }

            return View(cUSTOMER_ORDER.ToList());
        }

        // GET: CUSTOMER_ORDER/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUSTOMER_ORDER cUSTOMER_ORDER = db.CUSTOMER_ORDER.Find(id);
            ViewBag.OrderDetailList = db.CUSTOMER_ORDER_DETAIL.Where(m => m.OrderID == id).ToList();

            return View(cUSTOMER_ORDER);
        }

        // GET: CUSTOMER_ORDER/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.People, "PersonID", "PersonName");
            ViewBag.ShipperID = new SelectList(db.People, "PersonID", "PersonName");
            ViewBag.StaffID = new SelectList(db.People, "PersonID", "PersonName");
            return View();
        }

        // POST: CUSTOMER_ORDER/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderDate,OrderTotalPrice,OrderStatus,OrderShippingMethod,OrderPaymentMethod,StaffID,CustomerID,ShipperID")] CUSTOMER_ORDER cUSTOMER_ORDER)
        {
            if (ModelState.IsValid)
            {
                db.CUSTOMER_ORDER.Add(cUSTOMER_ORDER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.CustomerID);
            ViewBag.ShipperID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.ShipperID);
            ViewBag.StaffID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.StaffID);
            return View(cUSTOMER_ORDER);
        }

        // GET: CUSTOMER_ORDER/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUSTOMER_ORDER cUSTOMER_ORDER = db.CUSTOMER_ORDER.Find(id);
            if (cUSTOMER_ORDER == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.CustomerID);
            ViewBag.ShipperID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.ShipperID);
            ViewBag.StaffID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.StaffID);
            return View(cUSTOMER_ORDER);
        }

        // POST: CUSTOMER_ORDER/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,OrderDate,OrderTotalPrice,OrderStatus,OrderShippingMethod,OrderPaymentMethod,StaffID,CustomerID,ShipperID")] CUSTOMER_ORDER cUSTOMER_ORDER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cUSTOMER_ORDER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.CustomerID);
            ViewBag.ShipperID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.ShipperID);
            ViewBag.StaffID = new SelectList(db.People, "PersonID", "PersonName", cUSTOMER_ORDER.StaffID);
            return View(cUSTOMER_ORDER);
        }

        // GET: CUSTOMER_ORDER/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUSTOMER_ORDER cUSTOMER_ORDER = db.CUSTOMER_ORDER.Find(id);
            if (cUSTOMER_ORDER == null)
            {
                return HttpNotFound();
            }
            return View(cUSTOMER_ORDER);
        }

        // POST: CUSTOMER_ORDER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CUSTOMER_ORDER cUSTOMER_ORDER = db.CUSTOMER_ORDER.Find(id);
            db.CUSTOMER_ORDER.Remove(cUSTOMER_ORDER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Cancel
        public ActionResult CancelByCustomer(int id)
        {
            CUSTOMER_ORDER cUSTOMER_ORDER = db.CUSTOMER_ORDER.Find(id);
            if (cUSTOMER_ORDER.CUSTOMER_ORDER_STATUS.LastOrDefault(m => m.OrderID == cUSTOMER_ORDER.OrderID).StatusID == 2)
            {
                CUSTOMER_ORDER_STATUS sTATUS = new CUSTOMER_ORDER_STATUS();
                sTATUS.OrderID = cUSTOMER_ORDER.OrderID;
                sTATUS.StatusID = 3;
                sTATUS.UpdateTime = DateTime.Now;
                db.CUSTOMER_ORDER_STATUS.Add(sTATUS);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Hủy đơn thành công";
            }
            return RedirectToAction("Index", "Manage");
        }

        //[ValidateAntiForgeryToken]
        public ActionResult CancelByNotDelivering(int id)
        {
            CUSTOMER_ORDER_STATUS cusOrderStatus = new CUSTOMER_ORDER_STATUS();
            cusOrderStatus.OrderID = id;
            cusOrderStatus.StatusID = 8;
            cusOrderStatus.UpdateTime = DateTime.Now;
            db.CUSTOMER_ORDER_STATUS.Add(cusOrderStatus);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return Json(new { redirectToAction = true, actionUrl = Url.Action("Index") });
        }

        public ActionResult ChangeStatus(int id)
        {
            var editionID = db.CUSTOMER_ORDER_DETAIL.FirstOrDefault(e => e.OrderID == id).EditionID;
            var amount = db.CUSTOMER_ORDER_DETAIL.FirstOrDefault(e => e.OrderID == id).DetailQuantity;
            var amountAvailable = db.STOCK_INVENTORY.FirstOrDefault(a => a.EditionID == editionID).InventoryAvailableStock;
            var currentStatus = db.CUSTOMER_ORDER_STATUS.ToList().LastOrDefault(c => c.OrderID == id).StatusID;

            if (amountAvailable < amount && currentStatus == 2)
            {
                TempData["ErrorMessage"] = "Không đủ số lượng";
                return RedirectToAction("Index", "CUSTOMER_ORDER");
            }
            var parameter1 = new SqlParameter("@orderID", id);
            db.Database.ExecuteSqlCommand("sp_switch_status @orderID", parameter1);
            db.SaveChanges();

            string accID = User.Identity.GetUserId();
            var CustomerID = db.People.FirstOrDefault(p => p.AccountID == accID).PersonID;

            var cusOrderStatus = db.CUSTOMER_ORDER_STATUS.ToList().LastOrDefault(o => o.OrderID == id).StatusID;
            if (Convert.ToInt32(cusOrderStatus) == 4)
            {
                CUSTOMER_ORDER cusOrder = db.CUSTOMER_ORDER.FirstOrDefault(o => o.OrderID == id);
                cusOrder.StaffID = Convert.ToInt32(CustomerID);
            }
            //else if (Convert.ToInt32(cusOrderStatus) == 6)
            //{
            //    var deliverAccID = db.AspNetRoles.FirstOrDefault(a => a.Id == "3").AspNetUsers.FirstOrDefault().Id;
            //    var deliverID = db.People.FirstOrDefault(d => d.AccountID == deliverAccID).PersonID;
            //    CUSTOMER_ORDER cusOrder = db.CUSTOMER_ORDER.FirstOrDefault(o => o.OrderID == id);
            //    cusOrder.ShipperID = Convert.ToInt32(deliverID);
            //}
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Deliver(int id)
        {
            string accID = User.Identity.GetUserId();
            var CustomerID = db.People.FirstOrDefault(p => p.AccountID == accID).PersonID;
            var cusOrderStatus = db.CUSTOMER_ORDER_STATUS.OrderByDescending(o => o.StatusID).FirstOrDefault(c => c.OrderID == id).StatusID;
            CUSTOMER_ORDER cusOrder = db.CUSTOMER_ORDER.FirstOrDefault(o => o.OrderID == id);

            if (Convert.ToInt32(cusOrderStatus) == 6 && cusOrder.ShipperID == null)
            {
                cusOrder.ShipperID = CustomerID;
            }
            else if(Convert.ToInt32(cusOrderStatus) == 6 && cusOrder.ShipperID != null)
            {
                cusOrder.ShipperID = null;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DetailOrder(int id)
        {
            CUSTOMER_ORDER cUSTOMER_ORDER = db.CUSTOMER_ORDER.Find(id);
            ViewBag.OrderDetailList = db.CUSTOMER_ORDER_DETAIL.Where(m => m.OrderID == id).ToList();
            return View(cUSTOMER_ORDER);
        }

        public ActionResult RenderPartialView(int orderid)
        {
            ViewBag.orderID = orderid;
            return PartialView("_CreatePartialViewShipConfirm");
        }

        public ActionResult CreateShipConfirm([Bind(Include = "ConfirmationID,ConfirmationImage,OrderID")] SHIP_CONFIRMATION shipConfirm)
        {
            shipConfirm.ConfirmationDate = DateTime.Now;
            var id = User.Identity.GetUserId();
            var currentCus = db.People.FirstOrDefault(c => c.AccountID.Equals(id)).PersonID;
            shipConfirm.ShipperID = Convert.ToInt32(currentCus);

            if (ModelState.IsValid)
            {
                bool isAttached = false;
                foreach (string filename in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[filename];
                    if (file != null && file.ContentLength > 0)
                    {
                        isAttached = true;
                        break;
                    }
                }
                if (isAttached)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        string uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                        var path = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                        file.SaveAs(path);
                        shipConfirm.ConfirmationImage = uniqueFileName;
                        db.SHIP_CONFIRMATION.Add(shipConfirm);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
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
    }
}
