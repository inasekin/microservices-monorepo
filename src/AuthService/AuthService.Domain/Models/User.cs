﻿namespace AuthService.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}