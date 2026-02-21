
using Microservices.Common.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
        builder.Services.AddCors(options =>
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
            if (allowedOrigins != null && allowedOrigins.Any())
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowCredentials()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            }
        });

        var telemetryUrl = builder.Configuration.GetSection("TelemetryUrl").Value;
        if(!string.IsNullOrWhiteSpace(telemetryUrl))
        {
            builder.Services.AddOpenTelemetry()
                    .ConfigureResource(resource =>
                    {
                        resource.AddService("platformservice");
                    })
                    .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()// Automatically create spans for incoming HTTP requests (ASP.NET Core middleware)
                .AddHttpClientInstrumentation()         // Automatically create spans for outgoing HTTP calls (HttpClient)
                .AddConsoleExporter()
                .AddOtlpExporter(options =>        // Export spans to OpenTelemetry Collector
                {
                    options.Endpoint = new Uri(telemetryUrl);
                    options.Protocol = OtlpExportProtocol.Grpc;
                    // Endpoint of the collector inside Docker network
                    // Using default protocol = gRPC
                    // If needed, we could explicitly set:
                    // options.Protocol = OtlpExportProtocol.Grpc;
                }))
                    .WithMetrics(m => {
                        m.AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(telemetryUrl);
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
                    });
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("platformservice"));
                options.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(telemetryUrl);
                    otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        }

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
