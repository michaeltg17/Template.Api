namespace Api.Exceptions
{
    public class UnauthorizedException(string message) : TemplateApiException(message)
    {
    }
}
