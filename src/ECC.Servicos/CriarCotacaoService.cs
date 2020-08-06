using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeAvisos;
using ECC.EntidadeCotacao;
using ECC.EntidadeParametroSistema;
using ECC.EntidadePedido;
using ECC.EntidadeStatus;
using ECC.Servicos.Abstrato;
using ECC.Servicos.ModelService;


namespace ECC.Servicos
{
    public class CriarCotacaoService : IGeraCotacao
    {

        #region Variáveis
        private readonly IEntidadeBaseRep<StatusSistema> _statusSisRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<HistStatusCotacao> _histStatusCotacaoRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Propriedades

        public TimeSpan HoraCotacao { get; set; }

        #endregion

        #region Contrutor

        public CriarCotacaoService() : this(new DbFactory()) { }

        public CriarCotacaoService(DbFactory dbFactory)
            : this(
                new EntidadeBaseRep<StatusSistema>(dbFactory),
                new EntidadeBaseRep<Pedido>(dbFactory),
                new EntidadeBaseRep<HistStatusCotacao>(dbFactory),
                new EntidadeBaseRep<Cotacao>(dbFactory),
                new EntidadeBaseRep<ParametroSistema>(dbFactory),
                new EntidadeBaseRep<Avisos>(dbFactory),
                new UnitOfWork(dbFactory))
        { }

        public CriarCotacaoService(
                IEntidadeBaseRep<StatusSistema> statusSisRep,
                IEntidadeBaseRep<Pedido> pedidoRep,
                IEntidadeBaseRep<HistStatusCotacao> histStatusCotacaoRep,
                IEntidadeBaseRep<Cotacao> cotacaoRep,
                IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
                IEntidadeBaseRep<Avisos> avisosRep,
                IUnitOfWork unitOfWork)
        {
            _statusSisRep = statusSisRep;
            _pedidoRep = pedidoRep;
            _histStatusCotacaoRep = histStatusCotacaoRep;
            _cotacaoRep = cotacaoRep;
            _parametroSistemaRep = parametroSistemaRep;
            _avisosRep = avisosRep;
            _unitOfWork = unitOfWork;
            //this.HoraCotacao = Convert.ToDateTime(_parametroSistemaRep.FirstOrDefault(x => x.Codigo == "HORA_COTACAO").Valor).TimeOfDay;
        }
        #endregion

        #region Métodos
        public GeraCotacaoViewModel GeraCotacaoPedidos()
        {
            GeraCotacaoViewModel cotacao = new GeraCotacaoViewModel();

            var statusId = _statusSisRep.FirstOrDefault(x => x.WorkflowStatusId == 14 && x.Ordem == 1).Id;
            var qtdPedidos = _pedidoRep.GetAll().Count(x => !x.FlgCotado && x.DtCotacao <= DateTime.Now);
           

            if (qtdPedidos > 0)
            {
                var pOut = new SqlParameter
                {
                    ParameterName = "@cotacaoId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                _pedidoRep.ExecuteWithStoreProcedure("stp_upd_pedidos_sem_cotar @cotacaoId out", pOut);


                //Resgata ID da cotação inserida
                var cotacaoID = Convert.ToInt32(pOut.Value);

                //Historico de Cotação
                var cotacaoHistorico = new HistStatusCotacao
                {
                    Ativo = true,
                    CotacaoId = cotacaoID,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacaoId = 1,
                    StatusSistemaId = statusId
                };
                _histStatusCotacaoRep.Add(cotacaoHistorico);

                var cot = _cotacaoRep.GetSingle(cotacaoID);
                var naService = new NotificacoesAlertasService();
                var listaUsuario = naService.TrataNovaCotacaoFornecedor(cot, (int)TipoAviso.NovaCotacao);

                foreach (var retAviso in listaUsuario)
                {
                    var aviso = new Avisos
                    {
                        UsuarioCriacaoId = 1,
                        DtCriacao = DateTime.Now,
                        Ativo = true,
                        IdReferencia = cot.Id,
                        DataUltimoAviso = DateTime.Now,
                        ExibeNaTelaAvisos = true,
                        TipoAvisosId = (int)TipoAviso.NovaCotacao, //Id TipoAviso para esse aviso
                        URLPaginaDestino = "/#/cotacoes",
                        TituloAviso = "Cotação Pendente de Resposta",
                        ToolTip = "Cotação Pendente de Resposta",
                        DescricaoAviso = "Cotação " + cot.Id + " Pendente de Resposta",
                        ModuloId = 4, //Modulo Fornecedor
                        UsuarioNotificadoId = retAviso.Usuario.Id

                    };

                    _avisosRep.Add(aviso);
                }

                _unitOfWork.Commit();

                var usuarios = listaUsuario.Select(x => x.Usuario).Distinct();
                var listaToken = usuarios.Select(x => x.TokenSignalR).ToList();

                cotacao.CotacaoId = cot.Id;
                cotacao.ListaTokenUsuarios = listaToken;
                
                return cotacao;
            }

            return cotacao;

        }

        #endregion

    }

}
