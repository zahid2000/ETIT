using AutoMapper;
using ETIT.Models;
using ETIT.ViewModels.Basket;

namespace ETIT.Utilities.Profiles;

public class BasketProfile:Profile
{
    public BasketProfile()
    {
        CreateMap<Product, BasketItemDetailVM>()
            .ForMember(b=>b.ImagePath,opt=>opt.MapFrom(b=>b.ProductImages.Where(i => i.IsMain).FirstOrDefault().Path))
            .ForMember(b=>b.CategoryName,opt=>opt.MapFrom(b=>b.Category.Name));
    }
}
