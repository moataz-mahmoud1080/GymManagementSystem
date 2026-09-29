using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.AccountLogin;
using GymManagementSystem.Controllers;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager ,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController>  logger

            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()=> View();
        [HttpPost]
        public async Task<IActionResult> Login(AcountViewModel model ,CancellationToken ct=default)
        {
            if(!ModelState.IsValid) return View(model);

            var user= await _userManager.FindByEmailAsync(model.Email);
            if (user is null || string.IsNullOrWhiteSpace(user.UserName))
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password");
                return View(model);
            }
           var result= await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} Signed In");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning($"User {user.UserName} Is Locked Out");
                ModelState.AddModelError("InvalidLogin", "This Account Is Temporarly Locked Try Again Later");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email Or Password");
                return View(model);
            }

        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
