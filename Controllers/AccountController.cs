﻿using System.Security.Claims;
using IdentityApp.Interfaces;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ISendGridEmail _sendGridEmail;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, ISendGridEmail sendGridEmail,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sendGridEmail = sendGridEmail;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/");
            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new {userId = user.Id, code = code}, protocol: HttpContext.Request.Scheme);
                await _sendGridEmail.SendEmailAsync(model.Email, "Reset Email Confirmation",
                    "Please Reset emil by going to this link <a href=\"" + callbackUrl + "\">Link</a>");

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "User not found");
                    return View();
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code,
                    resetPasswordViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
            }

            return View(resetPasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel logingViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(logingViewModel.Email, logingViewModel.Password,
                    logingViewModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(logingViewModel);
                }
            }

            return View(logingViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync("Pokemon"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Pokemon"));
                await _roleManager.CreateAsync(new IdentityRole("Trainer"));
            }
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem()
            {
                Value = "Pokemon",
                Text = "Pokemon"
            });
            listItems.Add(new SelectListItem()
            {
                Value = "Trainer",
                Text = "Trainer"
            });
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.RoleList = listItems;
            registerViewModel.ReturnUrl = returnUrl;
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string? returnUtl = null)
        {
            registerViewModel.ReturnUrl = returnUtl;
            returnUtl = returnUtl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new AppUser { Email = registerViewModel.Email, UserName = registerViewModel.UserName};
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    if (registerViewModel.RoleSelected != null && registerViewModel.RoleSelected.Length > 0 && registerViewModel.RoleSelected == "Trainer")
                    {
                        await _userManager.AddToRoleAsync(user, "Trainer");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Pokemon");
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUtl);
                }
                ModelState.AddModelError("Password", "User could not be created. Password is not unique enough");
            }

            return View(registerViewModel);

        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnurl = null)
        {
            var redirect = Url.Action("ExternalLoginCallback", "Account", new{ReturnUrl = returnurl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirect);

            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnurl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, "Error from external provider");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result =
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                    isPersistent: false);
            if (result.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnurl);
            }
            else
            {
                ViewData["ReturnUrl"] = returnurl;
                ViewData["DisplayName"] = info.ProviderDisplayName;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginViewModel{Email = email});
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string? returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }

                var user = new AppUser{UserName = model.Name, Email = model.Email};
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "User");
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnurl);
                    }
                }
                ModelState.AddModelError("Email", "User already exists");
            }
            ViewData["ReturnUrl"] = returnurl;
            return View(model);
        }

    }
}
