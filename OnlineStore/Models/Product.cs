using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        public int ProductCategoryId { get; set; }


        [Display(Name = "Product Category")]
        public string  ProductCategoryName { get; set; }

        [Display(Name = "Unit Price")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Img Url")]
        public string ImgUrl { get; set; }

        [Display(Name = "Img Stream")]
        public string ImgStram { get; set; }

        public bool Active { get; set; }

    }
}
