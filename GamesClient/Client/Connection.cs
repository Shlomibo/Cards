using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text.Json;
using System.Web;
using DTOs;

namespace GamesClient.Client;

/// <inheritdoc cref="IConnection{TState, TMove}"/>
public sealed class Connection<TState, TMove> : IConnection<TState, TMove>
    where TState : State
{
    private const int BUFFER_SIZE = 10 * 1024;

    private static readonly JsonSerializerOptions _serializerOptions = JsonOptions.SetJsonSerializationOptions();
    private readonly GameClientOptions _options;
    private readonly ClientWebSocket _webSocket;
    private readonly Func<HttpClient> _httpClientFactory;
    private readonly CancellationTokenSource _cancellation = new();
    private StateUpdate<TState>? _lastState;

    private Guid? ConnectionId => _lastState?.CurrentPlayer.ConnectionId;
    /// <inheritdoc/>
    public string TableName { get; }

    /// <inheritdoc/>
    public string PlayerName { get; }

    /// <inheritdoc/>
    public IObservable<StateUpdate<TState>> GameState { get; }

    /// <summary>
    /// Gets a value indicating whether the connection has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    internal Connection(
        GameClientOptions options,
        string tableName,
        string playerName,
        ClientWebSocket webSocket,
        Func<HttpClient> httpClientFactory)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName);
        ArgumentException.ThrowIfNullOrEmpty(playerName);
        _options = options;
        TableName = tableName;
        PlayerName = playerName;
        _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        GameState = CreateGameStateObservable();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;

            try
            {
                _cancellation.Cancel();
            }
            finally
            {
                _cancellation.Dispose();
                _webSocket.Dispose();
            }
        }
    }

    /// <inheritdoc/>
    public async Task PlayMove(TMove move, CancellationToken cancellation)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        using var totalCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            _cancellation.Token,
            cancellation);

        ArrayBufferWriter<byte> buffer = new(BUFFER_SIZE);
        using Utf8JsonWriter jsonWriter = new(buffer);

        JsonSerializer.Serialize(jsonWriter, move, _serializerOptions);
        await _webSocket.SendAsync(
            buffer.WrittenMemory,
            WebSocketMessageType.Text,
            endOfMessage: true,
            totalCancellation.Token);
    }

    /// <inheritdoc/>
    public async Task StartGame(CancellationToken cancellation)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        if (ConnectionId is not Guid connId)
        {
            throw new InvalidOperationException("Please subscribe for state updates before starting a game.");
        }

        using var totalCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            _cancellation.Token,
            cancellation);

        Uri uri = new(_options.BaseUrl, $"start/{HttpUtility.UrlEncode(TableName)}");
        using HttpRequestMessage request = new(HttpMethod.Post, uri);
        request.Headers.Add("x-connection-id", connId.ToString());

        var httpClient = _httpClientFactory();
        using var response = await httpClient.SendAsync(request, totalCancellation.Token);
        response.EnsureSuccessStatusCode();
    }

    private IObservable<StateUpdate<TState>> CreateGameStateObservable()
    {
        return Observable.Create((IObserver<StateUpdate<TState>> observer) =>
        {
            var buffer = new byte[BUFFER_SIZE];

            CancellationTokenSource cancellation = new();
            var totalCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellation.Token,
                cancellation.Token);

            if (_lastState != null)
            {
                observer.OnNext(_lastState);
            }

            Task.Run(HandleWebSocket, totalCancellation.Token);

            return () =>
            {
                try
                {
                    cancellation.Cancel();
                }
                finally
                {
                    cancellation.Dispose();
                    totalCancellation.Dispose();
                }
            };

            async Task HandleWebSocket()
            {
                var ct = totalCancellation.Token;
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        var result = await _webSocket.ReceiveAsync(buffer, ct);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _webSocket.CloseAsync(
                                WebSocketCloseStatus.NormalClosure,
                                null,
                                ct);
                            observer.OnCompleted();

                            break;
                        }
                        else if (result.MessageType != WebSocketMessageType.Text)
                        {
                            await _webSocket.CloseAsync(
                                WebSocketCloseStatus.InvalidMessageType,
                                "Only text messages are supported",
                                ct);

                            throw new InvalidOperationException(
                                $"Received invalid message type: {result.MessageType}");
                        }

                        var stateUpdate = JsonSerializer.Deserialize<StateUpdate<TState>>(
                            buffer.AsSpan(..result.Count),
                            _serializerOptions);

                        if (stateUpdate != null)
                        {
                            _lastState = stateUpdate;
                            observer.OnNext(stateUpdate);
                        }
                    }
                }
                catch (TaskCanceledException) when (cancellation.IsCancellationRequested)
                {
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            }
        });
    }
}
