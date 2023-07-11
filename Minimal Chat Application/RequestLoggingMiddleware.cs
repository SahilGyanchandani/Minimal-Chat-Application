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

namespace Minimal_Chat_Application
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var timeOfCall = DateTime.Now;

            // Fetch email from auth token (you need to implement this logic)
            var email = GetEmailFromAuthToken(context.User);

            // Log the request
            var logEntry = new RequestLog
            {
                IpAddress = ipAddress,
                Email = email,
                Timestamp = timeOfCall
            };

            dbContext.RequestLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();

            await _next(context);
        }

        //private async Task<string> ReadRequestBody(HttpRequest request)
        //{
        //    request.EnableBuffering();
        //    using var reader = new StreamReader(request.Body, leaveOpen: true);
        //    var body = await reader.ReadToEndAsync();
        //    request.Body.Position = 0; // Reset the position so the request can be read again
        //    return body;
        //}

        private string GetEmailFromAuthToken(ClaimsPrincipal user)
        {
            var emailClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            return emailClaim;
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
