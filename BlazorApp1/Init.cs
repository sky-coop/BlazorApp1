using BlazorApp1.Session;

namespace BlazorApp1;

public static class Init
{
    public static void InitApp(this WebApplication _)
    {
        LoginDB.Init();
    }
}
