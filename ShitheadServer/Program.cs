using ShitheadServer.Server.DST;

const string ROUTE_TABLE = "table";
const string ROUTE_MASTER = "masterConnection";

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddJsonConsole();
builder.Services.AddSingleton<ShitheadServer.Server.ShitheadServer>();


var app = builder.Build();

app.MapPost("/sheethead/create", CreateShitheadTable);
app.MapPost($"/sheethead/{{{ROUTE_TABLE}}}/join", JoinShitheadTable);
app.MapPost($"/sheethead/{{{ROUTE_TABLE}}}/{{{ROUTE_MASTER}}}/start", StartGame);

app.Run();

static async Task<IResult> CreateShitheadTable(
	HttpContext contex,
	ShitheadServer.Server.ShitheadServer server)
{
	var body = await contex.Request.ReadFromJsonAsync<CreateTableRequest>();

	if (!contex.WebSockets.IsWebSocketRequest ||
		string.IsNullOrEmpty(body?.TableName) ||
		string.IsNullOrEmpty(body?.MasterName))
	{
		return Results.BadRequest();
	}

	using var ws = await contex.WebSockets.AcceptWebSocketAsync();

	await server.CreateTable(body.TableName, body.MasterName, ws);

	return Results.NoContent();
}

static async Task<IResult> JoinShitheadTable(HttpContext context, ShitheadServer.Server.ShitheadServer server)
{
	string? tableName = context.Request.RouteValues[ROUTE_TABLE] as string;
	var body = await context.Request.ReadFromJsonAsync<JoinTableRequest>();

	if (!context.WebSockets.IsWebSocketRequest ||
		string.IsNullOrEmpty(tableName) ||
		string.IsNullOrEmpty(body?.PlayerName))
	{
		return Results.BadRequest();
	}

	using var ws = await context.WebSockets.AcceptWebSocketAsync();

	await server.JoinTable(tableName, body.PlayerName, ws);

	return Results.NoContent();
}

static IResult StartGame(
	HttpContext context,
	ShitheadServer.Server.ShitheadServer server)
{
	string? tableName = context.Request.RouteValues[ROUTE_TABLE] as string;
	string? connIdStr = context.Request.RouteValues[ROUTE_MASTER] as string;

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