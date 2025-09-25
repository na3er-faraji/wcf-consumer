namespace WcfConsumer.Application.Dtos
{
    public class ApiResult<T>
    {
        public ApiStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static ApiResult<T> Ok(T data) =>
            new ApiResult<T> { Status = ApiStatus.Ok, Data = data };

        public static ApiResult<T> NotFound(string error) =>
            new ApiResult<T> { Status = ApiStatus.NotFound, ErrorMessage = error };

        public static ApiResult<T> Unavailable(string error) =>
            new ApiResult<T> { Status = ApiStatus.Unavailable, ErrorMessage = error };
    }
}