using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ECC.API_Web.InfraWeb.Validators;
using ECC.EntidadeFormaPagto;
using ECC.Entidades.EntidadePessoa;

namespace ECC.API_Web.Models
{
    public class FornecedorViewModel : IValidatableObject
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
        public string Responsavel { get; set; }
        // public decimal VlPedidoMin { get; set; }
        string _vlPedidoMin;
        public string VlPedidoMin
        {
            get
            {
                return _vlPedidoMin;
            }
            set
            {
                _vlPedidoMin = value.Replace("R$", "");
            }
        }
        public bool Ativo { get; set; }
        public string DescAtivo { get; set; }
        public int TelefoneId { get; set; }
        public string DddTelComl { get; set; }
        public string TelefoneComl { get; set; }
        public string DddCel { get; set; }
        public string Celular { get; set; }
        public string Contato { get; set; }
        public string Descricao { get; set; }
        public string PalavrasChaves { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoEntrega { get; set; }
        public int PrazoEntrega { get; set; }
        public bool PrimeiraAvista { get; set; }
        public EnderecoViewModel Endereco { get; set; }
        public string Completo { get; set; }
        public List<FornecedorFormaPagtoViewModel> FormaPagtos { get; set; }
        public List<FormaPagtoViewModel> FormasPagamento { get; set; }
        public List<FornecedorPrazoSemanalViewModel> FornecedorPrazoSemanal { get; set; }
        public List<FornecedorRegiaoViewModel> FornecedorRegiao { get; set; }
        public UsuarioViewModel Usuario { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new FornecedorViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}