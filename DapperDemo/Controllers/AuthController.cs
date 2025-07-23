using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DapperDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthRepository _repo;

        public AuthController(AuthRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(UserDto dto)
        {
            if (_repo.Register(dto))
                return RedirectToAction("Login");

            ViewBag.Error = "Email already exists.";
            return View(dto);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(UserDto dto)
        {
            var user = _repo.AuthenticateUser(dto.Email, dto.Password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role); 
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials.";
            return View(dto);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login");
        }

        public IActionResult MyProfile()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var user = _repo.GetUserByEmail(email);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordDto model)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var success = _repo.ChangePassword(email, model.CurrentPassword, model.NewPassword);

            if (success)
            {
                ViewBag.Message = "Password changed successfully!";
                return View();
            }

            ViewBag.Error = "Current password is incorrect.";
            return View(model);
        }


    }
}
