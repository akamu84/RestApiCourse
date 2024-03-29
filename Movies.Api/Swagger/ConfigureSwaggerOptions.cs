﻿using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IHostEnvironment _environment;
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IHostEnvironment environment)
    {
        _provider = provider;
        _environment = environment;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please provide a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("An example application with OpenAPI, Swashbuckle, and API versioning.");
        var info = new OpenApiInfo
        {
            Title = "Example API",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact { Name = "Bill Mei", Email = "bill.mei@somewhere.com" },
            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };

        if (description.IsDeprecated) text.Append(" This API version has been deprecated.");

        if (description.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');

            if (policy.HasLinks)
            {
                text.AppendLine();

                foreach (var link in policy.Links)
                    if (link.Type == "text/html")
                    {
                        text.AppendLine();

                        if (link.Title.HasValue) text.Append(link.Title.Value).Append(": ");

                        text.Append(link.LinkTarget.OriginalString);
                    }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}