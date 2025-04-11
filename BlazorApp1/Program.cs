using BlazorApp1;
using BlazorApp1.Components;
using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// 获取本机IP地址
string localIPAddress = GetLocalIPAddress();
string url = $"https://{localIPAddress}:5000"; // 你可以修改端口号
// 监听本机IP地址并配置端口
builder.WebHost.UseUrls(url);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.InitApp();
app.Run();


// 获取本机IP地址
static string GetLocalIPAddress()
{
    foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork &&
            !IPAddress.IsLoopback(ip))
        {
            return ip.ToString();
        }
    }
    throw new Exception("未找到本地局域网 IP 地址");
}