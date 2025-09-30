namespace CompanyApi.DTOs.ResponseDtos
{
    public class Result<T> : IResult<T>
    {
        public List<string> Messages { get; set; } = new List<string>();
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public List<ValidationResults> ValidationErrors { get; set; } = new List<ValidationResults>();
        public int Code { get; set; }

        public static Task<Result<T>> SuccessAsync(string? message = null, int code = 200)
        {
            return Task.FromResult(new Result<T>
            {
                Succeeded = true,
                Messages = message == null ? new List<string>() : new List<string> { message },
                Code = code
            });
        }

        public static Task<Result<T>> SuccessAsync(T data, string? message = null, int code = 200)
        {
            return Task.FromResult(new Result<T>
            {
                Succeeded = true,
                Messages = message == null ? new List<string>() : new List<string> { message },
                Data = data,
                Code = code
            });
        }

        public static Task<Result<T>> FailureAsync(T data, string message = "", int code = 400)
        {
            return Task.FromResult(new Result<T>
            {
                Succeeded = false,
                Messages = string.IsNullOrEmpty(message) ? new List<string>() : new List<string> { message },
                Data = data,
                Code = code
            });
        }

        public static Task<Result<T>> FailureAsync(string message = "", int code = 400)
        {
            return Task.FromResult(new Result<T>
            {
                Succeeded = false,
                Messages = string.IsNullOrEmpty(message) ? new List<string>() : new List<string> { message },
                Data = default,
                Code = code
            });
        }

        public static Task<Result<T>> FailureAsync(T data, List<string> messages = null, int code = 400)
        {
            return Task.FromResult(new Result<T>
            {
                Succeeded = false,
                Messages = messages ?? new List<string>(),
                Data = data,
                Code = code
            });
        }

        public static Task<Result<T>> FailureAsync(List<string> messages = null, int code = 400)
        {
            return Task.FromResult(new Result<T>
            {
                Succeeded = false,
                Messages = messages ?? new List<string>(),
                Code = code
            });
        }



        // Synchronous versions for convenience
        public static Result<T> Success(string? message = null, int code = 200)
        {
            return new Result<T>
            {
                Succeeded = true,
                Messages = message == null ? new List<string>() : new List<string> { message },
                Code = code
            };
        }

        public static Result<T> Success(T data, string? message = null, int code = 200)
        {
            return new Result<T>
            {
                Succeeded = true,
                Messages = message == null ? new List<string>() : new List<string> { message },
                Data = data,
                Code = code
            };
        }

        public static Result<T> Failure(string message, int code = 400)
        {
            return new Result<T>
            {
                Succeeded = false,
                Messages = new List<string> { message },
                Code = code
            };
        }

        public static Result<T> Failure(List<string> messages, int code = 400)
        {
            return new Result<T>
            {
                Succeeded = false,
                Messages = messages,
                Code = code
            };
        }

        public static Result<T> Failure(T data, string message, int code = 400)
        {
            return new Result<T>
            {
                Succeeded = false,
                Messages = new List<string> { message },
                Data = data,
                Code = code
            };
        }

        public static Result<T> Failure(T data, List<string> messages, int code = 400)
        {
            return new Result<T>
            {
                Succeeded = false,
                Messages = messages,
                Data = data,
                Code = code
            };
        }

        // Method to add validation errors
        public void AddValidationError(string propertyName, string errorMessage, object? attemptedValue = null, string errorCode = "")
        {
            ValidationErrors.Add(new ValidationResults(propertyName, errorMessage, attemptedValue, errorCode));
        }

        // Method to add multiple validation errors
        public void AddValidationErrors(IEnumerable<ValidationResults> errors)
        {
            ValidationErrors.AddRange(errors);
        }
    }
}
