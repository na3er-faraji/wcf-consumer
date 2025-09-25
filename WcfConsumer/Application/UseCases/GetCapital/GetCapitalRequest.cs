using System.ComponentModel.DataAnnotations;

namespace WcfConsumer.Application.UseCases.GetCapital
{
    public record GetCapitalRequest(
        [Required]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "ISO code must be exactly 2 letters.")]
        string IsoCode
    );
}
