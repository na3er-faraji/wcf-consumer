using System;
using WcfConsumer.Application.Dtos;
using WcfConsumer.Application.Interfaces;
using WcfConsumer.Domain.Interfaces;
using WcfConsumer.Domain.Models;
using WcfConsumer.Infrastructure.Decorators;

namespace WcfConsumer.Infrastructure
{
    public class CountryService : ICountryService
    {
        private readonly IResilientApiExecutor _executor;
        private readonly ICountryClientFactory _clientFactory;

        public CountryService(IResilientApiExecutor executor, ICountryClientFactory clientFactory)
        {
            _executor = executor;
            _clientFactory = clientFactory;
        }

        [Cacheable(ExpireMinutes = 120)]
        public async Task<CapitalResult> GetCapitalAsync(string countryCode)
        {
            var client = _clientFactory.CreateClient(); 

            var result = await _executor.ExecuteAsync(async () =>
            {
                var capital = await client.GetCapitalAsync(countryCode);
                if (string.IsNullOrWhiteSpace(capital) || capital == "Country not found in the database")
                    return ApiResult<CapitalResult>.NotFound("No capital found");

                return ApiResult<CapitalResult>.Ok(new CapitalResult
                {
                    CountryCode = countryCode.ToUpper(),  
                    Capital = capital
                });

            });

            return result.Data!;
        }
    }
}
