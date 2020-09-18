using Data;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Web.Models.Home;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyGameDb _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _context = new MyGameDb();
            _httpContextAccessor = httpContextAccessor;
        }

        //GET: /Index
        public IActionResult Index()
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") == 1)
            {
                return Redirect("/Home/WelcomePage");
            }
            else
            {
                LoginViewModel model = new LoginViewModel();

                return View(model);
            }
        }

        //POST: /Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") == 1)
            {
                return Redirect("/Home/WelcomePage");
            }
            else
            {
                User user = _context.Users.ToArray().Where(u => u.Username == model.Username).FirstOrDefault();
                if (user != null)
                {
                    if (user.PasswordHash == GetPasswordHash(model.PasswordHash))
                    {
                        SessionExtensions.Set<int>(_session, "LoggedIn", 1);
                        CookieExtensions.SetCookie(Response, "Username", user.Username);

                        return Redirect("/Home/WelcomePage");
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordHash", "Password does not match!");
                        return View(model);
                    }
                }

                ModelState.AddModelError("Username", "User with that username does not exist!");
                return View(model);
            }
        }

        //GET: /WelcomePage
        public IActionResult WelcomePage()
        {
            ViewData["Title"] = "Welcome";

            return View();
        }

        //GET: /Register
        public IActionResult Register()
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") == 1)
            {
                return Redirect("/Home/WelcomePage");
            }
            else
            {
                RegisterViewModel model = new RegisterViewModel();

                return View(model);
            }
        }

        //POST: /Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") == 1)
            {
                return Redirect("/Home/WelcomePage");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    User user = new User
                    {
                        Username = model.Username,
                        PasswordHash = GetPasswordHash(model.Password),
                        GamesWon = 0
                    };

                    if (_context.Users.ToArray().Select(u => u.Username).ToArray().Contains(user.Username))
                    {
                        ModelState.AddModelError("Username", "User with that username already exists");
                        return View(model);
                    }

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    SessionExtensions.Set<int>(_session, "LoggedIn", 1);
                    CookieExtensions.SetCookie(Response, "Username", user.Username);

                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
        }

        //POST: /Game
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Game()
        {
            if (SessionExtensions.Get<int>(_session, "LoggedIn") != 1)
            {
                return Redirect("/Home/WelcomePage");
            }
            else
            {
                SessionExtensions.Set<string>(_session, "GuessKey", "");

                return Redirect("/Game");
            }
        }

        //GET: /Logout
        public IActionResult Logout()
        {
            SessionExtensions.Set<int>(_session, "LoggedIn", 0);
            CookieExtensions.RemoveCookie(Response, "Username");

            return Redirect("/");
        }

        private string GetPasswordHash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}