using IntegrationTests.Fixtures;
using Xunit;

namespace IntegrationTests.Collections;

[CollectionDefinition(nameof(DevelopmentApiCollectionFixture))]
public class DevelopmentApiCollectionFixture : ICollectionFixture<TestFixture>
{
}