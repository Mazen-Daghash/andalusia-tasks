using System;

namespace ProductsApi.Errors;

public class ConflictException : Exception
{
    public ConflictException() { }
    public ConflictException(string? message) : base(message) { }
}
