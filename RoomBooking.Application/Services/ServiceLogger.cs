
using Microsoft.Extensions.Logging;
using RoomBooking.Core.Abstractions.Services;

namespace RoomBooking.Application.Services;

public class ServiceLogger<T> : IServiceLogger<T>
{
    private readonly ILogger<T> _logger;
    public ServiceLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogResultCollection<TItem>(
        IEnumerable<TItem> collection,
        string serviceName, 
        string methodName,
        string? entityName = null)
    {
        var count = collection.Count();
        var entityType = entityName ?? typeof(TItem).Name;
        
        if (count == 0 )
        {
            _logger.LogWarning("{Service}.{Method}: No {Entity} found",
                serviceName,
                methodName,
                entityType);
            return;
        }
        _logger.LogInformation("{Service}.{Method}: completed. Found {Count} {Entity}",
            serviceName,
            methodName,
            count,
            entityType);
    }

    public void LogResult(T? item,
        string serviceName, 
        string methodName, 
        string? entityName = null)
    {
        _logger.LogInformation("{Service}.{Method}: completed.",
            serviceName,
            methodName);
    }

    public void LogError(Exception exception,
        string serviceName, 
        string methodName, 
        string? additionalInfo = null)
    {
        _logger.LogError(
            exception,
            "{Service}.{Method}: failed. {AdditionalInfo}",
            serviceName,
            methodName,
            additionalInfo ?? string.Empty);
    }
}