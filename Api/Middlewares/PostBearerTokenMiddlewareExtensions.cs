using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Middlewares;

public static class PostBearerTokenMiddlewareExtensions
{
    public static IApplicationBuilder UsePostBearerTokenGuard(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await next();
                return;
            }

            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Missing or invalid Authorization header. Use Bearer token."
                });
                return;
            }

            var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (!authResult.Succeeded || authResult.Principal is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Invalid or expired token."
                });
                return;
            }

            context.User = authResult.Principal;
            await next();
        });
    }
}