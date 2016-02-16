using AdminPanel.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdminPanel.Areas.Admin.ViewModels
{
    public class EditColorViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Color:")]
        [Required(ErrorMessage = "Type color")]
        public string Name { get; set; }

        public FilePath FilePath { get; set; }
    }
}