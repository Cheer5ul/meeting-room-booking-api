using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Core.Results;

public class Result
{
    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = isSuccess;
        Error = error ??  Error.None;
    }    
    protected Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Count != 0 ||
            !isSuccess && errors.Count == 0)
        {
            throw new ArgumentException("Invalid errors", nameof(errors));
        }
        IsSuccess = isSuccess;
        Errors = errors ?? new List<Error>();
    }
    private bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public Error? Error { get; }
    public List<Error> Errors { get; } = [];
    
    public static Result Success() => new(true, Error.None); //calls constructor, passes success value and empty error
    public static Result Failure(Error error) => new(false, error);
    public static Result MultipleFailure(List<Error> errors) =>new(false, errors);
    
}

public class Result<T> : Result
{
   public T? Value { get; }

   private Result(T value) : base(true, Error.None)
   {
       Value = value ?? throw new ArgumentNullException(nameof(value));
   }
   private Result(Error error) : base(false,  error) { }
   private Result(List<Error> errors) : base(false,  errors) { }

   //operator only for successful values 
   public static implicit operator Result<T>(T value) => new(value);

   public static Result<T> Success(T value) => new(value);
   //new hides inherited method Result.Failure and creates a new one
   public new static Result<T> Failure(Error error) => new(error);
   public static Result<T> MultipleFailures(List<Error> errors) => new(errors);
   
}