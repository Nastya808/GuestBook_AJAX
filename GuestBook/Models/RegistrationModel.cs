﻿namespace GuestBookApp.Models
{
    public class RegistrationModel
    {
        public string LoginName { get; set; } = string.Empty;  
        public string Password { get; set; } = string.Empty;  
        public string ConfirmPassword { get; set; } = string.Empty;  
    }
}
