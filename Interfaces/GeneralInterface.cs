using NelBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Interfaces
{
    public interface GeneralInterface
    {
        Task<ApplicationUser> GetLoggedinUser();
        bool Userexists(string email);
    }
}
