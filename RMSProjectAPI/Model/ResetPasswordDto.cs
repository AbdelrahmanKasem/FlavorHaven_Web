﻿using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Model
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
