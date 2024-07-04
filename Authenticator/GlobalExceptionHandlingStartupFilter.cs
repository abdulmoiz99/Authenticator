using Authenticator.Model.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Authenticator
{
    public class GlobalExceptionHandlingStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                var logger = builder.ApplicationServices.GetRequiredService<ILogger<GlobalExceptionHandlingStartupFilter>>();
                builder.UseGlobalExceptionHandler(logger);
                next(builder);
            };
        }
    }
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                var response = new AutoResponseDto<string>
                {
                    Exception = exception?.Message
                };

                if (exception != null)
                {
                    logger.LogError(exception, exception.Message);
                    response.AddError(exception.Message);
                }

                response.StatusCode = HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }));

            return app;
        }
    }
}
