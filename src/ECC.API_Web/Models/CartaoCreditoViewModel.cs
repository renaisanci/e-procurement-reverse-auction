using ECC.API_Web.InfraWeb.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECC.API_Web.Models
{
    public class CartaoCreditoViewModel : IValidatableObject
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Numero { get; set; }

        public string DataVencimento { get; set; }

        public string Cvc { get; set; }

        public string TokenCartaoGerenciaNet { get; set; }

        public int MembroId { get; set; }

        public int CartaoBandeiraId { get; set; }

        public string DescricaoCartaoBandeira { get; set; }

        public bool Padrao { get; set; }

        public bool Ativo { get; set; }

       
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CartaoCreditoViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}