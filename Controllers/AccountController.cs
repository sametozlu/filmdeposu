using System.Security.Claims;
using FilmSerileri.Entities;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class AccountController : Controller
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly IAppEmailSender _emailSender;

  public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAppEmailSender emailSender)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _emailSender = emailSender;
  }

  [HttpGet]
  public IActionResult Login(string? returnUrl = null) =>
    View(new LoginViewModel { ReturnUrl = returnUrl });

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> Login(LoginViewModel model)
  {
    if (!ModelState.IsValid) return View(model);

    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
    if (result.Succeeded)
      return Redirect(string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/" : model.ReturnUrl);

    ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
    return View(model);
  }

  [HttpGet]
  public IActionResult Register() => View(new RegisterViewModel());

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> Register(RegisterViewModel model)
  {
    if (!ModelState.IsValid) return View(model);

    var user = new ApplicationUser
    {
      UserName = model.Email,
      Email = model.Email,
      DisplayName = model.DisplayName
    };

    var result = await _userManager.CreateAsync(user, model.Password);
    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
        ModelState.AddModelError(string.Empty, error.Description);
      return View(model);
    }

    await _signInManager.SignInAsync(user, isPersistent: true);
    return RedirectToAction("Index", "Home");
  }

  [HttpPost, ValidateAntiForgeryToken]
  public IActionResult ExternalLogin(string provider, string? returnUrl = null)
  {
    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    return Challenge(properties, provider);
  }

  [HttpGet]
  public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
  {
    if (remoteError != null)
    {
      TempData["AccountMessage"] = "external_failed";
      return RedirectToAction(nameof(Login));
    }

    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
      TempData["AccountMessage"] = "external_failed";
      return RedirectToAction(nameof(Login));
    }

    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
    if (result.Succeeded)
      return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);

    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrWhiteSpace(email))
    {
      TempData["AccountMessage"] = "external_failed";
      return RedirectToAction(nameof(Login));
    }

    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      user = new ApplicationUser
      {
        UserName = email,
        Email = email,
        DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0],
        EmailConfirmed = true
      };

      var createResult = await _userManager.CreateAsync(user);
      if (!createResult.Succeeded)
      {
        TempData["AccountMessage"] = "external_failed";
        return RedirectToAction(nameof(Login));
      }
    }

    await _userManager.AddLoginAsync(user, new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName));
    await _signInManager.SignInAsync(user, isPersistent: true);
    return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
  }

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> Logout()
  {
    await _signInManager.SignOutAsync();
    return RedirectToAction("Index", "Home");
  }

  [HttpGet]
  public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
  {
    if (!ModelState.IsValid) return View(model);

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user != null)
    {
      var token = await _userManager.GeneratePasswordResetTokenAsync(user);
      var link = Url.Action("ResetPassword", "Account",
        new { email = model.Email, token }, Request.Scheme)!;

      await _emailSender.SendAsync(model.Email, "Film Deposu - Şifre Sıfırlama",
        $"<p>Şifreni sıfırlamak için tıkla: <a href=\"{link}\">{link}</a></p>");
    }

    // Hesabın var olup olmadığını sızdırmamak için her durumda aynı mesaj
    TempData["AccountMessage"] = "reset_sent";
    return RedirectToAction(nameof(Login));
  }

  [HttpGet]
  public IActionResult ResetPassword(string email, string token) =>
    View(new ResetPasswordViewModel { Email = email, Token = token });

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
  {
    if (!ModelState.IsValid) return View(model);

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user != null)
    {
      var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
      if (!result.Succeeded)
      {
        foreach (var error in result.Errors)
          ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
      }
    }

    TempData["AccountMessage"] = "reset_done";
    return RedirectToAction(nameof(Login));
  }
}
