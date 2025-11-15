using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Features.Shared.Helpers;

public static class AuthenticationHelper
{
    public static async Task LoginUser(HttpContext context, User user)
    {
        var claims = new List<Claim>
            {new("Username", user.Username), new("Token", user.Token), new("Image", user.Image)};

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
            {ExpiresUtc = DateTimeOffset.UtcNow.AddDays(60), IsPersistent = true};

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties
        );
    }

    public static async Task Logout(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }


    public static User? GetUser(this HttpContext context)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated) return null;

        var token = context.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value ??
                    throw new Exception("Token not found");
        var image = context.User.Claims.FirstOrDefault(c => c.Type == "Image")?.Value ??
                    throw new Exception("Image not found");
        var username = context.User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value ??
                       throw new Exception("Username not found");
        return new User
        {
            Token = token,
            Image = image,
            Username = username,
            Bio = "",
            Email = ""
        };
    }
}