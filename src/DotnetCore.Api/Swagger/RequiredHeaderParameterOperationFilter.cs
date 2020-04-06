using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DotnetCore.Api.Swagger
{
    public class RequiredHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Culture (tr-TR or en-US)",
                Schema = new OpenApiSchema
                {
                    Type = "String",
                    Default = new OpenApiString("tr-TR")
                }
            });
        }
    }
}