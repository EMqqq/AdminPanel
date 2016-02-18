using AdminPanel.Entities;
using AdminPanel.DataAccessLayer;
using AdminPanel.Areas.Admin.ViewModels;
using AdminPanel.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System;
using AdminPanel.Areas.Admin.Helpers;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class SuppliersController : AdminController
    {
        AdminPanelContext db = new AdminPanelContext();

        /// <summary>
        /// GET: Admin/Suppliers
        /// </summary>
        /// <returns> display suppliers from database </returns>
        public ActionResult Index()
        {
            List<Supplier> suppliers = db.Suppliers.Include(d => d.DeliveryMethod).Include(f => f.FilePath).OrderBy(c => c.Name).ToList();
            return View(suppliers);
        }

        /// <summary>
        /// GET: Admin/Suppliers/AddSupplier
        /// </summary>
        /// <returns> supplier's add form </returns>
        [HttpGet]
        public ActionResult AddSupplier()
        {
            Supplier supplier = new Supplier();

            // get delivery methods SelectList for supplier
            ViewBag.DeliveryMethodID = new SelectList(Retriever.GetDeliveryMethods(), "DeliveryMethodId", "Name", supplier);

            return View(supplier);
        }

        /// <summary>
        /// add new supplier with image to database
        /// </summary>
        /// <param name="supplier"> supplier from GET Method </param>
        /// <param name="upload"> input file </param>
        /// <returns> add supplier or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSupplier(Supplier supplier, HttpPostedFileBase upload)
        {
            if (upload == null)
            {
                ModelState.AddModelError("NoImage", "Upload supplier's image");
            }

            if (ModelState.IsValid)
            {
                Guid number = Guid.NewGuid();

                // assign upload to color and save on server
                FilePath image = new FilePath()
                {
                    FileType = FileType.supplierImage,
                    FileName = Path.GetFileName(number + "-" + upload.FileName),
                };

                supplier.FilePath = image;
                supplier.FilePathId = image.FilePathId;
                
                upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Suppliers"), image.FileName));

                // save changes
                db.Suppliers.Add(supplier);
                image.Suppliers.Add(supplier);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // return if invalid
            return View(supplier);
        }

        /// <summary>
        /// GET: Admin/Suppliers/EditSupplier
        /// </summary>
        /// <param name="id"> supplier's id </param>
        /// <returns> supplier's edit form </returns>
        [HttpGet]
        public ActionResult EditSupplier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Supplier supplier = db.Suppliers.Include(f => f.FilePath).FirstOrDefault(s => s.SupplierId == id);

            if (supplier == null)
            {
                return HttpNotFound();
            }

            // assign default view model's values
            EditSupplierViewModel model = new EditSupplierViewModel()
            {
                DeliveryMethodId = supplier.DeliveryMethodId,
                Id = supplier.SupplierId,
                Name = supplier.Name,
                Price = supplier.Price,
                TransportTime = supplier.TransportTime,
                Supplier = supplier,
                FilePathId = supplier.FilePathId,
                FilePath = supplier.FilePath,
                getDeliveryMethods = new SelectList(Retriever.GetDeliveryMethods(), "DeliveryMethodId", "Name", supplier.DeliveryMethodId)
            };

            return View(model);
        }

        /// <summary>
        /// edit supplier in database
        /// </summary>
        /// <param name="model"> view model from GET Method </param>
        /// <param name="upload"> input file ( if exists delete old supplier's image) </param>
        /// <returns> save supplier changes or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSupplier(EditSupplierViewModel model, HttpPostedFileBase upload)
        {
            Supplier supplier = await db.Suppliers.Include(f => f.FilePath).FirstOrDefaultAsync(s => s.SupplierId == model.Id);
            FilePath actualImage = db.FilePaths.Where(c => c.FilePathId == supplier.FilePathId).FirstOrDefault();
            
            // get a path to image on server
            string actualImagePath = Request.MapPath("~/Content/Images/Suppliers/" + actualImage.FileName);

            if (ModelState.IsValid)
            {
                // check if upload exists
                // if exists delete old from server,
                // assign new to color and save on server
                if (upload != null && upload.ContentLength > 0)
                {
                    System.IO.File.Delete(actualImagePath);

                    Guid number = Guid.NewGuid();

                    FilePath photo = new FilePath()
                    {
                        FileType = FileType.supplierImage,
                        FileName = Path.GetFileName(number + "-" + upload.FileName),
                    };

                    supplier.FilePath = photo;
                    supplier.FilePathId = photo.FilePathId;
                    
                    upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Suppliers"), photo.FileName));
                }

                // assign properties from model
                supplier.DeliveryMethodId = model.DeliveryMethodId;
                supplier.Name = model.Name;
                supplier.Price = model.Price;
                supplier.TransportTime = model.TransportTime;

                // save changes
                db.Entry(supplier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // return if invalid
            return View(model);
        }

        /// <summary>
        /// delete supplier with image from database
        /// </summary>
        /// <param name="id"> supplier's id </param>
        /// <returns> GET: Admin/Supplier </returns>
        [HttpPost]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            Supplier supplier = await db.Suppliers.Where(s => s.SupplierId == id).FirstOrDefaultAsync();
            FilePath image = await db.FilePaths.Where(f => f.FilePathId == supplier.FilePathId).FirstOrDefaultAsync();

            // get path to image and delete
            string filePath = Request.MapPath("~/Content/Images/Suppliers/" + image.FileName);
            System.IO.File.Delete(filePath);
            db.FilePaths.Remove(image);

            // delete supplier
            db.Suppliers.Remove(supplier);

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// close database connections
        /// </summary>
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