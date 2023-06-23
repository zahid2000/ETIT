using ETIT.Areas.Admin.ViewModels;
using ETIT.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ETIT.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public UsersController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: UserController
    public async Task<IActionResult> Index()
    {
        List<UserVM> userVMs = new List<UserVM>();
        var users = await _userManager.Users.ToListAsync();
        if (users == null) return Problem("Not Found");
        foreach (AppUser user in users)
        {
            userVMs.Add(new UserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.PhoneNumber,
                UserName = user.UserName,
                Roles = (await _userManager.GetRolesAsync(user))
            });
        }
        return View(userVMs);
    }

    public async Task<IActionResult> ChangeRole(string id, string role = "User")
    {
        AppUser user = await _userManager.FindByIdAsync(id);
        if (user == null) return Problem("Not Found");
        IList<string> roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(role))
        {
            foreach (var item in roles)
                await _userManager.RemoveFromRoleAsync(user, item);
            await _userManager.AddToRoleAsync(user, role);
        }
        return RedirectToAction(nameof(Index));

    }
}
