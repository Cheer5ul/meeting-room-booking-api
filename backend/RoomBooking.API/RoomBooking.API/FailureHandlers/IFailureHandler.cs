using Microsoft.AspNetCore.Mvc;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.API.FailureHandlers;

public interface IFailureHandler
{
    ActionResult HandleFailure(Result result, HttpContext httpContext);
    IReadOnlyList<Error> GetAllErrors(Result result);
    int GetStatusCode(Result result);
    string GetErrorType(Result result);
    string GetErrorTitle(Result result);
    string? GetErrorDetail(Result result);
    Dictionary<string, object>? GetErrorExtensions(IReadOnlyList<Error> errors);
}