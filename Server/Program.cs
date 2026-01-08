using LegalAI.ThirdPartyApi.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        // 添加内存缓存（用于限流）
        builder.Services.AddMemoryCache();
        // 注册 Swagger 服务
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LegalAI API",
                Version = "v1",
                Description = "智能法律咨询助手 API 接口文档",
                Contact = new OpenApiContact
                {
                    Name = "LegalAI Team",
                    Email = "support@legalai.com",
                    Url = new Uri("https://www.legalai.com")
                }
            });

            // 添加 XML 注释支持
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // 注册 AI 服务
        builder.Services.AddAIProviders(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LegalAI API V1");
                c.RoutePrefix = ""; // 设置访问路径为 /
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();


        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}