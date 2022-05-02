﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shoping.Data;
using Shoping.Data.Entities;
using Shoping.Models;

namespace Shoping.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        //administra todo lo relacionado a los usuarios
        private readonly UserManager<User> _userManager;
        //administra todo lo relacionado a los roles
        private readonly RoleManager<IdentityRole> _roleManager;
        //maneja todo lo relacionado al inicio de sesion
        private readonly SignInManager<User> _signInManager;

        public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }

        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users
            .Include(u => u.City)
            .FirstOrDefaultAsync(u => u.Email == email);

        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
