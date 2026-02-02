
using Microservices.Common.Services;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Services;

namespace PlatformService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<PlatformServiceContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("db"));
        });
        builder.Services.AddSingleton<IMessageSender, ServiceBusMessageSender>();
        builder.Services.AddCors(options => {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
            if (allowedOrigins != null && allowedOrigins.Any())
            {
                options.AddDefaultPolicy(policy => {
                    policy.WithOrigins(allowedOrigins)
                        .AllowCredentials()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            }
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseCors();

        //app.UseAuthorization();

        app.MapControllers();

        app.Services.Initialiser().GetAwaiter().GetResult();

        app.Run();
    }
}
