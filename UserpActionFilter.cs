using NelBank.Data;
using NelBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;


namespace NelBank
{
    public class UserpActionFilter : ActionFilterAttribute
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public int vvb;

        public UserpActionFilter(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        
      
        


        private async Task<ApplicationUser> GetUseronAsync()
        {
            var uss = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var tt = uss;
            return uss;
        }
       

        public int? GetRolId()
        {
            try
            {

                ApplicationUser user = GetUseronAsync().Result;
                return user.RoleId; 
            }
            catch
            {
                return 0;
            }

        }
        public string GetRoleName()
        {
            try
            {


                var uss = GetUseronAsync().Result;
                var tt = uss.RoleId;
                var myrol = _context.Roles.Where(i => i.Id == tt).FirstOrDefault();
                if (myrol != null)
                {
                    return myrol.Name;
                }
                else
                    return string.Empty;
            }
            catch
            {
                return "";
            }

        }



        public string GetLoggedinEmail()
        {
            try
            {

                ApplicationUser user = GetUseronAsync().Result;
                if (user != null)
                {
                    return user.Email;
                }
                else
                    return string.Empty; ;
            }
            catch
            {
                return string.Empty;
            }

        }
          public string GetLoggedName()
        {
            try
            {

                ApplicationUser user = GetUseronAsync().Result;
                if (user != null)
                {
                    return user.LastName;
                }
                else
                    return string.Empty; ;
            }
            catch
            {
                return string.Empty;
            }

        }

        public ApplicationUser GetUser()
        {
            ApplicationUser applicationUser = new ApplicationUser();
            try
            {

                ApplicationUser user = GetUseronAsync().Result;
                if (user != null)
                {
                    return user;
                }
                else
                    return applicationUser;
            }
            catch
            {
                return applicationUser;
            }

        }


       
       


        public int GetUserId()
        {
            try
            {

                ApplicationUser user = GetUseronAsync().Result;
                if(user!=null)
                {
                    return user.Id;
                }
                else
                return 0;
            }
            catch
            {
                return 0;
            }
            
        }
        private bool IsAjaxRequest(HttpRequest requestBase)
        {
            if (requestBase == null)
                throw new ArgumentNullException(nameof(requestBase));
            if (requestBase.Headers != null)
                return requestBase.Headers["X-Requested-width"] == "XMLHttpRequest";
            return false;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
                base.OnActionExecuted(filterContext);
            var rslt = filterContext.Result as ViewResult;
           if(rslt != null)
            {
                if (GetUserId() != 0)
                {
                   
                    rslt.ViewData["Roled"] = GetRolId();
                    rslt.ViewData["Rolename"] = GetRoleName();                   
                    rslt.ViewData["MyId"] = GetUserId();                   
                    rslt.ViewData["userloggedin"] = GetLoggedinEmail();                   

                    rslt.ViewData["MyUser"] = GetUser();
                    rslt.ViewData["MyName"] = GetLoggedName();
                }

                
            }
            


        }

    }
}