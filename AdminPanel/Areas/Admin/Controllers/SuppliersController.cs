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
using AdminPanel.Abstract;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class SuppliersController : AdminController
    {
        private ITRepository<AdminPanelContext, Supplier> repository;
        private ITRepository<AdminPanelContext, FilePath> repoFilePath;

        public SuppliersController(ITRepository<AdminPanelContext, Supplier> repository,
                                ITRepository<AdminPanelContext, FilePath> repoFilePath)
        {
            this.repository = repository;
            this.repoFilePath = repoFilePath;
        }

        /// <summary>
        /// GET: Admin/Suppliers
        /// </summary>
        /// <returns> display suppliers from database </returns>
        public ActionResult Index()
        {
            List<Supplier> suppliers = repository.GetAll.OrderBy(c => c.Name).ToList();
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
            ViewBag.DeliveryMethodID = new SelectList(Retriever.GetDeliveryMethods(), "DeliveryMethodId", "Name", null);

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
        public ActionResult AddSupplier(Supplier supplier, HttpPostedFileBase upload)
        {
            ViewBag.DeliveryMethodID = new SelectList(Retriever.GetDeliveryMethods(), "DeliveryMethodId", "Name", null);

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
                repository.Add(supplier);
                image.Suppliers.Add(supplier);
                repository.Save();

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
        public ActionResult EditSupplier(int id)
        {
            Supplier supplier = repository.Get(id);

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
        public ActionResult EditSupplier(EditSupplierViewModel model, HttpPostedFileBase upload)
        {
            Supplier supplier = repository.Get(model.Id);
            FilePath actualImage = repoFilePath.GetAll.Where(c => c.FilePathId == supplier.FilePathId).FirstOrDefault();
            
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
                repository.Update(supplier);
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
        public ActionResult DeleteSupplier(int id)
        {
            Supplier supplier = repository.Get(id);
            FilePath image = repoFilePath.GetAll.Where(f => f.FilePathId == supplier.FilePathId).FirstOrDefault();

            // get path to image and delete
            string filePath = Request.MapPath("~/Content/Images/Suppliers/" + image.FileName);
            System.IO.File.Delete(filePath);
            repoFilePath.Delete(image);

            // delete supplier
            repository.Delete(supplier);

            return RedirectToAction("Index");
        }
    }
}