using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;
using System.Web;
using DTOs;
using DTOs.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamesClient.Client;

public abstract record GameClientOptions
{
    private readonly string _route;

    public GameClientOptions(string route)
    {
        _route = route;
    }

    [Required]
    public required Uri BaseUrl
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(BaseUrl));

            var path = value.PathAndQuery.Split('?')[0];

            field = path.EndsWith(_route)
                ? value
                : new Uri(value, _route);
        }
    }
}

public abstract class GameClient<TOptions, TState, TMove> : IClient<TState, TMove>
    where TOptions : GameClientOptions
    where TState : State
{
    private readonly HttpClient _httpClient;

    protected static JsonSerializerOptions SerializerOptions { get; } = JsonOptions.SetJsonSerializationOptions();

    protected ILogger Logger { get; }
    protected TOptions Options { get; }

    public GameClient(
        IOptions<TOptions> options,
        ILogger logger,
        HttpClient httpClient)
    {
        Logger = logger;
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _httpClient.BaseAddress = Options.BaseUrl;
    }

    public async Task<CanJoinTableResponse> CanJoinTable(
        string tableName,
        string playerName,
        CancellationToken cancellation)
    {
        Uri uri = new(Options.BaseUrl, PlayersSubPath(tableName, playerName));
        using var response = await _httpClient.GetAsync(uri, cancellation);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CanJoinTableResponse>(
            SerializerOptions,
            cancellation)
            ?? throw new InvalidOperationException("Failed to get response data from server");
    }

    public async Task<IConnection<TState, TMove>> CreateTable(
        string tableName,
        string playerName,
        CancellationToken cancellation)
    {
        Uri uri = new(Options.BaseUrl, $"create/{PlayersSubPath(tableName, playerName)}");
        ClientWebSocket ws = new();
        await ws.ConnectAsync(uri, cancellation);

        return new Connection<TState, TMove>(
            Options,
            tableName,
            playerName,
            ws,
            _httpClient);
    }

    public async Task<IConnection<TState, TMove>> JoinTable(string tableName, string playerName, CancellationToken cancellation)
    {
        Uri uri = new(Options.BaseUrl, $"join/{PlayersSubPath(tableName, playerName)}");
        ClientWebSocket ws = new();
        await ws.ConnectAsync(uri, cancellation);

        return new Connection<TState, TMove>(
            Options,
            tableName,
            playerName,
            ws,
            _httpClient);
    }

    private static string PlayersSubPath(string tableName, string playerName)
    {
        tableName = HttpUtility.HtmlEncode(tableName);
        playerName = HttpUtility.HtmlEncode(playerName);

        return $"{tableName}/{playerName}";
    }
}
