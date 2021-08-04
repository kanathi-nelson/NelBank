using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NelBank.Data;
using NelBank.Interfaces;
using NelBank.Models;
using NelBank.Viewmodels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public readonly GeneralInterface generalInterface_;
        public Dictionary<string, string> DataItems_ = new Dictionary<string, string>();




        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public HomeController(SignInManager<ApplicationUser> signInManager, GeneralInterface generalinter_,
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            generalInterface_ = generalinter_;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var user_ = await generalInterface_.GetLoggedinUser();
            var mytrans_ = _context
                .Accounts
                .Include(q => q.User)
                .Include(q => q.Transactions)
                .Where(o => o.OwnerId == user_.Id)
                .FirstOrDefault();
            var myaccs = _context
                .Transactions
                .Include(q => q.Accounts)
                .Include(q => q.TransactionTypes)
                .Include(q => q.User)
                .Where(o => o.UserId == user_.Id)
                .Take(10)
                .ToList();
            ViewBag.MyAccs = myaccs;
            return View(mytrans_);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string returnUrl)
        {

            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                bool isexists = generalInterface_.Userexists(viewModel.Email);
                if (isexists == false)
                {
                    ModelState.AddModelError(string.Empty, "Login failed. There is no account matching the entered Email.");
                    return View(viewModel);
                }
                try
                {
                    //bool myblocked = user_.Blocked;
                    //if (myblocked == true)
                    //{
                    //    ModelState.AddModelError(string.Empty, "Login failed. Account is blocked. Please contact Admin.");
                    //    return View(viewModel);
                    //}
                    var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);
                    //var result = await _signInManager.CheckPasswordSignInAsync(user_, viewModel.Password, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        //if (myuser != null)
                        //{
                        //    if (myuser.Selected == true)
                        //    {
                        //        var user = _context.Users.Where(i => i.Email == viewModel.Email).FirstOrDefault();
                        //        user.Issubmitted = true;

                        //        var aicapplicant = generalInterface_.ApplicantByEmail(viewModel.Email);
                        //        aicapplicant.IsSelected = true;

                        //        _context.Users.Update(user);
                        //        _context.AICApplicant.Update(aicapplicant);
                        //        _context.SaveChanges();
                        //    }
                        //}

                        var newuser = _context.Users.Where(i => i.Email == viewModel.Email).FirstOrDefault();

                        _logger.LogInformation("User logged in.");
                        if (returnUrl != "/")
                        {
                            return LocalRedirect(returnUrl);
                        }
                        
                            return RedirectToAction("Index", "Home");
                        
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Login failed.Please check your Password/Email");
                        return View(viewModel);

                    }

                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    var inn = ex.InnerException;
                    ModelState.AddModelError(string.Empty, msg);
                    return View(viewModel);
                }
            }
            else
            {
                //ModelState.AddModelError(string.Empty, "Login failed.Please check your email/password");
                return View(viewModel);
            }



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignupViewModel viewModel)
        {

            ViewData["IsDone"] = "False";
            ViewData["Err"] = "";
            int age = 0;

            if (ModelState.IsValid)
            {
                age = GetAge(viewModel.DateofBirth);
                if (age < 18)
                {
                    ModelState.AddModelError(string.Empty, "Cannot register a user who is less than 18 years old.");
                    //ViewData["MyGender"] = new SelectList(FillGender(), "Select");
                    return View(viewModel);
                }
                
                var s = new Guid();
                var Confirmtoken = s.ToString();

                var user = new ApplicationUser
                {
                    Deleted = false,
                    Blocked = false,
                    UserName = viewModel.Email,
                    CreatedDate = DateTime.Now,
                    Address = viewModel.Address,
                    CreatedTime = DateTime.UtcNow,
                    Email = viewModel.Email,
                    FirstName = viewModel.FirstName,
                    MiddleName = viewModel.MiddleName,
                    LastName = viewModel.LastName,
                    PhoneNumber = viewModel.PhoneNumber,
                    Gender = viewModel.Gender,
                    //RoleId = viewModel.SelectedRole,
                    ConfirmationToken = Confirmtoken,
                    DateofBirth = viewModel.DateofBirth,
                    PhoneNo = viewModel.PhoneNumber
                };

                var newusa = _context.Users.Where(p => p.Email == viewModel.Email).FirstOrDefault();
                if (newusa != null)
                {
                    ModelState.AddModelError(string.Empty, "A user with the entered Email already exists, please Login or change the email.");                 
                    return View();
                }
                BankingDataViewmodel viewmodel = new BankingDataViewmodel();
                    
                    try
                {
                    List<BankingDataViewmodel> vmodel = new List<BankingDataViewmodel>();
                    var myfile = Path.Combine(Environment.CurrentDirectory, "BankingData.json");
                    using (StreamReader r = new StreamReader(myfile))
                    {
                        string json = r.ReadToEnd();
                        vmodel = JsonConvert.DeserializeObject<List<BankingDataViewmodel>>(json);
                    }
                    var myacc_ = vmodel.Where(t => t.AccountNo.ToUpper() == viewModel.AccountNumber).FirstOrDefault();
                    if(myacc_==null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed; The account number specified does not exist.");                        
                        return View();
                    }
                    else
                    {
                        viewmodel.AccountBalance = myacc_.AccountBalance;
                        viewmodel.AccountNo = myacc_.AccountNo;
                        viewmodel.AccountType = myacc_.AccountType;
                    }
                   
                }
                catch (Exception ex)
                {

                }
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    //var rolid = viewModel.SelectedRole;
                    //var rolename = _context.Roles.Find(rolid);
                    //var rname = rolename.Name;

                    var usa = _context.Users.Where(p => p.Email == viewModel.Email).FirstOrDefault();
                    AddUserAccount(viewmodel, usa.Id);

                    _logger.LogInformation("User created a new account with password.");

                    //await _userManager.AddToRoleAsync(usa, rname);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var st = Guid.NewGuid();
                    var code = st.ToString();

                    usa.ConfirmationToken = code;

                    _context.Update(usa);
                    _context.SaveChanges();

                    string emailFor = "VerifyAccount";
                    //string pass = Input.Password;
                    //string mail = usa.Email;

                    //_Interface.SendVerificationLinkEmail(usa.Email, code, emailFor);                  


                    return RedirectToAction("Login", "Home");
                }



                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["Err"] = "Please check if you entered the correct details";
            return View();

        }

        public void AddUserAccount(BankingDataViewmodel viewmodel, int userid)
        {
            try
            {
                Accounts accounts = new Accounts()
                {
                    AccountBalance = viewmodel.AccountBalance,
                    AccountType = viewmodel.AccountType,
                    CreatedDate = DateTime.Now,
                    CreatedTime = DateTime.Now,
                    OwnerId = userid
                };
                _context.Add(accounts);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }

         public static int GetAge(DateTime birthDate)
        {
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthDate.Year;

            if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
                age--;

            return age;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
