using System.ComponentModel.DataAnnotations;
using System.Net;
using DTOs.Responses;
using Games.Filters;
using Games.Services.Shithead;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShitheadController : ControllerBase
{
    private readonly ILogger<ShitheadController> _logger;
    private readonly IShitheadTablesManager _tablesManager;

    public ShitheadController(
        ILogger<ShitheadController> logger,
        IShitheadTablesManager tablesManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tablesManager = tablesManager ?? throw new ArgumentNullException(nameof(tablesManager));
    }

    [Route("create/{tableName}/{playerName}/{totalPlayersCount}")]
    public async Task CreateTable(
        string tableName,
        string playerName,
        int totalPlayersCount,
        CancellationToken cancellation)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, new
            {
                Message = "Connection must be a web socket",
            });
        }

        using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
    }

    [Route("join/{tableName}/{playerName}")]
    public async Task JoinTable(string tableName, string playerName, CancellationToken cancellation)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, new
            {
                Message = "Connection must be a web socket",
            });
        }

        using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("start/{tableName}")]
    public async Task<ActionResult> StartGame(
        string tableName,
        [Required, FromHeader(Name = "x-connection-id")] Guid masterId,
        CancellationToken cancellationToken)
    {
        return NoContent();
    }

    [HttpGet("{tableName}/{playerName}")]
    public async Task<ActionResult<CanJoinTableResponse>> CanJoin(
        string tableName,
        string playerName,
        CancellationToken cancellation)
    {
        bool result = _tablesManager.CanJoinTable(tableName, playerName);
        return Ok(new CanJoinTableResponse { CanJoin = result });
    }
}
