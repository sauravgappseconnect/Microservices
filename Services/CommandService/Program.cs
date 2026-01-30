
using CommandService.Data;
using Microsoft.EntityFrameworkCore;

namespace CommandService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<CommandServiceContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("db"));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Services.Initialiser().GetAwaiter().GetResult();

        app.Run();
    }
}
