using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleUtils;
using ConsoleUtils.Output;
using GamesClient.Shithead;

namespace LameShithead.States;

public abstract record State(Context Context)
{
    public async Task<State> NextState(CancellationToken cancellation)
    {
        try
        {
            return await NextStateUnsafe(cancellation);
        }
        catch (Exception ex) when (this is not ErrorState)
        {
            return new ErrorState(Context, ex);
        }
    }

    protected abstract Task<State> NextStateUnsafe(CancellationToken cancellation);

    protected Task<string> GetValueFromUser(string prompt, CancellationToken cancellation) =>
        GetValueFromUser(ConsoleOutput.FromString(prompt), cancellation);

    protected Task<string> GetValueFromUser(IConsoleOutput prompt, CancellationToken cancellation)
    {
        return Task.Run(GetValueFromUserSync, cancellation);

        async Task<string> GetValueFromUserSync()
        {
            await Context.Console.WriteLine(prompt, cancellation);

            return await Context.Console.ReadLine(cancellation) ?? "";
        }
    }

    protected Task<T> GetOptionFromUser<T>(
        string prompt,
        IReadOnlyCollection<(int Key, string DisplayValue, T Value)> options,
        CancellationToken cancellation)
        =>
        GetOptionFromUser(
            ConsoleOutput.FromString(prompt),
            options,
            cancellation);

    protected async Task<T> GetOptionFromUser<T>(
        IConsoleOutput prompt,
        IReadOnlyCollection<(int Key, string DisplayValue, T Value)> options,
        CancellationToken cancellation)
    {
        var optionByKey = options.ToDictionary(opt => opt.Key);

        var optionsStrings = string.Join("\n\t", options
                .Select(opt => $"${opt.Key}: {opt.DisplayValue}"));

        prompt = ConsoleOutput.Interpolate($"{prompt}\n\t{options}");

        return await GetValueFromUser<T>(prompt, IsValidOption, cancellation);


        bool IsValidOption(string str, [NotNullWhen(true)] out T? value, [NotNullWhen(false)] out string? error)
        {
            value = default;
            error = default;

            if (!int.TryParse(str, out int parsed)
                || !optionByKey.TryGetValue(parsed, out var selectedOption))
            {
                error = $"'{str}' is not a valid option";
                return false;
            }

            value = selectedOption.Value!;
            return true;
        }
    }

    protected Task<T> GetValueFromUser<T>(
        string prompt,
        Parser<T> parser,
        CancellationToken cancellation)
        =>
        GetValueFromUser(ConsoleOutput.FromString(prompt), parser, cancellation);

    protected async Task<T> GetValueFromUser<T>(
        IConsoleOutput prompt,
        Parser<T> parser,
        CancellationToken cancellation)
    {
        var resultStr = await GetValueFromUser(prompt, cancellation);
        T? result;

        while (!parser(resultStr, out result, out var error))
        {
            var invalidInputMessage = ConsoleOutput.Interpolate($"Invalid input: {error}")
                .Stylize(Style.Red.Forward);

            var errorPrompt = ConsoleOutput.Interpolate($"{invalidInputMessage}\n{prompt}");

            resultStr = await GetValueFromUser(errorPrompt, cancellation);
        }

        return result;
    }

    protected static Parser<string> NonEmpty(string inputName) =>
        (input, [NotNullWhen(true)] out output, [NotNullWhen(false)] out error) =>
        {
            output = null;
            error = null;

            if (input == "")
            {
                error = $"{inputName} must have a value";
                return false;
            }
            else
            {
                output = input;
                return true;
            }
        };
}

public record Context(
    IConsole Console,
    IShitheadClient ShitheadClient);

public delegate bool Parser<T>(
    string str,
    [NotNullWhen(true)] out T? result,
    [NotNullWhen(false)] out string? error);
