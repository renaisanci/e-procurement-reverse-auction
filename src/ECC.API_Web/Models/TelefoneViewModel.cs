using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ECC.API_Web.InfraWeb.Validators;

namespace ECC.API_Web.Models
{
    public class TelefoneViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string DddTelComl { get; set; }
        public string TelefoneComl { get; set; }
        public string DddCel { get; set; }
        public string Celular { get; set; }
        public string Contato { get; set; }
        public bool Ativo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new TelefoneViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }

    }
}