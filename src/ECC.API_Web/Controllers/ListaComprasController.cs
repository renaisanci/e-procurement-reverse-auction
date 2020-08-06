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
using ECC.EntidadeListaCompras;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Authorize(Roles = "Membro")]
    [RoutePrefix("api/listaCompras")]
    public class ListaComprasController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly ICalendarioFeriadoService _calendarioFeriadoService;
        private readonly INotificacoesAlertasService _notificacacoesAlertasService;
        private readonly IEntidadeBaseRep<ListaCompras> _listaComprasRep;
        private readonly IEntidadeBaseRep<ListaComprasItem> _listaComprasItemRep;
        private readonly IEntidadeBaseRep<ListaComprasRemoveForn> _listaComprasRemoveFornRep;



        public ListaComprasController(

            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<Usuario> usuarioRep,
          IEntidadeBaseRep<ListaCompras> listaComprasRep,
         IEntidadeBaseRep<ListaComprasItem> listaComprasItemRep,
          IEntidadeBaseRep<ListaComprasRemoveForn> listaComprasRemoveFornRep,

        ICalendarioFeriadoService calendarioFeriadoService,
            INotificacoesAlertasService notificacoesAlertasService,
        IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _membroRep = membroRep;
            _notificacacoesAlertasService = notificacoesAlertasService;
            _calendarioFeriadoService = calendarioFeriadoService;
            _listaComprasRep = listaComprasRep;
            _listaComprasItemRep = listaComprasItemRep;
            _listaComprasRemoveFornRep = listaComprasRemoveFornRep;



        }

        [HttpPost]
        [Route("inserirLista")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, InserirListaComprasViewModel listaCompras)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                if (_listaComprasRep.FindBy(x => x.NomeLista == listaCompras.NomeLista && x.Ativo  && x.UsuarioCriacaoId == usuario.Id).Any())
                {

                    ModelState.AddModelError("Lista já Existente", "Lista: " + listaCompras.NomeLista + " já existe .");
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {

               
                    var membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                    var lista = new ListaCompras
                    {
                        NomeLista = listaCompras.NomeLista,
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Ativo = true


                    };
                    _listaComprasRep.Add(lista);


                    foreach (var item in listaCompras.ListaCompras)
                    {
                        var itemLista = new ListaComprasItem
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Quantidade = item.quantity,
                            ProdutoId = item.sku,
                            Ativo = true,
                            FlgOutraMarca = item.flgOutraMarca,
                            QtdForne = item.qtdForn
                        };

                        lista.ListaComprasItens.Add(itemLista);
                    }

                    _unitOfWork.Commit();

                    foreach (var item in listaCompras.RemFornPedCot)
                    {
                        var removeFornLista = new ListaComprasRemoveForn
                        {

                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            FonecedorId = item.forn,
                            ProdutoId = item.prd,
                            ListaComprasId = lista.Id,
                            Ativo = true

                        };

                        _listaComprasRemoveFornRep.Add(removeFornLista);

                    }


                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK);

                }

                return response;
            });

        }


        [HttpGet]
        [Route("listaComprasById/{listaId:int}")]
        public HttpResponseMessage GetListaComprasById(HttpRequestMessage request, int listaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                var camImagem = ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"];

                var listaCompras = _listaComprasRep.FindBy(x => x.Id == listaId).Select(x => x.ListaComprasItens.Select(u => new
                {
                    flgOutraMarca = u.FlgOutraMarca,
                    sku = u.ProdutoId,
                    qtdForn = u.QtdForne,
                    quantity = u.Quantidade,
                    name = u.Produto.DescProduto,
                    imagem = u.Produto.Imagens.Count > 0 ? camImagem + u.Produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                               u.Produto.SubCategoria.Id.ToString() + @"/" + u.Produto.Imagens.FirstOrDefault().CaminhoImagem : "../../../Content/images/fotoIndisponivel.png"
                })).FirstOrDefault();

                var listaRemoveFornecedores = _listaComprasRep.FindBy(x => x.Id == listaId).Select(x => x.ListaComprasRemoveFornecedores.Select(f => new { prd = f.ProdutoId, forn = f.FonecedorId })).FirstOrDefault();

                response = request.CreateResponse(HttpStatusCode.OK, new { listaCompras, listaRemoveFornecedores });




                return response;
            });
        }


        [HttpGet]
        [Route("getListaCompras")]
        public HttpResponseMessage GetListaCompras(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var listaCompras = _listaComprasRep.FindBy(x => x.UsuarioCriacaoId == usuario.Id && x.Ativo).Select(u => new { u.NomeLista, u.Id, TotalItens = u.ListaComprasItens.Count() });
                response = request.CreateResponse(HttpStatusCode.OK, listaCompras);
                return response;
            });
        }


        [HttpPost]
        [Route("listaComprasRemove/{listaId:int}")]
        public HttpResponseMessage ListaComprasRemove(HttpRequestMessage request, int listaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var listaCompras = _listaComprasRep.GetSingle(listaId);

                listaCompras.Ativo = false;
                _listaComprasRep.Edit(listaCompras);
                _unitOfWork.Commit();
                response = request.CreateResponse(HttpStatusCode.OK);

                return response;
            });
        }


    }
}
