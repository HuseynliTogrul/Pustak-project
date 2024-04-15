using Pustak.Models.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustak.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int SellPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int Rating { get; set; }
        [NotMapped]
        public List<IFormFile>? Files {  get; set; }
        [NotMapped]
        public IFormFile? IsMain { get; set; }
        [NotMapped]
        public IFormFile? IsHover { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category? category { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
    }
}