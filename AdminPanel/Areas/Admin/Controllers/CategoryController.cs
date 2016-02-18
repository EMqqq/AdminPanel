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

        /// <summary>
        /// GET: Admin/Category
        /// </summary>
        /// <returns> display categories from database </returns>
        public ActionResult Index()
        {
            return View(repository.GetCategories().OrderBy(c => c.CategoryName));
        }

        /// <summary>
        /// GET: Admin/Category/AddCategory
        /// </summary>
        /// <returns> category's add form </returns>
        [HttpGet]
        public ActionResult AddCategory()
        {
            Category category = new Category();
            return View(category);
        }

        /// <summary>
        /// add new category to database
        /// </summary>
        /// <param name="category"> category from GET Method </param>
        /// <returns> add category or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory([Bind(Include = "CategoryId, CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                repository.Add(category);
                return RedirectToAction("Index");
            }
            
            return View(category);
        }
        
        /// <summary>
        /// GET: Admin/Category/EditCategory
        /// </summary>
        /// <param name="id"> category's id </param>
        /// <returns> category's edit form </returns>
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
        
        /// <summary>
        /// edit category in database
        /// </summary>
        /// <param name="category"> category from GET method </param>
        /// <returns> save changes or display errors </returns>
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

        /// <summary>
        /// delete category from database
        /// </summary>
        /// <param name="categoryId"> category's id </param>
        /// <returns> GET: Admin/Category </returns>
        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            Category category = repository.GetCategories().FirstOrDefault(i => i.CategoryId == categoryId);
            repository.Delete(category);

            return RedirectToAction("Index");
        }
    }
}