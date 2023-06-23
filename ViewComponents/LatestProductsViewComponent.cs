using ETIT.DAL;
using ETIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETIT.ViewComponents;

public class LatestProductsViewComponent:ViewComponent
{
    private readonly AppDbContext _context;

    public LatestProductsViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<Product> products=await _context.Products
                                                    .Where(p=>!p.IsDeleted)
                                                    .OrderByDescending(p=>p.Id)
                                                    .Include(p=>p.ProductImages)
                                                    .ToListAsync();
        return await Task.FromResult( View(products));
    }
}
