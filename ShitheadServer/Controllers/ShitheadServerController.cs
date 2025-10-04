using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShitheadServer.Controllers;

[Route("api/shithead")]
[ApiController]
public class ShitheadServerController : ControllerBase
{
	private const string ROUTE_TABLE = "table";
	private const string ROUTE_PLAYER_NAME = "playerName";

	private readonly Server.ShitheadServer _server;

	public ShitheadServerController(
		Server.ShitheadServer server)
	{
		_server = server ?? throw new ArgumentNullException(nameof(server));
	}

	[HttpGet]
	public ActionResult<string> GetVersion()
	{
		var assembly = typeof(ShitheadServerController).Assembly;
		return $"Shithead v{assembly.GetName().Version}";
	}

	[HttpGet("{table}/create/{playerName}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> Create(
		[FromRoute(Name = ROUTE_TABLE)]
		[RegularExpression(@"^[\w\d -]+$", ErrorMessage = "Table name must not be empty")]
		string tableName,
		[FromRoute(Name = ROUTE_PLAYER_NAME)]
		[RegularExpression(@"^[\w\d -]+$", ErrorMessage = "Player name must not be empty")]
		string tableMasterName,
		CancellationToken cancellation)
	{
		if (!HttpContext.WebSockets.IsWebSocketRequest)
		{
			return BadRequest();
		}

		var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

		await _server.CreateTable(tableName, tableMasterName, ws, cancellation);

		return NoContent();
	}

	[HttpGet("{table}/join/{playerName}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> JoinPlayer(
		[FromRoute(Name = ROUTE_TABLE)]
		[RegularExpression(@"^[\w\d -]+$", ErrorMessage = "Table name must not be empty")]
		string tableName,
		[FromRoute(Name = ROUTE_PLAYER_NAME)]
		[RegularExpression(@"^[\w\d -]+$", ErrorMessage = "Player name must not be empty")]
		string playerName,
		CancellationToken cancellation)
	{
		await _server.JoinTable(
				tableName,
				playerName,
				() => HttpContext.WebSockets.AcceptWebSocketAsync(),
				cancellation);

		return NoContent();
	}

	[HttpPost("{table}/start/{connectionId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public ActionResult StartGame(
		[FromRoute(Name = ROUTE_TABLE)]
		[RegularExpression(@"^[\w\d -]+$", ErrorMessage = "Table name must not be empty")]
		string tableName,
		Guid connectionId)
	{
		try
		{
			_server.StartGame(tableName, connectionId);
			return NoContent();
		}
		catch (InvalidOperationException)
		{
			return BadRequest();
		}
	}
}
