using ETIT.Areas.Admin.ViewModels;
using ETIT.DAL;
using ETIT.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETIT.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles ="Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public DashboardController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        DashboardVM dashboardVM = new DashboardVM() { 
        UserCount= await _userManager.Users.CountAsync(),
        ProductCount=await _context.Products.CountAsync(),
        CompanyCount=await _context.Companies.CountAsync(),
        OrderCount=await _context.Orders.CountAsync(),
        Orders=await _context.Orders.OrderByDescending(o=>o.Id).Include(o=>o.AppUser).Include(o=>o.Product).ToListAsync(),
        };
        return View(dashboardVM);
    }
}
