using ECC.API_Web.Models;
using FluentValidation;

namespace ECC.API_Web.InfraWeb.Validators
{
    public class MembroDemandaViewModelValidator : AbstractValidator<MembroDemandaViewModel>
    {
        public MembroDemandaViewModelValidator()
        {
            RuleFor(membro => membro.Observacao)
                .Length(0, 300).WithMessage("Observação deve ter entre 1 - 300 caracteres");
        }
    }
}