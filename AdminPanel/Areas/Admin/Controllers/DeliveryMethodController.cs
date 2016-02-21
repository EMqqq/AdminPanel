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
        private ITRepository<AdminPanelContext, DeliveryMethod> repository;

        public DeliveryMethodController(ITRepository<AdminPanelContext, DeliveryMethod> repository)
        { this.repository = repository; }

        /// <summary>
        /// GET: Admin/DeliveryMethod
        /// </summary>
        /// <returns> display delivery methods from database </returns>
        public ActionResult Index()
        {
            return View(repository.GetAll.OrderBy(c => c.Name));
        }

        /// <summary>
        /// GET: Admin/DeliveryMethod/AddDeliveryMethod
        /// </summary>
        /// <returns> delivery method's add form </returns>
        [HttpGet]
        public ActionResult AddDeliveryMethod()
        {
            DeliveryMethod deliveryMethod = new DeliveryMethod();
            return View(deliveryMethod);
        }

        /// <summary>
        /// add new delivery method to database
        /// </summary>
        /// <param name="deliveryMethod"> delivery method from GET Method </param>
        /// <returns> add category or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDeliveryMethod(DeliveryMethod deliveryMethod)
        {
            if (ModelState.IsValid)
            {
                repository.Add(deliveryMethod);
                return RedirectToAction("Index");
            }

            return View(deliveryMethod);
        }

        /// <summary>
        /// GET: Admin/DeliveryMethod/EditDeliveryMethod
        /// </summary>
        /// <param name="id"> delivery method's id </param>
        /// <returns> delivery method's edit form </returns>
        [HttpGet]
        public ActionResult EditDeliveryMethod(int id)
        {
            DeliveryMethod deliveryMethod = repository.Get(id);

            if (deliveryMethod == null)
            {
                return HttpNotFound();
            }

            return View(deliveryMethod);
        }

        /// <summary>
        /// edit delivery method in database
        /// </summary>
        /// <param name="deliveryMethod"> delivery method from GET method </param>
        /// <returns> save changes or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDeliveryMethod([Bind(Include = "DeliveryMethodId, Name")] DeliveryMethod deliveryMethod)
        {
            if (ModelState.IsValid)
            {
                repository.Update(deliveryMethod);
                return RedirectToAction("Index");
            }
            
            return View(deliveryMethod);
        }

        /// <summary>
        /// delete delivery method from database
        /// </summary>
        /// <param name="deliveryMethodId"> delivery method's id </param>
        /// <returns> GET: Admin/DeliveryMethod </returns>
        [HttpPost]
        public ActionResult DeleteDeliveryMethod(int deliveryMethodId)
        {
            DeliveryMethod deliveryMethod = repository.Get(deliveryMethodId);
            repository.Delete(deliveryMethod);

            return RedirectToAction("Index");
        }
    }
}