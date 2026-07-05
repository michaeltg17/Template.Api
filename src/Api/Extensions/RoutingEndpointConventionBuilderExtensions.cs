namespace Api.Extensions
{
    public static class RoutingEndpointConventionBuilderExtensions
    {
        public static TBuilder WithTestName<TBuilder>(this TBuilder builder, string endpointName)
            where TBuilder : IEndpointConventionBuilder
        {
            builder.WithName("Test." + endpointName);
            return builder;
        }

        public static TBuilder WithApiName<TBuilder>(this TBuilder builder, string endpointName)
            where TBuilder : IEndpointConventionBuilder
        {
            builder.WithName("Api." + endpointName);
            return builder;
        }
    }
}
