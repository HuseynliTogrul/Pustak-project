using Pustak.Models.BaseModel;

namespace Pustak.Models
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }

        //public Brand()
        //{
        //    Products = new HashSet<Product>();
        //}
    }
}