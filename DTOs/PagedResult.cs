namespace CompanyApi.DTOs
{
    public class PagedResult<T> : Result<IEnumerable<T>>
    {
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata(1, 0, 10);

        public static Task<PagedResult<T>> SuccessAsync(IEnumerable<T> data, int currentPage, int totalCount, int pageSize, string? message = null, int code = 200)
        {
            var pagination = new PaginationMetadata(currentPage, totalCount, pageSize);
            return Task.FromResult(new PagedResult<T>
            {
                Succeeded = true,
                Messages = message == null ? new List<string>() : new List<string> { message },
                Data = data,
                Code = code,
                Pagination = pagination
            });
        }

        public static Task<PagedResult<T>> FailureAsync(string message, int code = 400)
        {
            return Task.FromResult(new PagedResult<T>
            {
                Succeeded = false,
                Messages = new List<string> { message },
                Code = code,
                Pagination = new PaginationMetadata(1, 0, 10)
            });
        }

        public static Task<PagedResult<T>> FailureAsync(List<string> messages, int code = 400)
        {
            return Task.FromResult(new PagedResult<T>
            {
                Succeeded = false,
                Messages = messages,
                Code = code,
                Pagination = new PaginationMetadata(1, 0, 10)
            });
        }

        // Synchronous versions
        public static PagedResult<T> Success(IEnumerable<T> data, int currentPage, int totalCount, int pageSize, string? message = null, int code = 200)
        {
            var pagination = new PaginationMetadata(currentPage, totalCount, pageSize);
            return new PagedResult<T>
            {
                Succeeded = true,
                Messages = message == null ? new List<string>() : new List<string> { message },
                Data = data,
                Code = code,
                Pagination = pagination
            };
        }

        public static PagedResult<T> Failure(string message, int code = 400)
        {
            return new PagedResult<T>
            {
                Succeeded = false,
                Messages = new List<string> { message },
                Code = code,
                Pagination = new PaginationMetadata(1, 0, 10)
            };
        }
    }
}
