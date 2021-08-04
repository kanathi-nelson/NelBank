using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NelBank.Data;
using NelBank.Interfaces;
using NelBank.Models;
using NelBank.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountsController> _logger;
        private readonly ApplicationDbContext _context;
        public readonly GeneralInterface generalInterface_;
        public Dictionary<string, string> DataItems_ = new Dictionary<string, string>();




        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public AccountsController(SignInManager<ApplicationUser> signInManager, GeneralInterface generalinter_,
            ILogger<AccountsController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            generalInterface_ = generalinter_;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: AccountsController
        public ActionResult Index()
        {
            return View();
        }
        public IEnumerable<string> FillBanks()
        {
            List<string> Gend = new List<string>();
            Gend.Add("Nel-Bank");
            Gend.Add("I&M Bank");
            Gend.Add("Co-op Bank");           
            return Gend.AsEnumerable();
        }
        // GET: AccountsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
          // GET: AccountsController/Details/5
          [HttpGet]
        public ActionResult AccountTransfer()
        {
            ViewData["MyBank"] = new SelectList(FillBanks(), "Select Bank");
            return View();
        } 
        // GET: AccountsController/Details/5
        [HttpPost]
        public IActionResult AccountTransfer(AccountTransferViewmodel accountTransfer)
        {

            if (ModelState.IsValid)
            {
                
                var new_ = new BankApiController(_signInManager,generalInterface_,_logger,_context,_userManager).AddTransaction(accountTransfer);
                var rst = new_ as ObjectResult;
                var myval = rst.Value;
                string stringval = myval.ToString();
                if(stringval.ToUpper().Contains("FAILED"))
                {
                    ViewData["MyBank"] = new SelectList(FillBanks(), accountTransfer.Bank);
                    ModelState.AddModelError(string.Empty,stringval );
                    return View(accountTransfer);

                }else
                {
                    return RedirectToAction("TransactionHist");
                }
            }
            return View(accountTransfer);
        }

       

        // GET: AccountsController/Details/5
        public async Task<ActionResult> TransactionHist()
        {
            var user_ =await generalInterface_.GetLoggedinUser();
            var mytrans_ = _context
                .Transactions
                .Include(q=>q.Accounts)
                .Include(q=>q.TransactionTypes)
                .Include(q=>q.User)
                .Where(o => o.UserId == user_.Id)
                .ToList();
            return View(mytrans_);
        }
           // GET: AccountsController/Details/5
        public ActionResult AccountWithdrawal()
        {
            return View();
        }

        // GET: AccountsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
