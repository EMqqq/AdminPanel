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

            supplier.FilePath = new FilePath();

            PopulateDeliveryMethodDropDownList(supplier);

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
            PopulateDeliveryMethodDropDownList();

            if (ModelState.IsValid)
            {
                supplier.FilePath = new FilePath();

                if (upload != null && upload.ContentLength > 0)
                {

                    FilePath photo = new FilePath()
                    {
                        FileType = FileType.supplierImage,
                        FileName = Path.GetFileName(upload.FileName),
                    };

                    supplier.FilePath = photo;
                    supplier.FilePathId = photo.FilePathId;

                    upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Suppliers"), photo.FileName));
                }

                db.Suppliers.Add(supplier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(supplier);
            }
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
                getDeliveryMethods = PopulateDeliveryMethods(supplier)
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

            model.FilePath = supplier.FilePath;
            model.getDeliveryMethods = PopulateDeliveryMethods(supplier);

            FilePath actualImage = supplier.FilePath;

            string actualImagePath = null;

            if (actualImage != null)
            {
                actualImagePath = Request.MapPath("~/Content/Images/Suppliers/" + actualImage.FileName);
            }

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (actualImagePath != null)
                    {
                        System.IO.File.Delete(actualImagePath);
                    }

                    supplier.FilePath = new FilePath();

                    FilePath photo = new FilePath()
                    {
                        FileType = FileType.supplierImage,
                        FileName = Path.GetFileName(upload.FileName),
                    };

                    supplier.FilePath = photo;
                    supplier.FilePathId = photo.FilePathId;

                    upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Suppliers"), photo.FileName));
                }

                supplier.DeliveryMethodId = model.DeliveryMethodId;
                supplier.Name = model.Name;
                supplier.Price = model.Price;
                supplier.TransportTime = model.TransportTime;

                db.Entry(supplier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// delete supplier from database
        /// </summary>
        /// <param name="id"> supplier's id </param>
        /// <returns> GET: Admin/Supplier </returns>
        [HttpPost]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            Supplier supplier = await db.Suppliers.Where(s => s.SupplierId == id).FirstOrDefaultAsync();

            FilePath image = supplier.FilePath;

            string filePath = Request.MapPath("~/Content/Images/Suppliers/" + image.FileName);
            System.IO.File.Delete(filePath);

            db.FilePaths.Remove(image);
            db.Suppliers.Remove(supplier);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        ///  HELPERS
        /// </summary>

        private SelectList PopulateDeliveryMethods(Supplier supplier)
        {
            IQueryable<DeliveryMethod> deliveryMethodQuery = from c in db.DeliveryMethods
                                                             orderby c.DeliveryMethodId
                                                             select c;
            return new SelectList(deliveryMethodQuery, "DeliveryMethodId", "Name", supplier.DeliveryMethodId);
        }

        private void PopulateDeliveryMethodDropDownList(object selectedDeliveryMethod = null)
        {
            IOrderedQueryable<DeliveryMethod> deliveryMethodQuery = from c in db.DeliveryMethods
                                                                    orderby c.DeliveryMethodId
                                                                    select c;
            ViewBag.DeliveryMethodID = new SelectList(deliveryMethodQuery, "DeliveryMethodId", "Name", selectedDeliveryMethod);
        }
    }
}