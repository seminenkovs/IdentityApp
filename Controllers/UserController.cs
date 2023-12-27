﻿using IdentityApp.Data;
using IdentityApp.Models;
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

    }
}
