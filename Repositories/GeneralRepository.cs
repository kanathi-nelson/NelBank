using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NelBank.Data;
using NelBank.Interfaces;
using NelBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Repositories
{
    public class GeneralRepository :GeneralInterface
    {
        private readonly ApplicationDbContext _Context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Dictionary<int, string> MonthNames = new Dictionary<int, string>();

        public GeneralRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _Context = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApplicationUser> GetLoggedinUser()
        {
            var userloggedin = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return userloggedin;
        }
        public bool Userexists(string email)
        {
            var newusa = _Context.Users.Where(p => p.Email == email).FirstOrDefault();
            if (newusa != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
