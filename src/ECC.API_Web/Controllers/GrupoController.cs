using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutoMapper;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System;
using ECC.EntidadeMenu;

namespace ECC.API_Web.Controllers
{
    [Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/usuariogrupo")]
    public class GrupoController : ApiControllerBase
    {

        private readonly IEntidadeBaseRep<PermissaoGrupo> _grupoPermissaoRep;
        private readonly IEntidadeBaseRep<Grupo> _grupoRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Menu> _menuRep;

        public GrupoController(IEntidadeBaseRep<Usuario> usuarioRep, 
            IEntidadeBaseRep<PermissaoGrupo> grupoPermissaoRep, 
            IEntidadeBaseRep<Menu> menuRep, 
            IEntidadeBaseRep<Grupo> grupoRep, 
            IEntidadeBaseRep<Erro> _errosRepository, 
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _grupoRep = grupoRep;
            _usuarioRep = usuarioRep;
            _menuRep = menuRep;
            _grupoPermissaoRep = grupoPermissaoRep;
        }

        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Grupo> listaGrupo = null;
                int totalListaGrupo = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    listaGrupo = _grupoRep.FindBy(c => c.DescGrupo.ToLower().Contains(filter))
                        .OrderBy(c => c.DescGrupo)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalListaGrupo = _grupoRep
                        .GetAll()
                        .Count(c => c.DescGrupo.ToLower().Contains(filter));
                }
                else
                {
                    listaGrupo = _grupoRep.GetAll()
                        .OrderBy(c => c.DescGrupo)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalListaGrupo = _grupoRep.GetAll().Count();
                }

                IEnumerable<GrupoViewModel> GruposVM = Mapper.Map<IEnumerable<Grupo>, IEnumerable<GrupoViewModel>>(listaGrupo);

                PaginacaoConfig<GrupoViewModel> pagSet = new PaginacaoConfig<GrupoViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalListaGrupo,
                    TotalPages = (int)Math.Ceiling((decimal)totalListaGrupo / currentPageSize),
                    Items = GruposVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, GrupoViewModel grupoVM)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    Grupo novoGrupo = new Grupo()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        DescGrupo = grupoVM.DescGrupo,
                        Ativo = grupoVM.Ativo
                    };
                    _grupoRep.Add(novoGrupo);

                    _unitOfWork.Commit();

                    // Update view model
                    grupoVM = Mapper.Map<Grupo, GrupoViewModel>(novoGrupo);
                    response = request.CreateResponse(HttpStatusCode.Created, grupoVM);

                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, GrupoViewModel grupoVM)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Grupo novoGrupo = _grupoRep.GetSingle(grupoVM.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    novoGrupo.AtualizarGrupo(grupoVM, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    grupoVM = Mapper.Map<Grupo, GrupoViewModel>(novoGrupo);
                    response = request.CreateResponse(HttpStatusCode.OK, grupoVM);

                }

                return response;
            });
        }

        [HttpGet]
        [Route("getmenu/{idgrupo:int=0}/{modulo:int=0}")]
        public HttpResponseMessage GetMenu(HttpRequestMessage request, int? idgrupo, int? modulo)
        {
            int GrupoId = idgrupo.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                IEnumerable<MenuViewModel> menuVM = null;
                List<MenuPermissaoViewModel> menuVMresponse = new List<MenuPermissaoViewModel>();

                var menu = _menuRep.GetAll().Where(m => m.ModuloId == modulo && m.Ativo);
                menuVM = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(menu);

                var menuVMPermissao = _grupoPermissaoRep.GetAll().Where(m => m.GrupoId == idgrupo && m.Ativo);

                var menuVM2 = menuVM.Where(m => m.Url != null && m.Url.Contains("#/"));

                MenuPermissaoViewModel mnp;
                //MontaStrMenu strMenuObj = new MontaStrMenu();
                foreach (MenuViewModel mn in menuVM2)
                {
                    mnp = new MenuPermissaoViewModel();
                    string strMenu = "";

                    //strMenu = strMenuObj.MontaStrMenu(mn.MenuPaiId, menuVM.ToList());
                    strMenu = Util.MontaStrMenu(mn.MenuPaiId, menuVM.ToList());
                    strMenu += " > " + mn.DescMenu;

                    mnp.DescMenu = strMenu;
                    mnp.Id = mn.Id;
                    mnp.MenuPaiId = mnp.MenuPaiId;

                    mnp.ModuloId = mnp.ModuloId;
                    mnp.Relacionado = menuVMPermissao.Count(m => m.MenuId == mn.Id) > 0 ? true : false;
                    mnp.selected = menuVMPermissao.Count(m => m.MenuId == mn.Id) > 0 ? true : false;

                    menuVMresponse.Add(mnp);

                }

                response = request.CreateResponse(HttpStatusCode.OK, menuVMresponse.OrderBy(p => p.DescMenu));

                return response;
            });


        }

        [HttpPost]
        [Route("atualizarpermissao/{GrupoID:int}")]
        public HttpResponseMessage AtualizarPermissao(HttpRequestMessage request, int GrupoID, IEnumerable<MenuPermissaoViewModel> menuPermissaoVM)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {

                    var grupoPermissao = _grupoPermissaoRep.GetAll().Where(x => x.GrupoId == GrupoID);

                    if (grupoPermissao.Any())
                    {
                        _grupoPermissaoRep.DeleteAll(grupoPermissao);
                        _unitOfWork.Commit();
                    }


                    IEnumerable<MenuViewModel> menuVM = null;
                    List<PermissaoGrupo> lstPermissaoGrupo =  new List<PermissaoGrupo>();

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var menu = _menuRep.GetAll().Where(m => m.ModuloId == 1 && m.Ativo);
                    int IdMenuPai;
                    menuVM = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(menu);


                    PermissaoGrupo permissaoGrupo = null;
                    PermissaoGrupo permissaoGrupoItem = null;

                    foreach (MenuPermissaoViewModel mnp in menuPermissaoVM)
                    {
                        if (mnp.selected)
                        {
                            permissaoGrupo = new PermissaoGrupo();
                            permissaoGrupo.UsuarioCriacao = usuario;
                            permissaoGrupo.DtCriacao = DateTime.Now;
                            permissaoGrupo.Ativo = true;

                            permissaoGrupo.GrupoId = GrupoID;
                            permissaoGrupo.MenuId = mnp.Id;
                            permissaoGrupo.PerfilId = 1;

                            permissaoGrupo.FlgVisualizarMenu = mnp.selected;
                            // TODO: Incluir permissões de Consultar, Alterar, Incluir e Excluir

                            
                            IdMenuPai = (int)(menu.Single(m => m.Id == mnp.Id).MenuPaiId == null ? 0 : menu.Single(m => m.Id == mnp.Id).MenuPaiId);
                            if (IdMenuPai > 0)
                            {
                                var lstPG = Util.PermissaoIdPai(IdMenuPai, lstPermissaoGrupo, menuVM);
                                    
                                foreach (PermissaoGrupoViewModel item in lstPG)
                                {
                                    permissaoGrupoItem = new PermissaoGrupo();
                                    permissaoGrupoItem.UsuarioCriacao = usuario;
                                    permissaoGrupoItem.DtCriacao = DateTime.Now;
                                    permissaoGrupoItem.Ativo = true;

                                    permissaoGrupoItem.GrupoId = GrupoID;
                                    permissaoGrupoItem.MenuId = item.MenuId;
                                    permissaoGrupoItem.PerfilId = 1;

                                    permissaoGrupoItem.FlgVisualizarMenu = mnp.selected;
                                    // TODO: Incluir permissões de Consultar, Alterar, Incluir e Excluir
                                    lstPermissaoGrupo.Add(permissaoGrupoItem);
                                }

                            }
                            lstPermissaoGrupo.Add(permissaoGrupo);

                        }
                    }
                    //_grupoPermissaoRep.Add();
                    foreach(var item in lstPermissaoGrupo){
                        _grupoPermissaoRep.Add(item);
                    }
                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }

                return response;
            });
        }

    }
}