using BlazorApp1;
using BlazorApp1.Components;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

//var cert = new X509Certificate2(
//    Path.Combine(AppContext.BaseDirectory, "Input/FtpCert1.pfx"),
//    "CertFtp");
//string localIPAddress = GetLocalIPAddress();
//string url = $"https://{localIPAddress}:5000"; // 你可以修改端口号
//// 监听本机IP地址并配置端口
//builder.WebHost.UseUrls(url);
//// ✅ 配置 HTTPS 使用自定义证书
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.Listen(IPAddress.Parse(localIPAddress), 5000, listenOptions =>
//    {
//        listenOptions.UseHttps(cert);
//    });
//});

// 从 Render 环境变量获取端口（否则默认为 5000）
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

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