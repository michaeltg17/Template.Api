using System.Reflection;
using Xunit;
using Xunit.DependencyInjection;

namespace IntegrationTests
{
    internal class BeforeAfterTestConfiguration() : BeforeAfterTest
    {
        public override ValueTask BeforeAsync(object? testClassInstance, MethodInfo methodUnderTest)
        {
            if (testClassInstance is not Test test)
                return ValueTask.CompletedTask;

            var collectionFixtureName = testClassInstance.GetType().GetCustomAttribute<CollectionAttribute>()?.Name;

            return test.Initialize(collectionFixtureName);
        }
    }
}
