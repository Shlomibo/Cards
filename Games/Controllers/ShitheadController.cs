using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using DTOs;
using DTOs.Responses;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using Games.Filters;
using Games.Services.Shithead;
using GameServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shithead.Moves;
using Shithead.State;
using JsonOptions = DTOs.JsonOptions;

namespace Games.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShitheadController : ControllerBase
{
    private const int BUFFER_SIZE = 10 * 1024;
    private static readonly JsonSerializerOptions _serializationOptions = JsonOptions.SetJsonSerializationOptions();

    private readonly ILogger<ShitheadController> _logger;
    private readonly IShitheadTablesManager _tablesManager;

    public ShitheadController(
        ILogger<ShitheadController> logger,
        IShitheadTablesManager tablesManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tablesManager = tablesManager ?? throw new ArgumentNullException(nameof(tablesManager));
    }

    [Route("create/{tableName}/{playerName}")]
    public async Task CreateTable(
        string tableName,
        string playerName,
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
        using var connection = _tablesManager.CreateTable(tableName, playerName);

        await BindConnectionAndSocket(socket, connection, cancellation);
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
        using var connection = _tablesManager.JoinTable(tableName, playerName);

        await BindConnectionAndSocket(socket, connection, cancellation);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("start/{tableName}")]
    public ActionResult StartGame(
        string tableName,
        [Required, FromHeader(Name = "x-connection-id")] Guid masterId)
    {
        _tablesManager.StartGame(
            tableName,
            masterId,
            table => new InitOptions { PlayersCount = table.Players.Count });

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

    private async Task BindConnectionAndSocket(
        WebSocket socket,
        Connection<
            ShitheadState,
            ShitheadState.SharedShitheadState,
            ShitheadState.ShitheadPlayerState,
            Move,
            ShitheadGameState,
            ShitheadMove> connection,
        CancellationToken cancellation)
    {
        try
        {
            connection.StateUpdated += SendGameStateUpdate;
            await ReceiveMoves();
        }
        finally
        {
            connection.StateUpdated -= SendGameStateUpdate;
        }

        async Task ReceiveMoves()
        {
            var receiveBuffer = new byte[BUFFER_SIZE];

            while (!cancellation.IsCancellationRequested
                && socket.State == WebSocketState.Open)
            {
                var data = await socket.ReceiveAsync(receiveBuffer, cancellation);

                if (data.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        null,
                        cancellation);

                    break;
                }
                else if (data.MessageType == WebSocketMessageType.Binary)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.InvalidMessageType,
                        "Only text messages are supported",
                        cancellation);

                    throw new HttpResponseException(
                        HttpStatusCode.BadRequest,
                        new Error("Only text messages are supported"));
                }

                try
                {
                    var move = JsonSerializer.Deserialize<ShitheadMove>(
                        receiveBuffer.AsSpan()[..data.Count],
                        _serializationOptions);

                    if (move != null)
                    {
                        connection.PlayMove(move);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Received invalid data");

                    await socket.CloseAsync(
                        WebSocketCloseStatus.InvalidPayloadData,
                        ex.Message,
                        cancellation);

                    break;
                }
            }
        }

        async void SendGameStateUpdate(object? sender, StateUpdatedEventArgs<ShitheadGameState> e)
        {
            try
            {
                ArrayBufferWriter<byte> buffer = new(BUFFER_SIZE);
                using Utf8JsonWriter utf8JsonWriter = new(buffer);
                JsonSerializer.Serialize(utf8JsonWriter, e.State, _serializationOptions);

                await socket.SendAsync(
                    buffer.WrittenMemory,
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "State processing failed!");

                throw new HttpResponseException(
                    HttpStatusCode.InternalServerError,
                    new Error("State processing failed"),
                    ex);
            }
        }
    }
}
