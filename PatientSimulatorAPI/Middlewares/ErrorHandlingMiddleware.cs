
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PatientSimulatorAPI.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await WriteErrorAsync(context, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await WriteErrorAsync(context, ex.Message);
            }
            catch (Azure.RequestFailedException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                await WriteErrorAsync(context, "Upstream service error: " + ex.Message);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await WriteErrorAsync(context, "An unexpected error occurred.");
                // TODO: log ex
            }
        }

        private static Task WriteErrorAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            var errorObj = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(errorObj);
        }

    }
}

