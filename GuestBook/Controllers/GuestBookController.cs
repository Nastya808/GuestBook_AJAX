using Microsoft.AspNetCore.Mvc;
using GuestBookApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace GuestBookApp.Controllers
{
    public class GuestBookController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Message> _messageRepository;

        public GuestBookController(IUserRepository userRepository, IRepository<Message> messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string name, string password)
        {
            var user = await _userRepository.GetByNameAsync(name); 
            if (user != null && user.Pwd == password)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Registration(string name, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Name and password cannot be empty" });
            }

            if (password != confirmPassword)
            {
                return Json(new { success = false, message = "Passwords do not match" });
            }

            var existingUser = await _userRepository.GetByNameAsync(name);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "User already exists" });
            }

            var user = new User { Name = name, Pwd = password }; 
            await _userRepository.AddAsync(user);

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> AddMessage(string newMessage)
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "User is not logged in" });
            }

            if (string.IsNullOrEmpty(newMessage))
            {
                return Json(new { success = false, message = "Message cannot be empty" });
            }

            var user = await _userRepository.GetByNameAsync(userName); 
            if (user != null)
            {
                var message = new Message
                {
                    Id_User = user.Id,
                    MessageText = newMessage,
                    MessageDate = DateTime.Now
                };
                await _messageRepository.AddAsync(message);

                return Json(new
                {
                    success = true,
                    user = user.Name,
                    message = message.MessageText,
                    date = message.MessageDate.ToString("g")
                });
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "User is not logged in" });
            }

            HttpContext.Session.Remove("UserName"); 
            return Json(new { success = true });
        }


    }
}
