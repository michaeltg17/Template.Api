using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;
using Xunit.DependencyInjection;

namespace IntegrationTests
{
    internal class BeforeAfterTestConfiguration(
        IServiceProvider serviceProvider,
        ITestOutputHelperAccessor testOutputHelperAccessor)
        : BeforeAfterTest
    {
        public override ValueTask BeforeAsync(object? testClassInstance, MethodInfo methodUnderTest)
        {
            if (testClassInstance is not Test test)
                return ValueTask.CompletedTask;

            var collectionName = testClassInstance.GetType().GetCustomAttribute<CollectionAttribute>()?.Name;

            var fixtureType = collectionName switch
            {
                nameof(DevelopmentApiCollection) => typeof(DevelopmentWebApplicationFactoryFixture),
                nameof(ProductionApiCollection) => typeof(ProductionWebApplicationFactoryFixture),
                _ => throw new IntegrationTestsException("Expected development or production collection name.")
            };

            test.WebApplicationFactoryFixture = (WebApplicationFactoryFixture)serviceProvider.GetRequiredService(fixtureType);
            test.TestOutputHelper = testOutputHelperAccessor.Output!;
            return test.Initialize();
        }
    }
}
