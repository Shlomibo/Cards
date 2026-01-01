using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
            if (value.Scheme != Uri.UriSchemeHttp && value.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Invalid Url scheme!", nameof(BaseUrl));
            }

            var path = value.PathAndQuery.Split('?')[0];

            field = path.EndsWith(_route)
                ? value
                : new Uri(value, _route);

            var wsScheme = BaseUrl.Scheme switch
            {
                string scheme when scheme == Uri.UriSchemeHttp => Uri.UriSchemeWs,
                string scheme when scheme == Uri.UriSchemeHttps => Uri.UriSchemeWss,
                _ => throw new UnreachableException(),
            };

            UriBuilder webSocketUri = new(value)
            {
                Scheme = wsScheme
            };

            WebSocketUri = webSocketUri.Uri;
        }
    }

    public Uri WebSocketUri { get; private set; } = null!;
}

public abstract class GameClient<TOptions, TState, TMove> : IClient<TState, TMove>
    where TOptions : GameClientOptions
    where TState : State
{
    private readonly IHttpClientFactory _httpClientFactory;

    protected static JsonSerializerOptions SerializerOptions { get; } = JsonOptions.SetJsonSerializationOptions();

    protected ILogger Logger { get; }
    protected TOptions Options { get; }

    public GameClient(
        IOptions<TOptions> options,
        ILogger logger,
        IHttpClientFactory httpClientFactory)
    {
        Logger = logger;
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<CanJoinTableResponse> CanJoinTable(
        string tableName,
        string playerName,
        CancellationToken cancellation)
    {
        Uri uri = new(Options.BaseUrl, PlayersSubPath(tableName, playerName));
        var httpClient = GetHttpClient();
        using var response = await httpClient.GetAsync(uri, cancellation);

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
        Uri uri = new(Options.WebSocketUri, $"create/{PlayersSubPath(tableName, playerName)}");
        ClientWebSocket ws = new();
        await ws.ConnectAsync(uri, cancellation);

        return new Connection<TState, TMove>(
            Options,
            tableName,
            playerName,
            ws,
            GetHttpClient);
    }

    public async Task<IConnection<TState, TMove>> JoinTable(string tableName, string playerName, CancellationToken cancellation)
    {
        Uri uri = new(Options.WebSocketUri, $"join/{PlayersSubPath(tableName, playerName)}");
        ClientWebSocket ws = new();
        await ws.ConnectAsync(uri, cancellation);

        return new Connection<TState, TMove>(
            Options,
            tableName,
            playerName,
            ws,
            GetHttpClient);
    }

    private static string PlayersSubPath(string tableName, string playerName)
    {
        tableName = HttpUtility.HtmlEncode(tableName);
        playerName = HttpUtility.HtmlEncode(playerName);

        return $"{tableName}/{playerName}";
    }

    private HttpClient GetHttpClient() =>
        _httpClientFactory.CreateClient(GetType().Name);
}
