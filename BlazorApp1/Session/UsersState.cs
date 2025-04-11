using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace BlazorApp1.Session;

public static class UsersState
{
    public const string CookieKey = "login_user";
    public static TimeSpan TimeOutSpan { get; } = TimeSpan.FromHours(1);
    public static string? InstantUsername { get; set; } = null;

    public static void Logout(HttpContext context)
    {
        context.Response.Cookies.Delete(CookieKey);
    }

    public static string? GetUsername(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue(CookieKey, out var username))
        {
            return username;
        }
        return null;
    }
}
