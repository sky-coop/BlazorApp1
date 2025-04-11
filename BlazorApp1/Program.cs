#define Render // ← 部署 Render 时启用，调试时注释掉这行

using BlazorApp1;
using BlazorApp1.Components;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

#if Render
// Render 环境配置（使用 http 并从环境变量获取端口）
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#else
// 本地调试配置（使用 https 和证书）
var cert = new X509Certificate2(
    Path.Combine(AppContext.BaseDirectory, "Input/FtpCert1.pfx"),
    "CertFtp");

string localIPAddress = GetLocalIPAddress();
string url = $"https://{localIPAddress}:5000";
builder.WebHost.UseUrls(url);

// 配置 Kestrel 使用 HTTPS 和本地证书
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Parse(localIPAddress), 5000, listenOptions =>
    {
        listenOptions.UseHttps(cert);
    });
});
#endif

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
#if !Render
app.UseHttpsRedirection();
#endif

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.InitApp();
app.Run();


// 获取本机IP地址
#pragma warning disable CS8321 // 已声明本地函数，但从未使用过
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
#pragma warning restore CS8321 // 已声明本地函数，但从未使用过
