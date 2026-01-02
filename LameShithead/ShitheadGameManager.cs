using System;
using ConsoleUtils;
using LameShithead.States;
using LameShithead.States.GameSelection;
using Microsoft.Extensions.Hosting;

namespace LameShithead;

public sealed class ShitheadGameManager : IHostedService, IDisposable
{
    private bool _disposedValue;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IConsole _console;
    private readonly IHostApplicationLifetime _appLifetime;
    private State _currentState;

    public ShitheadGameManager(
        IConsole console,
        IHostApplicationLifetime appLifetime)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));

        _currentState = new SelectPlayerNameState(new Context(_console));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        RanGame();
    }

    private async void RanGame()
    {
        try
        {
            while (!_cancellation.IsCancellationRequested)
            {
                _currentState = await _currentState.NextState(_cancellation.Token);
            }
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            _appLifetime.StopApplication();
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellation.CancelAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (!_cancellation.IsCancellationRequested)
                {
                    _cancellation.Cancel();
                }

                _cancellation.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ShitheadGameManager()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
