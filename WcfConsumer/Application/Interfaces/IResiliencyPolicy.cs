using WcfConsumer.Application.Dtos;

namespace WcfConsumer.Application.Interfaces
{
    public interface IResiliencyPolicy
    {
        Task<ApiResultWithReport<T>> ExecuteWithReportAsync<T>(Func<Task<ApiResult<T>>> action);
    }
}