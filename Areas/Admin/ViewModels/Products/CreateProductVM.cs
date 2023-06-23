using ETIT.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETIT.Areas.Admin.ViewModels.Products;

public class CreateProductVM
{

    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public int CompanyId { get; set; }
    public int CategoryId { get; set; }
    [ValidateNever]
    public List<Company> Companies { get; set; }
    [ValidateNever]
    public List<Category> Categories { get; set; }
    public List<IFormFile> Images { get; set; }
}
