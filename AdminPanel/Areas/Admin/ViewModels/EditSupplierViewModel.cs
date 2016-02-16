using AdminPanel.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdminPanel.Areas.Admin.ViewModels
{
    public class EditSupplierViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

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

        public SelectList getDeliveryMethods { get; set; }
        public Supplier Supplier { get; set; }
        public FilePath FilePath { get; set; }
    }
}