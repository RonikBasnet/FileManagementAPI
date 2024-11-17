using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileUploadMethods = context.MethodInfo.GetCustomAttributes(true)
            .OfType<HttpPostAttribute>()
            .Any();

        if (fileUploadMethods && operation.RequestBody != null)
        {
            var fileUploadContent = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["file"] = new OpenApiSchema()
                        {
                            Type = "string",
                            Format = "binary"
                        },
                        ["title"] = new OpenApiSchema()
                        {
                            Type = "string"
                        },
                        ["category"] = new OpenApiSchema()
                        {
                            Type = "string"
                        }
                    },
                    Required = new HashSet<string> { "file" }
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = fileUploadContent
                }
            };
        }
    }
}