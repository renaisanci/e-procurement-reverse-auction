using ECC.API_Web.InfraWeb.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECC.API_Web.Models
{
    public class MembroDemandaViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public int MembroId { get; set; }
        public int CategoriaId { get; set; }
        public string DescCategoria { get; set; }
        public int SubCategoriaId { get; set; }
        public string DescSubCategoria { get; set; }
        public int PeriodicidadeId { get; set; }
        public string DescPeriodicidade { get; set; }
        public int UnidadeMedidaId { get; set; }
        public string DescUnidadeMedida { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }

        public string DescAtivo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new MembroDemandaViewModelValidator().Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}