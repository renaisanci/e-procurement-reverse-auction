using System;
using ECC.API_Web.InfraWeb.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECC.API_Web.Models
{
    public class MembroViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public int PerfilId { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string InscEstadual { get; set; }
        public DateTime? DtFundacao { get; set; }
        public string Email { get; set; }
        public string Comprador { get; set; }
        public bool Ativo { get; set; }
        public string DescAtivo { get; set; }
        public string Completo { get; set; }
        public int? FranquiaId { get; set; }
        public int TelefoneId { get; set; }
        public string DddTelComl { get; set; }
        public string TelefoneComl { get; set; }
        public string DddCel { get; set; }
        public string Celular { get; set; }
        public string Contato { get; set; }
        public int TipoPessoa { get; set; }
        public string Cep { get; set; }
        public DateTime? DataFimPeriodoGratuito { get; set; }
        public bool Vip { get; set; }
        public UsuarioViewModel Usuario { get; set; }
        public FranquiaViewModel Franquia { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new MembroViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }



}