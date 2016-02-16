using System.Linq;
using System.Web.Mvc;
using AdminPanel.Abstract;
using AdminPanel.Entities;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class SizeController : AdminController
    {
        private ISizesRepository repository;

        public SizeController(ISizesRepository repository)
        { this.repository = repository; }

        public ActionResult Index()
        {
            return View(repository.GetSizes().OrderBy(s => s.SizeId));
        }

        public ActionResult AddSize()
        {
            Size size = new Size();

            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSize(Size size)
        {
            if (ModelState.IsValid)
            {
                repository.Add(size);
                return RedirectToAction("Index");
            }
            else
            {
                return View(size);
            }
        }

        [HttpPost]
        public ActionResult DeleteSize(int sizeId)
        {
            Size size = repository.GetSizes().FirstOrDefault(i => i.SizeId == sizeId);
            repository.Delete(size);
            return RedirectToAction("Index");
        }
    }
}