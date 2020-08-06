
(function () {
    'use strict';

    angular.module('ECCFranquia', ['common.core', 'common.ui', 'uiGmapgoogle-maps'])
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
        $urlRouterProvider.otherwise('/login');

        $stateProvider

            .state('login', {
                url: "/login",
                authenticate: false,
                templateUrl: "scripts/SPAFranquia/login/login.html",
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

            .state('Painel', {
                authenticate: true,
                url: "/",
                templateUrl: "scripts/SPAFranquia/home/index.html",
                controller: "indexCtrl",
                ncyBreadcrumb: {
                    label: 'Painel',
                    description: ''
                }
            })

            .state('recuperasenha', {
                authenticate: false,
                url: "/recuperasenha",
                templateUrl: "scripts/SPAFranquia/login/recuperasenha.html",
                controller: "recuperasenhaCtrl",
                ncyBreadcrumb: {
                    label: 'Recuperar Senha',
                    description: ''
                }
            })

            .state('termoUso', {
                authenticate: true,
                url: "/termoUso",
                templateUrl: "scripts/SPAFranquia/termoUso/termoUso.html",
                controller: "termoUsoCtrl"
            })

            .state('privacidade', {
                authenticate: true,
                url: "/privacidade",
                templateUrl: "scripts/SPAFranquia/termoUso/privacidade.html",
                controller: "privacidadeCtrl"
            })

            .state('perfil', {
                authenticate: true,
                url: "/perfil",
                templateUrl: "scripts/SPAFranquia/perfil/perfil.html",
                controller: "perfilCtrl"
            })

            .state('empresa', {
                authenticate: true,
                url: "/perfil/empresa",
                templateUrl: "scripts/SPAFranquia/perfil/empresa.html",
                controller: "empresaCtrl"
            })

            .state('configuracao', {
                authenticate: true,
                url: "/perfil/configuracao",
                templateUrl: "scripts/SPAFranquia/perfil/configuracao.html",
                controller: "configuracaoCtrl"
            })

            .state('pagamento', {
                authenticate: true,
                url: "/pagamento",
                templateUrl: "scripts/SPAFranquia/pagamento/pagamento.html",
                controller: "pagamentoCtrl"
            })

             .state('aprovacaopromocao', {
                 authenticate: true,
                 url: "/aprovacaopromocao",
                 templateUrl: "scripts/SPAFranquia/promocao/aprovacaopromocao.html",
                 controller: "aprovacaopromocaoCtrl"
             })

            .state('franqueados', {
                authenticate: true,
                url: "/franqueados",
                templateUrl: "scripts/SPAFranquia/franqueados/franqueados.html",
                controller: "franqueadosCtrl"
            })

            .state('dataCotacaoPedido', {
                authenticate: true,
                url: "/dataCotacaoPedido",
                templateUrl: "scripts/SPAFranquia/dataCotacao/dataCotacaoPedido.html",
                controller: "dataCotacaoPedidoCtrl"
            })

            .state('relacionarproduto', {
                authenticate: true,
                url: "/relacionarproduto",
                templateUrl: "scripts/SPAFranquia/produto/relacionarproduto.html",
                controller: "relacionarprodutoCtrl"
            })

            .state('fornecedor', {
                authenticate: true,
                url: "/fornecedor",
                templateUrl: "scripts/SPAFranquia/fornecedor/fornecedor.html",
                controller: "fornecedorCtrl"
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
        $rootScope.repository = $cookieStore.get('repositoryFraRefresh') || {};

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