using WcfConsumer.Application.Dtos;
using WcfConsumer.Application.Interfaces;

namespace WcfConsumer.Infrastructure.Resiliency
{
    public class ResilientApiExecutor : IResilientApiExecutor
    {
        private readonly IResiliencyPolicy _defaultPolicy;

        public ResilientApiExecutor(IResiliencyPolicy defaultPolicy)
        {
            _defaultPolicy = defaultPolicy;
        }

        public async Task<ApiResultWithReport<T>> ExecuteAsync<T>(
            Func<Task<ApiResult<T>>> soapCall,
            IResiliencyPolicy? customPolicy = null)
        {
            var policy = customPolicy ?? _defaultPolicy;
            return await policy.ExecuteWithReportAsync(soapCall);
        }
    }
}