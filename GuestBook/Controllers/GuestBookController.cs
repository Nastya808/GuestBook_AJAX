using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestBookApp.Data;
using GuestBookApp.Models;
using System.Threading.Tasks;

namespace GuestBookApp.Controllers
{
    public class GuestBookController : Controller
    {
        private readonly GuestBookContext _context;

        public GuestBookController(GuestBookContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string loginName, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == loginName && u.Pwd == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Registration(string loginName, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return Json(new { success = false, message = "Passwords do not match" });
            }

            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Name == loginName);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "User already exists" });
            }

            var user = new User { Name = loginName, Pwd = password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
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
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.Messages.Include(m => m.User).ToListAsync();
            return View(new IndexModel { Messages = messages });
        }
    }
}
