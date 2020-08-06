using System;
using ECC.API_Web.InfraWeb.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECC.API_Web.Models
{
    public class MembroPFViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public int PerfilId { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cro { get; set; }
        public string Rg { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool Vip { get; set; }
        public string DescAtivo { get; set; }
        public int TipoPessoa { get; set; }
        public string Completo { get; set; }
        public int Sexo { get; set; }
        public string Cep { get; set; }

        public string Comprador { get; set; }
 
 
        public int TelefoneId { get; set; }
        public string DddTelComl { get; set; }
        public string TelefoneComl { get; set; }
        public string DddCel { get; set; }
        public string Celular { get; set; }
        public string Contato { get; set; }
        public UsuarioViewModel Usuario { get; set; }
        public FranquiaViewModel Franquia { get; set; }

        public DateTime? DataFimPeriodoGratuito { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new MembroPFViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }

    }



}