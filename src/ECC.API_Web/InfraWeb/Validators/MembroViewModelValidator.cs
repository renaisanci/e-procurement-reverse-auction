using ECC.API_Web.Models;
using FluentValidation;

namespace ECC.API_Web.InfraWeb.Validators
{
    public class MembroViewModelValidator : AbstractValidator<MembroViewModel>
    {


        public MembroViewModelValidator()
        {
            RuleFor(membro => membro.NomeFantasia).NotEmpty()
                .Length(1, 100).WithMessage("Nome Fantasia deve ter entre 1 - 100 caracteres");

            RuleFor(membro => membro.Cnpj).NotEmpty() 
                .Length(1, 15).WithMessage("CNPJ deve entre 1 - 14 digitos");

            RuleFor(membro => membro.RazaoSocial).NotEmpty()
            .Length(1, 100).WithMessage("Razão social deve ter entre 1 - 100 caracteres");

            RuleFor(membro => membro.Cnpj).NotEmpty().Must(UtilValidator.validarCNPJ)
                   .WithMessage("CNPJ Inválido.");


        }



    }
}