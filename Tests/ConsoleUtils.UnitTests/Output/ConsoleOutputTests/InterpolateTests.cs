using System;
using AutoFixture;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class InterpolateTests : ConsoleOutputTestsBase
{
    [Test]
    public void WhenInterpolatingEmptyString()
    {
        var subject = ConsoleOutput.Interpolate($"");
        Validate(subject, "");
    }

    [Test]
    public void WhenInterpolatingJustAString()
    {
        var subject = ConsoleOutput.Interpolate($"abcd");
        Validate(subject, "abcd");
    }

    [Test]
    public void WhenValueIsPrefix()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"{value}def");
        Validate(subject, $"{value}def");
    }

    [Test]
    public void WhenValueIsInfix()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"abc{value}def");
        Validate(subject, $"abc{value}def");
    }

    [Test]
    public void WhenValueIsSuffix()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"abc{value}");
        Validate(subject, $"abc{value}");
    }

    [Test]
    public void WhenValueIsPrefixWithAlignment()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"{value,2}def");
        Validate(subject, $"{value,2}def");
    }

    [Test]
    public void WhenValueIsInfixWithAlignment()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"abc{value,4}def");
        Validate(subject, $"abc{value,4}def");
    }

    [Test]
    public void WhenValueIsSuffixWithAlignment()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"abc{value,3}");
        Validate(subject, $"abc{value,3}");
    }

    [Test]
    public void WhenValueIsPrefixWithNegativeAlignment()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"{value,-2}def");
        Validate(subject, $"{value,-2}def");
    }

    [Test]
    public void WhenValueIsInfixWithNegativeAlignment()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"abc{value,-4}def");
        Validate(subject, $"abc{value,-4}def");
    }

    [Test]
    public void WhenValueIsSuffixWithNegativeAlignment()
    {
        var value = Fixture.Create<int>();
        var subject = ConsoleOutput.Interpolate($"abc{value,-3}");
        Validate(subject, $"abc{value,-3}");
    }

    [Test]
    public void WhenValueIsPrefixWithFormat()
    {
        var value = Fixture.Create<float>();
        var subject = ConsoleOutput.Interpolate($"abc{value:##.##}");
        Validate(subject, $"abc{value:##.##}");
    }

    [Test]
    public void WhenValueIsInfixWithFormat()
    {
        var value = Fixture.Create<float>();
        var subject = ConsoleOutput.Interpolate($"abc{value:##.##}def");
        Validate(subject, $"abc{value:##.##}def");
    }

    [Test]
    public void WhenValueIsSuffixWithFormat()
    {
        var value = Fixture.Create<float>();
        var subject = ConsoleOutput.Interpolate($"abc{value:##.##}");
        Validate(subject, $"abc{value:##.##}");
    }

    [Test]
    public void WhenValueIsPrefixWithFormatAndAlignment()
    {
        var value = Fixture.Create<float>();
        var subject = ConsoleOutput.Interpolate($"abc{value,2:##.##}");
        Validate(subject, $"abc{value,2:##.##}");
    }

    [Test]
    public void WhenValueIsInfixWithFormatAndAlignment()
    {
        var value = Fixture.Create<float>();
        var subject = ConsoleOutput.Interpolate($"abc{value,3:##.##}def");
        Validate(subject, $"abc{value,3:##.##}def");
    }

    [Test]
    public void WhenValueIsSuffixWithFormatAndAlignment()
    {
        var value = Fixture.Create<float>();
        var subject = ConsoleOutput.Interpolate($"abc{value,4:##.##}");
        Validate(subject, $"abc{value,4:##.##}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsPrefix()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"{output}def");
        Validate(subject, $"{value}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsInfix()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output}def");
        Validate(subject, $"abc{value}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsSuffix()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output}");
        Validate(subject, $"abc{value}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsPrefixWithAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"{output,2}def");
        Validate(subject, $"{value,2}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsInfixWithAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,4}def");
        Validate(subject, $"abc{value,4}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsSuffixWithAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,3}");
        Validate(subject, $"abc{value,3}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsPrefixWithNegativeAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"{output,-2}def");
        Validate(subject, $"{value,-2}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsInfixWithNegativeAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,-4}def");
        Validate(subject, $"abc{value,-4}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsSuffixWithNegativeAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,-3}");
        Validate(subject, $"abc{value,-3}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsPrefixWithFormat()
    {
        var value = Fixture.Create<float>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output:##.##}");
        Validate(subject, $"abc{value}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsInfixWithFormat()
    {
        var value = Fixture.Create<float>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output:##.##}def");
        Validate(subject, $"abc{value}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsSuffixWithFormat()
    {
        var value = Fixture.Create<float>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output:##.##}");
        Validate(subject, $"abc{value}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsPrefixWithFormatAndAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,2:##.##}");
        Validate(subject, $"abc{value,2}");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsInfixWithFormatAndAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,3:##.##}def");
        Validate(subject, $"abc{value,3}def");
    }

    [Test]
    public void WhenAnotherConsoleOutputIsSuffixWithFormatAndAlignment()
    {
        var value = Fixture.Create<int>();
        var output = ConsoleOutput.FromValue(value);
        var subject = ConsoleOutput.Interpolate($"abc{output,4:##.##}");
        Validate(subject, $"abc{value,4}");
    }

    [Test]
    public void WhenInterpolatingMoreThanOneValue()
    {
        var values = Fixture.CreateMany<float>(3).ToArray();
        var subject = ConsoleOutput.Interpolate($"{values[0]}abc{values[1],4:##.##}def{values[2]}");
        Validate(subject, $"{values[0]}abc{values[1],4:##.##}def{values[2]}");
    }
}
