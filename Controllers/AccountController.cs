using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkolprojektLab1.CustomIdentity;
using SkolprojektLab1.Data;
using SkolprojektLab1.Models;
using SkolprojektLab1.ViewModels;

namespace SkolprojektLab1.Controllers
{
    public class AccountController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly SignInManager<Employee> signInManager;
        private readonly ApplicationDbContext context;

        public AccountController(CustomUserManager _userManager, SignInManager<Employee> _signInManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            context = _context;
        }
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var user = await userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null)
            {
                var passwordCheck = await userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (!String.IsNullOrEmpty(loginViewModel.returnUrl))
                        {
                            return Redirect(loginViewModel.returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewData["Error"] = "Wrong credentials. Please, try again";
                return View(loginViewModel);
            }
            ViewData["Error"] = "Wrong credentials. Please, try again";
            return View(loginViewModel);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var user = await userManager.FindByEmailAsync(registerViewModel.Email);
            if (user != null)
            {
                ModelState.AddModelError("Email", "This email already exists. Try again.");
                return View(registerViewModel);
            }

            var username = await context.Employees.FirstOrDefaultAsync(u => u.UserName == registerViewModel.Username);
            if (username != null)
            {
                ModelState.AddModelError("Username", "This username is already taken. Try again.");
                return View(registerViewModel);
            }

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.Street.ToLower() == registerViewModel.Street.ToLower() && a.City.ToLower() == registerViewModel.City.ToLower());
            if (address == null)
            {
                address = new Address
                {
                    Street = registerViewModel.Street,
                    City = registerViewModel.City
                };
                await context.Addresses.AddAsync(address);
                await context.SaveChangesAsync();
            }


            var newUser = new Employee()
            {
                FirstName = registerViewModel.Firstname,
                LastName = registerViewModel.Lastname,
                FK_Adress = address.Id,
                FK_Role = registerViewModel.roleType,
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                EmailConfirmed = true
            };

            var newUserResponse = await userManager.CreateAsync(newUser, registerViewModel.Password);


            if (newUserResponse.Succeeded)
            {
                TempData["Confirm"] = "The user has been created successfully!";
                return RedirectToAction("Confirmation", "Home");
            }
            TempData["Confirm"] = "There was an error. Please try again.";
            return RedirectToAction("Confirmation", "Home");
        }

        //make "ACCESS DENIED"-page URL IS:
        //Account/AccessDenied
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser()
        {
            List<Role> roleList = new List<Role>();
            roleList = await context.Roles.ToListAsync();
            var response = new RegisterViewModel();
            response.roleType = 0;
            response.roleList = new List<Role>();
            response.roleList.AddRange(roleList);
            return View(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Invalid"] = "Invalid properties. Please make sure all fields are filld!";
                return RedirectToAction("CreateUser", "Account");
            }

            var user = await userManager.FindByEmailAsync(registerViewModel.Email);
            if (user != null)
            {
                TempData["Email"] = "This email already exists. Try again.";
                return RedirectToAction("CreateUser", "Account");
            }

            var username = await context.Employees.FirstOrDefaultAsync(u => u.UserName == registerViewModel.Username);
            if (username != null)
            {
                TempData["Username"] = "This username is already taken. Try again.";
                return RedirectToAction("CreateUser", "Account");
            }

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.Street.ToLower() == registerViewModel.Street.ToLower() && a.City.ToLower() == registerViewModel.City.ToLower());
            if (address == null)
            {
                address = new Address
                {
                    Street = registerViewModel.Street,
                    City = registerViewModel.City
                };
                await context.Addresses.AddAsync(address);
                await context.SaveChangesAsync();
            }

            var newUser = new Employee()
            {
                FirstName = registerViewModel.Firstname,
                LastName = registerViewModel.Lastname,
                FK_Adress = address.Id,
                FK_Role = registerViewModel.roleType,
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                EmailConfirmed = true
            };

            var newUserResponse = await userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
            {
                //should redirect to confirmation page and send the "TempData" to it.
                TempData["Confirm"] = "The user has been created successfully!";
                return RedirectToAction("Confirmation", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData["LoggedOut"] = "You have been logged out successfully!";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
