﻿namespace AppDating.API.DTO
{
    public class AppUserDTO
    {
        public required string Username { get; set; }
        public required string knownAs { get; set; }
        public required string Token { get; set; }
        public required string Gender { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
