﻿namespace GuestBookApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  
        public string Pwd { get; set; } = string.Empty;  
    }
}
