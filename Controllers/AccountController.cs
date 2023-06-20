using ETIT.Models.Auth;
using ETIT.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETIT.Controllers;

public class AccountController : Controller
{
 private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }


    public async Task<IActionResult> Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async  Task<IActionResult> Register([FromForm]RegisterVM registerVM)
    {
        return Ok(registerVM);
    }
    public async Task<IActionResult> Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginVM loginVM)
    {
        return Ok(loginVM);
    }
}
