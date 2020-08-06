using ECC.API_Web.InfraWeb.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ECC.API_Web.Models
{
    public class LoginViewModel : IValidatableObject
    {


        public string UsuarioEmail { get; set; }
        public string Senha { get; set; }

        public int perfilModulo { get; set; }
        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new LoginViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}