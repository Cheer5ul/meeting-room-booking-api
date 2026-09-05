using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace RoomBooking.API.Middlewares.ExceptionHandlers;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    ///  Tries to handle an exception successfully
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> true if able to handle exception successfully</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        
        logger.LogError(exception, "--------------------- UNHANDLED EXCEPTION OCCURED --------------------");

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()? .Activity;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails()
            // {
            //     Type = exception.GetType().Name,
            //     Title = "An error occured",
            //     Detail = exception.Message,
            //     Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            //     Extensions = new Dictionary<string, object?>()
            //     {
            //         {"requestId",  httpContext.TraceIdentifier},
            //         {"traceId", activity?.Id}
            //     }
            // }
        });
    }
}