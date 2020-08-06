using System.Linq;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;


namespace ECC.Dados.Extensions
{
    public static class MembroExtensions
    {
        public static int CnpjExistente(this IEntidadeBaseRep<Membro> membroRep, string cnpj)
        {
            return
                membroRep.GetAll()
                    .Count(x => x.Pessoa.PessoaJuridica.Cnpj == cnpj && x.Pessoa.PessoaJuridica.Ativo);
        }



        public static int CpfExistente(this IEntidadeBaseRep<Membro> membroRep, string cpf)
        {
            return
                membroRep.GetAll()
                    .Count(x => x.Pessoa.PessoaFisica.Cpf == cpf && x.Pessoa.PessoaFisica.Ativo);
        }


        public static int CroExistente(this IEntidadeBaseRep<Membro> membroRep, string cro)
        {
            return
                membroRep.GetAll()
                    .Count(x => x.Pessoa.PessoaFisica.Cro == cro && x.Pessoa.PessoaFisica.Ativo);
        }



    }
}
