using System;
using AutoFixture;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class StylizeTests : ConsoleOutputTestsBase
{
    [Test]
    public void WhenHavingNoStyles()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Stylize();

        Validate(testSubject, value);
    }

    [Test]
    public void WhenHavingAStyles()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var style = Fixture.Create<Style>();
        var testSubject = output.Stylize(style);

        Validate(testSubject, $"\e[{style.Code}m{value}\e[{style.ResetCode}m");
    }

    [Test]
    public void WhenUsing2IdenticalStyles()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var style = Fixture.Create<Style>();
        var testSubject = output.Stylize(style, style);

        Validate(testSubject, $"\e[{style.Code}m{value}\e[{style.ResetCode}m");
    }

    [Test]
    public void WhenUsing2DifferentStyles()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var style1 = Fixture.Create<Style>();
        var style2 = Fixture.Create<Style>();
        var testSubject = output.Stylize(style1, style2);

        Validate(testSubject, $"\e[{style1.Code};{style2.Code}m{value}\e[{style1.ResetCode};{style2.ResetCode}m");
    }
}
