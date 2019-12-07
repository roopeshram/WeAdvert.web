using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        // GET: /<controller>/
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _UserManager;
        private readonly CognitoUserPool _pool;
        public Accounts(SignInManager<CognitoUser> signinManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signinManager;
            _UserManager = userManager;
            _pool = pool;

        }
       public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("User Exists", "User with this email already exists");
                    return View(model);
                }
                
                //user.Attributes.Add(Amazon.CognitoIdentityProvider.Model.AttributeType., model.Email);
                
                var createuser = await _UserManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (createuser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var  user = await _UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Not Found", "A User with the given email address not found");
                    return View(model);
                }
                var result = await (_UserManager as Amazon.AspNetCore.Identity.Cognito.CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code,true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                    return View(model);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                     return RedirectToAction("Create", "AdvertManagement");
                }
                {
                    ModelState.AddModelError("login error", "Email and Pwd do not match");
                }
            }
            return View("Login", model);
        }

    }
}
