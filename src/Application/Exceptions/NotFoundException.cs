namespace Application.Exceptions;

public class NotFoundException(string entityName, long[] ids)
    : AppException($"The following ids '{string.Join(", ", ids)}' were not found for entity '{entityName}'.")
{
    public IEnumerable<long> NotFoundIds { get; } = ids;
}