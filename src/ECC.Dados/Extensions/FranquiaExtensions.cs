using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;

namespace ECC.Dados.Extensions
{
    public  static class FranquiaExtensions
    {

        public static int CnpjExistente(this IEntidadeBaseRep<Franquia> franquiaRep, string cnpj)
        {
            return
                franquiaRep.GetAll()
                    .Count(x => x.Pessoa.PessoaJuridica.Cnpj == cnpj && x.Pessoa.PessoaJuridica.Ativo);
        }


    }
}
