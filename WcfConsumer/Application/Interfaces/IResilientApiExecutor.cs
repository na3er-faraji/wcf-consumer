using WcfConsumer.Application.Dtos;

namespace WcfConsumer.Application.Interfaces
{
    public interface IResilientApiExecutor
    {
        Task<ApiResultWithReport<T>> ExecuteAsync<T>(Func<Task<ApiResult<T>>> soapCall, IResiliencyPolicy? customPolicy = null);
    }
}

