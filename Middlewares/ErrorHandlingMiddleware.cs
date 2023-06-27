using System.Net;
using System.Text.Json;

namespace AMS.Middlewares
{

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var error = new
                {
                    success = false,
                    message = ex.Message
                };

                var json = JsonSerializer.Serialize(error);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
