using System.Text;
using Carhub.Lub.WebApi.Contexts;
using Carhub.Lub.WebApi.Filters;
using Carhub.Lub.WebApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Carhub.Lub.WebApi;

public static class Extensions
{
    public static IServiceCollection AddCarhubWebApiUtils(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services
            .Configure<AuthOptions>(configuration.GetSection(AuthOptions.OptionsName))
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddTransient<IContextFactory, ContextFactory>()
            .AddTransient<IContext>(sp => sp.GetRequiredService<IContextFactory>().Create())
            .AddSwagger(serviceName)
            .AddAuth();

        return services;
    }

    public static WebApplication UseCarhubWebApiUtils(this WebApplication app)
    {
        app.UseAuthentication();

        app.UseAuthorization();

        return app;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = $"CarHub {serviceName}", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT token must be provided",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        var options = services.GetOptions<AuthOptions>(AuthOptions.OptionsName);

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidAudience = options.ValidAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        if (string.IsNullOrWhiteSpace(options.IssuerSigningKey))
            throw new ArgumentException("Missing issuer signing key.", nameof(options.IssuerSigningKey));

        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;

        var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
            tokenValidationParameters.NameClaimType = options.NameClaimType;

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;

        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.MetadataAddress = options.MetadataAddress;
                o.SaveToken = o.SaveToken;
                o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.IncludeErrorDetails = options.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(options.Challenge))
                    o.Challenge = options.Challenge;
            });

        services.AddSingleton(options);
        services.AddSingleton(tokenValidationParameters);

        foreach (var policy in options.Policies)
            services.AddAuthorizationBuilder()
                .AddPolicy(policy, y => y.RequireClaim("permissions", policy));

        return services;
    }

    private static T GetOptions<T>(this IServiceCollection services, string sectionName)
        where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName)
        where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}