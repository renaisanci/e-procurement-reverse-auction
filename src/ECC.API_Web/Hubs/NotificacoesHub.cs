using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeUsuario;
using ECC.Servicos.ModelService;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ECC.API_Web.Hubs
{
    [HubName("Notificacoes")]
    public class NotificacoesHub : Hub
    {
        private readonly IHubContext _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public NotificacoesHub() : this(new UnitOfWork(new DbFactory()))
        {

        }

        public NotificacoesHub(IUnitOfWork unitOfWork)
        {
            this._hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacoesHub>();
            this._unitOfWork = unitOfWork;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var usuarioRep = new EntidadeBaseRep<Usuario>(this._unitOfWork.DbFactory);

            if (stopCalled)
            {
                // Sabemos que Stop () foi chamado no cliente,
                // ea conexão desligou  
                var usuario = usuarioRep.FirstOrDefault(x => x.TokenSignalR == Context.ConnectionId);

                if (usuario != null)
                {
                    usuario.TokenSignalR = "";
                    usuario.Logado = false;
                    usuario.DtUsuarioSaiu = DateTime.Now;
                    usuarioRep.EditComCommit(usuario);
                }
                Thread.Sleep(90000);
            }
            else
            {
                // Este servidor não ouviu falar do cliente nos últimos ~ 35 segundos.
                // Se o SignalR estiver atrás de um balanceador de carga com escalabilidade configurada,
                // o cliente ainda pode estar conectado a outro servidor SignalR.
                var usuario = usuarioRep.FirstOrDefault(x => x.TokenSignalR == Context.ConnectionId);



                if (usuario != null)
                {
                    usuario.TokenSignalR = "";
                    usuario.Logado = false;
                    usuario.DtUsuarioSaiu = DateTime.Now;
                    usuarioRep.EditComCommit(usuario);
                }
                Thread.Sleep(90000);

            }

            return base.OnDisconnected(stopCalled);

        }

        public void NotificaNovaCotacao(GeraCotacaoViewModel cotacao)
        {
            var lista = cotacao.ListaTokenUsuarios;
            this._hubContext.Clients.Clients(lista).notificaNovaCotacao(cotacao.CotacaoId);
        }

        public void NotificaMembroSolicitaFornecedor(string nomeMemmbro, IEnumerable<Usuario> fornecedor)
        {
            var lista = fornecedor.Select(f => f.TokenSignalR).ToList();
            this._hubContext.Clients.Clients(lista).notificaMembroSolicitaFornecedor(nomeMemmbro);
        }

        public void LembreteFornecedorAceiteMembros(List<string> usuarios)
        {
            this._hubContext.Clients.Clients(usuarios.ToArray()).lembreteFornecedorAceiteMembros();
        }

        public void NotificaFornecedorAceitouMembro(string nomeFornecedor, IEnumerable<Usuario> membro)
        {
            var lista = membro.Select(m => m.TokenSignalR).ToList();
            this._hubContext.Clients.Clients(lista).notificaFornecedorAceitouMembro(nomeFornecedor);
        }

        public void CotacaoNovoPreco(string usuarioToken, int cotacaoId, DateTime dtFechamento, object cotacaoProds, DateTime dtIni)
        {
            this._hubContext.Clients.Clients(new[] { usuarioToken }).cotacaoNovoPreco(cotacaoId, dtFechamento, cotacaoProds);
        }

        public async void CotacoesAtualizar()
        {
            await this._hubContext.Clients.All.cotacoesAtualizar();
        }

        public async void CarregaAvisosHub(IEnumerable<Usuario> usuarios)
        {
            var lista = usuarios.Select(f => f.TokenSignalR).ToList();
            await this._hubContext.Clients.Clients(lista).carregaAvisosFornecedor();
        }

        public void NotificaFornecedorNovaCotacao(int cotacaoId, List<string> tokenUsuarios)
        {
            this._hubContext.Clients.Clients(tokenUsuarios).notificaFornecedorNovaCotacao(cotacaoId);
        }

        //private void ClearDiconectaUsuario()
        //{
        //    var _usuarioRep = new EntidadeBaseRep<Usuario>(this._unitOfWork.DbFactory);
        //    _usuarioRep.ExecuteWithStoreProcedure("stp_upd_disconecta_user @connectionTokenId ",
        //                                       new System.Data.SqlClient.SqlParameter("@connectionTokenId", Context.ConnectionId) 

        //                                       );


        //}
    }
}