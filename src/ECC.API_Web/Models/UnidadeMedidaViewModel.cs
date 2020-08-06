using ECC.API_Web.InfraWeb.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECC.API_Web.Models
{
    public class UnidadeMedidaViewModel : IValidatableObject
    {
        public string DescUnidadeMedida { get; set; }
        public string DescAtivo { get; set; }
        public bool Ativo { get; set; }
        public int Id { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new UnidadeMedidaViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}