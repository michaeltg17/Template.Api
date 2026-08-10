using IntegrationTests.Fixtures;
using Xunit;

namespace IntegrationTests.Collections;

[CollectionDefinition(nameof(ProductionApiCollectionFixture))]
public class ProductionApiCollectionFixture : ICollectionFixture<TestFixture>
{
}