using System.Linq;
using System.Web.Mvc;
using AdminPanel.Abstract;
using AdminPanel.Entities;
using AdminPanel.DataAccessLayer;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class SizeController : AdminController
    {
        private ITRepository<AdminPanelContext, Size> repository;

        public SizeController(ITRepository<AdminPanelContext, Size> repository)
        { this.repository = repository; }

        /// <summary>
        /// GET: Admin/Size
        /// </summary>
        /// <returns>  display sizes from database  </returns>
        public ActionResult Index()
        {
            return View(repository.GetAll.OrderBy(s => s.SizeId));
        }

        /// <summary>
        /// GET: Admin/Size/AddSize
        /// </summary>
        /// <returns> size's add form </returns>
        [HttpGet]
        public ActionResult AddSize()
        {
            Size size = new Size();
            return View(size);
        }

        /// <summary>
        /// add new size to database
        /// </summary>
        /// <param name="size"> size from GET Method </param>
        /// <returns> add category or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSize(Size size)
        {
            if (ModelState.IsValid)
            {
                repository.Add(size);
                return RedirectToAction("Index");
            }

            return View(size);
        }

        /// <summary>
        /// delete size from database
        /// </summary>
        /// <param name="sizeId"> size's id </param>
        /// <returns> GET: Admin/Size </returns>
        [HttpPost]
        public ActionResult DeleteSize(int sizeId)
        {
            Size size = repository.Get(sizeId);
            repository.Delete(size);

            return RedirectToAction("Index");
        }
    }
}