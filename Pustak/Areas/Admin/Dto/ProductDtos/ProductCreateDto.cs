using System;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Areas.Admin.Dtos
{
    public class ProductCreateDto
	{
        public string Name { get; set; } = null!;
        public int SellPrice { get; set; }
        public int DiscountPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        [Range(0,6)]
        public  int Rating { get; set; }
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }

        public IFormFile IsMain { get; set; } = null!;
        public IFormFile IsHover { get; set; } = null!;
        public List<IFormFile> AdditionalFiles { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
    }
}

