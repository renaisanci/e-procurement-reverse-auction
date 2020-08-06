using System.Linq;
using ECC.Dados.Repositorio;
using ECC.EntidadeEndereco;

namespace ECC.Dados.Extensions
{
    public static class EnderecoExtensions
    {
        public static int VerificaEnderecoJaCadastrado(this IEntidadeBaseRep<Endereco> enderecoRep, string cep, int pessoaId)
        {
            return
                enderecoRep.GetAll()
                    .Count(x => x.Cep==cep && x.PessoaId==pessoaId && x.Ativo);
        }
    }
}
