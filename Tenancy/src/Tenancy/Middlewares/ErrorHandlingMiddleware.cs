using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Tenancy.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        const string Error = "Internal server error";
        public RequestDelegate Next { get; set; }
        
        public ErrorHandlingMiddleware(RequestDelegate next) 
            => this.Next = next;

        public async Task Invoke(HttpContext ctx, ILogger<ErrorHandlingMiddleware> log)
        {
            try
            {
                await Next(ctx);
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message);

                if (!ctx.Response.HasStarted)
                {
                    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await ctx.Response.WriteAsync(Error);
                }
            }
        }
    }
}
