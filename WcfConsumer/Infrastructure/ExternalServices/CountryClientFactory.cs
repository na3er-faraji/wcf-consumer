using WcfConsumer.Application.Interfaces;

namespace WcfConsumer.Infrastructure.Soap.ExternalServices
{
    public class CountryClientFactory : ICountryClientFactory
    {
        public ICountryClient CreateClient()
        {
            return new CountryClient();
        }
    }
}

