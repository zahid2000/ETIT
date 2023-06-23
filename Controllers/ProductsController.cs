using ETIT.DAL;
using ETIT.Models;
using ETIT.ViewModels;
using ETIT.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ETIT.Controllers;

public class ProductsController:Controller
{
    private readonly AppDbContext  _context;
    private const string COOKIES_BASKET = "basket";

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1, int take = 12)
    {
        List<Product> products = await _context.Products
            .Where(p => !p.IsDeleted)
            .OrderByDescending(s => s.Id)
            .Skip((page - 1) * take)
            .Take(take)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .ToListAsync();
        int pageCount = await GetPageCount(take);
        PaginateVM<Product> model = new PaginateVM<Product>
        {
            Data = products,
            CurrentPage = page,
            PageCount = pageCount,
            HasNext = page < pageCount,
            HasPreview = page > 1,
            Take = take
        };

        return View(model);
    }

    private async Task<int> GetPageCount(int take)
    {
        int serviceCount = await _context.Products.CountAsync();
        return (int)Math.Ceiling((double)serviceCount / take);
    }

    public async Task<IActionResult> Detail(int id)
    {
        List<BasketItemVM>? basketItemVMList;
        if (Request.Cookies[COOKIES_BASKET] != null)
            basketItemVMList = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies[COOKIES_BASKET]);
        else
            basketItemVMList = new List<BasketItemVM> { };
        BasketItemVM? basketItemVM = basketItemVMList?.Where(b => b.ProductId == id).FirstOrDefault();
        ViewBag.BasketItemCount = basketItemVM?.Count;
        Product? product=await _context.Products
                                                .Where(p=>p.Id==id)
                                                .Include(p=>p.Category)
                                                .Include(p=>p.ProductImages)
                                                .Include(p=>p.Company)
                                                .FirstOrDefaultAsync();
        if (product == null) return RedirectToAction(nameof(Index));
        return View(product);
    }
}
