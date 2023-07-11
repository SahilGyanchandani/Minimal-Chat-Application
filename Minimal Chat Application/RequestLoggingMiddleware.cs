using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Minimal_Chat_Application.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace Minimal_Chat_Application
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory logger)
        {
            _next = next;
            _logger = logger.CreateLogger("Custom");
        }

        public async Task Invoke(HttpContext context)
        {
            var currentUser = context.User.FindFirst(ClaimTypes.Email);
            if (currentUser == null)
            {
                _logger.LogInformation(" Custom");
            }
            var userEmailDetail = currentUser?.Value;
            _logger.LogInformation($"User Email: {userEmailDetail}");
            
            await _next(context);
        }
      
    }


        // Extension method used to add the middleware to the HTTP request pipeline.
        public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
    

}
