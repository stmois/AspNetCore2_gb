using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels.Identity;

namespace WebStore.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View(new RegisterUserViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            UserName = model.UserName,
        };

        var registrationResult = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(true);

        if (registrationResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Role.USERS).ConfigureAwait(true);
            await _signInManager.SignInAsync(user, false).ConfigureAwait(true);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in registrationResult.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    public IActionResult Login(string returnUrl)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginResult = await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            true).ConfigureAwait(true);

        if (loginResult.Succeeded)
        {
            return LocalRedirect(model.ReturnUrl ?? "/");
        }

        ModelState.AddModelError("", "Неверное имя пользователя, или пароль");

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync().ConfigureAwait(true);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}