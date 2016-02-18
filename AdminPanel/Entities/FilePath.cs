using AdminPanel.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace AdminPanel.Entities
{
    public class FilePath
    {
        [HiddenInput(DisplayValue = false)]
        public int FilePathId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public FileType FileType { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }
        public virtual ICollection<Color> Colors { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
    }
}