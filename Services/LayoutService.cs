using ETIT.DAL;
using ETIT.Models;
using ETIT.ViewModels.Basket;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ETIT.Services;

public class LayoutService
{
    private readonly AppDbContext _context;
    private const string COOKIES_BASKET = "basket";
    private readonly IHttpContextAccessor _httpContext;
    public LayoutService(AppDbContext context, IHttpContextAccessor httpContext)
    {
        _context = context;
        _httpContext = httpContext;
    }
    public async Task<(double TotalPrice,int Count)> GetBasketInfo()
    {
        List<BasketItemDetailVM> basketItemDetailVMs = new List<BasketItemDetailVM>();
        List<BasketItemVM>? basketItemVMs=null;
        if (_httpContext.HttpContext.Request.Cookies[COOKIES_BASKET]!=null)
        {
            basketItemVMs = JsonConvert.DeserializeObject<List<BasketItemVM>>(_httpContext.HttpContext.Request.Cookies[COOKIES_BASKET]);
        }
        if (basketItemVMs == null) return (0, 0);
        double sum = 0;
        foreach (BasketItemVM item in basketItemVMs!)
        {
            Product? product = await _context.Products
                                            .Where(s => !s.IsDeleted && s.Id == item.ProductId)
                                            .FirstOrDefaultAsync();
            sum += product?.Price??0 * item.Count;
        }
        return (sum,basketItemVMs!.Sum(b=>b.Count));
    }
}
