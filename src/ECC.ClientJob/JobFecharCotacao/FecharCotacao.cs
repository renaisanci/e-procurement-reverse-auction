using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.Servicos;
using ECC.Servicos.Util;
using Quartz;

namespace ECC.ClientJob.JobFecharCotacao
{
    public class FecharCotacao : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS03";
        private const string LogName = "ECJ_WS03Log";
        private EventLog eventLog = new EventLog();
    
        #endregion


        #region Execute

        public override void Execute(IJobExecutionContext context)
        {

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, LogName);
            }

            eventLog.Source = LogSource;
            eventLog.Log = LogName;


            try
            {
                eventLog.WriteEntry("Serviço para fechar cotação iniciado", EventLogEntryType.Information, 1);

                //Chamar método para fechar cotação
                FecharCotacaoService();

            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Erro ao chamar método para fechar cotação: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion


        #region Métodos

        private void FecharCotacaoService()
        {
            var cotacaoAtualID = 0;

            var listaFornecedoresMelhorPreco = new List<Fornecedor>();

            try
            {
                eventLog.WriteEntry("Início processamento Cotações.", EventLogEntryType.Information);
                List<Usuario> listaUsuarioDeuCotacao = new List<Usuario>();
                List<Pedido> listaMembroPedido = null;
                CotacaoPedidos cotacaoAnteior = null;

                int statusID = 0;

                //Seta usuário Padrão de Robô
                SessaoEconomiza.UsuarioId = 1;

                CotacaoService CS = new CotacaoService();
                //Consulta Cotações em aberto para processar
                List<CotacaoPedidos> listaCotacaoPedido = CS.ListarCotacaoPedidos().OrderBy(ob => ob.CotacaoId).ThenBy(tb => tb.PedidoId).ToList(); //Lista todos Pedidos das cotações Fechadas.

                //Verifica quantidade de cotacoes
                /////qtdCotacoes = listaCotacaoPedido.GroupBy(gb => gb.CotacaoId).Select(s => new { s.Key }).Count();
                /////var teste = listaCotacaoPedido.GroupBy(gb => gb.CotacaoId).Select( s=> new { t = s.Key, c = s.Count() });

                foreach (var itemCotacao in listaCotacaoPedido)
                {
                    var pedidoCotacao = itemCotacao.Pedido;
                    var resultadoCotacao = CS.ListarResultadoCotacao(itemCotacao.CotacaoId); //Lista os resultados da cotação
                    int PedidoComLance = 0;

                    if (cotacaoAnteior == null || cotacaoAnteior.CotacaoId != cotacaoAtualID)
                        cotacaoAnteior = itemCotacao;
                    cotacaoAtualID = itemCotacao.CotacaoId; //pega cotação atual para tratamento

                    //Atualiza Cotação com todos pedidos Cotados
                    if (cotacaoAtualID != cotacaoAnteior.CotacaoId)
                    {
                        fctAtualizarCotacaoEncerrada(cotacaoAnteior.CotacaoId);
                        CS.EmailNotificacaoFornecedor(listaUsuarioDeuCotacao.Distinct().ToList(), cotacaoAnteior.CotacaoId);
                        CS.SmsNotificacaoFornecedor(listaUsuarioDeuCotacao.Distinct().ToList(), cotacaoAnteior.CotacaoId);

                        listaUsuarioDeuCotacao = new List<Usuario>();
                    }
                    listaMembroPedido = new List<Pedido>();

                    if (resultadoCotacao.Count > 0)
                    {
                        foreach (ItemPedido itemPedido in pedidoCotacao.ItemPedidos) //Varre todos os pedidos da Cotação
                        {
                            decimal precoUnitario = 0;
                            var resultadoCotacaoMenorValor = resultadoCotacao.FindAll(rc => rc.ProdutoId == itemPedido.ProdutoId && rc.PrecoNegociadoUnit > 0); // Filtra único produto na Resultado Cotação que tenha o lance mairo que zero
                            if (!resultadoCotacaoMenorValor.Any())
                            {
                                itemPedido.Ativo = false; //inativa item sem lance
                                CS.SalvarItemPedido(itemPedido);
                                continue;
                            }
                            PedidoComLance++;

                            // verifica se cotaçao tem fornecedor que não atende todos os clientes
                            decimal precoMedio = 0;
                            Usuario usuarioDeuCotacao = null;
                            if (resultadoCotacao.GroupBy(rc => rc.Qtd).Select(n => new { n.Key }).Count() > 1) //Verifica se na cotação existe divisão Membro / Fornecedor
                            {
                                List<Fornecedor> LFornec = CS.ListaFornecedoresPorMembro(itemPedido.Pedido.MembroId);
                                var ValidaSeTemResultadoItemCotado = resultadoCotacaoMenorValor.Where(w =>
                                        w.Cotacao.CotacaoPedidos.Select(ww => ww.PedidoId == itemPedido.PedidoId).Any() // identifica se é pedido correto
                                        && LFornec.Contains(w.Fornecedor));

                                if (!ValidaSeTemResultadoItemCotado.Any()) //valida se algum Fornecedor vinculado ao Membro deu lance.
                                {
                                    PedidoComLance--;
                                    itemPedido.Ativo = false; //inativa Item sem lance
                                    CS.SalvarItemPedido(itemPedido);
                                    continue;
                                }

                                precoUnitario = ValidaSeTemResultadoItemCotado.Min(mp => mp.PrecoNegociadoUnit); // Pega menor valor da lista

                                precoMedio = ValidaSeTemResultadoItemCotado.Average(ap => ap.PrecoNegociadoUnit); // Pega valor médio da lista

                                itemPedido.PrecoMedioUnit = precoMedio;
                                itemPedido.PrecoNegociadoUnit = precoUnitario;

                                var objFornecedor = resultadoCotacaoMenorValor.Where(w =>
                                    w.Cotacao.CotacaoPedidos.Select(ww => ww.PedidoId == itemPedido.PedidoId).Any() // identifica se é pedido correto
                                    && LFornec.Contains(w.Fornecedor)).FirstOrDefault(x => x.PrecoNegociadoUnit == precoUnitario);

                                itemPedido.FornecedorId = objFornecedor.FornecedorId;

                                usuarioDeuCotacao = objFornecedor.UsuarioCriacao;

                                itemPedido.FornecedorUsuario = usuarioDeuCotacao;
                                itemPedido.FornecedorUsuarioId = usuarioDeuCotacao.Id;

                                //Pegando observação.
                                var itemObservacao =
                                    resultadoCotacaoMenorValor.OrderByDescending(o => o.DtCriacao)
                                        .FirstOrDefault(x => x.ProdutoId == itemPedido.ProdutoId
                                                             && !string.IsNullOrEmpty(x.Observacao)
                                                             && x.FornecedorId == itemPedido.FornecedorId);

                                itemPedido.Observacao = itemObservacao != null ? itemObservacao.Observacao : string.Empty;
                            }
                            else //Cotação com todos fornecedores atendendo todos clientes
                            {
                                precoUnitario = resultadoCotacaoMenorValor.Min(mp => mp.PrecoNegociadoUnit);
                                precoMedio = resultadoCotacaoMenorValor.Average(ap => ap.PrecoNegociadoUnit);

                                itemPedido.PrecoMedioUnit = precoMedio;
                                itemPedido.PrecoNegociadoUnit = precoUnitario;
                                itemPedido.FornecedorId = resultadoCotacaoMenorValor.FirstOrDefault().FornecedorId;

                                usuarioDeuCotacao = resultadoCotacaoMenorValor.FirstOrDefault().UsuarioCriacao;
                                itemPedido.FornecedorUsuario = usuarioDeuCotacao;
                                itemPedido.FornecedorUsuarioId = usuarioDeuCotacao.Id;

                                //Pegando observação.
                                var itemObservacao =
                                    resultadoCotacaoMenorValor.OrderByDescending(o => o.DtCriacao)
                                        .FirstOrDefault(x => x.ProdutoId == itemPedido.ProdutoId
                                                             && !string.IsNullOrEmpty(x.Observacao)
                                                             && x.FornecedorId == itemPedido.FornecedorId);

                                itemPedido.Observacao = itemObservacao != null ? itemObservacao.Observacao : string.Empty;
                            }
                            //Salva Item Pedido
                            CS.SalvarItemPedido(itemPedido);
                            listaFornecedoresMelhorPreco.Add(CS.BuscaFornecedorById(int.Parse(itemPedido.FornecedorId.ToString())));
                            listaUsuarioDeuCotacao.Add(usuarioDeuCotacao);
                        }

                        if (PedidoComLance > 0)
                            statusID = CS.BuscaStatusSistema(12, 3).Id;// Pedido com lance em algum produto
                        else
                            statusID = CS.BuscaStatusSistema(12, 9).Id; // Pedido sem lance

                    }
                    else
                    {
                        // Cotação sem Lances
                        foreach (ItemPedido itemPedido in pedidoCotacao.ItemPedidos) //Varre todos os itens do pedido desativando
                        {
                            itemPedido.Ativo = false; //inativa Item sem lance
                            CS.SalvarItemPedido(itemPedido);
                        }
                        // Apenas atualiza Status do Pedido para tratamento
                        statusID = CS.BuscaStatusSistema(12, 9).Id; // Pedido sem itens
                    }

                    pedidoCotacao.StatusSistemaId = statusID; //23
                    CS.SalvarPedido(pedidoCotacao);

                    listaMembroPedido.Add(pedidoCotacao);
                    //Inserir Histórico Pedido
                    var pedidoHistorico = new HistStatusPedido
                    {
                        Ativo = true,
                        DtCriacao = DateTime.Now,
                        Pedido = pedidoCotacao,
                        PedidoId = pedidoCotacao.Id,
                        StatusSistemaId = statusID
                    };
                    //Status do histórico
                    CS.InserirHistoricoPedido(pedidoHistorico);

                    CS.EmailNotificacaoMembro(listaMembroPedido.Distinct().ToList());
                    CS.SmsNotificacaoMembro(listaMembroPedido.Distinct().ToList());

                }//foreach (var itemCotacao in listaCotacaoPedido)

                //Atualiza ultima Cotação do laço, ou quando temos apenas uma cotação
                if (cotacaoAtualID > 0 && cotacaoAtualID == cotacaoAnteior.CotacaoId)
                {
                    fctAtualizarCotacaoEncerrada(cotacaoAtualID);
                    CS.EmailNotificacaoFornecedor(listaUsuarioDeuCotacao.Distinct().ToList(), cotacaoAtualID);
                    CS.SmsNotificacaoFornecedor(listaUsuarioDeuCotacao.Distinct().ToList(), cotacaoAtualID);

                }

                eventLog.WriteEntry("Fim processamento Cotações.", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Cotação - " + cotacaoAtualID + " > " + ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
            }
        }

        private void fctAtualizarCotacaoEncerrada(int CotacaoID)
        {
            CotacaoService CS = new CotacaoService();

            // Buscar Status da cotação ordem 4
            int statusID = CS.BuscaStatusSistema(14, 4).Id; //28

            //Atualiza Cotação
            ECC.EntidadeCotacao.Cotacao cotacaoAtualizar;
            cotacaoAtualizar = CS.BuscarCotacao(CotacaoID);
            cotacaoAtualizar.StatusSistemaId = statusID; // 28 verificar Status ID
            CS.AtualizarCotacao(cotacaoAtualizar);

            //Historico de Cotação
            HistStatusCotacao cotacaoHistorico = new HistStatusCotacao()
            {
                Ativo = true,
                Cotacao = cotacaoAtualizar,
                CotacaoId = CotacaoID,
                DtCriacao = DateTime.Now,
                StatusSistemaId = statusID // 28 verificar Status ID
            };
            CS.InserirHistoricoCotacao(cotacaoHistorico);
        }

        #endregion

    }
}