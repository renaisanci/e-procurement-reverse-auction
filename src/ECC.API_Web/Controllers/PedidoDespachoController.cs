using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using ECC.API_Web.Hubs;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Data.SqlClient;
using System.Data;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Requests;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.EntidadeStatus;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeSms;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.Servicos;
using ECC.Entidades.EntidadePessoa;
using ECC.EntidadeParametroSistema;
using ECC.Servicos.ModelService;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/pedidoDespacho")]
    public class PedidoDespachoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<HistStatusPedido> _histStatusPedido;
        private readonly IEntidadeBaseRep<HistStatusCotacao> _histStatusCotacao;
        private readonly IEntidadeBaseRep<Produto> _produtoRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedorRep;
        private readonly IEntidadeBaseRep<StatusSistema> _statusSisRep;
        private readonly IEntidadeBaseRep<ItemPedido> _itemPedidoRep;
        private readonly IEntidadeBaseRep<AvaliacaoFornecedor> _avaliacaoFornecedorRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IEntidadeBaseRep<Emails> _emailRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmail;
        private readonly IEntidadeBaseRep<FornecedorRegiao> _fornecedorRegiaoRep;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IUtilService _ultilService;
        private readonly ICalendarioFeriadoService _calendarioFeriadoService;
        private readonly INotificacoesAlertasService _notificacacoesAlertasService;
        private readonly IPagamentoService _pagamentoService;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<RemoveFornPedido> _removeFornPedidoRep;
        private readonly IEntidadeBaseRep<IndisponibilidadeProduto> _indisponibilidadeProdutoRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacaoRep;



        public PedidoDespachoController(
            IEntidadeBaseRep<Produto> produtoRep,
            IUtilService ultilService,
            IEntidadeBaseRep<StatusSistema> statusSisRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep,
            IEntidadeBaseRep<Pedido> pedidoRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<HistStatusPedido> histStatusPedido,
            IEntidadeBaseRep<HistStatusCotacao> histStatusCotacao,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<ItemPedido> itemPedidoRep,
            IEntidadeBaseRep<AvaliacaoFornecedor> avaliacaoFornecedorRep,
            IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
            IEntidadeBaseRep<Cotacao> cotacaoRep,
            IEntidadeBaseRep<Sms> smsRep,
            IEntidadeBaseRep<Emails> emailRep,
            IEntidadeBaseRep<TemplateEmail> templateEmail,
            IEntidadeBaseRep<FornecedorRegiao> fornecedorRegiaoRep,
            IEntidadeBaseRep<ProdutoPromocional> produtoPromocionalRep,
            IEntidadeBaseRep<Endereco> enderecoRep,
            IPagamentoService pagamentoService,
            IEntidadeBaseRep<Avisos> avisosRep,
            IEntidadeBaseRep<RemoveFornPedido> removeFornPedidoRep,
            IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
            IEntidadeBaseRep<IndisponibilidadeProduto> indisponibilidadeProdutoRep,
            IEntidadeBaseRep<ResultadoCotacao> resultadoCotacaoRep,
 

        ICalendarioFeriadoService calendarioFeriadoService,
            INotificacoesAlertasService notificacoesAlertasService,
        IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _pedidoRep = pedidoRep;
            _fornecedorRep = fornecedorRep;
            _histStatusPedido = histStatusPedido;
            _histStatusCotacao = histStatusCotacao;
            _membroRep = membroRep;
            _membroFornecedorRep = membroFornecedorRep;
            _emailRep = emailRep;
            _statusSisRep = statusSisRep;
            _ultilService = ultilService;
            _produtoRep = produtoRep;
            _itemPedidoRep = itemPedidoRep;
            _avaliacaoFornecedorRep = avaliacaoFornecedorRep;
            _membroCategoriaRep = membroCategoriaRep;
            _cotacaoRep = cotacaoRep;
            _smsRep = smsRep;
            _templateEmail = templateEmail;
            _fornecedorRegiaoRep = fornecedorRegiaoRep;
            _enderecoRep = enderecoRep;
            _pagamentoService = pagamentoService;
            _avisosRep = avisosRep;
            _removeFornPedidoRep = removeFornPedidoRep;
            _parametroSistemaRep = parametroSistemaRep;
            _notificacacoesAlertasService = notificacoesAlertasService;
            _indisponibilidadeProdutoRep = indisponibilidadeProdutoRep;
            _calendarioFeriadoService = calendarioFeriadoService;
            _resultadoCotacaoRep = resultadoCotacaoRep;
        }

 

 

 

        [HttpPost]
        [Route("itensPedidoDespachado")]
        public HttpResponseMessage ItensPedidoDespachado(HttpRequestMessage request, List<ItemPedidoViewModel> itenspedidoVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var idPedido = itenspedidoVm?.FirstOrDefault().PedidoId;
                var pedido = _pedidoRep.FirstOrDefault(x => x.Id == idPedido);

                //Pegamos os ítens de pedidos que foram aprovados.
                var itensPedido = pedido.ItemPedidos
                    .Where(x => x.AprovacaoFornecedor && x.Ativo && x.FornecedorId == fornecedor.Id).ToList();


                foreach (var item in itensPedido)
                {
                    item.UsuarioAlteracao = usuario;
                    item.DataEntregaFornecedor = DateTime.Now;
                    item.FlgDespacho = true;
                    _itemPedidoRep.Edit(item);
                }

                _ultilService.EnviaEmailPedido(pedido.Id, 6, usuario);

                //pega o telefone do usuário q criou o pedido
                var usuariosMembro = pedido.UsuarioCriacao;

                Sms sms = new Sms()
                {
                    UsuarioCriacao = usuario,
                    DtCriacao = DateTime.Now,
                    Numero = usuariosMembro. Telefones.Select(t => t.DddCel).FirstOrDefault() + usuariosMembro.Telefones.Select(t => t.Celular).FirstOrDefault(),
                    Mensagem = "Economiza Já-Fornecedor Despachou para Entrega itens do seu pedido " + pedido.Id,
                    Status = StatusSms.NaoEnviado,
                    OrigemSms = TipoOrigemSms.FornecedorDespachoItensPedido,
                    Ativo = true

                };

                _smsRep.Add(sms);

                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true, pedidoId = pedido.Id });

                return response;
            });
        }


     


   


    }
}
