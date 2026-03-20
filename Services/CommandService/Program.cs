
using CommandService.Data;
using CommandService.HostedServices;
using CommandService.Services;
using Microservices.Common.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
        builder.Services.AddHostedService<MessageProcessor>();
        builder.Services.AddSingleton<IMessageReceiver, ServiceBusMessageReceiver>();

        var telemetryUrl = builder.Configuration.GetSection("TelemetryUrl").Value;
        if (!string.IsNullOrWhiteSpace(telemetryUrl))
        {
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(options =>
                {
                    options.AddService("commandservice");
                })
                .WithTracing(options =>
                {
                    options.AddAspNetCoreInstrumentation()
                    .AddSource("servicebusmessagereceiver")
                    .AddConsoleExporter()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(exp =>
                    {
                        exp.Endpoint = new Uri(telemetryUrl);
                        exp.Protocol = OtlpExportProtocol.Grpc;
                    });
                })
                .WithMetrics(m =>
                {
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
                    .AddService("commandservice"));
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
