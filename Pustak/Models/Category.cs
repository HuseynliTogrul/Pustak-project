using Pustak.Models.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustak.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public int ParentCategoryId { get; set; }

        //[NotMapped]
        //public IFormFile File { get; set; }
        public ICollection<Product>? Products { get; set;}
    }
}