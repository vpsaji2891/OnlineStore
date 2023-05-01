using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace OnlineStore.Service
{
    public class ProductService
    {
        public interface IProductService
        {
            public IEnumerable<SelectListItem> CategoryListItem();
        }
    }
}
