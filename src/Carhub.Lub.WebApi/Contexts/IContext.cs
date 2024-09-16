namespace Carhub.Lub.WebApi.Contexts;

public interface IContext
{
    string RequestId { get; }
    string TraceId { get; }
    IIdentityContext Identity { get; }
}