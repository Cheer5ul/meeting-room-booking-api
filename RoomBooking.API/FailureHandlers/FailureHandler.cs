using Microsoft.AspNetCore.Mvc;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.API.FailureHandlers;

public class FailureHandler : IFailureHandler
{
    public ActionResult HandleFailure(Result result, HttpContext httpContext)
    {
        if(!result.IsFailure)
            throw new InvalidOperationException("Cannot handle success result.");
        
        var statusCode = GetStatusCode(result);
        var allErrors = GetAllErrors(result);

        var problem = new ProblemDetails()
        {
            Type = GetErrorType(result),
            Title = GetErrorTitle(result),
            Detail = GetErrorDetail(result),
            Status = statusCode,
            Instance = $"{httpContext.Request.Method} " +
                       $"{httpContext.Request.Scheme}" +
                       $"://{httpContext.Request.Host}" +
                       $"{httpContext.Request.Path}",
            Extensions = GetErrorExtensions(allErrors)!
        };

        return new ObjectResult(problem);
    }

    public IReadOnlyList<Error> GetAllErrors(Result result)
    {
        var errors = new List<Error>();
        
        if(result.Errors.Any())
            errors.AddRange(result.Errors);

        return errors;
    }

    public int GetStatusCode(Result result){
        
        var allErrors = GetAllErrors(result);

        if (!allErrors.Any())
            return StatusCodes.Status500InternalServerError;


        foreach (var error in allErrors)
        {
            if(ErrorCodeToStatus.TryGetValue(error.Code, out int statusCode))
                return statusCode;
        }


        return StatusCodes.Status400BadRequest;
    }

    private static readonly Dictionary<string, int> ErrorCodeToStatus = new()
    {
        ["NotFound"] = StatusCodes.Status404NotFound,
        ["Unauthorized"] = StatusCodes.Status401Unauthorized,
        ["Forbidden"] = StatusCodes.Status403Forbidden,
        ["Conflict"] = StatusCodes.Status409Conflict,
        ["ValidationFailed"] = StatusCodes.Status400BadRequest,
    };

    public string GetErrorType(Result result)
    {
        if(result.Errors.Any())
            return result.Errors.First().Code;
        
        return "unknown";
    }

    public string GetErrorTitle(Result result)
    {
        if (result.Errors.Count == 1)
            return result.Errors[0].Code;
        
        return "Multiple errors occurred";
    }

    public string? GetErrorDetail(Result result)
    {
        
        if (result.Errors.Count == 1)
            return result.Errors[0].Description;
        
        return string.Join("; ", result.Errors.Select(e => e.Description));
    }

    public Dictionary<string, object>? GetErrorExtensions(IReadOnlyList<Error> errors)
    {
        if (!errors.Any())
            return null;
        
        var extensions = new Dictionary<string, object>();
        
        extensions["errors"] = errors.Select(e => new
        {
            code = e.Code,
            description = e.Description,
        }).ToList();

        
        return extensions.Any() ?  extensions : null;
    }
}