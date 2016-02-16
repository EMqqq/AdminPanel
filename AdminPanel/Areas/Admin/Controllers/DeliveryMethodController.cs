using AdminPanel.Entities;
using AdminPanel.Abstract;
using AdminPanel.DataAccessLayer;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class DeliveryMethodController : AdminController
    {
        private IDeliveryMethodRepository repository;

        public DeliveryMethodController(IDeliveryMethodRepository repository)
        { this.repository = repository; }

        public ActionResult Index()
        {
            return View(repository.GetDeliveryMethods().OrderBy(c => c.Name));
        }

        [HttpGet]
        public ActionResult AddDeliveryMethod()
        {
            DeliveryMethod deliveryMethod = new DeliveryMethod();

            return View(deliveryMethod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDeliveryMethod([Bind(Include = "DeliveryMethodId, Name")] DeliveryMethod deliveryMethod)
        {
            if (ModelState.IsValid)
            {
                repository.Add(deliveryMethod);
                return RedirectToAction("Index");
            }
            else
            {
                return View(deliveryMethod);
            }
        }

        [HttpGet]
        public ActionResult EditDeliveryMethod(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DeliveryMethod deliveryMethod = repository.GetDeliveryMethods().Where(c => c.DeliveryMethodId == id).FirstOrDefault();

            if (deliveryMethod == null)
            {
                return HttpNotFound();
            }

            return View(deliveryMethod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDeliveryMethod([Bind(Include = "DeliveryMethodId, Name")] DeliveryMethod deliveryMethod)
        {
            if (ModelState.IsValid)
            {
                using (AdminPanelContext db = new AdminPanelContext())
                {
                    db.Entry(deliveryMethod).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            return View(deliveryMethod);
        }

        [HttpPost]
        public ActionResult DeleteDeliveryMethod(int deliveryMethodId)
        {
            DeliveryMethod deliveryMethod = repository.GetDeliveryMethods().FirstOrDefault(i => i.DeliveryMethodId == deliveryMethodId);
            repository.Delete(deliveryMethod);
            return RedirectToAction("Index");
        }
    }
}