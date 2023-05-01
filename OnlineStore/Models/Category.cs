using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        public string CategorytName { get; set; }

        public bool Active { get; set; }
    }
}
