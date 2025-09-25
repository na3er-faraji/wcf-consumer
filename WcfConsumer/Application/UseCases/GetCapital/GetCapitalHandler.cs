using WcfConsumer.Domain.Interfaces;
using WcfConsumer.Domain.Models;

namespace WcfConsumer.Application.UseCases.GetCapital
{
    public class GetCapitalHandler
    {
        private readonly ICountryService _countryService;

        public GetCapitalHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public Task<CapitalResult> Handle(GetCapitalQuery query)
        {
            return _countryService.GetCapitalAsync(query.isoCode);
        }
    }
}
