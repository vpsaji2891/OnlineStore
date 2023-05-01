using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Entity;
using OnlineStore.Extension;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineStore.Service.ProductService;

namespace OnlineStore.Controllers
{

    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICustomExtension _customExtension;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context,
            ICustomExtension customExtension, IHttpContextAccessor contextAccessor, IProductService productService)
        {
            _logger = logger;
            _context = context;
            _customExtension = customExtension;
            _contextAccessor = contextAccessor;
            _productService = productService;
        }

        [AllowAnonymous]
        public JsonResult ProductList(string searchData, string sortKey)
        {
            List<Product> product = new List<Product>();

            var category = _context.Categories.ToList();

            bool validateNull = string.IsNullOrEmpty(searchData);

            if (!validateNull) //is search string available
            {
                product = (from item in _context.Products where item.ProductName.Contains(searchData) select item).ToList();
            }
            else
            {
                product = _context.Products.ToList();
            }

            int key = Convert.ToInt32(sortKey);

            switch (key) //sort items
            {
                case 1:
                    product = product.OrderBy(o => o.ProductPrice).ToList();
                    break;
                case 2:
                    product = product.OrderBy(o => o.ProductCategoryId).ToList();
                    break;
                default:
                    product = product.OrderBy(o => o.ProductCategoryName).ToList();
                    break;
            }

            foreach (var item in product) //binding  category name
                item.ProductCategoryName = (category.Where(c => c.Id == item.ProductCategoryId).Select(c => c.CategorytName).FirstOrDefault());


            return Json(product);
        }

        // GET: Product

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var product = _context.Products.ToList();
            var category = _context.Categories.ToList();

            foreach (var item in product)
                item.ProductCategoryName = (category.Where(c => c.Id == item.ProductCategoryId).Select(c => c.CategorytName).FirstOrDefault());

            return View(product);
        }

        // GET: Product/Details/5

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var product = new Product();
            product.Active = true;
            product.ProductCategoryId = 0;

            ViewBag.CategoryList = _productService.CategoryListItem();
            return View(product);
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ProductName,ProductCategoryId,ProductPrice,ImgUrl,ImgStram,Active")] Product product)
        {
            if (product.ProductCategoryId == 0)
            {
                ModelState.AddModelError("ProductCategoryId", "Please select category.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = _productService.CategoryListItem();
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryList = _productService.CategoryListItem();
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ProductCategoryId,ProductPrice,ImgUrl,ImgStram,Active")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (product.ProductCategoryId == 0)
            {
                ModelState.AddModelError("ProductCategoryId", "Please select category.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = _productService.CategoryListItem();
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryList = _productService.CategoryListItem();
            return View(product);
        }

        // POST: Product/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<JsonResult> AddComment(string productComment, string productId)
        {
            var status = new Status();
            try
            {


                var comment = new Comment();
                int userid = Convert.ToInt32(_contextAccessor.HttpContext.Session.GetString("UserId"));
                if (userid == 0)
                {
                    throw new Exception("User session expired");
                }
                comment.ProductId = Convert.ToInt32(productId.Trim());
                comment.ProductComment = productComment.TrimStart().TrimEnd();
                comment.CreatedUserId = userid;
                comment.CreatedDate = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();

                status.Result = true;
                status.Message = "Comment added sucsessfuly.";
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
                status.Result = false;
                status.Message = "Error Occured while adding the comment.!";
            }

            return Json(status);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public  JsonResult GetComment(string productId)
        {
            var comment = (from item in _context.Comments
                           where item.ProductId == Convert.ToInt32(productId.ToString())
                           select item).OrderBy(o => o.CreatedDate).ToList();

            var users = _context.Users.ToList();

            foreach(var item in comment) //bind AuthorName
            {
                item.CommentAuthor = users.Where(s=>s.Id==item.CreatedUserId).Select(s => s.Name).FirstOrDefault();
            }
            
            return Json(comment);
        }
    }
}
