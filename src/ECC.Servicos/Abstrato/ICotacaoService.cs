using System.Collections.Generic;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeStatus;
using ECC.EntidadeUsuario;

namespace ECC.Servicos.Abstrato
{
   public  interface ICotacaoService
   {
       Cotacao BuscarCotacao(int cotacaoID); 
       StatusSistema BuscaStatusSistema(int pWorkflowStatusId, int pOrdem);
       List<CotacaoPedidos> ListarCotacaoPedidos();
       List<Pedido> ListarPedidos(int intOrdem);
       List<ResultadoCotacao> ListarResultadoCotacao(int cotacaoID);
       List<Fornecedor> ListaFornecedoresPorMembro(int MembroID);
       void SalvarPedido(Pedido pedidoSalvar);
       void SalvarItemPedido(ItemPedido itemPedidoSalvar);
       void AtualizarCotacao(Cotacao cotacao);
       void InserirHistoricoCotacao(HistStatusCotacao histCotacao);
       void InserirHistoricoPedido(HistStatusPedido histPedido);

       void EmailNotificacaoFornecedor(List<Usuario> fornecedores, int CotacaoId);

       void SmsNotificacaoFornecedor(List<Usuario> fornecedores, int CotacaoId);
       
   }
}
