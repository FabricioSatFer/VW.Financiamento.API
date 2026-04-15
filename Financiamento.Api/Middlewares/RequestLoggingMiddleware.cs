using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Serilog.Log.Logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

            var requestBody = await ReadRequestBodyAsync(context.Request);

            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            Exception? exceptionCaught = null;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBodyStream);

                var logTemplate = "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms";

                if (exceptionCaught == null && context.Response.StatusCode < 400)
                {
                    _logger.Information(logTemplate,
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.Warning(logTemplate + " - Request: {RequestBody} - Response: {ResponseBody}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        SanitizeRequestBody(requestBody),
                        SanitizeResponseBody(responseBody));
                }
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }

            request.Body.Position = 0;
            var bodyAsText = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            return bodyAsText;
        }

        private string SanitizeRequestBody(string requestBody)
        {
            if (string.IsNullOrWhiteSpace(requestBody))
                return string.Empty;

            var sanitized = requestBody;
            var sensitiveFields = new[] { "senha", "password", "token", "secret", "authorization" };

            foreach (var field in sensitiveFields)
            {
                sanitized = System.Text.RegularExpressions.Regex.Replace(
                    sanitized,
                    $"\"{field}\"\\s*:\\s*\"[^\"]*\"",
                    $"\"{field}\":\"***REDACTED***\"",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return sanitized;
        }

        private string SanitizeResponseBody(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return string.Empty;

            var sanitized = responseBody;
            var sensitiveFields = new[] { "token", "senha", "password" };

            foreach (var field in sensitiveFields)
            {
                sanitized = System.Text.RegularExpressions.Regex.Replace(
                    sanitized,
                    $"\"{field}\"\\s*:\\s*\"[^\"]*\"",
                    $"\"{field}\":\"***REDACTED***\"",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return sanitized;
        }
    }
}
