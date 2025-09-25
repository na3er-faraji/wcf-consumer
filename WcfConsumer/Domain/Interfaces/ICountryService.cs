using WcfConsumer.Domain.Models;

namespace WcfConsumer.Domain.Interfaces
{
    public interface ICountryService
    {
        Task<CapitalResult> GetCapitalAsync(string countryCode);
    }
}
