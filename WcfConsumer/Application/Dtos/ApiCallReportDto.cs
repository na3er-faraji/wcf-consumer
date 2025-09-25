namespace WcfConsumer.Application.Dtos
{
    public class ApiCallReportDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = "";
        public string CacheKey { get; set; } = "";
        public DateTime Timestamp { get; set; }

        public double TotalExecutionTimeSeconds { get; set; }
        public int RetryCount { get; set; }
        public int TimeoutCount { get; set; }
        public bool FallbackTriggered { get; set; }
        public bool CircuitBroken { get; set; }
    }
}
