using ECC.API_Web.Models;
using FluentValidation;
 
namespace ECC.API_Web.InfraWeb.Validators
{


    public class RegistrarUsuarioViewModelValidator : AbstractValidator<RegistrarUsuarioViewModel>
        {
            public RegistrarUsuarioViewModelValidator()
            {
                RuleFor(r => r.UsuarioEmail).NotEmpty().EmailAddress()
                    .WithMessage("Email Inválido!");

                RuleFor(r => r.Nome).NotEmpty()
                    .WithMessage("Nome do Usuário obrigatório.");

                RuleFor(r => r.Senha).NotEmpty()
                    .WithMessage("Senha Inválida!");
            }
        }

        public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
        {
            public LoginViewModelValidator()
            {
                RuleFor(r => r.UsuarioEmail).NotEmpty().EmailAddress()
                    .WithMessage("Email Inválido!");

                RuleFor(r => r.Senha).NotEmpty()
                    .WithMessage("Senha inválida");
            }
        }
    
}