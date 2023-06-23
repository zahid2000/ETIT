using AutoMapper;
using ETIT.DAL;
using ETIT.Models;
using ETIT.Models.Auth;
using ETIT.ViewModels.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ETIT.Controllers;

public class BasketController : Controller
{
    private const string COOKIES_BASKET = "basket";
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    public BasketController(AppDbContext appDbContext, IMapper mapper, UserManager<AppUser> userManager)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        List<BasketItemDetailVM>? basketItemDetailVMs = new List<BasketItemDetailVM>();
        List<BasketItemVM>? basketItemVMs = null;
        if (Request.Cookies[COOKIES_BASKET]!=null)
           basketItemVMs = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        if (basketItemVMs == null) return View(basketItemDetailVMs);
        foreach (BasketItemVM item in basketItemVMs!)
        {
            Product? product = await _appDbContext.Products
                                            .Where(s => !s.IsDeleted && s.Id == item.ProductId)
                                            .Include(s => s.Category)
                                            .Include(s => s.Company)
                                            .Include(s => s.ProductImages)
                                            .FirstOrDefaultAsync();
            BasketItemDetailVM basketItemDetailVM = _mapper.Map<BasketItemDetailVM>(product);
            basketItemDetailVM.Count = item.Count;
            if (basketItemDetailVM !=null)
                basketItemDetailVMs.Add(basketItemDetailVM);

        }
        return View(basketItemDetailVMs);
    }
    public IActionResult DeleteBasketItem(int id) {
        List<BasketItemVM>? basketItemVMList;
        if (Request.Cookies[COOKIES_BASKET] != null)
            basketItemVMList = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        else
            basketItemVMList = new List<BasketItemVM> { };
        BasketItemVM? basketItemVM = basketItemVMList?.Where(b => b.ProductId == id).FirstOrDefault();
        if (basketItemVM == null) return RedirectToAction(nameof(Index));
        basketItemVMList?.Remove(basketItemVM);
        AddCookie(basketItemVMList);
        return RedirectToAction(nameof(Index));

    }
    public IActionResult IncrementBasketItem(int id)
    {
        List<BasketItemVM>? basketItemVMList;
        if (Request.Cookies[COOKIES_BASKET] != null)
            basketItemVMList = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        else
            basketItemVMList = new List<BasketItemVM> { };
        BasketItemVM? basketItemVM = basketItemVMList?.Where(b => b.ProductId == id).FirstOrDefault();
        if (basketItemVM == null) return RedirectToAction(nameof(Index));
        basketItemVM.Count++;
        AddCookie(basketItemVMList);
        return RedirectToAction(nameof(Index));

    }
    public IActionResult DecrementBasketItem(int id)
    {
        List<BasketItemVM>? basketItemVMList;
        if (Request.Cookies[COOKIES_BASKET] != null)
            basketItemVMList = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        else
            basketItemVMList = new List<BasketItemVM> { };
        BasketItemVM? basketItemVM = basketItemVMList?.Where(b => b.ProductId == id).FirstOrDefault();
        if (basketItemVM == null) return RedirectToAction(nameof(Index));
        basketItemVM.Count--;
        AddCookie(basketItemVMList);
        return RedirectToAction(nameof(Index));

    }
    public IActionResult AddBasket(int id,string? returnUrl)
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
        AddCookie(basketItemVMList);
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Products");
    }
    [Authorize]
    public async Task<IActionResult> OrderedBasket()
    {
        List<BasketItemVM>? basketItemVMList=null;
        if (Request.Cookies[COOKIES_BASKET] != null)
            basketItemVMList = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        if (basketItemVMList == null) return RedirectToAction(nameof(Index));
        AppUser user = await _userManager.FindByNameAsync(User.Identity?.Name);
        if (user == null) return RedirectToAction(nameof(Index));
        foreach (var basketItem in basketItemVMList)
        {
            Order order = new Order
            {
                ProductId = basketItem.ProductId,
                AppUserId = user.Id,
                CreatedDate = DateTime.Now,

                Price = (await _appDbContext.Products.FindAsync(basketItem.ProductId)).Price,
                Quantity = basketItem.Count
            };
            await _appDbContext.Orders.AddAsync(order);
        }
       await _appDbContext.SaveChangesAsync();
        AddCookie(new List<BasketItemVM>());
        return RedirectToAction(nameof(Index));
    }
    private void AddCookie(List<BasketItemVM>? basketItemVMList)
    {
        Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketItemVMList?.OrderBy(s => s.ProductId)), new CookieOptions { MaxAge = TimeSpan.FromDays(15) });
    }
}
