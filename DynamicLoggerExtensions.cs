using System.Reflection.Emit;
using DynamicLogger.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DynamicLogger;

public static class DynamicLoggerExtensions
{
    // Service configuration extension
    public static ILoggingBuilder AddLoggerServices(this ILoggingBuilder logging, IServiceCollection services)
    {
        logging.ClearProviders();
        services.AddSingleton<ConsoleLoggerProvider>();
        logging.AddProvider(new DynamicLoggerProvider(services.BuildServiceProvider().GetRequiredService<ConsoleLoggerProvider>()));
        return logging;
    }

    public static IEndpointRouteBuilder MapLoggerEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // GET: Returns all log categories currently tracked.
        endpoints.MapGet("/logging/getCategories", (IServiceProvider services) =>
        {
            return Results.Ok(DynamicLoggerChanger.GetCategories());
        });

        // POST: // Set log level for all categories in your dictionary
        endpoints.MapPost("/logging/setlevel/{level}", (string level, IServiceProvider services) =>
        {
            if (Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
            {
                DynamicLoggerChanger.SetLogLevel("Default", parsedLevel);
                return Results.Ok($"Log level set to {parsedLevel}");
            }
            return Results.BadRequest("Invalid log level. Use: Trace, Debug, Information, Warning, Error, Critical, None");
        });

        // POST: Sets log level for all non-system (application-specific) categories.
        endpoints.MapPost("/logging/set-nonsystem-level/{level}", (string level, IServiceProvider services) =>
        {
            if (Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
            {
                DynamicLoggerChanger.SetLogLevelToNonSystemCategories(parsedLevel);
                return Results.Ok($"Log level set to {parsedLevel}");
            }
            return Results.BadRequest("Invalid log level. Use: Trace, Debug, Information, Warning, Error, Critical, None");
        });

        // POST: Sets log level for a specific category.
        endpoints.MapPost("/logging/set-category-level/{category}/{level}", (string category, string level, IServiceProvider services) =>
        {
            if (Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
            {
                DynamicLoggerChanger.SetLogLevel(category, parsedLevel);
                return Results.Ok(
                    category == "Default"
                    ? $"Log level set to {parsedLevel} for all categories"
                    : $"Log level for '{category}' set to {parsedLevel}"
                );
            }
            return Results.BadRequest("Invalid log level. Use: Trace, Debug, Information, Warning, Error, Critical, None");
        });

        // POST: Sets log level for all categories with a given prefix.
        endpoints.MapPost("/logging/set-prefix-level/{prefix}/{level}", (string prefix, string level, IServiceProvider services) =>
        {
            if (Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
            {
                DynamicLoggerChanger.SetLogLevelToPrefixCategory(prefix, parsedLevel);
                return Results.Ok($"Log level set to {parsedLevel}");
            }
            return Results.BadRequest("Invalid log level. Use: Trace, Debug, Information, Warning, Error, Critical, None");
        });

        return endpoints;
    }

    // Variant allowing a configurable path prefix for setlevel
    public static IEndpointRouteBuilder MapLoggerEndpoint(this IEndpointRouteBuilder endpoints, string path)
    {
        endpoints.MapPost("{path}/{level}", (string level, IServiceProvider services) =>
        {
            if (Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
            {
                DynamicLoggerChanger.SetLogLevel("Default", parsedLevel);
                return Results.Ok($"Log level set to {parsedLevel}");
            }
            return Results.BadRequest("Invalid log level. Use: Trace, Debug, Information, Warning, Error, Critical, None");
        });



        return endpoints;
    }
}
