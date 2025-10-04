using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

const string ROUTE_TABLE = "table";
const string ROUTE_MASTER = "masterConnection";
const string ROUTE_PLAYER_NAME = "playerName";

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

string baseRoute = $"/shithead/{{{ROUTE_TABLE}}}";

app.MapGet(baseRoute, () => "hello");
app.MapGet(baseRoute + $"/create/{{{ROUTE_PLAYER_NAME}}}", CreateShitheadTable);
app.MapGet(baseRoute + $"/join/{{{ROUTE_PLAYER_NAME}}}", JoinShitheadTable);
app.MapPost(baseRoute + $"/start/{{{ROUTE_MASTER}}}", StartGame);

app.UseStaticFiles(new StaticFileOptions
{
	HttpsCompression = HttpsCompressionMode.Compress,
});

app.Run();

static async Task<IResult> CreateShitheadTable(
	HttpContext context,
	[FromRoute(Name = ROUTE_TABLE)] string tableName,
	[FromRoute(Name = ROUTE_PLAYER_NAME)] string tableMasterName,
	ShitheadServer.Server.ShitheadServer server)
{
	if (!context.WebSockets.IsWebSocketRequest ||
		string.IsNullOrEmpty(tableName) ||
		string.IsNullOrEmpty(tableMasterName))
	{
		return Results.BadRequest();
	}

	using var ws = await context.WebSockets.AcceptWebSocketAsync();

	await server.CreateTable(tableName, tableMasterName, ws);

	return Results.NoContent();
}

static async Task<IResult> JoinShitheadTable(
	HttpContext context,
	[FromRoute(Name = ROUTE_TABLE)] string tableName,
	[FromRoute] string playerName,
	ShitheadServer.Server.ShitheadServer server)
{
	return !context.WebSockets.IsWebSocketRequest ||
		string.IsNullOrEmpty(tableName) ||
		string.IsNullOrEmpty(playerName)
			? Results.BadRequest()
			: await server.JoinTable(
				tableName,
				playerName,
				() => context.WebSockets.AcceptWebSocketAsync());
}

static IResult StartGame(
	HttpContext context,
	[FromRoute(Name = ROUTE_TABLE)] string tableName,
	[FromRoute(Name = ROUTE_MASTER)] Guid connId,
	ShitheadServer.Server.ShitheadServer server)
{
	if (string.IsNullOrEmpty(tableName))
	{
		return Results.BadRequest();
	}

	return server.StartGame(tableName, connId);
}
