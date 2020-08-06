using ECC.API_Web.Models;
using FluentValidation;

namespace ECC.API_Web.InfraWeb.Validators
{
    public class CartaoCreditoViewModelValidators : AbstractValidator<CartaoCreditoViewModel>
    {
        public CartaoCreditoViewModelValidators()
        {

            RuleFor(r => r.Nome).NotEmpty()
                .WithMessage("Nome do titular do cartão é obrigatório.");

            RuleFor(r => r.Numero).NotEmpty()
                .WithMessage("Numero do cartão é obrigatório.");

            RuleFor(r => r.Cvc).NotEmpty()
           .WithMessage("Cóigo de segurança é obrigatório");

            RuleFor(r => r.DataVencimento).NotEmpty()
             .WithMessage("Data de vencimento do cartão é obrigatório.");

           // RuleFor(r => r.TokenCartaoGerenciaNet).NotEmpty()
           //.WithMessage("Token de transação é obrigatório.");

        }
    }

}