using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustak.Data;
using Pustak.Extensions;
using Pustak.Models;

namespace Pustak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                                                   .Include(x => x.ProductImages)
                                                   .Include(x => x.category)
                                                   .ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Product product)
        {
            if (_context.Products.Any(p => p.Name == product.Name))
            {
                ModelState.AddModelError("", "Product already exists");
                return View(product);
            }
            product.ProductImages = new List<ProductImage>();
            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {

                    if (!file.CheckFileSize(2))
                    {
                        ModelState.AddModelError("Files", "Files cannot be more than 2mb");
                        return View(product);
                    }


                    if (!file.CheckFileType("image"))
                    {
                        ModelState.AddModelError("Files", "Files must be image type!");
                        return View(product);
                    }

                    var filename = await file.SaveFileAsync(_env.WebRootPath, "client", "assets", "imgs/products");
                    var additionalProductImages = CreateProduct(filename, false, false, product);

                    product.ProductImages.Add(additionalProductImages);

                }
            }
            if (!product.IsMain.CheckFileSize(2))
            {
                ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
                return View(product);
            }


            if (!product.IsMain.CheckFileType("image"))
            {
                ModelState.AddModelError("MainFile", "Files must be image type!");
                return View(product);
            }

            var mainFileName = await product.IsMain.SaveFileAsync(_env.WebRootPath, "client", "assets", "imgs/products");
            var mainProductImageCreate = CreateProduct(mainFileName, false, true, product);

            product.ProductImages.Add(mainProductImageCreate);

            if (!product.IsHover.CheckFileSize(2))
            {
                ModelState.AddModelError("HoverFile", "Files cannot be more than 2mb");
                return View(product);
            }


            if (!product.IsHover.CheckFileType("image"))
            {
                ModelState.AddModelError("HoverFile", "Files must be image type!");
                return View(product);
            }

            var hoverFileName = await product.IsHover.SaveFileAsync(_env.WebRootPath, "client", "assets", "imgs/products");
            var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, product);
            product.ProductImages.Add(hoverProductImageCreate);



            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ProductImage CreateProduct(string url, bool isHover, bool isMain, Product product)
        {
            return new ProductImage
            {
                Url = url,
                IsHover = isHover,
                IsMain = isMain,
                Product = product
            };
        }
    }
}
