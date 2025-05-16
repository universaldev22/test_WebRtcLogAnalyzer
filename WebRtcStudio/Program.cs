using Microsoft.OpenApi.Models;
using WebRtcLogAnalyzer.Application.Contracts;
using WebRtcLogAnalyzer.Infrastructure.Parsing;
using WebRtcLogAnalyzer.Application.DTOs;

public class Program  // Changed to public
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ---------- Services ----------
        builder.Services.AddSingleton<ILogAnalysisService, LogParser>();

        // ---------- Swagger / OpenAPI ----------
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opts =>
        {
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WebRTC Log Analyzer API",
                Version = "v1",
                Description = "Returns aggregated user-activity and error statistics from webrtc_studio.log"
            });

            // Add response type info to swagger
            opts.MapType<LogAnalysisResult>(() => new OpenApiSchema
            {
                Type = "object",
                Properties = {
                    ["uniqueUsers"] = new OpenApiSchema { Type = "integer" },
                    ["userActivity"] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema { Type = "object", Properties = {
                            ["userId"] = new OpenApiSchema { Type = "string" },
                            ["event"] = new OpenApiSchema { Type = "string" },
                            ["timestamp"] = new OpenApiSchema { Type = "string" }
                        }}
                    },
                    ["errors"] = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = {
                            ["ERROR"] = new OpenApiSchema { Type = "integer" },
                            ["CRITICAL"] = new OpenApiSchema { Type = "integer" },
                            ["WARNING"] = new OpenApiSchema { Type = "integer" }
                        }
                    }
                }
            });
        });

        var app = builder.Build();

        // ---------- Middleware ----------
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Log Analyzer v1");
                c.RoutePrefix = "swagger";  // → http://localhost:5000/swagger
            });
        }

        // ---------- Endpoints ----------
        app.MapGet("/api/loganalysis",
            async (ILogAnalysisService service, CancellationToken ct) =>
            {
                var contentRoot = builder.Environment.ContentRootPath;
                var solutionRoot = Directory.GetParent(contentRoot)!.FullName;
                string originLogFilePath = Path.Combine(solutionRoot, "logs", "webrtc_studio.log");

                if (!File.Exists(originLogFilePath))
                {
                    return Results.NotFound($"Log file not found at: {originLogFilePath}");
                }

                var result = await service.AnalyzeAsync(originLogFilePath, ct);
                return Results.Ok(new
                {
                    uniqueUsers = result.UniqueUsers,
                    userActivity = result.UserActivity,
                    errors = result.Errors
                });
            })
           .WithName("GetLogAnalysis")
           .WithSummary("Returns log-analysis results")
           .WithDescription("Parses webrtc_studio.log and returns user JOIN/LEAVE events, the unique user count, and error counts grouped by severity.")
           .Produces<LogAnalysisResult>(200); // Define response type

        // ---------- Run ----------
        app.Run();
    }
}

