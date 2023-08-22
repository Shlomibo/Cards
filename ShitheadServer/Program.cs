using Microsoft.AspNetCore.Mvc;

const string ROUTE_TABLE = "table";
const string ROUTE_MASTER = "masterConnection";
const string ROUTE_PLAYER_NAME = "playerName";

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddJsonConsole();
builder.Services.AddSingleton<ShitheadServer.Server.ShitheadServer>();

var app = builder.Build();

app.UseWebSockets();
string baseRoute = $"/shithead/{{{ROUTE_TABLE}}}";

app.MapGet(baseRoute, () => "hello");
app.MapGet(baseRoute + $"/create/{{{ROUTE_PLAYER_NAME}}}", CreateShitheadTable);
app.MapGet(baseRoute + $"/join/{{{ROUTE_PLAYER_NAME}}}", JoinShitheadTable);
app.MapPost(baseRoute + $"/start/{{{ROUTE_MASTER}}}", StartGame);

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
	if (!context.WebSockets.IsWebSocketRequest ||
		string.IsNullOrEmpty(tableName) ||
		string.IsNullOrEmpty(playerName))
	{
		return Results.BadRequest();
	}

	using var ws = await context.WebSockets.AcceptWebSocketAsync();

	await server.JoinTable(tableName, playerName, ws);

	return Results.NoContent();
}

static IResult StartGame(
	HttpContext context,
	[FromRoute(Name = ROUTE_TABLE)]string tableName,
	[FromRoute(Name = ROUTE_MASTER)] string connIdStr,
	ShitheadServer.Server.ShitheadServer server)
{

	if (!context.WebSockets.IsWebSocketRequest ||
		string.IsNullOrEmpty(tableName) ||
		string.IsNullOrEmpty(connIdStr) ||
		!Guid.TryParse(connIdStr, out var connId))
	{
		return Results.BadRequest();
	}

	server.StartGame(tableName, connId);
	return Results.NoContent();
}