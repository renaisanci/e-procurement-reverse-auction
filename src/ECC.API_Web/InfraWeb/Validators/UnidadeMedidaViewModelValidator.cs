using ECC.API_Web.Models;
using FluentValidation;


namespace ECC.API_Web.InfraWeb.Validators
{
    public class UnidadeMedidaViewModelValidator : AbstractValidator<UnidadeMedidaViewModel>
    {
        public UnidadeMedidaViewModelValidator()
        {
            RuleFor(unidade => unidade.DescUnidadeMedida).NotEmpty()
                .Length(1, 100).WithMessage("Descrição deve ter entre 1 - 100 caracteres");

        }
    }
}