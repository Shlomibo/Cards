using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole(options =>
{
	options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions { Indented = true };
	options.IncludeScopes = true;
});
builder.Services.AddSingleton<ShitheadServer.Server.ShitheadServer>();

var app = builder.Build();

app.UseWebSockets();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
	HttpsCompression = HttpsCompressionMode.Compress,
});

await app.RunAsync();
