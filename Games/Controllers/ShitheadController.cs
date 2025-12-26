using System.Net;
using Games.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShitheadController : ControllerBase
{
    [Route("create/{tableName}/{playerName}")]
    public async Task CreateTable(string tableName, string playerName, CancellationToken cancellation)
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
    [HttpPost("start/{tableName}/{masterId}")]
    public async Task<ActionResult> StartGame(string tableName, Guid masterId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}
