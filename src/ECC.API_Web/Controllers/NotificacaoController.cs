using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeAvisos;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace ECC.API_Web.Controllers
{
    [Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/notificacao")]
    public class NotificacaoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Notificacao> _notificacaoRep;
        private readonly IEntidadeBaseRep<TipoAvisos> _tipoAvisosRep;

        private readonly IMembershipService _membershipService;

        public NotificacaoController(
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Notificacao> notificacaoRep,
            IEntidadeBaseRep<TipoAvisos> tipoAvisosRep,

            IMembershipService membershipService,
             IEntidadeBaseRep<Erro> errosRepository,
             IUnitOfWork unitOfWork)
            : base(usuarioRep, errosRepository, unitOfWork)
        {
            _membershipService = membershipService;
            _usuarioRep = usuarioRep;
            _notificacaoRep = notificacaoRep;
            _tipoAvisosRep = tipoAvisosRep;
        }

        [HttpGet]
        [Route("tipoNotificacao")]
        public HttpResponseMessage GetTipoNotificacao(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var modulo = 0;
                switch (usuario.PerfilId)
                {
                    case 1:
                        modulo = 1;
                        break;
                    case 2:
                        modulo = 3;
                        break;
                    case 3:
                        modulo = 4;
                        break;
                }

                var notificacao = _tipoAvisosRep.GetAll().Where(x => x.Ativo && x.ModuloId.Equals(modulo) && x.Notificacoes.Any(y => y.Ativo)).ToList();

                var notificacaoVM = Mapper.Map<IEnumerable<TipoAvisos>, IEnumerable<TipoNotificacaoViewModel>>(notificacao);

                notificacaoVM.ForEach(x =>
                {
                    x.Notificacoes.ForEach(y =>
                    {
                        y.Checked = !usuario.Notificacoes.Any(z => z.Ativo && z.NotificacaoId.Equals(y.Id));
                    });
                });

                var response = request.CreateResponse(HttpStatusCode.OK, notificacaoVM);

                return response;
            });
        }

        [HttpPost]
        [Route("atualizarNotificacaoUsuario")]
        public HttpResponseMessage AtualizarNotificacaoUsuario(HttpRequestMessage request, NotificacaoViewModel notificacao)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var notif = usuario.Notificacoes.FirstOrDefault(x => x.NotificacaoId.Equals(notificacao.Id));

                if (notif != null)
                {
                    notif.DtAlteracao = DateTime.Now;
                    notif.UsuarioAlteracao = usuario;
                    notif.Ativo = !notificacao.Checked;
                }
                else
                {
                    notif = new UsuarioNotificacao
                    {
                        UsuarioId = usuario.Id,
                        NotificacaoId = notificacao.Id,
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Ativo = !notificacao.Checked
                    };

                    usuario.Notificacoes.Add(notif);
                }

                _unitOfWork.Commit();

                var response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }
    }
}