using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using Serilog.Events;

namespace S3_ForwardStream.Helpers
{
    public static class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("HttpRequestClientHostIP", httpContext.Connection.RemoteIpAddress);
            diagnosticContext.Set("HttpRequestUrl", httpContext.Request.GetDisplayUrl());
            diagnosticContext.Set("UserName", httpContext.User.Identity.Name ?? "(anonymous)");

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex)
        {
            if (ex != null)
            {
                return LogEventLevel.Error;
            }

            if (ctx.Response.StatusCode > 499)
            {
                return LogEventLevel.Error;
            }

            return IsHealthCheckEndpoint(ctx) ? LogEventLevel.Verbose : LogEventLevel.Information;
        }

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            return ctx.Request.Path.StartsWithSegments("/health");
        }
    }
}
