using AutoMapper;
using ETIT.Areas.Admin.ViewModels.Products;
using ETIT.Models;

namespace ETIT.Utilities.Profiles;

public class ProductProfile:Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductVM, Product>();
        CreateMap<UpdateProductVM, Product>().ReverseMap();
    }
}
