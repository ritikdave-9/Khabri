using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Common.Utils;
namespace Khabri.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var startTime = DateTime.UtcNow;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";


            CustomLogger.LogInformation($"Request: {request.Method} {request.Path} | IP: {ipAddress}");

            await _next(context);

            var statusCode = context.Response.StatusCode;
            var elapsed = DateTime.UtcNow - startTime;

            CustomLogger.LogInformation($"Response: {request.Method} {request.Path} | IP: {ipAddress} - Status: {statusCode} - Time: {elapsed.TotalMilliseconds} ms");
        }
    }

}