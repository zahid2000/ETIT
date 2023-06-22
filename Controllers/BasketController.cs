using AutoMapper;
using ETIT.DAL;
using ETIT.Models;
using ETIT.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ETIT.Controllers;

public class BasketController : Controller
{
    private const string COOKIES_BASKET = "basket";
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    public BasketController(AppDbContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        List<BasketItemDetailVM> basketItemDetailVMs = new List<BasketItemDetailVM>();
        //List<BasketItemVM>? basketItemVMs = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        //foreach (BasketItemVM item in basketItemVMs!)
        //{
        //    Product? product =await  _appDbContext.Products
        //                                    .Where(s => !s.IsDeleted && s.Id == item.ProductId)
        //                                    .Include(s => s.Category)
        //                                    .Include(s => s.ProductImages)
        //                                    .FirstOrDefaultAsync();
        //    BasketItemDetailVM basketItemVM = _mapper.Map<BasketItemDetailVM>(product);
        //    basketItemDetailVMs.Add(basketItemVM);
        //}
        return View(basketItemDetailVMs);
    }

    public IActionResult AddBasket(int id)
    {
        List<BasketItemVM>? basketItemVMList;
        if (Request.Cookies[COOKIES_BASKET] != null)
            basketItemVMList = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        else
            basketItemVMList = new List<BasketItemVM> { };

        BasketItemVM? cookiesBasket = basketItemVMList?.Where(s => s.ProductId == id).FirstOrDefault();
        if (cookiesBasket != null)
            cookiesBasket.Count++;
        else
        {
            BasketItemVM basketVM = new BasketItemVM() { ProductId = id, Count = 1 };
            basketItemVMList?.Add(basketVM);
        }
        Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketItemVMList?.OrderBy(s => s.ProductId)));
        return RedirectToAction("Index", "Products");
    }
}
