using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadeRobos;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.Servicos;
using ECC.Servicos.Util;
using ECC.EntidadeUsuario;
using ECC.EntidadeSms;
using ECC.EntidadeCotacao;

namespace ECC.SrvWin.NotificacoesAlertas
{
    public partial class NotificacoesAlertas : ServiceBase
    {

        #region Private Variables

        private Timer _timerNotificacaoAlertas;

        private enum TipoCliente
        {
            Membro = 1,
            Fornecedor = 2
        };


        #endregion Private Variables


        #region Servico e Timer Operacoes

        public NotificacoesAlertas()
        {

            InitializeComponent();

            if (!EventLog.SourceExists("ECJ_WS04"))
            {
                EventLog.CreateEventSource("ECJ_WS04", "ECJ_WS04Log");
            }
            eventLog1.Source = "ECJ_WS04";
            eventLog1.Log = "ECJ_WS04Log";

#if (DEBUG)
            OnStart(null);
#endif
        }

        protected override void OnStart(string[] args)
        {
            _timerNotificacaoAlertas = new Timer(TimeSpan.FromSeconds(Convert.ToInt32(ParametrosAppConfig.TimerNotificacoesAlertas)).TotalMilliseconds)
            {
                Enabled = true
            };
            eventLog1.WriteEntry("EconomizaJa_WSAlertas - Serviço de Notificações Alertas Iniciado", EventLogEntryType.Information, 1);

#if (!DEBUG)
            _timerNotificacaoAlertas.Elapsed += _timerNotificacaoAlertas_Elapsed;
            _timerNotificacaoAlertas.Start();
#else
            this.fctNotificacaoAlertas();
#endif

        }

        protected override void OnStop()
        {
            if (_timerNotificacaoAlertas != null)
            {
                _timerNotificacaoAlertas.Stop();
                _timerNotificacaoAlertas.Dispose();
            }
            eventLog1.WriteEntry("EconomizaJa_WSNotificacaoAlerta - Serviço de Notificação Alerta parado", EventLogEntryType.Information, 20);
        }

        private void _timerNotificacaoAlertas_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                eventLog1.WriteEntry("ECJ_WS04 - Chamando classe fctNotificacaoAlertas ", EventLogEntryType.Information, 1);
                this.fctNotificacaoAlertas();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("ECJ_WS04 - Erro Chamando classe FecharCotacao " + ex.Message, EventLogEntryType.Error, 156);
            }
        }

        #endregion Servico e Timer Operacoes


        #region Metodos
        private void fctNotificacaoAlertas()
        {
            NotificacoesAlertasService NAservice = new NotificacoesAlertasService();
            TipoAvisos objTipoAviso = new TipoAvisos();

            //Seta usuário Padrão de Robô
            SessaoEconomiza.UsuarioId = 17;

            //Valida Horario Util
            //das 9h as 17h
            int HoraUtilInicio = 9;
            int HoraUtilFim = 17;
            int horaAtual = DateTime.Now.Hour;
            if (horaAtual >= HoraUtilInicio && horaAtual <= HoraUtilFim && (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.DayOfWeek != DayOfWeek.Saturday))
            {
                //Trata Aceites dos Pedidos/Cotacoes
                eventLog1.WriteEntry("Inicio PedidoPendentedeAceiteMembro.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.PedidoPendentedeAceiteMembro);
                TrataPedidosPendentesAceiteMembro(objTipoAviso);

                eventLog1.WriteEntry("Inicio PedidoPendentedeAceiteFornecedor.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.PedidoPendentedeAceiteFornecedor);
                TrataPedidosPendentesAceiteFornecedor(objTipoAviso);

                ////Novo Fornecedor Cadastrado, aviso para membros.
                eventLog1.WriteEntry("Inicio NovoFornecedorAvisoMembro.", EventLogEntryType.Information);
                objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.NovoFornecedorAvisoMembro);
                NovoFornecedorAvisoMembro(objTipoAviso);

                ////Aceite de Fornecedor para Trabalhar com Membr/*os*/
                //eventLog1.WriteEntry("Inicio PendentedeAceiteFornecedorMembro.", EventLogEntryType.Information);
                //objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.PendentedeAceiteFornecedorMembro);
                //TrataAceiteFornecedorParaMembro(objTipoAviso);

                //Valida Prazo de Entrega do Pedido.


                ////Valida Novas Cotações, se Fornecedor ainda não deu o Lance.
                //eventLog1.WriteEntry("Inicio NovaCotacao.", EventLogEntryType.Information);
                //objTipoAviso = NAservice.GetTipoAviso((int)TipoAviso.NovaCotacao);
                //TrataFornecedorNovaCotacao(objTipoAviso);
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
                                "Economiza Já - Aguardando Aprovação de Pedido " + item.Id + ".",
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
                ///NAService.LimparTabelAvisosPorMembro(listaPedidoMembro, pTipoAviso.Id);

            }
            catch (Exception)
            {

                throw;
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
            catch (Exception)
            {

                throw;
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
                            corpoEmail = template.Replace("#NomeMembro#", item.Membro.Pessoa.PessoaJuridica.NomeFantasia);

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
            catch (Exception)
            {

                throw;
            }
        }

        private void TrataFornecedorNovaCotacao(TipoAvisos pTipoAviso)
        {
            try
            {
                NotificacoesAlertasService NAService = new NotificacoesAlertasService();
                EmailService EmailSrv = new EmailService();
                SmsService SmsSrv = new SmsService();


                List<EntidadeCotacao.Cotacao> listaPedidoMembro = NAService.ListarCotacoesPendentes();

                foreach (EntidadeCotacao.Cotacao item in listaPedidoMembro)
                {

                    List<RetornoAvisos> listaUsuario = NAService.TrataNovaCotacaoFornecedor(item, pTipoAviso.Id);

                    foreach (RetornoAvisos retAviso in listaUsuario)
                    {
                        //Enviar Email
                        if (retAviso.EnviarEmail)
                        {
                            EmailSrv.EnviarEmailViaRobo(
                                NAService.UsuarioRobo(),
                                "Pendência dar lance na Cotação",
                                retAviso.Usuario.UsuarioEmail,
                                "corpoemail Pendente dar lance na Cotação",
                                Origem.NovaCotacao);
                        }

                        //Enviar SMS
                        if (retAviso.EnviarSMS)
                        {
                            SmsSrv.EnviaSms(retAviso.Usuario.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + retAviso.Usuario.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                "Economiza Já - Cotação aguardnado seu lance.",
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
                        ////            URLPaginaDestino = "/#/cotacoes",
                        ////            TituloAviso = "Cotação Pendente de Lance",
                        ////            ToolTip = "Cotação Pendente de Lance",
                        ////            DescricaoAviso = "Cotação Pendente de Lance " + item.Id,
                        ////            ModuloId = pTipoAviso.ModuloId, //Modulo Membro
                        ////            UsuarioNotificadoId = retAviso.Usuario.Id
                        ////        });
                        ////}
                    }

                }

                NAService.LimparTabelAvisosPorNovasCotacoesFornecedor(listaPedidoMembro, pTipoAviso.Id);

            }
            catch (Exception)
            {

                throw;
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
                                DescricaoAviso = "Novos fornecedores cadastros em seu segmento",
                                ModuloId = pTipoAviso.ModuloId, //Modulo Membro
                                UsuarioNotificadoId = retAviso.Usuario.Id
                            });
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

       

        #endregion Metodos

    }
}
