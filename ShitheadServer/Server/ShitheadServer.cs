using GameServer;
using Shithead;
using Shithead.ShitheadMove;
using Shithead.State;
using ShitheadServer.Server.DST;
using ShitheadServer.Server.DST.State;
using System.Net.WebSockets;
using System.Text.Json;

namespace ShitheadServer.Server
{
	public sealed class ShitheadServer
	{
		private const int BUFFER_SIZE = 0x1_0000; // 64 KiB
		private static readonly JsonSerializerOptions serializationOptions = new JsonSerializerOptions
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

		public async Task CreateTable(string tableName, string masterName, WebSocket ws)
		{
			using var connection = this.server.CreateTable(tableName, masterName);
			await HandlePlayerConnection(ws, connection);
		}


		public async Task<IResult> JoinTable(
			string tableName,
			string playerName,
			Func<Task<WebSocket>> acceptSocket)
		{
			if (!this.server.CanJoinTable(tableName, playerName))
			{
				return Results.BadRequest();
			}

			using var ws = await acceptSocket();
			using var connection = this.server.JoinTable(tableName, playerName);
			await HandlePlayerConnection(ws, connection);

			return Results.NoContent();
		}

		public IResult StartGame(string tableName, Guid masterConnection)
		{
			if (!this.server.TryGetTable(tableName, out var table))
			{
				return Results.NotFound();
			}

			try
			{
				this.server.StartGame(
					tableName,
					masterConnection,
					new ShitheadInitOptions(table.Players.Count));
			}
			catch (InvalidOperationException)
			{
				return Results.Forbid();
			}

			return Results.NoContent();
		}

		private static async Task HandlePlayerConnection(
			WebSocket ws,
			Connection<
				ShitheadState,
				ShitheadState.SharedShitheadState,
				ShitheadState.ShitheadPlayerState,
				IShitheadMove,
				StateUpdate,
				ShitheadMove> connection)
		{
			var cancellation = new CancellationTokenSource();

			try
			{
				var receiveBuffers = new List<byte[]>();

				connection.StateUpdated += async (s, e) =>
				{
					try
					{
						await Send(ws, e.State, WebSocketMessageType.Text);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						cancellation.Cancel();
						ws.Dispose();
					}
				};

				while (ws.State is not (WebSocketState.Closed or WebSocketState.Aborted))
				{
					var currentBuffer = CreateBuffer();
					receiveBuffers.Add(currentBuffer);

					var data = await ws.ReceiveAsync(currentBuffer, cancellation.Token);

					if (data.MessageType == WebSocketMessageType.Binary)
					{
						await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, null, cancellation.Token);
					}
					else
					{
						if (data.EndOfMessage)
						{
							var fullData = GetFullData(receiveBuffers, currentBuffer, data);
							var deserializedMove = JsonSerializer.Deserialize<ShitheadMove>(fullData, serializationOptions)!;

							connection.PlayMove(deserializedMove);
							receiveBuffers.Clear();
						}
					}
				}
			}
			catch
			{
				cancellation.Cancel();
				throw;
			}
		}

		private static byte[] GetFullData(
			List<byte[]> receiveBuffers,
			byte[] currentBuffer,
			WebSocketReceiveResult data)
		{
			Array.Resize(ref currentBuffer, data.Count);
			byte[] fullData;

			if (receiveBuffers.Count == 1)
			{
				fullData = receiveBuffers[0];
			}
			else
			{
				fullData = new byte[receiveBuffers.Sum(buffer => buffer.Length)];
				int targetIndex = 0;

				foreach (var buffer in receiveBuffers)
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
			CancellationToken cancellationToken = default)
		{
			var serializedData = JsonSerializer.SerializeToUtf8Bytes(data, serializationOptions);
			await ws.SendAsync(serializedData, messageType, true, cancellationToken);
		}
	}
}
