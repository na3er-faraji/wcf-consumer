namespace WcfConsumer.Application.Dtos
{
    public class ApiResultWithReport<T> : ApiResult<T>
    {
        public Infrastructure.Resiliency.PolicyExecutionReport Report { get; set; }
            = new Infrastructure.Resiliency.PolicyExecutionReport();
    }
}
