using System.ServiceModel;
using WcfConsumer.Application.Interfaces;
using WcfConsumer.Infrastructure.ExternalServices.CountryInfoServiceReference;

namespace WcfConsumer.Infrastructure.Soap.ExternalServices
{
    public class CountryClient : ICountryClient
    {
        private readonly CountryInfoServiceSoapTypeClient _soapClient;

        public CountryClient()
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress("http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso");

            _soapClient = new CountryInfoServiceSoapTypeClient(binding, endpoint);
        }

        public async Task<string> GetCapitalAsync(string countryCode)
        {
            var response = await _soapClient.CapitalCityAsync(countryCode);
            return response.Body.CapitalCityResult;
        }
    }
}
