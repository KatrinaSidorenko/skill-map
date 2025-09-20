﻿using Microsoft.OpenApi.Models;
using SkillMap.Shared.Constants;

namespace SkillMap.Api.Extensions;

public static class SwaggerGenerationExtension
{
    public static IServiceCollection AddSwaggerGeneration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var playerAuthenticationSchema =
                GetJwtBearerOpenApiSecuritySchemeByAuthenticationSchema(AuthenticationSchema.User);

            options.AddSecurityDefinition(AuthenticationSchema.User, playerAuthenticationSchema);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                playerAuthenticationSchema,
                Array.Empty<string>()
            }
        });
        });

        return services;
    }

    private static OpenApiSecurityScheme GetJwtBearerOpenApiSecuritySchemeByAuthenticationSchema(string authenticationSchema)
    {
        return new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = authenticationSchema
            },
            Name = authenticationSchema,
            Description = $"JWT Authorization for {authenticationSchema}. Example: \"Bearer {{token}}\"",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        };
    }
}
