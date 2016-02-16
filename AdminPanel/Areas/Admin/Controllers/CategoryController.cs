using AdminPanel.Abstract;
using AdminPanel.DataAccessLayer;
using AdminPanel.Entities;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Data.Entity;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class CategoryController : AdminController
    {
        private ICategoriesRepository repository;

        public CategoryController(ICategoriesRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            return View(repository.GetCategories().OrderBy(c => c.CategoryName));
        }

        public ActionResult AddCategory()
        {
            Category category = new Category();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory([Bind(Include = "CategoryId, CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                repository.Add(category);
                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
        }

        [HttpGet]
        public ActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = repository.GetCategories().Where(c => c.CategoryId == id).FirstOrDefault();

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include = "CategoryId, CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                using (AdminPanelContext db = new AdminPanelContext())
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            Category category = repository.GetCategories().FirstOrDefault(i => i.CategoryId == categoryId);
            repository.Delete(category);
            return RedirectToAction("Index");
        }
    }
}