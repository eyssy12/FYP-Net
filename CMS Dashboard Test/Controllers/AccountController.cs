namespace CMS.Dashboard.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using CMS_Dashboard_Test.ViewModels.Account;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;
    using Models;
    using Services;

    [Authorize]
    public class AccountController : Controller
    {
        private readonly Entities Context;
        private readonly INotify<EmailMessage> EmailSender;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly ILogger Logger;

        public AccountController(
            Entities context,
            INotify<EmailMessage> emailSender,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            this.Context = context;
            this.EmailSender = emailSender;
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.Logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (this.TempData.ContainsKey("ForgotPasswordResult"))
            {
                ViewBag.ForgotPasswordResult = TempData["ForgotPasswordResult"].ToString();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (this.User.IsSignedIn())
            {
                return RedirectToLocal(returnUrl);
            }

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                SignInResult result = await this.SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    ApplicationUser user = this.Context.Users.Include(u => u.Person).SingleOrDefault(s => s.UserName == model.UserName);

                    IList<Claim> claims = new List<Claim>();
                    IList<Claim> userExistingClaims = await this.UserManager.GetClaimsAsync(user);

                    StudentPerson student = this.Context.StudentPersons
                        .Include(s => s.Student)
                        .SingleOrDefault(s => s.PersonId == user.Person.Id);

                    // TODO; add this these to a decision tree, or an IAction list to iterate though
                    if (!this.ClaimExists(userExistingClaims, "FirstName"))
                    {
                        claims.Add(new Claim("FirstName", user.Person.FirstName));
                    }
                    if (!this.ClaimExists(userExistingClaims, "LastName"))
                    {
                        claims.Add(new Claim("LastName", user.Person.LastName));
                    }
                    if (!this.ClaimExists(userExistingClaims, "FullName"))
                    {
                        claims.Add(new Claim("FullName", user.Person.FirstName + " " + user.Person.LastName));
                    }

                    if (student != null)
                    {
                        if (!this.ClaimExists(userExistingClaims, "EnrollmentDate"))
                        {
                            claims.Add(new Claim("EnrollmentDate", student.Student.EnrollmentDate.ToString("MMMM dd, yyyy")));
                        }
                    }
                    else
                    {
                        LecturerPerson lecturer = this.Context.LecturerPersons
                            .Include(l => l.Lecturer)
                            .SingleOrDefault(s => s.PersonId == user.Person.Id);

                        if (lecturer != null)
                        {
                            if (!this.ClaimExists(userExistingClaims, "HireDate"))
                            {
                                claims.Add(new Claim("HireDate", lecturer.Lecturer.HireDate.ToString("MMMM dd, yyyy")));
                            }
                        }
                    }

                    foreach (Claim claim in claims)
                    {
                        IdentityResult claimsResult = await this.UserManager.AddClaimAsync(user, claim);
                    }

                    this.Logger.LogInformation(1, "User logged in.");

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private bool ClaimExists(IList<Claim> claims, string type)
        {
            return claims.Any(s => s.Type == type);
        }
        
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);

                    Logger.LogInformation(3, "User created a new account with password.");

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await SignInManager.SignOutAsync();

            this.Logger.LogInformation(4, "User logged out.");

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await this.UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                EmailMessage message = new EmailMessage
                {
                    From = "fypcms@yahoo.co.uk",
                    Password = "Final_Year_Project",
                    To = user.Email,
                    Subject = "FYPCMS - Reset password request",
                    Body = "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>"
                };

                this.EmailSender.SendNotification(message);

                ViewBag.SubmitResult = "A reset link has been sent to your email.";

                return View();
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null 
                ? View("Error")
                : View();
        }
        
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                TempData["ForgotPasswordResult"] = "Your password has been reset";

                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            AddErrors(result);
            return View();
        }
        
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await this.UserManager.FindByIdAsync(this.HttpContext.User.GetUserId());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}