using ECC.API_Web.Models;
using FluentValidation;

namespace ECC.API_Web.InfraWeb.Validators
{
    public class EnderecoViewModelValidator : AbstractValidator<EnderecoViewModel>
    {

        public EnderecoViewModelValidator()
        {
            RuleFor(membro => membro.Cep).NotEmpty()
                 .Length(1, 8).WithMessage("Cep obrigatório, deve ter entre 1 - 7 digitos");

            RuleFor(membro => membro.LogradouroId).NotEmpty()
                   .WithMessage("Logradouro obrigatório.");

            RuleFor(membro => membro.EstadoId).NotEmpty()
                .WithMessage("Estado obrigatório.");

            RuleFor(membro => membro.CidadeId).NotEmpty()
              .WithMessage("Cidade obrigatório.");


            RuleFor(membro => membro.BairroId).NotEmpty()
              .WithMessage("Bairro obrigatório.");

          
            RuleFor(membro => membro.NumEndereco).NotEmpty()
               // .GreaterThan(0).WithMessage("Número tem que ser maior que Zero.")
           .WithMessage("Número obrigatório.");

            RuleFor(membro => membro.Endereco).NotEmpty()
                .WithMessage("Endereço obrigatório.");


        }

    }
}