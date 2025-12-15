// using MediatR;
// using Microsoft.Extensions.Logging;
//
// namespace RoomBooking.Application.Behaviors;
//
// public class LoggingPipelineBehavior<TRequest, TResponse>
//     : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IRequest<TResponse> //generic constraint
//     // where TResponse : Result
// {
//     private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;
//
//     public LoggingPipelineBehavior(
//         ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
//     {
//         _logger = logger;
//     }
//     
//     public async Task<TResponse> Handle(
//         TRequest request,
//         RequestHandlerDelegate<TResponse> next, 
//         CancellationToken cancellationToken)
//     {
//         //structured logging
//         _logger.LogInformation(
//             "Starting request {@RequestName}, {@DateTimeUtc}",
//             typeof(TRequest).Name,
//             DateTime.UtcNow);
//         
//         var result = await next(cancellationToken);
//
//         if (result.IsFailure)
//         {
//             _logger.LogError(
//                 "Request failure {@RequestName}, {@Error} {@DateTimeUtc}",
//                 typeof(TRequest).Name,
//                 result.Error,
//                 DateTime.UtcNow);
//         }
//         
//         _logger.LogInformation(
//             "Completed request {@RequestName}, {@DateTimeUtc}",
//             typeof(TRequest).Name,
//             DateTime.UtcNow);
//         
//         return result;
//     }
// }