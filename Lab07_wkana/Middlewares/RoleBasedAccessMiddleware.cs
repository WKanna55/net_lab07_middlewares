namespace Lab07_wkana.Middlewares;
using System.Security.Claims;
public class RoleBasedAccessMiddleware
{
    private readonly RequestDelegate _next;

    public RoleBasedAccessMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userRole = context.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Acceso denegado. El rol del usuario no tiene permisos.");
            return;
        }

        await _next(context); // Si el rol es adecuado, pasa al siguiente middleware
    }
}