using MediatR;
using Microsoft.Extensions.Logging;

namespace RoomBooking.Application.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> //generic constraint
    // where TResponse : Result
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(
        ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        
        try
        {
            var result = await next(cancellationToken);
            _logger.LogInformation(
                "Completed request {@RequestName}, {@DateTimeUtc}",
                typeof(TRequest).Name,
                DateTime.UtcNow);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Request failure {@RequestName}, {@Error} {@DateTimeUtc}",
                typeof(TRequest).Name,
                ex,
                DateTime.UtcNow);
            
            throw;
        }
        
    }
}