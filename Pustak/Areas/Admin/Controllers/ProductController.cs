//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustak.Data;
using Pustak.Extensions;
using Pustak.Models;
using Pustok.Areas.Admin.Dtos;

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
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_context.Products.Any(x => x.Name == dto.Name))
            {
                ModelState.AddModelError("", "Product already exists");
                return View(dto);
            }

            if (dto.AdditionalFiles != null)
            {
                foreach (var file in dto.AdditionalFiles)
                {

                    if (!file.CheckFileSize(2))
                    {
                        ModelState.AddModelError("AdditionalFiles", "Files cannot be more than 2mb");
                        return View(dto);
                    }


                    if (!file.CheckFileType("image"))
                    {
                        ModelState.AddModelError("AdditionalFiles", "Files must be image type!");
                        return View(dto);
                    }
                }
            }

            if (!dto.IsMain.CheckFileSize(2))
            {
                ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
                return View(dto);
            }


            if (!dto.IsMain.CheckFileType("image"))
            {
                ModelState.AddModelError("MainFile", "Files must be image type!");
                return View(dto);
            }

            if (!dto.IsHover.CheckFileSize(2))
            {
                ModelState.AddModelError("HoverFile", "Files cannot be more than 2mb");
                return View(dto);
            }


            if (!dto.IsHover.CheckFileType("image"))
            {
                ModelState.AddModelError("HoverFile", "Files must be image type!");
                return View(dto);
            }


            Product product = new()
            {
                Name = dto.Name,
                Description = dto.Description,
                SellPrice = dto.SellPrice,
                DiscountPrice = dto.DiscountPrice,
                Rating = dto.Rating,
                CategoryId = dto.CategoryId,
                ProductImages = null
            };

            var mainFileName = await dto.IsMain.SaveFileAsync(_env.WebRootPath, "Client", "image", "products");
            var mainProductImageCreate = CreateProduct(mainFileName, false, true, product);

            List<ProductImage> productImage = new();

            productImage.Add(mainProductImageCreate);

            product.ProductImages = productImage;


            var hoverFileName = await dto.IsHover.SaveFileAsync(_env.WebRootPath, "client", "image", "products");
            var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, product);
            product.ProductImages.Add(hoverProductImageCreate);

            foreach (var file in dto.AdditionalFiles)
            {
                var filename = await file.SaveFileAsync(_env.WebRootPath, "Client", "image", "products");
                var additionalProductImgs = CreateProduct(filename, false, false, product);
                product.ProductImages.Add(additionalProductImgs);
            }

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


        public async Task<IActionResult> Edit(int id)
        {

            if (id < 1) return NotFound();
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var product = await _context.Products.Include(x => x.ProductImages)
                                                 .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            ProductUpdateDto dto = new()
            {
                Name = product.Name,
                SellPrice = product.SellPrice,
                DiscountPrice = product.DiscountPrice,
                Rating = product.Rating,
                Description = product.Description,
                CategoryId = product.CategoryId,
                MainFilePath = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url ?? "",
                HoverFilePath = product.ProductImages.FirstOrDefault(x => x.IsHover)?.Url ?? "",
                AdditionalFilePaths = product.ProductImages.Where(x => !x.IsHover && !x.IsMain).Select(x => x.Url).ToList(),
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductUpdateDto dto)
        {

            ViewBag.Categories = await _context.Categories.ToListAsync();

            var existProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (existProduct is null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(dto);
            }


            var isExist = await _context.Products.AnyAsync(x => x.Name == dto.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Product already exists");
                return View(dto);
            }


            var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId);

            if (!isExistCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                return View(dto);
            }

            if (dto.AdditionalFiles is not null)
            {
                foreach (var file in dto.AdditionalFiles)
                {

                    if (!file.CheckFileSize(2))
                    {
                        ModelState.AddModelError("AdditionalFiles", "Files cannot be more than 2mb");
                        return View(dto);
                    }


                    if (!file.CheckFileType("image"))
                    {
                        ModelState.AddModelError("AdditionalFiles", "Files must be image type!");
                        return View(dto);

                    }
                }
                if (dto.IsMain != null)
                {
                    if (!dto.IsMain.CheckFileSize(2))
                    {
                        ModelState.AddModelError("MainFile", "File cannot be more than 2mb");
                        return View(dto);
                    }


                    if (!dto.IsMain.CheckFileType("image"))
                    {
                        ModelState.AddModelError("MainFile", "File must be image type!");
                        return View(dto);
                    }


                }
                if (dto.IsHover != null)
                {
                    if (!dto.IsHover.CheckFileSize(2))
                    {
                        ModelState.AddModelError("HoverFile", "File cannot be more than 2mb");
                        return View(dto);
                    }


                    if (!dto.IsHover.CheckFileType("image"))
                    {
                        ModelState.AddModelError("HoverFile", "File must be image type!");
                        return View(dto);
                    }
                }
            }


            if (dto.AdditionalFiles?.Count > 0)
            {
                foreach (var item in existProduct.ProductImages.Where(x => !x.IsMain && !x.IsHover))
                {
                    item.Url.DeleteFile(_env.WebRootPath, "Client", "image", "products");

                }
                foreach (var file in dto.AdditionalFiles)
                {
                    var filename = await file.SaveFileAsync(_env.WebRootPath, "Client", "image", "products");
                    var additionalProductImages = CreateProduct(filename, false, false, existProduct);
                    existProduct.ProductImages.Add(additionalProductImages);

                }
            }


            if (dto.IsMain is not null)
            {
                existProduct.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.DeleteFile(_env.WebRootPath, "Client", "image", "producs");
                var mainFileName = await dto.IsMain.SaveFileAsync(_env.WebRootPath, "Client", "image", "products");
                var mainProductImage = CreateProduct(mainFileName, false, true, existProduct);
                existProduct.ProductImages.Add(mainProductImage);

            }
            if (dto.IsHover is not null)
            {
                existProduct.ProductImages.FirstOrDefault(x => x.IsHover)?.Url.DeleteFile(_env.WebRootPath, "Client", "image", "products");
                var hoverFileName = await dto.IsHover.SaveFileAsync(_env.WebRootPath, "Client", "image", "products");
                var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, existProduct);
                existProduct.ProductImages.Add(hoverProductImageCreate);
            }


            existProduct.Name = dto.Name;
            existProduct.SellPrice = dto.SellPrice;
            existProduct.DiscountPrice = dto.DiscountPrice;
            existProduct.Rating = (int)dto.Rating;
            existProduct.Description = dto.Description;
            existProduct.CategoryId = dto.CategoryId;

            _context.Products.Update(existProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                return NotFound();

            product.IsDeleted = true;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
