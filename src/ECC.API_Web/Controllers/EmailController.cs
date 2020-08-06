
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using ECC.API_Web.InfraWeb;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.EntidadeSms;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;



namespace ECC.API_Web.Controllers
{


    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/email")]
    public class EmailController : ApiControllerBase
    {

        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEmailService _emailService;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IUtilService _utilEmailService;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;

        public EmailController(IEntidadeBaseRep<Sms> smsRep, IEmailService emailService, IUtilService utilEmailService, IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork,


                 IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep
            )
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _smsRep = smsRep;
            _usuarioRep = usuarioRep;
            _emailService = emailService;
            _utilEmailService = utilEmailService;
            _membroRep = membroRep; ;
            _fornecedorRep = fornecedorRep;
        }



        [AllowAnonymous]
        [Route("formularioSite/{razao}/{nome}")]
        [HttpPost]
        public HttpResponseMessage FormularioSite(HttpRequestMessage request, string razao, string nome)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                string[] strform = razao.Split('|');

                StringBuilder strForm = new StringBuilder();




                strForm.Append("<table style='height: 139px;' width='321'>");
                strForm.Append("<tbody>");
                strForm.Append("<tr>");
                strForm.Append("<td>&nbsp;Raz&atilde;o Social</td>");
                strForm.Append("<td>" + strform[0] + "</td>");
                strForm.Append("</tr>");
                strForm.Append("<tr>");
                strForm.Append("<td>&nbsp;Nome</td>");
                strForm.Append("<td>" + strform[1] + "</td>");
                strForm.Append("</tr>");
                strForm.Append("<tr>");
                strForm.Append("<td>&nbsp;Email</td>");
                strForm.Append("<td>" + strform[2] + "</td>");
                strForm.Append("</tr>");
                strForm.Append("<tr>");
                strForm.Append("<td>&nbsp;Telefone</td>");
                strForm.Append("<td>" + strform[3] + "</td>");
                strForm.Append("</tr>");
                strForm.Append("<tr>");
                strForm.Append("<td>&nbsp;Mensagem</td>");
                strForm.Append("<td>" + strform[4] + "</td>");
                strForm.Append("</tr>");
                strForm.Append("</tbody>");
                strForm.Append("</table>");
                strForm.Append("<p>&nbsp;</p>");



                //Envia email de boas vindas para o cliente
                _emailService.EnviaEmail("comercial@economizaja.com.br", "", strForm.ToString(), "Economiza Já - Contato Pelo Site ");



                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }


        [AllowAnonymous]
        [Route("confirmaSms")]
        [HttpGet]
        public HttpResponseMessage ConfirmaSms(HttpRequestMessage request, string phone, string smscenter, string text)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var idMsg = text.Split('@');

                var sms = _smsRep.GetSingle(idMsg[1].TryParseInt());

                sms.DataConfirmacaoEnvio = DateTime.Now;

                _smsRep.Edit(sms);
                _unitOfWork.Commit();

 
                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }




        [Route("enviaEmailMembro/{idMembro:int}")]
        [HttpPost]
        public HttpResponseMessage EnviarEmailMembro(HttpRequestMessage request, int idMembro)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;



                if (idMembro > 0)
                {

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    Membro membroAtual = _membroRep.GetSingle(idMembro);                
                    _utilEmailService.MembroInserirUsuarioEnviarEmail(idMembro, usuario.Id);


                    //pega o telefone do primeiro usuario
                    var membroTel = membroAtual.Pessoa.Usuarios.FirstOrDefault();

                    //Inserir SMS de boas vindas
                    Sms sms = new Sms
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Numero = membroTel.Telefones.Select(t => t.DddCel).FirstOrDefault() + membroTel.Telefones.Select(t => t.Celular).FirstOrDefault(),
                        Mensagem = "Economiza Já - BEM VINDO. Acesse membro.economizaja.com.br a senha são os 8 primeiros digitos do seu CNPJ ou CPF e email, ou siga instruções enviada no email",
                        Status = StatusSms.NaoEnviado,
                        OrigemSms = TipoOrigemSms.PedidoPromocionalPendenteAprovacao,
                        Ativo = true
                    };
                    _smsRep.Add(sms);
                    _unitOfWork.Commit();


                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }              

                return response;
            });
        }



        [Route("enviaEmailFornecedor/{idFornecedor:int}")]
        [HttpPost]
        public HttpResponseMessage EnviaEmailFornecedor(HttpRequestMessage request, int idFornecedor)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (idFornecedor > 0)
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var fornecedorAtual = _fornecedorRep.GetSingle(idFornecedor);

                    _utilEmailService.FornecedorInserirUsuarioEnviarEmail(idFornecedor, usuario.Id);

                    //pega o telefone do primeiro usuario
                    var fornecedorTel = fornecedorAtual.Pessoa.Usuarios.FirstOrDefault();

                    //Inserir SMS de boas vindas
                    Sms sms = new Sms
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Numero = fornecedorTel.Telefones.Select(t => t.DddCel).FirstOrDefault() + fornecedorTel.Telefones.Select(t => t.Celular).FirstOrDefault(),
                        Mensagem = "Economiza Já-BEM VINDO Acesse fornecedor.economizaja.com.br a senha são os 8 primeiros digitos d seu CNPJ e email cadastrado ou siga instruções enviada no email",
                        Status = StatusSms.NaoEnviado,
                        OrigemSms = TipoOrigemSms.PedidoPromocionalPendenteAprovacao,
                        Ativo = true
                    };
                    _smsRep.Add(sms);
                    _unitOfWork.Commit();


                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }

                return response;
            });
        }

    }
}
