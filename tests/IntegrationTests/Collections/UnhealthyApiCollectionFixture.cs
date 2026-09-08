using IntegrationTests.Fixtures;
using Xunit;

namespace IntegrationTests.Collections;

[CollectionDefinition(nameof(UnhealthyApiCollectionFixture))]
public class UnhealthyApiCollectionFixture : ICollectionFixture<TestFixture>
{
}
