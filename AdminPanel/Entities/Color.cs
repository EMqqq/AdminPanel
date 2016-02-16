using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace AdminPanel.Entities
{
    public class Color
    {
        [HiddenInput(DisplayValue = false)]
        public int ColorId { get; set; }

        [Display(Name = "Color:")]
        [Required(ErrorMessage = "Type color")]
        public string ColorName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int FilePathId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual FilePath FilePath { get; set; }
    }
}