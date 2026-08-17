using System;

namespace ProductsApi.Errors;

public class DueDateInPastException : Exception
{
    public DueDateInPastException() { }
    public DueDateInPastException(string? message) : base(message) { }
}
