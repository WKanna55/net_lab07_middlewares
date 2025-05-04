using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace Lab07_wkana.Middlewares;

public class ParameterValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _contextAccessor;
    public ParameterValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
        {
            var model = await DeserializeRequestBody(context);
            if (model == null)
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Cuerpo de la solicitud inválido o vacío.");
                return;
            }
            var validationErrors = ValidateModel(model);
            if (validationErrors.Any())
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync(string.Join("\n", validationErrors));
                return;
            }
        }
        await _next(context);
    }
    private async Task<object> DeserializeRequestBody(HttpContext context)
    {
        var contentType = context.Request.ContentType;

        if (contentType != null && contentType.Contains("application/json"))
        {
            context.Request.EnableBuffering(); // Habilita el buffering

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: System.Text.Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true); // deja el stream abierto

            var body = await reader.ReadToEndAsync();

            context.Request.Body.Position = 0; // Rebobina el stream para que el controller pueda leerlo

            var dtoType = context.GetEndpoint()?.Metadata?.OfType<ControllerActionDescriptor>()
                .FirstOrDefault()?.MethodInfo?.GetParameters().FirstOrDefault()?.ParameterType;

            if (dtoType != null)
            {
                var model = JsonConvert.DeserializeObject(body, dtoType);
                return model;
            }
        }
        return null;
    }

    private List<string> ValidateModel(object model)
    {
        var errors = new List<string>();
        var properties = model.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(model);
            if (value == null)
            {
                errors.Add($"El parámetro '{property.Name}' es obligatorio.");
            }
        }
        return errors;
    }
}