using AdminPanel.DataAccessLayer;
using AdminPanel.Entities;
using AdminPanel.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using AdminPanel.Areas.Admin.ViewModels;
using System;
using AdminPanel.Abstract;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class ColorController : AdminController
    {
        private ITRepository<AdminPanelContext, Color> repository;
        private ITRepository<AdminPanelContext, FilePath> repoFilePath;

        public ColorController(ITRepository<AdminPanelContext, Color> repository,
                                ITRepository<AdminPanelContext, FilePath> repoFilePath)
        {
            this.repository = repository;
            this.repoFilePath = repoFilePath;
        }

        /// <summary>
        /// GET: Admin/Color
        /// </summary>
        /// <returns> display colors from database </returns>
        public ActionResult Index()
        {
            List<Color> Colors = repository.GetAll.OrderBy(c => c.ColorName).ToList();
            return View(Colors);
        }

        /// <summary>
        /// GET: Admin/Color/AddColor
        /// </summary>
        /// <returns> color's add form </returns>
        [HttpGet]
        public ActionResult AddColor()
        {
            Color color = new Color();
            return View(color);
        }

        /// <summary>
        /// add new color with image to database
        /// </summary>
        /// <param name="color"> color from GET Method </param>
        /// <param name="upload"> input file </param>
        /// <returns> add color or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddColor(Color color, HttpPostedFileBase upload)
        {
            if (upload == null)
            {
                ModelState.AddModelError("NoImage", "Upload color's image");
            }

            if (ModelState.IsValid)
            {
                Guid number = Guid.NewGuid();

                // assign upload to color and save on server
                FilePath colorImage = new FilePath()
                {
                    FileType = FileType.colorImage,
                    FileName = Path.GetFileName(number + "-" + upload.FileName)
                };

                color.FilePath = colorImage;
                color.FilePathId = colorImage.FilePathId;
                
                upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Colors"), colorImage.FileName));

                // save changes
                repository.Add(color);
                colorImage.Colors.Add(color);
                repository.Save();

                return RedirectToAction("Index");
            }

            // return if invalid
            return View(color);
        }

        /// <summary>
        /// GET: Admin/Color/EditColor
        /// </summary>
        /// <param name="id"> color's id </param>
        /// <returns> color's edit form </returns>
        [HttpGet]
        public ActionResult EditColor(int id)
        {
            Color color = repository.Get(id);

            if (color == null)
            {
                return HttpNotFound();
            }

            // assign default view model's values
            EditColorViewModel model = new EditColorViewModel()
            {
                Id = color.ColorId,
                Name = color.ColorName,
                FilePath = color.FilePath
            };

            return View(model);
        }

        /// <summary>
        /// edit color in database
        /// </summary>
        /// <param name="model"> view model from GET Method </param>
        /// <param name="upload"> input file ( if exists delete old color's image) </param>
        /// <returns> save color changes or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditColor(EditColorViewModel model, HttpPostedFileBase upload)
        {
            Color color = repository.Get(model.Id);
            FilePath actualImage = repoFilePath.GetAll.Where(c => c.FilePathId == color.FilePathId).FirstOrDefault();
            
            // assign name from model
            color.ColorName = model.Name;

            // get a path to image on server
            string actualImagePath = Request.MapPath("~/Content/Images/Colors/" + actualImage.FileName);

            if (ModelState.IsValid)
            {
                // check if upload exists
                // if exists delete old from server,
                // assign new to color and save on server
                if (upload != null && upload.ContentLength > 0)
                {
                    System.IO.File.Delete(actualImagePath);

                    Guid number = Guid.NewGuid();

                    FilePath colorImage = new FilePath
                    {
                        FileType = FileType.colorImage,
                        FileName = Path.GetFileName(number + "-" + upload.FileName)
                    };

                    color.FilePath = colorImage;
                    color.FilePathId = colorImage.FilePathId;
                    
                    upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Colors"), colorImage.FileName));
                }
                
                repository.Update(color);

                return RedirectToAction("Index");
            }

            // return if invalid
            return View(model);
        }

        /// <summary>
        /// delete color with image from database
        /// </summary>
        /// <param name="colorId"> color's id </param>
        /// <returns> GET: Admin/Color </returns>
        [HttpPost]
        public ActionResult DeleteColor(int colorId)
        {
            Color color = repository.Get(colorId);
            FilePath image = repoFilePath.GetAll.Where(c => c.FilePathId == color.FilePathId).Single();

            // get path to image and delete
            string filePath = Request.MapPath("~/Content/Images/Colors/" + image.FileName);
            System.IO.File.Delete(filePath);
            repoFilePath.Delete(image);

            // delete color
            repository.Delete(color);

            return RedirectToAction("Index");
        }
    }
}