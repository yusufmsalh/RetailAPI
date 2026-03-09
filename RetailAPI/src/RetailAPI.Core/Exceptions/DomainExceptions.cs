namespace RetailAPI.Core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string name, object key) : base($"{name} with key '{key}' was not found.") { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public class InsufficientStockException : Exception
{
    public InsufficientStockException(string productName, int available, int requested)
        : base($"Insufficient stock for '{productName}'. Available: {available}, Requested: {requested}") { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access.") : base(message) { }
}
