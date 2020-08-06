
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutoMapper;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;
using ECC.EntidadeMenu;
using ECC.EntidadePessoa;

namespace ECC.API_Web.Controllers
{

    [Authorize(Roles = "Admin, Fornecedor, Membro, Franquia")]
    [RoutePrefix("api/menu")]
    public class MenuController : ApiControllerBase
    {
        #region Variaveis
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Menu> _menuRep;
        private readonly IEntidadeBaseRep<PermissaoGrupo> _permissaoGrupoRep;
        private readonly IEntidadeBaseRep<UsuarioGrupo> _usuarioGrupoRep;
        private readonly IEntidadeBaseRep<Modulo> _moduloRep;

        #endregion

        public MenuController(IEntidadeBaseRep<Modulo> moduloRep,
                              IEntidadeBaseRep<UsuarioGrupo> usuarioGrupoRep,
                              IEntidadeBaseRep<PermissaoGrupo> permissaoGrupoRep,
                              IEntidadeBaseRep<Menu> menuRep,
                              IEntidadeBaseRep<Usuario> usuarioRep,
                              IEntidadeBaseRep<Membro> membroRep,
                              IEntidadeBaseRep<Erro> errosRepository,
                              IUnitOfWork unitOfWork)
            : base(usuarioRep, errosRepository, unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _menuRep = menuRep;
            _permissaoGrupoRep = permissaoGrupoRep;
            _usuarioGrupoRep = usuarioGrupoRep;
            _moduloRep = moduloRep;
            this._membroRep = membroRep;
        }

        [HttpGet]
        [Route("menu")]
        public HttpResponseMessage GetMenu(HttpRequestMessage request, int perfilModulo)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var modulo = 0;
                switch (perfilModulo)
                {
                    case 1://ADM
                        modulo = 1;
                        break;

                    case 2://Membro
                        modulo = 3;
                        break;

                    case 3://Fornecedor
                        modulo = 4;
                        break;

                    case 4://Franquia
                        modulo = 5;
                        break;
                }

                if (usuario != null)
                {
                    //nao esquecer de colocar pra trazer os menus q estao ativos fazer isso em todos os metodos q a regra se aplica
                    IEnumerable<MenuViewModel> menuVM;

                    if (modulo == 1)
                    {
                        var menuAdm =
                          _usuarioGrupoRep.GetAll()
                              .Where(x => x.UsuarioId == usuario.Id)
                              .SelectMany(x => x.Grupo.PermissoesGrupos.Select(m => m.Menu))
                              .Where(x => x.Ativo)
                              .OrderBy(x=>x.Ordem)
                              .Distinct();

                        menuVM = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(menuAdm);
                    }
                    else
                    {
                        var menu = _menuRep.GetAll().Where(m => m.ModuloId == modulo && m.Ativo);

                        menuVM = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(menu);

                        switch (modulo)
                        {
                            case 3:
                                var membro = this._membroRep.FindBy(x => x.PessoaId == usuario.PessoaId).FirstOrDefault();

                                //Essa verificação é para não exibir o menu de pagamento para membros VIP
                                if (membro.Vip)
                                    menuVM = menuVM.Where(x => x.Id != 131);
                               
                                //Essa verificação foi feita para membros de franquia aonde não pode mostrar o menu de novos fornecedores
                                //Para desativar o menu tem que desativar o paí e os filhos se não da erro
                                //110=Fornecedor                                                     

                                    //if (membro.FranquiaId != null)
                                    //    menuVM = menuVM.Where(x => x.Id != 110 && x.MenuPaiId !=110);
                                break;
                        }
                    }

                    var mn = new MontaMenu(menuVM.ToList());

                    //string menuMontado = mn.CriaMenu();
                    response = request.CreateResponse(HttpStatusCode.OK, mn.OrderBy(p => p.Ordem));
                }

                return response;
            });
        }


        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{modulo:int=0}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize,int? modulo, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;
            int mod = modulo.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Menu> menu = null;
                int totalMenu = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    if (mod > 0)
                    {
                        menu = _menuRep.FindBy(c => c.ModuloId == mod &&
                             c.DescMenu.Contains(filter))
                             .OrderBy(c => c.Modulo.DescModulo).ThenBy(c => c.DescMenu)
                             .Skip(currentPage * currentPageSize)
                             .Take(currentPageSize)
                             .ToList();

                        totalMenu = _menuRep.GetAll()
                            .Count(c => c.ModuloId == mod &&
                             c.DescMenu.Contains(filter));
                    }
                    else
                    {
                        menu = _menuRep.FindBy(c => c.DescMenu.Contains(filter))
                              .OrderBy(c => c.Modulo.DescModulo)
                              .ThenBy(c => c.DescMenu)
                              .Skip(currentPage * currentPageSize)
                              .Take(currentPageSize)
                              .ToList();

                        totalMenu = _menuRep.GetAll()
                            .Count(c => c.DescMenu.Contains(filter));
                    }
                }
                else
                {
                    if (mod > 0)
                    {
                        menu = _menuRep.GetAll().Where(c=>c.ModuloId == mod)
                       .OrderBy(c => c.Modulo.DescModulo)
                       .ThenBy(c => c.DescMenu)
                       .Skip(currentPage * currentPageSize)
                       .Take(currentPageSize)
                       .ToList();

                        totalMenu = _menuRep.GetAll().Count(x=>x.ModuloId == mod);
                    }
                    else
                    {
                        menu = _menuRep.GetAll()
                      .OrderBy(c => c.Modulo.DescModulo)
                      .ThenBy(c => c.DescMenu)
                      .Skip(currentPage * currentPageSize)
                      .Take(currentPageSize)
                      .ToList();

                        totalMenu = _menuRep.GetAll().Count();
                    }

                  
                }

                IEnumerable<MenuViewModel> menuVM = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(menu);

                PaginacaoConfig<MenuViewModel> pagSet = new PaginacaoConfig<MenuViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMenu,
                    TotalPages = (int)Math.Ceiling((decimal)totalMenu / currentPageSize),
                    Items = menuVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [HttpGet]
        [Route("pesquisarPai/{filter?}")]
        public HttpResponseMessage GetPai(HttpRequestMessage request, MenuViewModel menuViewModel, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Menu menu = null;
                var menuPaiId = filter.TryParseInt();

                menu = _menuRep.FirstOrDefault(c => c.Id == menuPaiId);

                menuViewModel = Mapper.Map<Menu, MenuViewModel>(menu);
                response = request.CreateResponse(HttpStatusCode.Created, menuViewModel);

                return response;
            });
        }


        [HttpGet]
        [Route("modulos")]
        public HttpResponseMessage GetModulo(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                var modulos = _moduloRep.GetAll();

                IEnumerable<ModuloViewModel> modulosVM = Mapper.Map<IEnumerable<Modulo>, IEnumerable<ModuloViewModel>>(modulos);

                response = request.CreateResponse(HttpStatusCode.OK, modulosVM);

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, MenuViewModel menuViewModel)
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

                    Menu novoMenu = new Menu
                    {
                        UsuarioCriacaoId = usuario.Id,
                        DtCriacao = DateTime.Now,
                        Ativo = menuViewModel.Ativo,
                        ModuloId = menuViewModel.ModuloId,
                        DescMenu = menuViewModel.DescMenu,
                        MenuPaiId = menuViewModel.MenuPaiId,
                        Nivel = menuViewModel.Nivel,
                        Ordem = menuViewModel.Ordem,
                        Url = menuViewModel.Url,
                        FontIcon = menuViewModel.FontIcon,
                        Feature1 = menuViewModel.Feature1,
                        Feature2 = menuViewModel.Feature2
                    };

                    var pOut = new SqlParameter
                    {
                        ParameterName = "@New_Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };

                    var pMenuPaiId = new SqlParameter
                    {
                        ParameterName = "@MenuPaiId",
                        SqlDbType = SqlDbType.Int,
                        Value = novoMenu.MenuPaiId
                    };

                    if (novoMenu.MenuPaiId == 0)
                        pMenuPaiId.Value = DBNull.Value;

                    var pUrl = new SqlParameter
                    {
                        ParameterName = "@Url",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.Url
                    };

                    if (novoMenu.Url == null)
                        pUrl.Value = DBNull.Value;

                    var pFontIcon = new SqlParameter
                    {
                        ParameterName = "@FontIcon",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.FontIcon
                    };

                    if (novoMenu.FontIcon == null)
                        pFontIcon.Value = DBNull.Value;

                    var pFeature1 = new SqlParameter
                    {
                        ParameterName = "@Feature1",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.Feature1
                    };

                    if (novoMenu.Feature1 == null)
                        pFeature1.Value = DBNull.Value;

                    var pFeature2 = new SqlParameter
                    {
                        ParameterName = "@Feature2",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.Feature2
                    };

                    if (novoMenu.Feature2 == null)
                        pFeature2.Value = DBNull.Value;

                    _menuRep.ExecuteWithStoreProcedure("stp_ins_menu @UsuarioCriacaoId, @DtCriacao, @Ativo, @MenuPaiId, @ModuloId, " +
                                                       "@DescMenu, @Nivel, @Ordem, @Url, @FontIcon, @Feature1, @Feature2, @New_Id out",
                                                       new SqlParameter("@UsuarioCriacaoId", novoMenu.UsuarioCriacaoId),
                                                       new SqlParameter("@DtCriacao", novoMenu.DtCriacao),
                                                       new SqlParameter("@Ativo", novoMenu.Ativo == true ? '1' : '0'),
                                                       pMenuPaiId,
                                                       new SqlParameter("@ModuloId", novoMenu.ModuloId),
                                                       new SqlParameter("@DescMenu", novoMenu.DescMenu),
                                                       new SqlParameter("@Nivel", novoMenu.Nivel),
                                                       new SqlParameter("@Ordem", novoMenu.Ordem),
                                                       pUrl,
                                                       pFontIcon,
                                                       pFeature1,
                                                       pFeature2,
                                                       pOut
                                                       );

                    // Update view model
                    novoMenu.Id = Convert.ToInt32(pOut.Value.TryParseInt());
                    menuViewModel = Mapper.Map<Menu, MenuViewModel>(novoMenu);
                    response = request.CreateResponse(HttpStatusCode.Created, menuViewModel);
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, MenuViewModel menuViewModel)
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
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var novoMenu = _menuRep.GetSingle(menuViewModel.Id);

                    novoMenu.UsuarioAlteracaoId = usuario.Id;
                    novoMenu.DtAlteracao = DateTime.Now;
                    novoMenu.Ativo = menuViewModel.Ativo;
                    novoMenu.ModuloId = menuViewModel.ModuloId;
                    novoMenu.DescMenu = menuViewModel.DescMenu;
                    novoMenu.MenuPaiId = menuViewModel.MenuPaiId;
                    novoMenu.Nivel = menuViewModel.Nivel;
                    novoMenu.Ordem = menuViewModel.Ordem;
                    novoMenu.Url = menuViewModel.Url;
                    novoMenu.FontIcon = menuViewModel.FontIcon;
                    novoMenu.Feature1 = menuViewModel.Feature1;
                    novoMenu.Feature2 = menuViewModel.Feature2;

                    var pOut = new SqlParameter
                    {
                        ParameterName = "@New_Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };

                    var pMenuPaiId = new SqlParameter
                    {
                        ParameterName = "@MenuPaiId",
                        SqlDbType = SqlDbType.Int,
                        Value = novoMenu.MenuPaiId
                    };

                    if (novoMenu.MenuPaiId == 0)
                        pMenuPaiId.Value = DBNull.Value;

                    var pUrl = new SqlParameter
                    {
                        ParameterName = "@Url",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.Url
                    };

                    if (novoMenu.Url == null)
                        pUrl.Value = DBNull.Value;

                    var pFontIcon = new SqlParameter
                    {
                        ParameterName = "@FontIcon",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.FontIcon
                    };

                    if (novoMenu.FontIcon == null)
                        pFontIcon.Value = DBNull.Value;

                    var pFeature1 = new SqlParameter
                    {
                        ParameterName = "@Feature1",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.Feature1
                    };

                    if (novoMenu.Feature1 == null)
                        pFeature1.Value = DBNull.Value;

                    var pFeature2 = new SqlParameter
                    {
                        ParameterName = "@Feature2",
                        SqlDbType = SqlDbType.VarChar,
                        Value = novoMenu.Feature2
                    };

                    if (novoMenu.Feature2 == null)
                        pFeature2.Value = DBNull.Value;

                    _menuRep.ExecuteWithStoreProcedure("stp_upd_menu @Id, @UsuarioAlteracaoId, @DtAlteracao, @Ativo, @MenuPaiId, @ModuloId, " +
                                                       "@DescMenu, @Nivel, @Ordem, @Url, @FontIcon, @Feature1, @Feature2",
                                                       new SqlParameter("@Id", novoMenu.Id),
                                                       new SqlParameter("@UsuarioAlteracaoId", novoMenu.UsuarioAlteracaoId),
                                                       new SqlParameter("@DtAlteracao", novoMenu.DtAlteracao),
                                                       new SqlParameter("@Ativo", novoMenu.Ativo == true ? '1' : '0'),
                                                       pMenuPaiId,
                                                       new SqlParameter("@ModuloId", novoMenu.ModuloId),
                                                       new SqlParameter("@DescMenu", novoMenu.DescMenu),
                                                       new SqlParameter("@Nivel", novoMenu.Nivel),
                                                       new SqlParameter("@Ordem", novoMenu.Ordem),
                                                       pUrl,
                                                       pFontIcon,
                                                       pFeature1,
                                                       pFeature2,
                                                       pOut
                                                       );

                    // Update view model
                    menuViewModel = Mapper.Map<Menu, MenuViewModel>(novoMenu);
                    response = request.CreateResponse(HttpStatusCode.OK, menuViewModel);
                }

                return response;
            });
        }


    }
}
