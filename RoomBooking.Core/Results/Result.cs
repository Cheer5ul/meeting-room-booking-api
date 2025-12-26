using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Core.Results;

public class Result
{
    protected Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Count != 0 ||
            !isSuccess && errors.Count == 0)
        {
            throw new ArgumentException("Invalid errors", nameof(errors));
        }
        IsSuccess = isSuccess;
        Errors = errors;
    }

    // protected Result(bool isSuccess, Dictionary<string, string[]> errorsDictionary)
    // {
    //     if (isSuccess && errorsDictionary.Count != 0 ||
    //         !isSuccess && errorsDictionary.Count == 0)
    //     {
    //         throw new ArgumentException("Invalid errors", nameof(errorsDictionary));
    //     }
    //     IsSuccess = isSuccess;
    //     ErrorsDictionary = errorsDictionary;
    // }
    private bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public List<Error> Errors { get; }
    // public Dictionary<string, string[]> ErrorsDictionary { get; }
    
    public static Result Success() => new(isSuccess: true, errors: []); //calls constructor, passes success value and empty error
    public static Result Failures(List<Error> errors) => new(false, errors);
    // public static Result FailuresDicionary(Dictionary<string, string[]> errorsDictionary) =>
    //     new(false, errorsDictionary);
}

public class Result<T> : Result
{
   public T? Value { get; }

   private Result(T value) : base(isSuccess: true, errors: [])
   {
       Value = value ?? throw new ArgumentNullException(nameof(value));
   }
   private Result(List<Error> errors) : base(false,  errors) { }

   // private Result(Dictionary<string, string[]> errors) : base(false, errors) { }

   //operator only for successful values 
   public static implicit operator Result<T>(T value) => new(value);

   public static Result<T> Success(T value) => new(value);
   //'new' hides inherited method Result.Failure and creates a new one
   public new static Result<T> Failures(List<Error> errors) => new Result<T>(errors);
   // public new static Result<T> FailuresDicionary(Dictionary<string, string[]> errors) => new(errors);

}