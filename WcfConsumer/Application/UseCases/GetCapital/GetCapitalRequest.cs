using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WcfConsumer.Application.UseCases.GetCapital
{
    public record GetCapitalRequest(
        [Required]
        [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "ISO code must be exactly 2 letters.")]
        [FromRoute(Name = "isoCode")]
        string IsoCode
    );
}
