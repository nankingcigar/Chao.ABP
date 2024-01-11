using Chao.Abp.Swashbuckle.Option;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection;

public static class ChaoAbpSwaggerGenServiceCollectionExtensions
{
    public static IServiceCollection AddChaoAbpSwaggerGenWithOAuth(
        this IServiceCollection services,
        [NotNull] IEnumerable<AuthorityOption> authorityOptions,
        [NotNull] Dictionary<string, string> scopes,
        Action<SwaggerGenOptions>? setupAction = null,
        string authorizationEndpoint = "/connect/authorize",
        string tokenEndpoint = "/connect/token")
    {
        return services
            .AddAbpSwaggerGen()
            .AddSwaggerGen(
                options =>
                {
                    foreach (var authorityOption in authorityOptions)
                    {
                        var authorizationUrl = new Uri($"{authorityOption.Authority!.TrimEnd('/')}{authorizationEndpoint.EnsureStartsWith('/')}");
                        var tokenUrl = new Uri($"{authorityOption.Authority.TrimEnd('/')}{tokenEndpoint.EnsureStartsWith('/')}");
                        options.AddSecurityDefinition(authorityOption.Name, new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = authorizationUrl,
                                    Scopes = scopes,
                                    TokenUrl = tokenUrl
                                }
                            }
                        });
                    }
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "oauth2"
                                    }
                                },
                                Array.Empty<string>()
                            }
                    });
                    setupAction?.Invoke(options);
                });
    }
}