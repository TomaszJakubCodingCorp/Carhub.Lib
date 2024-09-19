using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Carhub.Lib.Logging;

public sealed class UserIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdEnricher() : this(new HttpContextAccessor())
    {
    }

    public UserIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_httpContextAccessor.HttpContext is null)
            return;
        
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "anonymous";
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId));
    }
}