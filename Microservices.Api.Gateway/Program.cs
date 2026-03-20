using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Yarp.ReverseProxy.Transforms;

namespace Microservices.Api.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddTransforms(builderContext => {
                    builderContext.AddResponseTransform(context =>
                    {
                        var traceId = Activity.Current?.TraceId.ToString();
                        context.HttpContext.Response.Headers["request-id"] = !string.IsNullOrEmpty(traceId) ? traceId : "no-trace";
                        return ValueTask.CompletedTask;
                    });
                });

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

            //app.UseHttpsRedirection();

            app.UseCors();
            app.MapReverseProxy();

            app.Run();
        }
    }
}
