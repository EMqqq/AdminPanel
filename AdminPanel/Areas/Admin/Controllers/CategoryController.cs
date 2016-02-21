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
        private ITRepository<AdminPanelContext, Category> repository;

        public CategoryController(ITRepository<AdminPanelContext, Category> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// GET: Admin/Category
        /// </summary>
        /// <returns> display categories from database </returns>
        public ActionResult Index()
        {
            return View(repository.GetAll.OrderBy(c => c.CategoryName));
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
        public ActionResult AddCategory(Category category)
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
        public ActionResult EditCategory(int id)
        {
            Category category = repository.Get(id);

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
        public ActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                repository.Update(category);
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
            Category category = repository.Get(categoryId);
            repository.Delete(category);

            return RedirectToAction("Index");
        }
    }
}