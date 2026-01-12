using LegalAI.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 👇 添加这行
builder.Services.AddMudServices();

builder.RootComponents.Add<App>("#app");

// 配置 HttpClient，使用相对路径或代理路径
var baseUrl = "https://172.25.48.223:7000/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUrl) });

await builder.Build().RunAsync();