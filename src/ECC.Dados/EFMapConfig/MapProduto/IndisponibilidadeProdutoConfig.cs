using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECC.Entidades.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class IndisponibilidadeProdutoConfig : EntidadeBaseConfig<IndisponibilidadeProduto>
    {
        public IndisponibilidadeProdutoConfig()
        {
            Property(t => t.ProdutoId).IsRequired();
            Property(t => t.FornecedorId).IsRequired();
            Property(t => t.InicioIndisponibilidade).IsOptional();
            Property(t => t.FimIndisponibilidade).IsOptional();
        }
    }
}
