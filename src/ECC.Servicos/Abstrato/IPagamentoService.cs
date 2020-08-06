using ECC.EntidadePessoa;
using ECC.EntidadeRecebimento;
using ECC.Entidades.EntidadeRecebimento;
using ECC.EntidadeUsuario;
using Gerencianet.SDK.Responses;
using System;

namespace ECC.Servicos.Abstrato
{
    public interface IPagamentoService
    {
        ParametrosRecebimento Parametros { get; }
        
        void GerarComissao(int idUsuario, int idPedido, int idFornecedor);

        void GerarFaturas();

        void GerarMensalidades();

        bool GerarMensalidadeMembro(Usuario usu, Membro membro, DateTime dataVencimento, TipoMovimentacao tipoPagamento);

        DateTime TrocaPlano(Usuario usu, Membro membro, DateTime dataVencimento, TipoMovimentacao tipoPagamento);

        bool CancelarPlano(int mensalidadeId);

        bool VerificaPlanoMembro(int idPessoa);

        bool VerificaFaturasFornecedor(int idPessoa);

        NotificationsResponse ReturnStatusFatura(string notification);
    }
}
