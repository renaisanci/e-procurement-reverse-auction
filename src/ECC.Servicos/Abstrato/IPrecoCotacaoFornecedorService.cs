using System.Collections.Generic;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.Entidades.EntidadeComum;
using ECC.EntidadeStatus;
using ECC.EntidadeUsuario;

namespace ECC.Servicos.Abstrato
{
    public interface IPrecoCotacaoFornecedorService
    {
        List<CotacaoUsuarios> PrecificarCotacaoFornecedor();
    }
}
