using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;
using Xunit.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    internal class BeforeAfterTestConfiguration(
        IServiceProvider serviceProvider,
        ITestOutputHelperAccessor testOutputHelperAccessor,
        TestFixture testFixture) : BeforeAfterTest
    {
        public override ValueTask BeforeAsync(object? testClassInstance, MethodInfo methodUnderTest)
        {
            if (testClassInstance is not Test test)
                return ValueTask.CompletedTask;

            var collectionFixtureName = testClassInstance.GetType().GetCustomAttribute<CollectionAttribute>()?.Name;

            test.TestFixture = testFixture;
            return test.Initialize(testOutputHelperAccessor.Output!, collectionFixtureName);
        }
    }
}
