﻿using System.ComponentModel.DataAnnotations;

namespace Ecommerce_app.Areas.Identity.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
