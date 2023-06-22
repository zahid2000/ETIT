using ETIT.DAL;
using ETIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETIT.Controllers;

public class ProductsController:Controller
{
    private readonly AppDbContext  _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<Product> products=await _context.Products
            .Where(p=>!p.IsDeleted)
            .OrderByDescending(p=>p.Id)
            .Include(p=>p.Category)
            .Include(p=>p.ProductImages)
            .ToListAsync();
        return View(products);
    }
}
