namespace RoomBooking.Core.Abstractions.Services;

public interface IServiceLogger<T>
{
    void LogResultCollection<TItem>(
        IEnumerable<TItem> collection,
        string serviceName, 
        string methodName,
        string? entityName = null);

    void LogResult(
        T? item,
        string serviceName, 
        string methodName, 
        string? entityName = null);

    void LogError(
        Exception exception,
        string serviceName,
        string methodName,
        string? additionalInfo = null);
}