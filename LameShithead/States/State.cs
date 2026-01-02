using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleUtils;
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

    protected Task<string> GetValueFromUser(Prompt prompt, CancellationToken cancellation)
    {
        return Task.Run(GetValueFromUserSync, cancellation);

        async Task<string> GetValueFromUserSync()
        {
            await (prompt.Color.HasValue
                ? Context.Console.WriteLine(prompt.Message, prompt.Color.Value, cancellation)
                : Context.Console.WriteLine(prompt.Message, cancellation));

            return await Context.Console.ReadLine(cancellation) ?? "";
        }
    }

    protected async Task<T> GetOptionFromUser<T>(
        Prompt prompt,
        IReadOnlyCollection<(int Key, string DisplayValue, T Value)> options,
        CancellationToken cancellation)
    {
        var optionByKey = options.ToDictionary(opt => opt.Key);
        prompt = prompt with
        {
            Message = string.Join("\n\t", options
                .Select(opt => $"${opt.Key}: {opt.DisplayValue}")
                .Prepend(prompt.Message)),
        };

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

    protected async Task<T> GetValueFromUser<T>(Prompt prompt, Parser<T> parser, CancellationToken cancellation)
    {
        var resultStr = await GetValueFromUser(prompt, cancellation);
        T? result;

        while (!parser(resultStr, out result, out var error))
        {
            var errorPrompt = prompt with
            {
                Message = string.Join('\n',
                    "Invalid input: " + error,
                    prompt.Message)
            };

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
