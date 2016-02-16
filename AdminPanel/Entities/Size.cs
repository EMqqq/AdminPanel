using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPanel.Entities
{
    public class Size
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number:")]
        [Range(35, 45, ErrorMessage = "Type number between: 35-45.")]
        public int SizeId { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}