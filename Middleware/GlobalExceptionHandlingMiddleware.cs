
using System.Net;
using System.Text.Json;
using NotFoundException = trading_app.exceptions.NotFoundException;
using BadRequestException = trading_app.exceptions.BadRequestException;
using UnauthorizedAccessException = trading_app.exceptions.UnauthorizedAccessException;

namespace trading_app.Middleware;

public class GlobalExceptionHandlingMiddleware
{
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next){
            _next = next;
        }

        public async Task Invoke(HttpContext context){
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                
                await HandleExceptionAsync(context, ex);
            }
        }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode status;
        var stackTrace = string.Empty;
        string message = "";
        var exceptionType = ex.GetType();

        if (exceptionType == typeof(BadRequestException)){
               status= HttpStatusCode.BadRequest;
               stackTrace = ex.StackTrace;
               message = ex.Message;      
        }
        else if (exceptionType == typeof(NotFoundException)){
               status= HttpStatusCode.NotFound;
               stackTrace = ex.StackTrace;
               message = ex.Message;
        }
        else if (exceptionType == typeof(UnauthorizedAccessException)){
               status= HttpStatusCode.Unauthorized;
               stackTrace = ex.StackTrace;
               message = ex.Message;
        }
        else{
               status= HttpStatusCode.InternalServerError;
               stackTrace = ex.StackTrace;
               message = ex.Message;
        }
        var exceptionResult = JsonSerializer.Serialize(new {statuscode = status,error = message, stacktrace = stackTrace});
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) status;
        return context.Response.WriteAsync(exceptionResult);
    }
}
