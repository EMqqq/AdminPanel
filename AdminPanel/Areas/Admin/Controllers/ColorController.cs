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

namespace AdminPanel.Areas.Admin.Controllers
{
    public class ColorController : AdminController
    {
        AdminPanelContext db = new AdminPanelContext();

        public ActionResult Index()
        {
            List<Color> Colors = db.Colors.Include(f => f.FilePath).OrderBy(c => c.ColorName).ToList();
            return View(Colors);
        }

        [HttpGet]
        public ActionResult AddColor()
        {
            Color color = new Color();

            color.FilePath = new FilePath();

            return View(color);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddColor(Color color, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                color.FilePath = new FilePath();

                if (upload != null && upload.ContentLength > 0)
                {
                    FilePath colorImage = new FilePath()
                    {
                        FileType = FileType.colorImage,
                        FileName = Path.GetFileName(upload.FileName),
                    };

                    color.FilePath = colorImage;
                    color.FilePathId = colorImage.FilePathId;

                    upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Colors"), colorImage.FileName));
                }

                db.Colors.Add(color);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(color);
            }
        }

        [HttpGet]
        public ActionResult EditColor(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Color color = db.Colors.Include(f => f.FilePath).Where(c => c.ColorId == id).FirstOrDefault();

            if (color == null)
            { return HttpNotFound(); }

            EditColorViewModel model = new EditColorViewModel()
            {
                Id = color.ColorId,
                Name = color.ColorName,
                FilePath = color.FilePath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditColor(EditColorViewModel model, HttpPostedFileBase upload)
        {
            Color color = await db.Colors.Include(f => f.FilePath).Where(c => c.ColorId == model.Id).FirstOrDefaultAsync();

            model.FilePath = color.FilePath;

            FilePath actualImage = color.FilePath;

            string actualImagePath = null;

            if (actualImage != null)
            {
                actualImagePath = Request.MapPath("~/Content/Images/Colors/" + actualImage.FileName);
            }

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (actualImagePath != null)
                    {
                        System.IO.File.Delete(actualImagePath);
                    }
                    color.FilePath = new FilePath();

                    FilePath colorImage = new FilePath
                    {
                        FileType = FileType.colorImage,
                        FileName = Path.GetFileName(upload.FileName),
                    };

                    color.FilePath = colorImage;
                    color.FilePathId = colorImage.FilePathId;

                    upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Colors"), colorImage.FileName));
                }

                color.ColorName = model.Name;
                db.Entry(color).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteColor(int colorId)
        {
            Color color = db.Colors.Where(c => c.ColorId == colorId).Single();

            FilePath image = color.FilePath;

            string filePath = Request.MapPath("~/Content/Images/Colors/" + image.FileName);
            System.IO.File.Delete(filePath);

            db.FilePaths.Remove(image);
            db.Colors.Remove(color);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}