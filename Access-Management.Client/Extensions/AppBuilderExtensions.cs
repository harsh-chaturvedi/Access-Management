using Access_Management.Client.Middleware;
using Access_Management.Client.Model;
using Microsoft.AspNetCore.Builder;

namespace Access_Management.Client.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseAPIAccessAuthorization(
        this IApplicationBuilder builder, APIAccessAuthorizationOptions options)
        {
            return builder.UseMiddleware<APIAccessAuthorizationMiddleware>(options);
        }
    }
}