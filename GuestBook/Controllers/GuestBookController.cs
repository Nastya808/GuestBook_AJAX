using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestBookApp.Data;
using GuestBookApp.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace GuestBookApp.Controllers
{
    public class GuestBookController : Controller
    {
        private readonly GuestBookContext _context;

        public GuestBookController(GuestBookContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var messages = await _context.Messages.Include(m => m.User).ToListAsync();
            var model = new GuestBookApp.Models.IndexModel
            {
                Messages = messages
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(string newMessage)
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(newMessage) && userName != null)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == userName);
                if (user != null)
                {
                    var message = new Message
                    {
                        Id_User = user.Id,
                        MessageText = newMessage,
                        MessageDate = DateTime.Now
                    };
                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        user = user.Name,
                        message = message.MessageText,
                        date = message.MessageDate.ToString("g")
                    });
                }
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string loginName, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == loginName && u.Pwd == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(string loginName, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View();
            }

            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Name == loginName);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "User already exists.");
                return View();
            }

            var user = new User
            {
                Name = loginName,
                Pwd = password
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
