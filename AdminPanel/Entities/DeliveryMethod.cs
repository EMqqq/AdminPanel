using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdminPanel.Entities
{
    public class DeliveryMethod
    {
        [HiddenInput(DisplayValue = false)]
        public int DeliveryMethodId { get; set; }

        [Display(Name = "Delivery method:")]
        [Required(ErrorMessage = "Type delivery method")]
        public string Name { get; set; }

        public virtual ICollection<Supplier> Suppliers { get; set; }
    }
}