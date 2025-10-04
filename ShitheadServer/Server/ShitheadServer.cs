using GameServer;
using Shithead;
using Shithead.ShitheadMove;
using Shithead.State;
using ShitheadServer.Server.DTO;
using ShitheadServer.Server.DTO.State;
using System.Net.WebSockets;
using System.Text.Json;

namespace ShitheadServer.Server;

public sealed class ShitheadServer
{
	private const int BUFFER_SIZE = 0x1_0000; // 64 KiB
	private static readonly JsonSerializerOptions serializationOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
	};

	private readonly GameServer.Server<
		ShitheadInitOptions,
		ShitheadState,
		ShitheadState.SharedShitheadState,
		ShitheadState.ShitheadPlayerState,
		IShitheadMove,
		StateUpdate,
		ShitheadMove> server = new(
			options => ShitheadGame.CreateGame(options.PlayersCount),
			(sharedState, playerState) => new StateUpdate(sharedState, playerState),
			move => move.ToGameMove());
	private readonly List<WebSocket> _playersSockets = [];
	private ILogger<ShitheadServer> Logger { get; }

	public ShitheadServer(ILogger<ShitheadServer> logger)
	{
		Logger = logger;
	}

	public async Task CreateTable(
		string tableName,
		string masterName,
		WebSocket ws,
		CancellationToken cancellation)
	{
		using var connection = server.CreateTable(tableName, masterName);
		await HandlePlayerConnection(ws, connection, cancellation);
	}

	public async Task JoinTable(
		string tableName,
		string playerName,
		Func<Task<WebSocket>> acceptSocket,
		CancellationToken cancellation)
	{
		if (!server.CanJoinTable(tableName, playerName))
		{
			throw new InvalidOperationException("Table and player names must not be empty.");
		}

		using var ws = await acceptSocket();
		using var connection = server.JoinTable(tableName, playerName);
		await HandlePlayerConnection(ws, connection, cancellation);
	}

	public void StartGame(string tableName, Guid masterConnection)
	{
		if (!server.TryGetTable(tableName, out var table))
		{
			throw new InvalidOperationException("Table does not exist.");
		}

		server.StartGame(
			tableName,
			masterConnection,
			new ShitheadInitOptions(table.Players.Count));
	}

	private async Task HandlePlayerConnection(
		WebSocket ws,
		Connection<
			ShitheadState,
			ShitheadState.SharedShitheadState,
			ShitheadState.ShitheadPlayerState,
			IShitheadMove,
			StateUpdate,
			ShitheadMove> connection,
		CancellationToken cancellation)
	{
		_playersSockets.Add(ws);

		using var cts = new CancellationTokenSource();
		using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellation, cts.Token);
		var token = combinedCts.Token;

		try
		{
			var receiveBuffers = new List<byte[]>();

			connection.StateUpdated += async (s, e) =>
			{
				try
				{
					await Send(ws, e.State, WebSocketMessageType.Text, token);
				}
				catch (Exception ex)
				{
					Logger.LogWarning(ex, "Failed to send game message!");
					cts.Cancel();
					ws.Dispose();
				}
			};

			while (ws.State is not (WebSocketState.Closed or WebSocketState.Aborted))
			{
				var currentBuffer = CreateBuffer();

				var data = await ws.ReceiveAsync(currentBuffer, token);

				if (data.MessageType == WebSocketMessageType.Binary)
				{
					await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, null, token);
				}
				else
				{
					if (!data.EndOfMessage)
					{
						receiveBuffers.Add(currentBuffer);
					}
					else
					{
						try
						{
							var fullData = GetFullData(receiveBuffers, currentBuffer, data);
							var deserializedMove = JsonSerializer.Deserialize<ShitheadMove>(fullData, serializationOptions)!;

							connection.PlayMove(deserializedMove);
						}
						catch (Exception ex)
						{
							Logger.LogWarning("Error parsing message: {error}", ex);
						}
						finally
						{
							receiveBuffers.Clear();
						}
					}
				}
			}
		}
		catch
		{
			cts.Cancel();
			throw;
		}
	}

	private static byte[] GetFullData(
		List<byte[]> receiveBuffers,
		byte[] lastBuffer,
		WebSocketReceiveResult data)
	{
		Array.Resize(ref lastBuffer, data.Count);
		byte[] fullData;

		if (receiveBuffers.Count == 0)
		{
			fullData = lastBuffer;
		}
		else
		{
			int fullDataSize = lastBuffer.Length + receiveBuffers.Sum(buffer => buffer.Length);
			fullData = new byte[fullDataSize];
			int targetIndex = 0;

			foreach (var buffer in receiveBuffers.Append(lastBuffer))
			{
				Array.Copy(buffer, 0, fullData, targetIndex, buffer.Length);
				targetIndex += buffer.Length;
			}
		}

		return fullData;
	}

	private static byte[] CreateBuffer() =>
		new byte[BUFFER_SIZE];

	private static async Task Send<T>(
		WebSocket ws,
		T data,
		WebSocketMessageType messageType,
		CancellationToken cancellationToken)
	{
		var serializedData = JsonSerializer.SerializeToUtf8Bytes(data, serializationOptions);
		await ws.SendAsync(serializedData, messageType, true, cancellationToken);
	}
}
