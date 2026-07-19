namespace Application.Exceptions;

public class NotAllFoundException<T>(long[] ids) : NotAllFoundException(typeof(T).Name, ids)
{
}