using AutoMapper;
using CoreAppAuthDemo.Models;
using CoreAppAuthDemo.Repositories;
using EmailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;
        private readonly IApplicationUserDetailsRepo _appUserDetailsInfo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(IMapper mapper, ILogger<AccountController> logger, IApplicationUserDetailsRepo appUserDetailsInfo, UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserDetailsInfo = appUserDetailsInfo;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(UserRegistrationModel userModel)
        {
            ViewData["Title"] = "Register";
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var user = _mapper.Map<User>(userModel);
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(userModel);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Email Confirmation Link", "Please confirm your email by clicking this link : " + confirmationLink, null);
            await _emailSender.SendEmailAsync(message);

            await _userManager.AddToRoleAsync(user, "Operator");
            return RedirectToAction(nameof(SuccessRegistration));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        [HttpGet]
        public IActionResult SuccessRegistration()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["Title"] = "Login";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel userModel, string returnUrl = null)
        {
            ViewData["Title"] = "Login";
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, userModel.RememberMe, true);
            if (result.RequiresTwoFactor)
            {
                var twoFactorAuthMethod = _appUserDetailsInfo.GetUserTwoFactorAuthMethod(userModel.Email);
                if (twoFactorAuthMethod.Equals("EmailOTP", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(nameof(LoginTwoFactor), new { userModel.Email, userModel.RememberMe, twoFactorAuthMethod, returnUrl });
                }
                if (twoFactorAuthMethod.Equals("PhoneOTP", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(nameof(LoginTwoFactor), new { userModel.Email, userModel.RememberMe, twoFactorAuthMethod, returnUrl });
                }
                if (twoFactorAuthMethod.Equals("TOTP", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(nameof(LoginTwoFactorTOTP), new { userModel.Email, userModel.RememberMe, returnUrl });
                }
            }
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                var forgotPassLink = Url.Action(nameof(ForgotPassword), "Account", new { }, Request.Scheme);
                var content = string.Format("Your account is locked out due to multiple invalid login attempts.To reset your password, please click this link: {0}", forgotPassLink);
                var message = new Message(new string[] { userModel.Email }, "Locked out account information", content, null);
                await _emailSender.SendEmailAsync(message);
                ModelState.AddModelError("", "The account is locked out due to multiple invalid login attempts.");
                return View();
            }
            else
            {
                var user = new User { Email = userModel.Email, UserName = userModel.Email };
                var IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (!IsEmailConfirmed)
                    ModelState.AddModelError("LoginError", "Email is not confirmed. Confirm Email first.");
                else
                    ModelState.AddModelError("LoginError", "Invalid UserName or Password");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginTwoFactor(string email, bool rememberMe, string twoFactorAuthMethod, string returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View(nameof(Error));
            }
            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                return View(nameof(Error));
            }
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var message = new Message(new string[] { email }, "Authentication OTP", "The OTP to access your account is : " + token, null);
            await _emailSender.SendEmailAsync(message);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginTwoFactor(TwoFactorAuthModel twoFactorModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(twoFactorModel);
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToAction(nameof(Error));
            }
            var result = await _signInManager.TwoFactorSignInAsync("Email", twoFactorModel.TwoFactorCode, twoFactorModel.RememberMe, rememberClient: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("LoginError", "The account is locked out due to multiple invalid login attempts.");
                return View();
            }
            else
            {
                ModelState.AddModelError("LoginError", "Invalid Login Attempt");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginTwoFactorTOTP(string email, bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View(nameof(Error), new ErrorModel { ErrorId = Guid.NewGuid().ToString(), ErrorMessage = "Unable to load two-factor authentication user." });
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginTwoFactorTOTPModel { ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //[HttpGet]
        //public async Task<IActionResult> LoginTwoFactorTOTP(string email, bool rememberMe, string returnUrl = null)
        //{
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        return View(nameof(Error), new ErrorModel { ErrorId = Guid.NewGuid().ToString(), ErrorMessage = "Unable to load two-factor authentication user." });
        //    }
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View(new LoginTwoFactorTOTPModel { ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginTwoFactorTOTP(LoginTwoFactorTOTPModel loginTwoFactorTOTPModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View(nameof(Error), new ErrorModel { ErrorId = Guid.NewGuid().ToString(), ErrorMessage = "Unable to load two-factor authentication user." });
            }

            var authenticatorCode = loginTwoFactorTOTPModel.Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, loginTwoFactorTOTPModel.RememberMe, loginTwoFactorTOTPModel.Input.RememberMachine);
            if(result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return RedirectToLocal(loginTwoFactorTOTPModel.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("LoginError", "The account is locked out due to multiple invalid login attempts.");
                return View();
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordModel);

            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ForgotPasswordConfirmation), new { IsUserValid = false });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Password Reset Token", "Click on this link to reset your password : " + callback, null);
            await _emailSender.SendEmailAsync(message);
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation(bool IsUserValid = true)
        {
            ViewData["Messsage"] = "";
            if (!IsUserValid)
                ViewData["Messsage"] = "User does not exists. Please enter a valid email address to reset password.";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            provider = provider.Equals("Azure AD") ? "OpenIdConnect" : provider;
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl, area = "Identity" });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (signInResult.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (signInResult.IsLockedOut)
                {
                    return RedirectToAction(nameof(ForgotPassword));
                }
                else
                {
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData["Provider"] = info.LoginProvider;
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    return View("ExternalLogin", new ExternalLoginModel { Email = email });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return View(nameof(Error));

            var user = await _userManager.FindByEmailAsync(model.Email);
            IdentityResult result;

            if (user != null)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
            else
            {
                model.Principal = info.Principal;
                user = _mapper.Map<User>(model);
                result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        var message = new Message(new string[] { user.Email }, "CoreApp Login", "You've been successfully logged in to CoreApp.", null);
                        await _emailSender.SendEmailAsync(message);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return View(nameof(ExternalLogin), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult Error(ErrorModel errorModel = null)
        {
            return View(errorModel);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
