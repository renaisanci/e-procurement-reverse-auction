
(function () {
    'use strict';

    angular.module('ECCCliente', ['common.core', 'common.ui'])
        .config(config)
        .run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $urlRouterProvider
           .otherwise('/login');
        $stateProvider
            .state('login', {
                url: "/login",
                authenticate: false,
                templateUrl: "scripts/SPACliente/login/login.html",
                controller: "loginCtrl",
                reload:true,
                ncyBreadcrumb: {
                    label: 'Login',
                    description: ''
                },
                resolve: {
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
            }).state('Painel', {
                authenticate: true,
                url: "/",
                templateUrl: "scripts/SPACliente/home/index.html",
                controller: "indexCtrl",
                ncyBreadcrumb: {
                    label: 'Painel',
                    description: ''
                }
            }).state('Carrinho', {
                authenticate: true,

                url: "/shoppingCart",
                templateUrl: "scripts/SPACliente/shoppingCart/shoppingCart.html",
                controller: "shoppingCartCtrl",
                ncyBreadcrumb: {
                    label: 'Carrinho de Compras',
                    description: ''
                }
            }).state('produto', {
                authenticate: true,
                url: "/produto/:id",
                templateUrl: "scripts/SPACliente/produto/produto.html",
                controller: "produtoCtrl",
                ncyBreadcrumb: {
                    label: 'Produto',
                    description: ''
                }
            }).state('recuperasenha', {
                authenticate: false,
                url: "/recuperasenha",
                templateUrl: "scripts/SPACliente/login/recuperasenha.html",
                controller: "recuperasenhaCtrl",
                ncyBreadcrumb: {
                    label: 'Recuperar Senha',
                    description: ''
                }
            }).state('meusPedidos', {
                authenticate: false,
                url: "/meusPedidos",
                templateUrl: "scripts/SPACliente/pedidos/meusPedidos.html",
                controller: "meusPedidosCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: 'Pedidos Cotacao',
                    description: ''
                }

            }).state('modalDetPedidos', {
				authenticate: false,
				cache: false,
			//	params: { cache: null },
                reload: true,
                url: "/modlaDetPedidos",
                templateUrl: "scripts/SPACliente/pedidos/detPedido.html",
                controller: "detPedidoCtrl",
                ncyBreadcrumb: {
                    label: 'Detalhe Pedidos Cotacao',
                    description: ''
				}
			}).state('modalListasCompras', {
				authenticate: false,
				cache: false,
				//	params: { cache: null },
				reload: true,
				url: "/modalListasCompras",
				templateUrl: "scripts/SPACliente/shoppingCart/listasCompras.html",
				controller: "listasComprasCtrl",
				ncyBreadcrumb: {
					label: 'Listas de Compras',
					description: ''
				}

			}).state('estoque', {
                authenticate: true,
                url: "/estoque",
                templateUrl: "scripts/SPACliente/estoque/estoque.html",
                controller: "estoqueCtrl",
                ncyBreadcrumb: {
                    label: 'Estoque',
                    description: ''
                }
            })
            .state('baixasestoque', {
                authenticate: true,
                url: "/baixasestoque",
                templateUrl: "scripts/SPACliente/estoque/baixasestoque.html",
                controller: "baixasestoqueCtrl",
                ncyBreadcrumb: {
                    label: 'Baixas Estoque',
                    description: ''
                }
            })
            .state('enderecoMembro', {
                authenticate: true,
                url: "/enderecoMembro",
                templateUrl: "scripts/SPACliente/cadendmembro/cadendmembro.html",
                controller: "cadendmembroCtrl",
                ncyBreadcrumb: {
                    label: 'Cadastro de Endereço Membro',
                    description: 'Cadastro de Endereço Membro'
                }
            })
         .state('fornecedor', {
             authenticate: true,
             url: "/fornecedor",
             templateUrl: "scripts/SPACliente/fornecedor/fornecedor.html",
             controller: "fornecedorCtrl",
             ncyBreadcrumb: {
                 label: 'Solicitação para fornecedor',
                 description: 'Solicitação para fornecedor'
             }
         })
         .state('avaliarEntrega', {
             authenticate: true,
             url: "/avaliarEntrega",
             templateUrl: "scripts/SPACliente/pedidos/avaliarEntrega.html",
             controller: "avaliarEntregaCtrl",
             ncyBreadcrumb: {
                 label: 'Avaliar Entrega',
                 description: 'Avaliar Entrega'
             }
         })
         .state('indicadores', {
             authenticate: true,
             url: "/indicadores",
             templateUrl: "scripts/SPACliente/relatorios/indicadores.html",
             controller: "indicadoresCtrl",
             ncyBreadcrumb: {
                 label: 'Indicadores',
                 description: 'Indicadores'
             }
         })

        .state('promocoes', {
            authenticate: true,
            url: "/promocoes",
            templateUrl: "scripts/SPACliente/promocoes/promocoes.html",
            controller: "promocoesCtrl",
            ncyBreadcrumb: {
                label: 'Promoçoes',
                description: 'Promoções'
            }
        })
        .state('termoUso', {
            authenticate: true,
            url: "/termoUso",
            templateUrl: "scripts/SPACliente/termoUso/termoUso.html",
            controller: "termoUsoCtrl"
        })
        .state('privacidade', {
            authenticate: true,
            url: "/privacidade",
            templateUrl: "scripts/SPACliente/termoUso/privacidade.html",
            controller: "privacidadeCtrl"
        })

        .state('perfil', {
            authenticate: true,
            url: "/perfil",
            templateUrl: "scripts/SPACliente/perfil/perfil.html",
            controller: "perfilCtrl"
        })

        .state('empresa', {
            authenticate: true,
            url: "/perfil/empresa",
            templateUrl: "scripts/SPACliente/perfil/empresa.html",
            controller: "empresaCtrl"
        })

        .state('configuracao', {
            authenticate: true,
            url: "/perfil/configuracao",
            templateUrl: "scripts/SPACliente/perfil/configuracao.html",
            controller: "configuracaoCtrl"
        })

            .state('pagamento', {
                authenticate: true,
                url: "/pagamento/:id",
                templateUrl: "scripts/SPACliente/pagamento/pagamento.html",
                controller: "pagamentoCtrl"
            })

        .state('meusPedidosPromocao', {
                authenticate: true,
                url: "/meusPedidosPromocao",
                templateUrl: "scripts/SPACliente/pedidos/meusPedidosPromocao.html",
                controller: "meusPedidosPromocaoCtrl"
            })

            .state('planos', {
                authenticate: true,
                url: "/perfil/planos",
                templateUrl: "scripts/SPACliente/perfil/planos.html",
                controller: "planosCtrl"
            });
    }

    run.$inject = ['$rootScope', '$location', '$cookieStore', '$http', '$state', 'membershipService', '$modalStack', 'apiService', '$templateCache'];

    function run($rootScope, $location, $cookieStore, $http, $state, membershipService, $modalStack, apiService, $templateCache) {
     

     
   
        $rootScope.$on("$stateChangeStart",
           function (event, toState) {

               //limpa o cache da pagina chamada
               //no caso de modal não vai passa por aqui enão tem q aplicar essa linha de codigo abaixo antes de chamar o modal
               $templateCache.remove(toState.templateUrl);
               //Fecha todos os modal caso o usuario clique no botão voltar do browser
               $modalStack.dismissAll();
               if (toState.authenticate && !membershipService.isUserLoggedIn()) {
                   $location.path('/');
               }
           });


        // handle page refreshes
        $rootScope.repository = $cookieStore.get('repositoryCliRefresh') || {};
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
})();



