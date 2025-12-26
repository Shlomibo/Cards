using DTOs;
using Games.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi()
        .AddSerilog()
        .ConfigureHttpJsonOptions(
            options => JsonOptions.SetJsonSerializationOptions(options.SerializerOptions))
        .AddSingleton<IAsyncExceptionFilter>(new HttpResponseExceptionFilter());

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapOpenApi();
    app.MapControllers();
    app.UseExceptionHandler();

    app.UseWebSockets();
    app.UseHttpsRedirection();
    app.UseHttpLogging();

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "💩");
    return -1;
}
