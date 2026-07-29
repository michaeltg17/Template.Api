using Api;

Application.DependencyConfigurator.ConfigureValidationWithCamelCase();

WebApplication
    .CreateBuilder(args)
    .AddDependencies()
    .Build()
    .Configure()
    .Run();