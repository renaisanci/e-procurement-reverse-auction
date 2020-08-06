using System.Linq;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;

namespace ECC.Dados.Extensions
{
    public static class FornecedorExtensions
    {


        public static int CnpjExistente(this IEntidadeBaseRep<Fornecedor> fornecedorRep, string cnpj)
        {
            return
                fornecedorRep.GetAll()
                    .Count(x => x.Pessoa.PessoaJuridica.Cnpj == cnpj && x.Pessoa.PessoaJuridica.Ativo);
        }
    }
}
