using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;

[assembly: IntentTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Api.Configuration;

public static class OpenApiConfiguration
{
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(
            options =>
            {
                options.AddSchemaTransformer(
                    (schema, context, cancellationToken) =>
                    {
                        if (context.JsonTypeInfo.Type.IsValueType || context.JsonTypeInfo.Type == typeof(String) || context.JsonTypeInfo.Type == typeof(string))
                        {
                            return Task.CompletedTask;
                        }

                        if (schema.Annotations == null || !schema.Annotations.TryGetValue("x-schema-id", out object? _))
                        {
                            return Task.CompletedTask;
                        }

                        var schemaId = SchemaIdSelector(context.JsonTypeInfo.Type);
                        schema.Annotations["x-schema-id"] = schemaId;
                        schema.Title = schemaId;

                        return Task.CompletedTask;
                    });
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new();
                    return Task.CompletedTask;
                });
            });
        return services;
    }

    private static string SchemaIdSelector(Type modelType)
    {
        if (modelType.IsArray)
        {
            var elementType = modelType.GetElementType()!;
            return $"{SchemaIdSelector(elementType)}Array";
        }

        var modelName = modelType.Name.Replace("+", "_");

        if (!modelType.IsConstructedGenericType)
        {
            return modelName;
        }

        var baseName = modelName.Split('`').First();

        var genericArgs = modelType.GetGenericArguments()
            .Select(SchemaIdSelector)
            .ToArray();

        return $"{baseName}Of{string.Join("And", genericArgs)}";
    }
}