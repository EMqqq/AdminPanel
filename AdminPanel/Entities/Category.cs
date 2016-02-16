using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdminPanel.Entities
{
    public class Category
    {
        [HiddenInput(DisplayValue = true)]
        public int CategoryId { get; set; }

        [Display(Name = "Category:")]
        [Required(ErrorMessage = "Type category")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}