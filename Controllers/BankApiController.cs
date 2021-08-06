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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NelBank.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankApiController : ControllerBase
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

        public BankApiController(SignInManager<ApplicationUser> signInManager, GeneralInterface generalinter_,
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

        [Produces("application/json")]
        [HttpGet]
        public IActionResult ConfirmLogin(string u1,string p1)
        {
            try
            {
                var usern_ = u1.ToUpper();
                var user_ = _context.Users
                    .Where(u => u.Email
                    .ToUpper()
                    .Contains(usern_))
                    .FirstOrDefault();

                if (user_ != null)
                {
                    var model = UserAuthentication(u1, p1).Result;
                    if (model == true)
                    {
                        int userid = 0;
                        userid = user_.Id;
                        var acc = GetAccounts(userid);
                        AccountLoginResp accountLoginResp = new AccountLoginResp()
                        {
                            AccountId = acc.Id,
                            FromAccount = acc.AccountNo,
                            UserId = userid,
                            Username = user_.FirstName + " " + user_.LastName,
                            Status= string.Empty,
                            Message= string.Empty,
                            AccountBalance = acc.AccountBalance,
                            AccountNo= acc.AccountNo,
                            AccountType= acc.AccountType
                        };
                        return Ok(accountLoginResp);

                    }
                    else
                    {
                        return BadRequest("Access denied.");

                    }
                }
                else
                {
                    return NotFound("Account not found.");
                }
            }
            catch(Exception es)
            {
                return BadRequest("An exception occurred.");
            }
        }
        async Task<bool> UserAuthentication(string uname, string pass)
        {
            var user_ = _context.Users.Where(u => u.Email == uname).FirstOrDefault();
            var usapass_ =await _signInManager.CheckPasswordSignInAsync(user_, pass, false);
            if(usapass_.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Accounts GetAccounts(int usaid)
        {
            var acc_ = _context.Accounts
                .Where(u => u.OwnerId == usaid).FirstOrDefault();
            return acc_;
        }
        // GET: api/<BankApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public IActionResult AddTransaction(AccountTransferViewmodel accountTransfer)
        {
            if (accountTransfer != null)
            {
                ApplicationUser usa = new ApplicationUser();
                int myuserid_;
                decimal bal = 0;
                if (accountTransfer.UserId == null || accountTransfer.UserId < 1)
                {
                    usa = generalInterface_.GetLoggedinUser().Result;
                    accountTransfer.UserId = usa.Id;
                    myuserid_ = usa.Id;
                }
                else
                {
                    myuserid_ =accountTransfer.UserId;
                }
                var account_ = _context.Accounts
                    .Where(o => o.OwnerId == myuserid_)
                    .FirstOrDefault();

                if (account_ != null)
                {
                    bal = Convert.ToDecimal(account_.AccountBalance);
                    accountTransfer.AccountId = account_.Id;
                    accountTransfer.FromAccount = account_.AccountNo;
                }
                if (bal < accountTransfer.Amount)
                {
                    return BadRequest("Failed, you have insifficient funds in your account.");
                }
                int? acctype = null;
                var type = _context.TransactionTypes.Where(y => y.Name.ToUpper().Contains("TRANSFER")).FirstOrDefault();
                if (type != null)
                {
                    acctype = type.Id;
                }
                Transactions transactions = new Transactions
                {
                    Account = accountTransfer.AccountId,
                    Amount = accountTransfer.Amount,
                    DebitedAccount = accountTransfer.FromAccount,
                    CreditedAccount = accountTransfer.AccountNo,
                    Bank = accountTransfer.Bank,
                    Debit = true,
                    IsInternal = true,
                    TransactionDate = DateTime.Now,
                    TransactionTime = DateTime.Now,
                    UserId = myuserid_,
                    TransactionType = acctype
                };
                _context.Add(transactions);
                _context.SaveChanges();

                var acc_ = _context.Accounts
                    .Where(T => T.Id == accountTransfer.AccountId)
                    .FirstOrDefault();
                if (acc_ != null)
                {
                    var mybal = Convert.ToDecimal(acc_.AccountBalance);
                    mybal -= accountTransfer.Amount;
                    acc_.AccountBalance = mybal.ToString();
                    _context.Update(acc_);
                    _context.SaveChanges();
                }

                return Ok("done");
            }
            return BadRequest("Failed, null request");
        }
        [HttpPost]
        public IActionResult TransferFunds(FundsWithdrawal fundsWithdrawal)
        {
            if (fundsWithdrawal != null)
            {
                ApplicationUser usa = new ApplicationUser();
                int myuserid_;
                decimal bal = 0;
                if (fundsWithdrawal.UserId == null || fundsWithdrawal.UserId < 1)
                {
                    usa = generalInterface_.GetLoggedinUser().Result;
                    fundsWithdrawal.UserId = usa.Id;
                    myuserid_ = usa.Id;
                }
                else
                {
                    myuserid_ = fundsWithdrawal.UserId.Value;
                }
                var account_ = _context.Accounts
                    .Where(o => o.OwnerId == myuserid_)
                    .FirstOrDefault();
                 var atmbal = _context.AtmData                 
                    .FirstOrDefault();

                if (account_ != null)
                {
                    bal = Convert.ToDecimal(account_.AccountBalance);
                }  
                if (atmbal != null)
                {
                    if(atmbal.AccountBalance<fundsWithdrawal.Amount)
                    {
                        return BadRequest("Failed, No cash in the ATM.");
                    }
                }
                if (bal < fundsWithdrawal.Amount)
                {
                    return BadRequest("Failed, you have insifficient funds in your account.");
                }

                int? acctype = null;
                var type = _context.TransactionTypes.Where(y => y.Name.ToUpper().Contains("THDRAW")).FirstOrDefault();
                if (type != null)
                {
                    acctype = type.Id;
                }
                Transactions transactions = new Transactions
                {
                    Account = account_.Id,
                    Amount = fundsWithdrawal.Amount,
                    DebitedAccount = account_.AccountNo,
                    CreditedAccount = "Cash",
                    Bank = "ATM",
                    Debit = true,
                    TransactionDate = DateTime.Now,
                    TransactionTime = DateTime.Now,
                    UserId =myuserid_,
                    TransactionType = acctype
                };
                _context.Add(transactions);
                _context.SaveChanges();

                
                if (account_ != null)
                {
                    var mybal = Convert.ToDecimal(account_.AccountBalance);
                    mybal -= fundsWithdrawal.Amount.Value;
                    account_.AccountBalance = mybal.ToString();
                    _context.Update(account_);
                    _context.SaveChanges();
                }

                return Ok("done");
            }
            return BadRequest("Failed, null request");
        }

        // GET api/<BankApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BankApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BankApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BankApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
