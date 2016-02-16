using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdminPanel.Entities
{
    public class Product
    {
        [HiddenInput(DisplayValue = false)]
        public int ProductId { get; set; }

        [Display(Name = "Name:")]
        [Required(ErrorMessage = "Type name")]
        public string ProductName { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description:")]
        public string Desc { get; set; }

        [Display(Name = "Material:")]
        [Required(ErrorMessage = "Type material")]
        public string Material { get; set; }

        [Display(Name = "Price:")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Type product price")]
        [Required(ErrorMessage = "Type new price")]
        public decimal NormalPrice { get; set; }

        [Display(Name = "Discount [%]:")]
        public int Discount { get; set; }

        [Display(Name = "After discount:")]
        public decimal Price { get; set; }

        [Display(Name = "Created: ")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Last edit:")]
        public DateTime? EditDate { get; set; }

        [Display(Name = "Category:")]
        [Required(ErrorMessage = "Select product's category")]
        public int CategoryId { get; set; }

        [Display(Name = "Color:")]
        [Required(ErrorMessage = "Select product's color")]
        public int ColorId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Color Color { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }
        public virtual ICollection<Size> Sizes { get; set; }
    }
}