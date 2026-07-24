using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Logging
{
    public static partial class ProductActionsLogging
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Product with id '{id}' created successfully.")]
        public static partial void LogProductCreated(this ILogger logger, long id);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Product with id '{id}' updated successfully.")]
        public static partial void LogProductUpdated(this ILogger logger, long id);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Products with ids '{ids}' deleted successfully.")]
        public static partial void LogProductsDeleted(this ILogger logger, IEnumerable<long> ids);
    }
}
