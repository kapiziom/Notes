using System.Collections;
using Swashbuckle.AspNetCore.Annotations;

namespace Notes.WebAPI.Infrastructure.Middleware;

public class HandledExceptionResponse
{
    [SwaggerSchema("Error Reference", Nullable = false)]
    public string Reference { get; set; }
        
    [SwaggerSchema("Error message", Nullable = false)]
    public string Message { get; set; }
        
    [SwaggerSchema("Error additional data", Nullable = true)]
    public IDictionary Data { get; set; }
}