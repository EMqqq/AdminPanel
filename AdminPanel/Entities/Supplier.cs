using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdminPanel.Entities
{
    public class Supplier
    {
        [HiddenInput(DisplayValue = false)]
        public int SupplierId { get; set; }

        [Display(Name = "Supplier:")]
        [Required(ErrorMessage = "Type supplier name")]
        public string Name { get; set; }

        [Display(Name = "Price:")]
        [Required(ErrorMessage = "Type price")]
        public decimal Price { get; set; }

        [Display(Name = "Delivery time:")]
        [Required(ErrorMessage = "Type delivery time")]
        public string TransportTime { get; set; }

        [Display(Name = "Delivery method:")]
        [Required(ErrorMessage = "Select delivery method")]
        public int DeliveryMethodId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int FilePathId { get; set; }

        public virtual FilePath FilePath { get; set; }
        public virtual DeliveryMethod DeliveryMethod { get; set; }
    }
}