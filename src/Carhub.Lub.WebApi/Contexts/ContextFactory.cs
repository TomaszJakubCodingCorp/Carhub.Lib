using Microsoft.AspNetCore.Http;

namespace Carhub.Lub.WebApi.Contexts;

internal sealed class ContextFactory(IHttpContextAccessor httpContextAccessor) : IContextFactory
{
    public IContext Create()
    {
        var httpContext = httpContextAccessor.HttpContext;

        return httpContext is null ? Context.Empty : new Context(httpContext);
    }
}