 using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ECC.API_Web.InfraWeb.Validators;

namespace ECC.API_Web.Models
{
    public class EnderecoViewModel : IValidatableObject
    {

        public int Id { get; set; }
        public int PessoaId { get; set; }
        public int EstadoId { get; set; }
        public int CidadeId { get; set; }
        public int BairroId { get; set; }
        public int LogradouroId { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public string Endereco { get; set; }
        public int NumEndereco { get; set; }
         
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string BairroDescNew { get; set; }        
        public string Estado { get; set; }
        public string Logradouro { get; set; }
        public string Referencia { get; set; }

        public bool Ativo { get; set; }
        public bool EnderecoPadrao { get; set; }
        public string DescAtivo { get; set; }

        public List<PeriodoEntregaViewModel> PeriodoEntrega { get; set; }
                
        public string DescHorarioEntrega { get; set; }

        public bool FornecedorNesteEndereco { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new EnderecoViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }

    }
}