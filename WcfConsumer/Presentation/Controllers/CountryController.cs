using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using WcfConsumer.Application.Exceptions;
using WcfConsumer.Application.UseCases.GetCapital;

namespace WcfConsumer.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly GetCapitalHandler _handler;

        public CountryController(GetCapitalHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("/countries/{isoCode}/capital")]
        public async Task<IActionResult> GetCapital([FromRoute] string isoCode)
        {
            isoCode = isoCode?.Trim().ToUpper() ?? "";

            if (isoCode.Length != 2 || !isoCode.All(char.IsLetter))
            {
                return BadRequest(new { error = "ISO code must be exactly 2 letters." });
            }
            try
            {
                var result = await _handler.Handle(new GetCapitalQuery(isoCode));
                return Ok(result); 
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message }); 
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { error = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }



    }
}