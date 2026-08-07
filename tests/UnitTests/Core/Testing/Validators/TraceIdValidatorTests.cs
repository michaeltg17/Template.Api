using Xunit;
using Core.Testing.Validators;
using AwesomeAssertions;

namespace UnitTests.Core.Testing.Validators;

public sealed class TraceIdValidatorTests
{
    public static readonly TheoryDataRow<string?, bool>[] TestCases =
    [
        new("00-bc43ec34fc2707cab2c1477979967041-146d776ead891946-00", true) { TestDisplayName = "Valid: standard" },
        new("00-00000000000000000000000000000000-146d776ead891946-00", false) { TestDisplayName = "Invalid: all zero trace" },
        new("00-bc43ec34fc2707cab2c1477979967041-0000000000000000-00", false) { TestDisplayName = "Invalid: all zero parent" },
        new("01-bc43ec34fc2707cab2c1477979967041-146d776ead891946-00", false) { TestDisplayName = "Invalid: wrong version" },
        new("00-bc43ec34fc2707cab2c1477979967041-146d776ead891946-0", false) { TestDisplayName = "Invalid: short flags" },
        new("00-bc43ec34fc2707cab2c1477979967041-146d776ead891946-00-extra", false) { TestDisplayName = "Invalid: extra chars" },
        new("", false) { TestDisplayName = "Invalid: empty" },
        new(null, false) { TestDisplayName = "Invalid: null" },
        new("      ", false) { TestDisplayName = "Invalid: whitespace" },
    ];

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Cases(string? traceId, bool expected)
    {
        //When
        var result = TraceIdValidator.IsValid(traceId!);

        //Then
        result.Should().Be(expected);
    }
}
