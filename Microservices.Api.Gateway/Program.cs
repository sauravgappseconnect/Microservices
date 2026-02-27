using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microservices.Api.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var telemetryUrl = builder.Configuration.GetSection("TelemetryUrl").Value;
            if (!string.IsNullOrWhiteSpace(telemetryUrl))
            {
                builder.Services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource.AddService("Microservices.Api.Gateway"))
                    .WithTracing(tracing =>
                    {
                        tracing.AddAspNetCoreInstrumentation();
                        tracing.AddHttpClientInstrumentation();
                        tracing.AddConsoleExporter();
                        tracing.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(telemetryUrl);
                            options.Protocol = OtlpExportProtocol.Grpc;
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
                        .AddService("Microservices.Api.Gateway"));
                    options.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(telemetryUrl);
                        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });
                });
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.MapReverseProxy();

            app.Run();
        }
    }
}
