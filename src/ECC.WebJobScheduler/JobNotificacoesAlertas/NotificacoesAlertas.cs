using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeSms;
using ECC.Servicos;
using ECC.Servicos.Util;
using Quartz;

namespace ECC.WebJobScheduler.JobNotificacoesAlertas
{
    public class NotificacoesAlertas : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS04";
        private const string LogName = "ECJ_WS04Log";
        private EventLog eventLog = new EventLog();

        private enum TipoCliente
        {
            Membro = 1,
            Fornecedor = 2
        };

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
                eventLog.WriteEntry("Serviço de notificações e alertas iniciado", EventLogEntryType.Information, 1);

                //Chamar método para enviar notificações e alertas
                fctNotificacaoAlertas();

            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Erro ao chamar método para enviar notificações e alertas: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion


        #region Métodos

        private void fctNotificacaoAlertas()
        {
            NotificacoesAlertasService NAservice = new NotificacoesAlertasService();
            TipoAvisos objTipoAviso = new TipoAvisos();

            //Seta usuário Padrão de Robô
            SessaoEconomiza.UsuarioId = int.Parse(ConfigurationManager.AppSettings["usuarioPadraoRobo"]);



            //Valida Horario Util
            //das 9h as 17h
            int HoraUtilInicio = 9;
            int HoraUtilFim = 17;
            int horaAtual = DateTime.Now.Hour;

            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
            {
                ////Valida Novas Cotações, se Fornecedor ainda não deu o Lance.
                eventLog.WriteEntry("Inicio NovaCotacao.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.NovaCotacao);
                TrataFornecedorNovaCotacao(objTipoAviso);
            }


            if (horaAtual >= HoraUtilInicio && horaAtual <= HoraUtilFim && (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.DayOfWeek != DayOfWeek.Saturday))
            {
                //Trata Aceites dos Pedidos/Cotacoes
                eventLog.WriteEntry("Inicio PedidoPendentedeAceiteMembro.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.PedidoPendentedeAceiteMembro);
                TrataPedidosPendentesAceiteMembro(objTipoAviso);

                eventLog.WriteEntry("Inicio PedidoPendentedeAceiteFornecedor.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.PedidoPendentedeAceiteFornecedor);
                TrataPedidosPendentesAceiteFornecedor(objTipoAviso);

                ////Novo Fornecedor Cadastrado, aviso para membros.
                eventLog.WriteEntry("Inicio NovoFornecedorAvisoMembro.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.NovoFornecedorAvisoMembro);
                NovoFornecedorAvisoMembro(objTipoAviso);

                // Antecipação do Pedido do Membro Agendado pelo Calendário.
                //eventLog.WriteEntry("Inicio NotificaAntecipacaoCotacaoPedidoMembro.", EventLogEntryType.Information);
                //objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.NovoFornecedorAvisoMembro);
                //AntecipacaoCotacaoPedidoMembro(objTipoAviso);


                ////Aceite de Fornecedor para Trabalhar com Membr/*os*/
                //eventLog.WriteEntry("Inicio PendentedeAceiteFornecedorMembro.", EventLogEntryType.Information);
                //objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.PendentedeAceiteFornecedorMembro);
                //TrataAceiteFornecedorParaMembro(objTipoAviso);

                //Valida Prazo de Entrega do Pedido.

            }

        }


        private void TrataPedidosPendentesAceiteMembro(TipoAvisos pTipoAviso)
        {
            try
            {
                NotificacoesAlertasService NAService = new NotificacoesAlertasService();
                EmailService EmailSrv = new EmailService();
                SmsService SmsSrv = new SmsService();


                List<Pedido> listaPedidoMembro = NAService.ListarPedidosPendentes(TipoCliente.Membro);


                foreach (Pedido item in listaPedidoMembro)
                {

                    List<RetornoAvisos> listaUsuario = NAService.TrataPedidoMembro(item, pTipoAviso.Id);

                    foreach (RetornoAvisos retAviso in listaUsuario)
                    {
                        //Enviar Email
                        if (retAviso.EnviarEmail)
                        {
                            var template = NAService.GetCorpoEmail(22);

                            if (retAviso.Usuario.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                            {


                                var corpoEmail = template.Replace("#NomeFantasia#", retAviso.Usuario.Pessoa.PessoaJuridica.NomeFantasia);
                                EmailSrv.EnviarEmailViaRobo(
                              NAService.UsuarioRobo(),

                               "Corra aprove seu pedido " + item.Id + " valores validos por 2 dias a partir da data de retorno da cotacao, Evite o CANCELAMENTO",
                              retAviso.Usuario.UsuarioEmail,
                              corpoEmail,
                              Origem.NovaCotacao);
                            }
                            else
                            {

                                var corpoEmail = template.Replace("#NomeFantasia#", retAviso.Usuario.Pessoa.PessoaFisica.Nome);
                                EmailSrv.EnviarEmailViaRobo(
                              NAService.UsuarioRobo(),


                              "Corra aprove seu pedido " + item.Id + " valores validos por 2 dias a partir da data de retorno da cotacao, Evite o CANCELAMENTO",
                              retAviso.Usuario.UsuarioEmail,
                              corpoEmail,
                              Origem.NovaCotacao);
                            }


                        }

                        //Enviar SMS
                        if (retAviso.EnviarSMS)
                        {
                            SmsSrv.EnviaSms(retAviso.Usuario.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + retAviso.Usuario.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                // "Economiza Já - Aguardando Aprovação de Pedido " + item.Id + ".",

                                "Economiza Já-Corra aprove seu pedido " + item.Id + " valores validos por 2 dias a partir da data de retorno da cotacao, Evite o CANCELAMENTO",
                                TipoOrigemSms.PedidosPendentesAprovacaoMembro);
                        }

                        if (retAviso.EnviarEmail || retAviso.EnviarSMS)
                        {
                            NAService.AtualizaDataAviso(retAviso.Aviso, NAService.UsuarioRobo().Id);
                        }

                        //Grava Aviso
                        ////if (retAviso.NovoAviso)
                        ////{

                        ////    NAService.AddAvisos(
                        ////        new Avisos()
                        ////        {
                        ////            Ativo = true,
                        ////            //Pedido = item,
                        ////            IdReferencia = item.Id,
                        ////            DataUltimoAviso = DateTime.Now,
                        ////            ExibeNaTelaAvisos = true,
                        ////            TipoAvisosId = pTipoAviso.Id,
                        ////            URLPaginaDestino = "/#/meusPedidos",
                        ////            TituloAviso = "Aceite Pendente",
                        ////            ToolTip = "",
                        ////            DescricaoAviso = "Aceite Pendente do pedido " + item.Id,
                        ////            ModuloId = 3, //Modulo Membro
                        ////            UsuarioNotificadoId = retAviso.Usuario.Id
                        ////        });
                        ////}
                    }

                }
                //testar melhor esse trecho para limpar avisos.
                NAService.LimparTabelAvisosPorMembro(listaPedidoMembro, pTipoAviso.Id);

            }
            catch (Exception ex)
            {

                throw new Exception("Erro TrataPedidosPendentesAceiteMembro", ex);
            }


        }

        private void TrataPedidosPendentesAceiteFornecedor(TipoAvisos pTipoAviso)
        {
            try
            {
                NotificacoesAlertasService NAService = new NotificacoesAlertasService();
                EmailService EmailSrv = new EmailService();
                SmsService SmsSrv = new SmsService();
                List<RetornoAvisos> retornoAvisos = new List<RetornoAvisos>();
                List<Fornecedor> listaFornecedoresUsuarios = new List<Fornecedor>();


                List<Pedido> listaPedidoFornecedor = NAService.ListarPedidosPendentes(TipoCliente.Fornecedor);

                foreach (Pedido item in listaPedidoFornecedor)
                {

                    listaFornecedoresUsuarios = NAService.ListaFornecedorPedido(item);

                    foreach (Fornecedor itemFornecedor in listaFornecedoresUsuarios.Distinct())
                    {
                        retornoAvisos = NAService.TrataPedidoFornecedor(item, itemFornecedor, pTipoAviso.Id);
                        foreach (RetornoAvisos retAviso in retornoAvisos)
                        {
                            //Enviar Email
                            if (retAviso.EnviarEmail)
                            {
                                var template = NAService.GetCorpoEmail(18);
                                var corpoEmail = template.Replace("#NomeFantasia#", retAviso.Usuario.Pessoa.PessoaJuridica.NomeFantasia);

                                EmailSrv.EnviarEmailViaRobo(
                                    NAService.UsuarioRobo(),
                                    "Pendência Aceite do Pedido " + item.Id,
                                    retAviso.Usuario.UsuarioEmail,
                                    corpoEmail,
                                    Origem.NovaCotacao);
                            }

                            //Enviar SMS
                            if (retAviso.EnviarSMS)
                            {
                                SmsSrv.EnviaSms(retAviso.Usuario.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + retAviso.Usuario.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                    "Economiza Já - Pedido. Aguardando Aprovação do Pedido " + item.Id + ".",
                                    TipoOrigemSms.PedidosPendentesAprovacaoMembro);
                            }

                            if (retAviso.EnviarEmail || retAviso.EnviarSMS)
                            {
                                NAService.AtualizaDataAviso(retAviso.Aviso, NAService.UsuarioRobo().Id);
                            }

                            //Grava Aviso
                            ////if (retAviso.NovoAviso)
                            ////{

                            ////    NAService.AddAvisos(
                            ////        new Avisos()
                            ////        {
                            ////            Ativo = true,
                            ////            //Pedido = item,
                            ////            IdReferencia = item.Id,
                            ////            DataUltimoAviso = DateTime.Now,
                            ////            ExibeNaTelaAvisos = true,
                            ////            TipoAvisosId = pTipoAviso.Id,
                            ////            //TipoAviso = pTipoAviso,
                            ////            URLPaginaDestino = "/#/pedidos",
                            ////            TituloAviso = "Aceite Pendente",
                            ////            ToolTip = "",
                            ////            DescricaoAviso = "Aceite Pendente do pedido " + item.Id, //email 18
                            ////            ModuloId = 4, //Modulo Fornecedor
                            ////            UsuarioNotificadoId = retAviso.Usuario.Id
                            ////        });
                            ////}
                        }
                    }
                }

                //Limpa tabela de Avisos 
                NAService.LimparTabelAvisosPorFornecedor(listaPedidoFornecedor, pTipoAviso.Id);

            }
            catch (Exception ex)
            {

                throw new Exception("Erro TrataPedidosPendentesAceiteFornecedor", ex);
            }


        }

        private void TrataAceiteFornecedorParaMembro(TipoAvisos pTipoAviso)
        {
            try
            {
                NotificacoesAlertasService NAService = new NotificacoesAlertasService();
                EmailService EmailSrv = new EmailService();
                SmsService SmsSrv = new SmsService();
                List<RetornoAvisos> retornoAvisos = new List<RetornoAvisos>();

                //Recupera fornecedores Que precisam dar o Aceite para solicitacao do Membro
                List<MembroFornecedor> listaMembroFornecedor = NAService.ListaPendenciasAceiteFornecedorParaMembro();


                foreach (MembroFornecedor item in listaMembroFornecedor)
                {
                    //Trata Permissão de Envio se Fornecedor quer receber essas informações por Email/SMS
                    retornoAvisos = NAService.TrataPendenciasAceiteFornecedorParaMembro(item, pTipoAviso.Id);
                    foreach (RetornoAvisos retAviso in retornoAvisos)
                    {
                        //Enviar Email
                        if (retAviso.EnviarEmail)
                        {
                            var template = NAService.GetCorpoEmail(9);
                            var corpoEmail = template.Replace("#NomeFornecedor#", retAviso.Usuario.Pessoa.PessoaJuridica.NomeFantasia);


                            if (item.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                            {
                                corpoEmail = template.Replace("#NomeMembro#", item.Membro.Pessoa.PessoaJuridica.NomeFantasia);
                            }
                            else
                            {
                                corpoEmail = template.Replace("#NomeMembro#", item.Membro.Pessoa.PessoaFisica.Nome);

                            }

                            EmailSrv.EnviarEmailViaRobo(
                                NAService.UsuarioRobo(),
                                "Pendência Aceite dos Pedidos",
                                retAviso.Usuario.UsuarioEmail,
                                corpoEmail,
                                Origem.NovaCotacao);
                        }

                        //Enviar SMS
                        if (retAviso.EnviarSMS)
                        {
                            SmsSrv.EnviaSms(retAviso.Usuario.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + retAviso.Usuario.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                "Economiza Já - Pedido. Aguardando Aprovação dos Pedidos.",
                                TipoOrigemSms.PedidosPendentesAprovacaoMembro);
                        }

                        //Grava Aviso
                        ////if (retAviso.NovoAviso)
                        ////{

                        ////    NAService.AddAvisos(
                        ////        new Avisos()
                        ////        {
                        ////            Ativo = true,
                        ////            //Pedido = item,
                        ////            IdReferencia = item.MembroId,
                        ////            DataUltimoAviso = DateTime.Now,
                        ////            ExibeNaTelaAvisos = true,
                        ////            TipoAvisosId = pTipoAviso.Id,
                        ////            //TipoAviso = pTipoAviso,
                        ////            URLPaginaDestino = "/#/membro",
                        ////            TituloAviso = "Pendente aceite novo Membro",
                        ////            ToolTip = "Novo Membro",
                        ////            DescricaoAviso = ("Pendente aceite novo Membro " + item.Membro.Pessoa.PessoaJuridica.NomeFantasia).Substring(0,99),
                        ////            ModuloId = 4, //Modulo Fornecedor
                        ////            UsuarioNotificadoId = retAviso.Usuario.Id
                        ////        });
                        ////}
                    }
                }

                //Limpa tabela de avisos solicitação novo membro para fornecedor
                NAService.LimparTabelAvisosPorSolicitacaoMembroFornecedor(listaMembroFornecedor, pTipoAviso.Id);
            }
            catch (Exception ex)
            {

                throw new Exception("Erro TrataAceiteFornecedorParaMembro", ex);
            }
        }

        private void TrataFornecedorNovaCotacao(TipoAvisos pTipoAviso)
        {
            try
            {
                NotificacoesAlertasService NAService = new NotificacoesAlertasService();
                EmailService EmailSrv = new EmailService();
                SmsService SmsSrv = new SmsService();
                var _templateEmail = EmailSrv.BuscaTemplate(41);
                var qtdHoraEnvioNotificacao = Convert.ToInt32(NAService.GetParametroSistema("NOTIF_FECH_COTACAO"));

                List<EntidadeCotacao.Cotacao> listaPedidoMembro = NAService.ListarCotacoesPendentes();

                foreach (EntidadeCotacao.Cotacao item in listaPedidoMembro)
                {
                    var qtdItens = item.CotacaoPedidos.SelectMany(x => x.Pedido.ItemPedidos).Sum(q => q.Quantidade);

                    var frase = qtdItens > 1 ? $"{qtdItens} itens" : $"{qtdItens} item";

                    var mensagemSms = $"Economiza Já - Daqui a pouco a cotação {item.Id} irá encerrar, temos {frase} para você dar o preço, não deixe de faturar. Se já respondeu desconsiderar este aviso.";

                    var templateCompleto = _templateEmail.Replace("#CotacaoId#", item.Id.ToString()).Replace("#Frase#", frase);

                    List<EntidadeUsuario.Usuario> listaUsuario = NAService.UsuariosFornecedoresResponderCotacao(item, qtdHoraEnvioNotificacao);

                    foreach (EntidadeUsuario.Usuario usuario in listaUsuario)
                    {
                        EmailSrv.EnviarEmailViaRobo(NAService.UsuarioRobo(), $"Economiza Já -  Daqui a pouco a cotação {item.Id} irá encerrar.", usuario.UsuarioEmail, templateCompleto, Origem.NovaCotacao);

                        SmsSrv.EnviaSms(usuario.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + usuario.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            mensagemSms, TipoOrigemSms.NovaCotacao);

                    }

                }

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao enviar email ou sms", ex);
            }

        }

        private void NovoFornecedorAvisoMembro(TipoAvisos pTipoAviso)
        {
            try
            {
                NotificacoesAlertasService NAService = new NotificacoesAlertasService();
                EmailService EmailSrv = new EmailService();
                SmsService SmsSrv = new SmsService();

                DateTime DtExec = NAService.UltimaExecucaoRotina(pTipoAviso.Id, "NotificacoesAlertas");

                List<RetornoAvisos> listaUsuario = NAService.VerificaNovoFornecedorCadastrado(pTipoAviso.Id, DtExec);
                NAService.GravaExecucaoRotina(pTipoAviso.Id, "NotificacoesAlertas");

                foreach (RetornoAvisos retAviso in listaUsuario)
                {
                    //Enviar Email
                    if (retAviso.EnviarEmail)
                    {
                        EmailSrv.EnviarEmailViaRobo(
                            NAService.UsuarioRobo(),
                            "Novos fornecedores",
                            retAviso.Usuario.UsuarioEmail,
                            "corpoemailNovos Fornecedores para se segmento",
                            Origem.NovaCotacao);
                    }

                    //Enviar SMS
                    if (retAviso.EnviarSMS)
                    {
                        SmsSrv.EnviaSms(retAviso.Usuario.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + retAviso.Usuario.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            "Economiza Já - Novos Fornecedores foram cadastrados para seu segmento.",
                            TipoOrigemSms.PedidosPendentesAprovacaoMembro);
                    }

                    if (retAviso.EnviarEmail || retAviso.EnviarSMS)
                    {
                        NAService.AtualizaDataAviso(retAviso.Aviso, NAService.UsuarioRobo().Id);
                    }

                    //Grava Aviso
                    if (retAviso.NovoAviso)
                    {

                        NAService.AddAvisos(
                            new Avisos()
                            {
                                Ativo = true,
                                //Pedido = item,
                                IdReferencia = retAviso.Usuario.PessoaId,
                                DataUltimoAviso = DateTime.Now,
                                ExibeNaTelaAvisos = true,
                                TipoAvisosId = pTipoAviso.Id,
                                URLPaginaDestino = "/#/fornecedor",
                                TituloAviso = "Novos Fornecedores",
                                ToolTip = "Novos Fornecedores",
                                DescricaoAviso = "Corre envie uma solicitação de cadastro de compra",
                                ModuloId = pTipoAviso.ModuloId, //Modulo Membro
                                UsuarioNotificadoId = retAviso.Usuario.Id
                            });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro NovoFornecedorAvisoMembro", ex);

            }
        }

        private void AntecipacaoCotacaoPedidoMembro(TipoAvisos pTipoAviso)
        {
            NotificacoesAlertasService NAService = new NotificacoesAlertasService();
            EmailService EmailSrv = new EmailService();
            SmsService SmsSrv = new SmsService();
            var _templateEmail = EmailSrv.BuscaTemplate(41);

            var listUsuariosPedido = NAService.AntecipacaoPedidoMembro();

            listUsuariosPedido.ForEach(x =>
            {

                var dataCotacaoHoje = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);

                EmailSrv.EnviarEmailViaRobo(NAService.UsuarioRobo(), $"Economiza Já -  Adiantamos o pedido {x.Key} para {dataCotacaoHoje}.", x.Value.UsuarioEmail, _templateEmail, Origem.NovaCotacao);

                var telefone = $"{x.Value.Telefones.FirstOrDefault(t => t.Ativo).DddCel}{x.Value.Telefones.FirstOrDefault(t => t.Ativo).Celular}";
                var mensagemSms = $"Economiza Já - Adiantamos a cotação do pedido {x.Key} para {dataCotacaoHoje}";

                SmsSrv.EnviaSms(telefone, mensagemSms, TipoOrigemSms.NovaCotacao);

            });
        }

        #endregion
    }
}