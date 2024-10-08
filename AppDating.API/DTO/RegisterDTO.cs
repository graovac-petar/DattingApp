﻿using System.ComponentModel.DataAnnotations;

namespace AppDating.API.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string? KnownAs { get; set; }

        [Required]
        public string? Gender { get; set; }
        [Required]
        public string? DateOfBirth { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;


    }
}
