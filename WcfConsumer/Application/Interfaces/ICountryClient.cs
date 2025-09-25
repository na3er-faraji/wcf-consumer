using WcfConsumer.Domain.Models;

namespace WcfConsumer.Application.Interfaces
{
    public interface ICountryClient
    {
        Task<string> GetCapitalAsync(string countryCode);
    }

    public interface ICountryClientFactory
    {
        ICountryClient CreateClient();
    }

}


