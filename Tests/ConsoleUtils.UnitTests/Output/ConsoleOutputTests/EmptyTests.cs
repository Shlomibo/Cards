using System;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class EmptyTests : ConsoleOutputTestsBase
{
    [Test]
    public void EmptyTest() =>
        Validate(ConsoleOutput.Empty(), "");
}
