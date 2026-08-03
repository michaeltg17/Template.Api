namespace Application.Exceptions;

public class NotFoundException(string entityName, long id) : TemplateApiException($"{entityName} with id '{id}' was not found.")
{
}