using ECC.API_Web.Models;
using FluentValidation;

namespace ECC.API_Web.InfraWeb.Validators
{
    public class MembroPFViewModelValidator : AbstractValidator<MembroPFViewModel>
    {


        public MembroPFViewModelValidator()
        {
           

         

          

            RuleFor(membro => membro.Cpf).NotEmpty().Must(UtilValidator.IsCpf)
                   .WithMessage("CPF Inválido.");


        }



    }
}