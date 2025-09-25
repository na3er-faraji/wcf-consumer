using System.Diagnostics;
using Polly;
using Polly.Timeout;
using WcfConsumer.Application.Dtos;
using WcfConsumer.Application.Interfaces;

namespace WcfConsumer.Infrastructure.Resiliency
{
    public class PollyResiliencyPolicy : IResiliencyPolicy
    {
        private readonly ILogger<PollyResiliencyPolicy> _logger;

        public PollyResiliencyPolicy(ILogger<PollyResiliencyPolicy> logger)
        {
            _logger = logger;
        }

        public async Task<ApiResultWithReport<T>> ExecuteWithReportAsync<T>(Func<Task<ApiResult<T>>> action)
        {
            var retryDetails = new List<PolicyExecutionReport.RetryDetail>();
            var timeoutCount = 0;
            var fallbackTriggered = false;
            var circuitBroken = false;

            var retryPolicy = Policy<ApiResult<T>>
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (ex, ts, retryCount, ctx) =>
                    {
                        retryDetails.Add(new PolicyExecutionReport.RetryDetail
                        {
                            Attempt = retryCount,
                            DelaySeconds = ts.TotalSeconds,
                            ErrorMessage = ex.Exception.Message
                        });
                        _logger.LogWarning("Retry {RetryCount} after {Delay}s due to error: {Message}",
                            retryCount, ts.TotalSeconds, ex.Exception.Message);
                    });

            var timeoutPolicy = Policy.TimeoutAsync<ApiResult<T>>(
                TimeSpan.FromSeconds(5),
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: (ctx, ts, ex) =>
                {
                    timeoutCount++;
                    _logger.LogWarning("Timeout after {Timeout}s", ts.TotalSeconds);
                    return Task.CompletedTask;
                });

            var circuitBreakerPolicy = Policy<ApiResult<T>>
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    5,
                    TimeSpan.FromSeconds(30),
                    onBreak: (ex, ts) =>
                    {
                        circuitBroken = true;
                        _logger.LogWarning("Circuit broken for {Duration}s due to {Message}", ts.TotalSeconds, ex.Exception.Message);
                    },
                    onReset: () => _logger.LogInformation("Circuit reset"),
                    onHalfOpen: () => _logger.LogInformation("Circuit half-open"));

            var fallbackPolicy = Policy<ApiResult<T>>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackValue: new ApiResult<T> { Status = ApiStatus.Unavailable, ErrorMessage = "Service unavailable" },
                    onFallbackAsync: (ex, ctx) =>
                    {
                        fallbackTriggered = true;
                        _logger.LogError(ex.Exception, "Fallback triggered");
                        return Task.CompletedTask;
                    });

            var policy = fallbackPolicy.WrapAsync(circuitBreakerPolicy.WrapAsync(timeoutPolicy.WrapAsync(retryPolicy)));

            var stopwatch = Stopwatch.StartNew();
            var result = await policy.ExecuteAsync(action);
            stopwatch.Stop();

            var report = new PolicyExecutionReport
            {
                TotalExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds,
                RetryCount = retryDetails.Count,
                TimeoutCount = timeoutCount,
                FallbackTriggered = fallbackTriggered,
                CircuitBroken = circuitBroken,
                RetryDetails = retryDetails
            };

            return new ApiResultWithReport<T>
            {
                Status = result.Status,
                ErrorMessage = result.ErrorMessage,
                Data = result.Data,
                Report = report
            };
        }
    }
}
