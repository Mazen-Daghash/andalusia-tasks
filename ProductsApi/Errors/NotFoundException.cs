using System;

namespace ProductsApi.Errors;

public class NotFoundException : Exception
{
    public NotFoundException() { }
    public NotFoundException(string? message) : base(message) { }
}
