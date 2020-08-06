using ECC.API_Web.InfraWeb.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ECC.API_Web.Models
{
    public class RegistrarUsuarioViewModel : IValidatableObject
    {

        public string Nome { get; set; }
        public string Senha { get; set; }
        public string UsuarioEmail { get; set; }
        public int PerfilId { get; set; }
        public int PessoaId { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new RegistrarUsuarioViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}