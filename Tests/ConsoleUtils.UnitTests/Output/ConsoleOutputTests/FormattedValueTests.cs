using System;
using System.Text;
using AutoFixture;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class FormattedValueTests : ConsoleOutputTestsBase
{
    private const string FORMAT = "yyyy-MM-dd";

    [Test]
    public void WhenOnlyValueIsProvided()
    {
        var value = Fixture.Create<string>();
        var testSubject = ConsoleOutput.FromValue(value);
        string expected = value;

        Validate(testSubject, expected);
    }

    [Test]
    public void WhenValueAndAlignmentAreProvided([Random(min: -10, max: 10, count: 5)] int alignment)
    {
        var value = Fixture.Create<string>();
        alignment = alignment switch
        {
            >= 0 and int i => i + 1,
            int i => i,
        };
        var testSubject = ConsoleOutput.FromValue(value, alignment: alignment);
        string expected = string.Format($$"""{0,{{alignment}}}""", value);

        Validate(testSubject, expected);
    }

    [Test]
    public void WhenValueAndFormatAreProvided()
    {
        var value = Fixture.Create<DateTime>();
        var testSubject = ConsoleOutput.FromValue(value, FORMAT);
        string expected = $"{value:yyyy-MM-dd}";

        Validate(testSubject, expected);
    }

    [Test]
    public void WhenValueAndFormatAreProvidedButValueDoesNotAcceptFormat()
    {
        var value = Fixture.Create<string>();
        var testSubject = ConsoleOutput.FromValue(value, FORMAT);
        string expected = value;

        Validate(testSubject, expected);
    }

    [Test]
    public void WhenValueAndFormatAndAlignmentAreProvided([Random(min: -10, max: 10, count: 5)] int alignment)
    {
        var value = Fixture.Create<DateTime>();
        alignment = alignment switch
        {
            >= 0 and int i => i + 1,
            int i => i,
        };
        var testSubject = ConsoleOutput.FromValue(value, FORMAT, alignment);
        string expected = string.Format($$"""{0,{{alignment}}:{{FORMAT}}}""", value);

        Validate(testSubject, expected);
    }

    [Test]
    public void WhenValueAndFormatAndAlignMentAreProvidedButValueDoesNotAcceptFormat(
        [Random(min: -10, max: 10, count: 1)] int alignment)
    {
        var value = Fixture.Create<string>();
        alignment = alignment switch
        {
            >= 0 and int i => i + 1,
            int i => i,
        };
        var testSubject = ConsoleOutput.FromValue(value, FORMAT, alignment);
        string expected = string.Format($$"""{0,{{alignment}}}""", value);

        Validate(testSubject, expected);
    }
}
