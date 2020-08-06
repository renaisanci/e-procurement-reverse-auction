
using ECC.API_Web.Models;
using FluentValidation;

namespace ECC.API_Web.InfraWeb.Validators
{
    public class UsuarioViewModelValidator : AbstractValidator<UsuarioViewModel>
    {


        public UsuarioViewModelValidator()
        {


            RuleFor(r => r.UsuarioEmail).NotEmpty().EmailAddress()
                  .WithMessage("Email Inválido!");

            RuleFor(r => r.UsuarioNome).NotEmpty()
                .WithMessage("Nome do Usuário obrigatório.");

            RuleFor(r => r.Senha).NotEmpty()
                .WithMessage("Senha Obrigatório!");

            RuleFor(r => r.ConfirmSenha).NotEmpty()
                .WithMessage("Confirmação de senha Obrigatório!");

            RuleFor(r => r.Senha).Equal(r => r.ConfirmSenha)
                .When(r => r.NovaSenha == null)
                .WithMessage("Confirmação de senha não esta igual a senha. ");
            
            RuleFor(r => r.NovaSenha).Equal(r => r.ConfirmSenha)
                .When(r => r.NovaSenha != null)
                .WithMessage("Confirmação de senha não esta igual a Nova senha. ");

            RuleFor(r => r.Senha).NotEmpty()
                .Length(1, 8)
                .When(r => r.NovaSenha == null)
                .WithMessage("Senha deve ter de 1 a 8 Caracteres");

            RuleFor(r => r.NovaSenha).NotEmpty()                
                .Length(1, 8)
                .When(r => r.NovaSenha != null)
                .WithMessage("Nova senha deve ter de 1 a 8 Caracteres");


        }


    }
}