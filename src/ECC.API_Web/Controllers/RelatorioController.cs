using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ECC.API_Web.InfraWeb;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeMenu;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.SMS;
using Microsoft.AspNet.Identity;
using ECC.API_Web.Models;
using System.Collections.Generic;

namespace ECC.API_Web.Controllers
{
    [Authorize(Roles = "Admin, Membro, Fornecedor")]
    [RoutePrefix("api/relatorio")]
    public class RelatorioController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;

        public RelatorioController(
            IEntidadeBaseRep<Usuario> usuarioRep,
                IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Erro> _errosRepository,
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _fornecedorRep = fornecedorRep;
        }

        [HttpGet]
        [Route("sms")]
        public HttpResponseMessage SMS(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {


        //necessatio para consumir webservice
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


                var tww = new TWW();

                var data = new { Creditos = tww.Creditos(), Validade = tww.ValidadeCreditos() };

                var response = request.CreateResponse(HttpStatusCode.OK, data);

                return response;
            });
        }



        [HttpGet]
        [Route("fornCotacaoEveolucao")]
        public HttpResponseMessage FornCotacaoEveolucao(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.Pessoa.Id == usuario.Pessoa.Id);

                var pFornecedorId = new SqlParameter
                {
                    ParameterName = "@FORNECEDORID",
                    SqlDbType = SqlDbType.BigInt,
                     Value = fornecedor.Id
                   
                };


                var relatCe = _usuarioRep.ExecWithStoreProcedure<IndicadorFornViewModel>("stp_fornecedor_rel_cotacao_evolucao @FORNECEDORID", pFornecedorId);

                _unitOfWork.Commit();

                var pForId = new SqlParameter
                {
                    ParameterName = "@FORN",
                    SqlDbType = SqlDbType.BigInt,
                    Value = fornecedor.Id

                };


                var relatCp = _usuarioRep.ExecWithStoreProcedure<IndicadorFornViewModel>("stp_fornecedor_rel_cotacao_partic @FORN", pForId);

              

                _unitOfWork.Commit();
                var relatCped = "";

                var response = request.CreateResponse(HttpStatusCode.OK, new { relatCe, relatCp, relatCped });
                return response;
            });
        }




        [HttpGet]
        [Route("fornCotacaoPedido")]
        public HttpResponseMessage FornCotacaoParticipou(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.Pessoa.Id == usuario.Pessoa.Id);

                var pForId = new SqlParameter
                {
                    ParameterName = "@FORNECEDORID",
                    SqlDbType = SqlDbType.BigInt,
                    Value = fornecedor.Id

                };

                var relatCped = _usuarioRep.ExecWithStoreProcedure<IndicadorFornCotPedidoViewModel>("stp_fornecedor_rel_cotacao_pedidos @FORNECEDORID", pForId);


             

                List<IndicadorFornCotPedidoViewModel> listapromo = new List<IndicadorFornCotPedidoViewModel>();
                List<IndicadorFornCotPedidoViewModel> listaCota = new List<IndicadorFornCotPedidoViewModel>();

                List<IndicadorFornCotPedidoViewModel> listaIndcador = new List<IndicadorFornCotPedidoViewModel>();


                foreach (var item in relatCped)
                {
                    if(item.TipoPedido== "promo")
                    {
                        IndicadorFornCotPedidoViewModel promo = new IndicadorFornCotPedidoViewModel();
                        promo.mes = item.mes;
                        promo.qtdPedCota = item.qtdPedCota;


                        listapromo.Add(promo);
                    }


                    if (item.TipoPedido == "cota")
                    {
                        IndicadorFornCotPedidoViewModel cota = new IndicadorFornCotPedidoViewModel();
                        cota.mes = item.mes;
                        cota.qtdPedCota = item.qtdPedCota;


                        listaCota.Add(cota);
                    }

                }


                foreach (var item in listaCota)
                {


                    IndicadorFornCotPedidoViewModel ind = new IndicadorFornCotPedidoViewModel();

                    ind.mesUnic = item.mes;
                    ind.a = item.qtdPedCota;


                    foreach (var item2 in listapromo)
                    {
                        if(item2.mes == item.mes)
                        {

                            ind.b = item2.qtdPedCota;
                        }
                    }

                    listaIndcador.Add(ind);
                }


                var response = request.CreateResponse(HttpStatusCode.OK, new { listaIndcador });
 
                return response;
            });
        }
    }
}


 