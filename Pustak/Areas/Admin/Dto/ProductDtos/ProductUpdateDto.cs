//using System.ComponentModel.DataAnnotations;

namespace Pustok.Areas.Admin.Dtos;

public class ProductUpdateDto
{
    public string Name { get; set; } = null!;
    public int SellPrice { get; set; }
    public int DiscountPrice { get; set; }
    public decimal Discount { get; set; }
    //[Range(0, 6)]
    public int Rating { get; set; }
    public string Description { get; set; } = null!;
    public int CategoryId { get; set; }

    public IFormFile? IsMain { get; set; } = null!;
    public string MainFilePath { get; set; } = null!;
    public IFormFile? IsHover { get; set; } = null!;
    public string HoverFilePath { get; set; } = null!;
    public List<IFormFile> AdditionalFiles { get; set; } = new();
    public List<string> AdditionalFilePaths { get; set; } = new();
    public List<int> TagIds { get; set; } = new();
}

