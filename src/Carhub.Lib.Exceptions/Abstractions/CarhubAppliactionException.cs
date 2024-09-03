namespace Carhub.Lib.Exceptions.Abstractions;

public abstract class CarhubApplicationException(string message)
    : Exception(message);