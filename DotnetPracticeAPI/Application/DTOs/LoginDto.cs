﻿namespace Application.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
    }
}
