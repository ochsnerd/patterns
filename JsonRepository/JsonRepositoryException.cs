namespace JsonRepository;

public class JsonRepositoryException : Exception
{
    public JsonRepositoryException() { }

    public JsonRepositoryException(string? message)
        : base(message) { }

    public JsonRepositoryException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
