using Microsoft.AspNetCore.Mvc;
using VacationManager.Data;
using VacationManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using VacationManager.Dtos;

namespace VacationManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Employee> _passwordHasher;

        public AccountController(ApplicationDbContext context, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        //  GET: /Account/Signup
        [HttpGet]
        public IActionResult Signup()
        {
            return View(); // Will load Views/Account/Signup.cshtml
        }

        // ✅ POST: /Account/Register
        [HttpPost]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var exists = await _context.Employees.AnyAsync(e => e.username == dto.username);
            if (exists)
            {
                ViewBag.Message = "Username already exists.";
                return View("Signup", dto);
            }

            // Password restrictions
            if (dto.password.Length < 8 ||
                !dto.password.Any(char.IsUpper) ||
                !dto.password.Any(char.IsLower) ||
                !dto.password.Any(char.IsDigit) ||
                !dto.password.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>?/\\`~".Contains(ch)))
            {
                ViewBag.Message = "Password must be at least 8 characters long, contain upper and lower case letters, a digit, and a special character.";
                return View("Signup", dto);
            }

            var employee = new Employee
            {
                username = dto.username,
                password = _passwordHasher.HashPassword(null, dto.password)
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // ✅ Auto-login after successful registration
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, employee.username),
        new Claim(ClaimTypes.Role, "Employee")
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }


        //  GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //  POST: /Account/Login (for Razor form)
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.username == dto.Username);
            if (employee == null)
            {
                ViewBag.Message = "Invalid username or password.";
                return View("Login", dto); // ✅ fixed
            }

            var result = _passwordHasher.VerifyHashedPassword(employee, employee.password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Message = "Invalid username or password.";
                return View("Login", dto); // ✅ fixed
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, employee.username),
        new Claim(ClaimTypes.Role, "Employee")
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home"); // ✅ Success
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


    }
}
