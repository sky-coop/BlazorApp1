using BlazorApp1.UserData;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorApp1.Session;
public abstract class AuthorizedComponentBase : ComponentBase
{
    [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected bool IsLoggedIn => CurrentUsername != null;
    protected string? CurrentUsername { get; set; }
    protected Data? UserData { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var context = HttpContextAccessor.HttpContext;
        if (context != null)
        {
            CurrentUsername = UsersState.GetUsername(context);
        }
        UserData = Data.Load(CurrentUsername);
    }

    protected void GotoHome()
    {
        Navigation.NavigateTo("/", true);
    }
}
