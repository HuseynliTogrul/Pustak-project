using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using Pustak.Data;

namespace Pustak.ViewComponents
{
    public class ProductsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _context.Products
                .Include(x => x.category)
                .Include(x => x.ProductImages)
                .OrderByDescending(x => x.Id).Take(20).ToListAsync();

            return View(products);
        }
    }
}
