namespace WcfConsumer.Infrastructure.Resiliency
{
    public class PolicyExecutionReport
    {
        public double TotalExecutionTimeSeconds { get; set; }
        public int RetryCount { get; set; }
        public int TimeoutCount { get; set; }
        public bool FallbackTriggered { get; set; }
        public bool CircuitBroken { get; set; }
        public List<RetryDetail> RetryDetails { get; set; } = new();

        public class RetryDetail
        {
            public int Attempt { get; set; }
            public double DelaySeconds { get; set; }
            public string? ErrorMessage { get; set; }
        }
    }
}
