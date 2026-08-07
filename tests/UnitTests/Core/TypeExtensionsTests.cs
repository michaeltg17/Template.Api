using Core.Extensions;
using Xunit;
using AwesomeAssertions;

namespace UnitTests.Core
{
    public class TypeExtensionsTests
    {
        [InlineData(typeof(Exception), "Exception", TestDisplayName = "Non-generic type")]
        [InlineData(typeof(List<>), "List", TestDisplayName = "Open generic type (arity 1)")]
        [InlineData(typeof(Dictionary<,>), "Dictionary", TestDisplayName = "Open generic type (arity 2)")]
        [Theory]
        public void GetNameWithoutGenericArity(Type type, string expectedName)
        {
            //When
            var name = type.GetNameWithoutGenericArity();

            //Then
            name.Should().Be(expectedName);
        }
    }
}
