using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snap.APIs.Errors;
using Snap.APIs.Extensions;
using Snap.APIs.Middlewares;
using Snap.Repository.Data;
using Snap.Repository.Seeders;
using Snap.Core.Entities;
using System.Text.Json.Serialization;
using System.Net.WebSockets;

namespace Snap.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. إعداد الخدمات (Services)
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            builder.Services.AddDbContext<SnapDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                })
            );

            builder.Services.AddIdentityServices();
            // ❌ تم إزالة SignalR

            builder.Services.AddHostedService<Snap.APIs.Services.OrderCancellationService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // إعداد CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // 2. تطبيق Migrations و Seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var dbContext = services.GetRequiredService<SnapDbContext>();
                    await dbContext.Database.MigrateAsync();

                    var userManager = services.GetRequiredService<UserManager<User>>();
                    await UserSeed.SeedUserAsync(userManager);
                }
                catch (Exception e)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(e, "An error occurred while applying migrations.");
                }
            }

            // 3. إعداد HTTP Pipeline
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // =================================================================
            // ⚡ بداية إعداد Raw WebSockets (الطريقة المحسنة)
            // =================================================================

            // إعداد خيارات الـ WebSocket (KeepAlive مهم جداً للموبايل)
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            app.UseWebSockets(webSocketOptions);

            // استخدام app.Map لتنظيم الكود وعزل مسار الـ WebSocket
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    // إنشاء Scope للحصول على DbContext بشكل آمن
                    using (var scope = context.RequestServices.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<SnapDbContext>();

                        // استدعاء المعالج وتمرير الـ DbContext
                        // ملاحظة: تأكد أن الدالة Handle في WebSocketConnectionHandler تقبل (HttpContext, WebSocket, SnapDbContext)
                        await WebSocketConnectionHandler.Handle(context, webSocket, dbContext);
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            });

            // =================================================================

            app.MapControllers();

            app.Run();
        }
    }
}