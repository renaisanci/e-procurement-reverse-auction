
(function () {
    'use strict';

    angular.module('ECCFornecedor', ['common.core', 'common.ui', 'uiGmapgoogle-maps'])
        .config(config)
        .config(['uiGmapGoogleMapApiProvider', function (GoogleMapApi) {
            GoogleMapApi.configure({
                key: 'AIzaSyCVtn53qNj_IgWVrRH-yco1qD4hx7fzsxY',
                // v: '3.20',
                libraries: 'weather,geometry,visualization'
            });
        }])
        .run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $urlRouterProvider
            .otherwise('/login');
        $stateProvider

            .state('login', {
                url: "/login",
                authenticate: false,
                templateUrl: "scripts/SPAFornecedor/login/login.html",
                controller: "loginCtrl",
                reload: true,
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
            }).state('Painel', {
                authenticate: true,
                url: "/",
                templateUrl: "scripts/SPAFornecedor/home/index.html",
                controller: "indexCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: 'Painel',
                    description: ''
                }
            }).state('Cotacoes', {
                authenticate: true,
                url: "/cotacoes",
                templateUrl: "scripts/SPAFornecedor/cotacoes/cotacoes.html",
                controller: "cotacoesCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: '<a href="#/cotacoes">Cotações</a>',
                    description: ''
                }
            })
            .state('Cotacao', {
                authenticate: true,
                url: "/cotacao",
                params: {
                    cotacaoSelect: null


                },
                templateUrl: "scripts/SPAFornecedor/cotacoes/cotacao.html",
                controller: "cotacaoCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: '<a href="#/cotacao/:id">Cotação</a>',
                    description: ''
                }
			})

			.state('Cotacaohist', {
				authenticate: true,
				url: "/cotacaohist",
				params: {
					cotacaoSelect: null


				},
				templateUrl: "scripts/SPAFornecedor/cotacoes/cotacaoHist.html",
				controller: "cotacaoHistCtrl",
				reload: true,
				ncyBreadcrumb: {
					label: '<a href="#/cotacao/:id">Cotação</a>',
					description: ''
				}
			})
            .state('Pedidos', {
                authenticate: true,
                url: "/pedidos",
                templateUrl: "scripts/SPAFornecedor/pedidos/pedidos.html",
                controller: "pedidosCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: '<a href="#/pedidos">Pedidos</a>',
                    description: ''
                }
			})

			.state('Pedidospessoafisica', {
				authenticate: true,
				url: "/pedidospessoafisica",
				templateUrl: "scripts/SPAFornecedor/pedidos/PedidosPF.html",
				controller: "pedidosPFCtrl",
				reload: true,
				ncyBreadcrumb: {
					label: '<a href="#/PedidosPF">Pedidos PF</a>',
					description: ''
				}
			})
            .state('PedidosGerados', {
                authenticate: true,
                url: "/pedidosGerados",
                templateUrl: "scripts/SPAFornecedor/pedidos/pedidosGerados.html",
                controller: "pedidosGeradosCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: '<a href="#/pedidosGerados">Pedidos Gerados</a>',
                    description: ''
                }
			})
			.state('PedidosGeradospf', {
				authenticate: true,
				url: "/pedidosGeradospf",
				templateUrl: "scripts/SPAFornecedor/pedidos/pedidosGeradosPF.html",
				controller: "pedidosGeradosPFCtrl",
				reload: true,
				ncyBreadcrumb: {
					label: '<a href="#/pedidosGerados">Pedidos Gerados PF</a>',
					description: ''
				}
			})
            .state('recuperasenha', {
                authenticate: false,
                url: "/recuperasenha",
                templateUrl: "scripts/SPAFornecedor/login/recuperasenha.html",
                controller: "recuperasenhaCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: 'Recuperar Senha',
                    description: ''
                }
            })

            .state('membro', {
                authenticate: true,
                url: "/membro",
                templateUrl: "scripts/SPAFornecedor/membro/solicitafornecedor.html",
                controller: "solicitafornecedorCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: 'Membro Solicita Fornecedor',
                    description: ''
                }
            })

            .state('cadpromocoes', {
                authenticate: true,
                url: "/cadpromocoes",
                templateUrl: "scripts/SPAFornecedor/promocoes/cadpromocoes.html",
                controller: "cadpromocoesCtrl",
                reload: true,
                ncyBreadcrumb: {
                    label: 'Cadastro de Promoções',
                    description: 'Cadastro de Promoções'
                }
            })
            .state('termoUso', {
                authenticate: true,
                url: "/termoUso",
                templateUrl: "scripts/SPAFornecedor/termoUso/termoUso.html",
                controller: "termoUsoCtrl",
                reload: true
            })

            .state('privacidade', {
                authenticate: true,
                url: "/privacidade",
                templateUrl: "scripts/SPAFornecedor/termoUso/privacidade.html",
                controller: "privacidadeCtrl",
                reload: true
            })

            .state('perfil', {
                authenticate: true,
                url: "/perfil",
                templateUrl: "scripts/SPAFornecedor/perfil/perfil.html",
                controller: "perfilCtrl",
                reload: true
            })

            .state('empresa', {
                authenticate: true,
                url: "/perfil/empresa",
                templateUrl: "scripts/SPAFornecedor/perfil/empresa.html",
                controller: "empresaCtrl",
                reload: true
            })

            .state('configuracao', {
                authenticate: true,
                url: "/perfil/configuracao",
                templateUrl: "scripts/SPAFornecedor/perfil/configuracao.html",
                controller: "configuracaoCtrl",
                reload: true
            })

			.state('pagamento', {
				cache: false,
                authenticate: true,
                url: "/pagamento",
                templateUrl: "scripts/SPAFornecedor/pagamento/pagamento.html",
                controller: "pagamentoCtrl",
                reload: true
            })

            .state('indisponibilidade', {
                authenticate: true,
                url: "/indisponibilidade",
                templateUrl: "scripts/SPAFornecedor/produtos/indisponibilidade.html",
                controller: "indisponibilidadeCtrl",
                reload: true
            })

            .state('fornecedorproduto', {
                authenticate: true,
                url: "/fornecedorproduto",
                templateUrl: "scripts/SPAFornecedor/produtos/fornecedorProduto.html",
                controller: "fornecedorProdutoCtrl",
                reload: true
            })

            .state('modalDetPedido', {
				authenticate: true,
				cache: false,
			//	params: { cache: null },
                url: "/modalDetPedido",
                templateUrl: "scripts/SPAFornecedor/pedidos/modalMotivoPedidoCancelado.html",
                controller: "modalDetPedidoGeradoCtrl",
                reload: true
			})


			.state('modalFornecedorProdutoQuantidade', {
				authenticate: true,
				cache: false,
				//	params: { cache: null },
				url: "/modalFornecedorProdutoQuantidade",
				templateUrl: "scripts/SPAFornecedor/produtos/modalFornecedorProdutoQuantidade.html",
				controller: "modalFornecedorProdutoQuantidadeCtrl",
				reload: true
			})

            .state('aprovacaopromocao', {
                authenticate: true,
                url: "/aprovacaopromocao",
                templateUrl: "scripts/SPAFornecedor/pedidos/aprovacaopromocao.html",
                controller: "aprovacaopromocaoCtrl",
                reload: true
			})
		    .state('aprovacaopromocaopf', {
			authenticate: true,
			url: "/aprovacaopromocaopf",
			templateUrl: "scripts/SPAFornecedor/pedidos/aprovacaopromocaoPF.html",
			controller: "aprovacaopromocaoPFCtrl",
			reload: true
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
        $rootScope.repository = $cookieStore.get('repositoryFornRefresh') || {};
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

    // isAuthenticated.$inject = ['membershipService', '$rootScope', '$location'];
    //function isAuthenticated(membershipService, $rootScope, $location) {
    //    if (!membershipService.isUserLoggedIn()) {
    //        $rootScope.previousState = $location.path();
    //        $location.path('/login');
    //    }
    //}

})();