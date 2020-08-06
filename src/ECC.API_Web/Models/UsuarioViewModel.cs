using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ECC.API_Web.InfraWeb.Validators;
using ECC.EntidadePessoa;

namespace ECC.API_Web.Models
{
    public class UsuarioViewModel : IValidatableObject
    {

        public int Id { get; set; }
        public int PerfilId { get; set; }
        public int PessoaId { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioEmail { get; set; }
        public string Senha { get; set; }
        public string TokenSignalRUser { get; set; }
        
        public string NovaSenha { get; set; }
        public string ConfirmSenha { get; set; }
        public bool FlgMaster { get; set; }
        public bool Ativo { get; set; }
        public string DescAtivo { get; set; }
        public string DescFlgMaster { get; set; }
        public int OrigemLogin { get; set; }

        public TelefoneViewModel TelefoneUsuario { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new UsuarioViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }

    }
}