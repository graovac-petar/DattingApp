﻿using System.Security.Claims;

namespace AppDating.API.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var username = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("No username found in token");
            return username;

        }
    }
}
