namespace ConsoleUtils.Output;

internal sealed class Underline(ConsoleOutput output) :
    OutputModBase(
        output,
        "\e[4m",
        "\e[24m");
