namespace Application.Exceptions;

public class NotFoundException<T>(long id) : NotFoundException(typeof(T).Name, id)
{
}