using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Entity;
using OnlineStore.Models;
using System.Collections.Generic;
using System.Linq;
using static OnlineStore.Service.ProductService;

namespace OnlineStore.Repository
{
    public class ProductRepository : IProductService
    {
        private ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Category> CategoryList()
        {
            var category = _context.Categories.ToList<Category>();

            return category;
        }

        public IEnumerable<SelectListItem> CategoryListItem()
        {
            var categoryList = CategoryList().OrderBy(o => o.CategorytName).ToList(); ;
            var item = new Category()
            {
                Id = 0,
                CategorytName = "Select Category"
            };

            categoryList.Insert(0, item);

            var items = categoryList.Select(role => new SelectListItem
            {
                Text = role.CategorytName.ToString(),
                Value = role.Id.ToString()
            });

            return items;
        }

    }
}
