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
    public void WhenValueIsPrefixConsoleOutput()
    {

    }

    [Test]
    public void WhenValueIsPrefixConsoleOutputWithFormatOrAlignment()
    {

    }

    [Test]
    public void WhenValueIsInfixConsoleOutput()
    {

    }

    [Test]
    public void WhenValueIsInfixConsoleOutputWithFormatOrAlignment()
    {

    }

    [Test]
    public void WhenValueIsSuffixConsoleOutput()
    {

    }

    [Test]
    public void WhenValueIsSuffixConsoleOutputWithFormatOrAlignment()
    {

    }

    [Test]
    public void WhenInterpolatingMoreThanOneValue()
    {
        var values = Fixture.CreateMany<float>(3).ToArray();
        var subject = ConsoleOutput.Interpolate($"{values[0]}abc{values[1],4:##.##}def{values[2]}");
        Validate(subject, $"{values[0]}abc{values[1],4:##.##}def{values[2]}");
    }
}
