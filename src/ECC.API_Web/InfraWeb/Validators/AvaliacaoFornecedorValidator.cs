using ECC.API_Web.Models;
using FluentValidation;


namespace ECC.API_Web.InfraWeb.Validators
{
    public class AvaliacaoFornecedorValidator : AbstractValidator<AvaliacaoFornecedorViewModel>
    {
        public AvaliacaoFornecedorValidator()
        {
            RuleFor(p => p.PedidoId).NotEmpty().WithMessage("Inserir id do pedido");
            RuleFor(p => p.FornecedorId).NotEmpty().WithMessage("Inserir o id do fornecedor");
            RuleFor(p => p.QualidadeProdutos).NotEmpty().WithMessage("A nota de qualidade dos produtos não pode ser nulo!");
            RuleFor(p => p.TempoEntrega).NotEmpty().WithMessage("A nota de tempo de entrega não pode ser nulo!");
            RuleFor(p => p.Atendimento).NotEmpty().WithMessage("A nota de atendimento não pode ser nulo!");

        }
    }
}