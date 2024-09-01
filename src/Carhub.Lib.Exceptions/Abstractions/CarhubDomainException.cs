namespace Carhub.Lib.Exceptions.Abstractions;

public abstract class CarhubDomainException(string message)
    : Exception(message);