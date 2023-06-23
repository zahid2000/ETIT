﻿using ETIT.Models.Auth;
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

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid) return View(registerVM);
        AppUser newUser = new AppUser()
        {
            FullName = registerVM.Fullname,
            UserName = registerVM.Username,
            Email = registerVM.Email,
            Address = registerVM.Address
        };
        IdentityResult registerResult = await _userManager.CreateAsync(newUser, registerVM.Password);
        if (!registerResult.Succeeded)
        {
            foreach (IdentityError error in registerResult.Errors)
                ModelState.AddModelError("", error.Description);
            return View(registerVM);
        }
      IdentityResult roleResult=  await _userManager.AddToRoleAsync(newUser,UserRoles.User.ToString());
        if (!roleResult.Succeeded)
        {
            foreach (IdentityError error in roleResult.Errors)
                ModelState.AddModelError("", error.Description);
            return View(registerVM);

        }
        return RedirectToAction(nameof(Login));
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM login, string? ReturnUrl)
    {
        if (!ModelState.IsValid) return View(login);
        AppUser user = await _userManager.FindByEmailAsync(login.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Email or Password is wrong!");
            return View(login);
        }
        Microsoft.AspNetCore.Identity.SignInResult signinResult =
            await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

        if (!signinResult.Succeeded)
        {
            ModelState.AddModelError("", "Email or Password is wrong!");
            return View(login);
        }
        await _signInManager.SignInAsync(user, login.RememberMe);
        if (Url.IsLocalUrl(ReturnUrl))
        {
            return Redirect(ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    //public async Task<IActionResult> AddRoles()
    //{
    //    foreach (var role in Enum.GetValues(typeof(UserRoles)))
    //    {
    //        if (!await _roleManager.RoleExistsAsync(role.ToString()))
    //        {
    //            await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
    //        }
    //    }
    //    return Json("Ok");
    //}
    public IActionResult AccessDenied(string? ReturnUrl)
    {
        return View();
    }
    public enum UserRoles
    {
        Admin,
        User,
        Moderator
    }
}

