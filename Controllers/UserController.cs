﻿using System.Security.Claims;
using IdentityApp.Data;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public UserController(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userList = _dbContext.AppUser.ToList();
            var roleList = _dbContext.UserRoles.ToList();
            var roles = _dbContext.Roles.ToList();
            // set user to "none" to make UI look better
            foreach (var user in userList)
            {
                var role = roleList.FirstOrDefault(r => r.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }
            return View(userList);
        }

        [HttpGet]
        public IActionResult Edit(string userId)
        {
            var user = _dbContext.AppUser.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRole = _dbContext.UserRoles.ToList();
            var roles = _dbContext.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
            if (role != null)
            {
                user.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }

            user.RoleList = _dbContext.Roles.Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.Id
            });

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (ModelState.IsValid)
            {
                var userDbValue = _dbContext.AppUser.FirstOrDefault(u => u.Id == user.Id);
                if (userDbValue == null)
                {
                    return NotFound();
                }

                var userRole = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == userDbValue.Id);
                if (userRole != null)
                {
                    var previousRoleName = _dbContext.Roles.Where(u => u.Id == userRole.RoleId)
                        .Select(e => e.Name).FirstOrDefault();
                    await _userManager.RemoveFromRoleAsync(userDbValue, previousRoleName);

                }

                await _userManager.AddToRoleAsync(userDbValue,
                    _dbContext.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);

                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            user.RoleList = _dbContext.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var user = _dbContext.AppUser.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.AppUser.Remove(user);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var existingUserClaim = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel()
            {
                UserId = userId,
            };

            foreach (Claim claim in ClaimStore.claimList)
            {
                UserClaim userClaim = new UserClaim()
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaim.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageClaims(UserClaimsViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userClaim = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, userClaim);

            if (!result.Succeeded)
            {
                return View(viewModel);
            }

            result = await _userManager.AddClaimsAsync(user,
                viewModel.Claims.Where(u => u.IsSelected)
                    .Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()))
                );
            if (!result.Succeeded)
            {
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
