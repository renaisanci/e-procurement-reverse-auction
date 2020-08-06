/// <reference path="cotacao/painel.html" />

(function () {
    'use strict';

    angular.module('ECCAdm', ['common.core', 'common.ui'])
        .config(config)
        .run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $urlRouterProvider
                 .otherwise('/login');
        $stateProvider
         .state('Painel', {
             authenticate: true,
             url: "/",
             templateUrl: "scripts/SPAAdm/home/index.html",
             controller: "indexCtrl",
             // resolve: { isAuthenticated: isAuthenticated },
             ncyBreadcrumb: {
                 label: 'Painel',
                 description: ''
             }
         }).state('Cotacao', {
             authenticate: true,
             url: "/cotacao",
             templateUrl: "scripts/SPAAdm/cotacao/painel.html",
             controller: "painelCtrl",
             // resolve: { isAuthenticated: isAuthenticated },
             ncyBreadcrumb: {
                 label: 'Painel Cotações',
                 description: ''
             }
         })
         .state('login', {
             url: "/login",
             authenticate: false,
             templateUrl: "scripts/SPAAdm/login/login.html",
             controller: "loginCtrl",
             ncyBreadcrumb: {
                 label: 'Login',
                 description: ''
             }, resolve: {
                 deps: [
                     '$ocLazyLoad',
                     function ($ocLazyLoad) {
                         return $ocLazyLoad.load({
                             serie: true,
                             files: [
                                 '/Content/js/jasny-bootstrap.min.js'
                             ]
                         });
                     }
                 ]
             }
			})

			.state('cadastro', {
				url: "/cadastro",
				authenticate: false,
				templateUrl: "scripts/SPAAdm/membro/membroCadastro.html",
				controller: "membroCadastroCtrl",
				ncyBreadcrumb: {
					label: 'Cadastro',
					description: ''
				}, resolve: {
					deps: [
						'$ocLazyLoad',
						function ($ocLazyLoad) {
							return $ocLazyLoad.load({
								serie: true,
								files: [
									'/Content/js/jasny-bootstrap.min.js'
								]
							});
						}
					]
				}
			})
            .state('recuperasenha', {
                authenticate: false,
                url: "/recuperasenha",
                templateUrl: "scripts/SPAAdm/login/recuperasenha.html",
                controller: "recuperasenhaCtrl",
                ncyBreadcrumb: {
                    label: 'Recuperar Senha',
                    description: ''
                }, resolve: {
                    deps: [
                        '$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load({
                                serie: true,
                                files: [
                                    '/Content/js/jasny-bootstrap.min.js'
                                ]
                            });
                        }
                    ]
                }
            })

          .state('fornecedor', {
              authenticate: true,
              url: "/fornecedor",
              templateUrl: "scripts/SPAAdm/fornecedor/fornecedor.html",
              controller: "fornecedorCtrl",
              ncyBreadcrumb: {
                  label: 'Cadastro > Fornecedor > Manutenção',
                  description: 'Manutenção do cadastro de fornecedor'
              }
              , resolve: {
                  // isAuthenticated: isAuthenticated,
                  deps: [
                      '$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load({
                              serie: true,
                              files: [
                                  '/Content/js/jasny-bootstrap.min.js'
                              ]
                          });
                      }
                  ]
              }
          })

          .state('usuarioadm', {
              authenticate: true,
              url: "/usuarioadm",
              templateUrl: "scripts/SPAAdm/usuario/usuarioAdm.html",
              controller: "usuarioAdmCtrl",
              ncyBreadcrumb: {
                  label: 'Cadastro > Usuário > Manutenção',
                  description: 'Manutenção do usuários Admin'
              }
              , resolve: {
                  // isAuthenticated: isAuthenticated,
                  deps: [
                      '$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load({
                              serie: true,
                              files: [
                                  '/Content/js/jasny-bootstrap.min.js'
                              ]
                          });
                      }
                  ]
              }
			})


	


          .state('membro', {
              authenticate: true,
              resolve: {

                  // isAuthenticated: isAuthenticated,
                  deps: [
                      '$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load({
                              serie: true,
                              files: [
                                  '/Content/js/jasny-bootstrap.min.js'
                              ]
                          });
                      }
                  ]
              },
              url: "/membro",

              templateUrl: "scripts/SPAAdm/membro/membro.html",
              controller: "membroCtrl",
              ncyBreadcrumb: {
                  label: 'Cadastro > Membro > Manutenção',
                  description: 'Manutenção do cadastro de membros'
              }
          })

           .state('unidadeMedida', {
               //  resolve: { isAuthenticated: isAuthenticated },
               url: "/unidadeMedida",
               authenticate: true,
               templateUrl: "scripts/SPAAdm/unidadeMedida/unidadeMedida.html",
               controller: "unidadeMedidaCtrl",

               ncyBreadcrumb: {
                   label: 'Cadastro > Unidade de Medida > Manutenção',
                   description: 'Manutenção do cadastro de unidade de medida de produtos'
               }
           })

        .state('statusSistema', {
            authenticate: true,
            // resolve: { isAuthenticated: isAuthenticated },        
            url: "/statusSistema",
            templateUrl: "scripts/SPAAdm/statusSistema/statusSistema.html",
            controller: "statusSistemaCtrl",
            ncyBreadcrumb: {
                label: 'Cadastro > Status Sistema > Status',
                description: 'Manutenção dos Status de workflow de sistema'
            }
        })

        .state('workflowStatus', {
            authenticate: true,
            // resolve: { isAuthenticated: isAuthenticated },
            url: "/workflowStatus",

            templateUrl: "scripts/SPAAdm/statusSistema/workflowStatus.html",
            controller: "workflowStatusCtrl",
            ncyBreadcrumb: {
                label: 'Cadastro > Status Sistema > Workflow',
                description: 'Manutenção de workflow de Status de sistema'
            }
        })

       .state('grupopermissao', {
           authenticate: true,
           // resolve: { isAuthenticated: isAuthenticated },
           url: "/grupopermissao",

           templateUrl: "scripts/SPAAdm/grupo/grupoPermissao.html",
           controller: "grupoPermissaoCtrl",
           ncyBreadcrumb: {
               label: 'Controle de Acesso > Grupo',
               description: 'Manutenção de Grupos de Usuários'
           }
       })

       .state('categoria', {
           authenticate: true,
           url: "/categoria",

           templateUrl: "scripts/SPAAdm/categoria/categoria.html",
           controller: "categoriaCtrl",
           //resolve: { isAuthenticated: isAuthenticated },
           ncyBreadcrumb: {
               label: 'Cadastro > Categoria ',
               description: 'Manutenção das categorias do sistema'
           }
       })


        .state('subcategoria', {
            authenticate: true,
            url: "/subcategoria",

            templateUrl: "scripts/SPAAdm/categoria/subcategoria.html",
            controller: "subcategoriaCtrl",
            //resolve: { isAuthenticated: isAuthenticated },
            ncyBreadcrumb: {
                label: 'Cadastro > Subcategoria ',
                description: 'Manutenção das subcategorias do sistema'
            }
        })

        .state('produto', {
            url: "/produto",
            authenticate: true,
            templateUrl: "scripts/SPAAdm/produto/produto.html",
            controller: "produtoCtrl",
            // resolve: { isAuthenticated: isAuthenticated },
            ncyBreadcrumb: {
                label: 'Cadastro > Produto ',
                description: 'Manutenção de produtos'
            }
        })
            .state('marca', {
                url: "/marca",
                authenticate: true,
                templateUrl: "scripts/SPAAdm/marca/marca.html",
                controller: "marcaCtrl",
                // resolve: { isAuthenticated: isAuthenticated },
                ncyBreadcrumb: {
                    label: 'Cadastro > Marca ',
                    description: 'Manutenção de produtos'
                }
            })

            .state('segmento', {
                url: "/segmento",
                authenticate: true,
                templateUrl: "scripts/SPAAdm/segmento/segmento.html",
                controller: "segmentoCtrl",
                // resolve: { isAuthenticated: isAuthenticated },
                ncyBreadcrumb: {
                    label: 'Cadastro > Segmento ',
                    description: 'Manutenção de segmentos'
                }
            })

            .state('Menu', {
                url: "/menu",
                authenticate: true,
                templateUrl: "scripts/SPAAdm/menu/menu.html",
                controller: "menuCtrl",
                // resolve: { isAuthenticated: isAuthenticated },
                ncyBreadcrumb: {
                    label: 'Cadastro > Menu ',
                    description: 'Manutenção de Menu'
                }
            })

        .state('fabricante', {
            authenticate: true,
            url: "/fabricante",
            templateUrl: "scripts/SPAAdm/fabricante/fabricante.html",
            controller: "fabricanteCtrl",
            // resolve: { isAuthenticated: isAuthenticated },
            ncyBreadcrumb: {
                label: 'Cadastro > Fabricante',
                description: 'Manutenção de Fabricante'
            }
        })

        .state('promocao', {
            authenticate: true,
            url: "/aprovarpromocao",
            templateUrl: "scripts/SPAAdm/produto/aprovarpromocao.html",
            controller: "aprovarpromocaoCtrl",
            ncyBreadcrumb: {
                label: 'Promoções > Aprovar',
                description: 'Aprovar Promoções'
            }
        })

         .state('emailBoasVinda', {
             authenticate: true,
             url: "/emailBoasVinda",
             templateUrl: "scripts/SPAAdm/email/emailBoasVinda.html",
             controller: "emailBoasVindaCtrl",
             // resolve: { isAuthenticated: isAuthenticated },
             ncyBreadcrumb: {
                 label: 'E-mail > Boas Vindas',
                 description: 'Email Boas Vindas'
             }
         })

            .state('franquias', {
                authenticate: true,
                url: "/franquias",
                templateUrl: "scripts/SPAAdm/franquias/franquias.html",
                controller: "franquiasCtrl",
                // resolve: { isAuthenticated: isAuthenticated },
                ncyBreadcrumb: {
                    label: 'Franquias > Manutenção',
                    description: 'Manutenção de Franquias'
                }
            });
    }


    run.$inject = ['$rootScope', '$location', '$cookieStore', '$http', '$state', 'membershipService', '$modalStack'];
    function run($rootScope, $location, $cookieStore, $http, $state, membershipService, $modalStack) {

        $rootScope.$on("$stateChangeStart",
          function (event, toState) {
              //Fecha todos os modal caso o usuario clique no botão voltar do browser
              $modalStack.dismissAll();
              if (toState.authenticate && !membershipService.isUserLoggedIn()) {
                  $location.path('/');
              }

              //Valida se o usuário tem acesso a pagina
              if (membershipService.isUserLoggedIn() && $location.$$url != "/")
                  membershipService.permissaoURL($location.$$url, $location);

          });
        // handle page refreshes
        $rootScope.repository = $cookieStore.get('repositoryAdmRefresh') || {};
        if ($rootScope.repository.loggedUser) {
            $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;
        }


        $(document).ready(function () {
            $(".fancybox").fancybox({
                openEffect: 'none',
                closeEffect: 'none'
            });

            $('.fancybox-media').fancybox({
                openEffect: 'none',
                closeEffect: 'none',
                helpers: {
                    media: {}
                }
            });

            $('[data-toggle=offcanvas]').click(function () {
                $('.row-offcanvas').toggleClass('active');
            });
        });
    }

    /*
    autenticaAcesso.$inject = ['membershipService', '$rootScope', '$location', '$http'];
    function autenticaAcesso(membershipService, $rootScope, $location, $http) {
        var url = "";
         $http.post('/api/conta/permissaourl/' + url, url, {
             //passa aqui o parametro
         }).success(function (data) {
             if (data)
                 $location.path('/');
         }).error(function (data, status) {
             //
         }).finally(function () {
             //
         });
    }
    */

    // isAuthenticated.$inject = ['membershipService', '$rootScope', '$location'];
    //function isAuthenticated(membershipService, $rootScope, $location) {
    //    if (!membershipService.isUserLoggedIn()) {

    //        $rootScope.previousState = $location.path();
    //        $location.path('/login');
    //    }
    //}

})();