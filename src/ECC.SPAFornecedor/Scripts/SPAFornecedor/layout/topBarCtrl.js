(function (app) {
    'use strict';

    app.controller('topBarCtrl', topBarCtrl);

    topBarCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService', 'Hub', 'SweetAlert', '$modal'];

    function topBarCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService, Hub, SweetAlert, $modal) {


     // function isMobile
     // detecta se o useragent e um dispositivo mobile
    
        function isMobile() {
            var userAgent = navigator.userAgent.toLowerCase();
            if (userAgent.search(/(android|iPad|iPhone|iPod)/i) != -1)
                return true;
        }

        $scope.totalPed = 0;
        $scope.openDetAvisos = openDetAvisos;

		var connection = $.hubConnection(apiService.identificaAmbiente() + '/signalr', { useDefaultPath: false });

        //connection.logging = true;
		var $hub = connection.createHubProxy('Notificacoes');

        $hub.on('notificaNovaCotacao', function (cotacaoId) {
            consultaPedidoCotacaoForn(cotacaoId);
            carregaAvisos();
            $rootScope.$apply();
		});
        
        $hub.on('carregaAvisosFornecedor', function () {
            carregaAvisos();
            $rootScope.$apply();
        });
        
        $hub.on('cotacaoNovoPreco', function (cotacaoId, dtFechamento, cotacaoPeds) {
          //  console.log(cotacaoId, dtFechamento, cotacaoPeds);
            $rootScope.$emit("cotacaoNovoPreco", cotacaoId, dtFechamento, cotacaoPeds);
            $rootScope.$apply();
        });

        $hub.on('cotacoesAtualizar', function () {
            $rootScope.$emit("cotacoesAtualizar");
            $rootScope.$apply();
        });

        $hub.on('notificaMembroSolicitaFornecedor', function (nomeMembro) {
            SweetAlert.swal({
                title: "Uma nova solicitação chegou!",
                text: "Corra, faça a análise do cadastro do " + nomeMembro + ", dê o aceite e potencialize ainda mais suas vendas.",
                type: "warning",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            });
            carregaAvisos();
            $scope.$apply();
        });

        $hub.on('lembreteFornecedorAceiteMembros', function () {
            SweetAlert.swal({
                title: "Existe membro pendente de aceite!",
                text: "Corra, faça a análise, dê o aceite e potencialize ainda mais suas vendas.",
                type: "warning",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            });

            $scope.$apply();
        });

        connection.start()
            .done(function () {
                //console.log('connection', connection);
                switch (connection.state) {
					case $.signalR.connectionState.connecting:
						console.log("connecting" + connection.id);
                        break;
					case $.signalR.connectionState.connected:
						if ($rootScope.repository.loggedUser)
							registraFornTokensignalR(connection.id);
						sessionStorage.TokenSignalRUserForn = connection.id;
						break;
                    case $.signalR.connectionState.reconnecting:
                        if ($rootScope.repository.loggedUser)
                            registraFornTokensignalR(connection.id);
                        sessionStorage.TokenSignalRUserForn = connection.id;
                        break;
					case $.signalR.connectionState.disconnected:
						console.log("disconnected");
                        break;
                }
            })
            .fail(function () {
                console.error('fail: ', connection);
            });

        //declaring the hub connection
        //var hub = new Hub('Notificacoes', {
        //    //client side methods
        //    listeners: {
        //        'notificaNovaCotacao': function (cotacaoId) {

        //            consultaPedidoCotacaoForn(cotacaoId);
        //            carregaAvisos();
        //            $rootScope.$apply();
        //        },

        //        'cotacaoNovoPreco': function (cotacaoId, dtFechamento, cotacaoPeds) {
        //            $rootScope.$emit("cotacaoNovoPreco", cotacaoId, dtFechamento, cotacaoPeds);
        //            $rootScope.$apply();
        //        },

        //        'cotacoesAtualizar': function () {
        //            $rootScope.$emit("cotacoesAtualizar");
        //            $rootScope.$apply();
        //        },

        //        'notificaMembroSolicitaFornecedor': function (nomeMembro) {

        //            SweetAlert.swal({
        //                title: "Uma nova solicitação chegou!",
        //                text: "Corra e aceite o membro " + nomeMembro + " e potencialize ainda mais suas vendas..",
        //                type: "warning",
        //                confirmButtonColor: "#DD6B55",
        //                confirmButtonText: "Ok"
        //            });
        //            carregaAvisos();
        //            $scope.$apply();
        //        }
        //    },

        //    stateChanged: function (state) {

        //        switch (state.newState) {
        //            case $.signalR.connectionState.connecting:
        //                //aqui é quando esta conectando no signalR
        //                //console.log(hub);
        //                break;
        //            case $.signalR.connectionState.connected:
        //                if ($rootScope.repository.loggedUser) {
        //                    registraFornTokensignalR(hub.connection.id);
        //                }
        //                sessionStorage.TokenSignalRUserForn = hub.connection.id;
        //                //aqui é quando esta conectado no signalR
        //                break;
        //            case $.signalR.connectionState.reconnecting:
        //                //aqui é quando esta reconectando no signalR
        //                break;
        //            case $.signalR.connectionState.disconnected:
        //                hub.connection.id = sessionStorage.tokenSignal;
        //                //aqui é quando esta disconectando no signalR
        //                break;


        //        }
        //    },
        //    rootPath: apiService.identificaAmbiente() + '/signalr'
        //});

        // 03---------------Registra o tokenSignalR para o usuário logado----
        function registraFornTokensignalR(tokenSignalR) {


            $scope.data = {
                TokenSignalRUser: tokenSignalR,
                OrigemLogin: isMobile() == true ? 2 : 1
            };

            apiService.post("/api/usuario/atualizarTokenSignalR", $scope.data,
                registraFornTokensignalRCompleted,
                registraFornTokensignalRFailed);
        }

        function registraFornTokensignalRCompleted(response) {
            //console.log(response);
        }

        function registraFornTokensignalRFailed(response) {
            console.error(response);
        }
        // 03----------------------------------------------------------------


        // 01--------Carrega as notificações do usuário e da empresa do mesmo-----------

        function carregaAvisos() {
            var config = {
                params: {
                    perfilModulo: 4
                }
            };

            if (membershipService.isUserLoggedIn())
                apiService.get('/api/avisos/avisosUsuarioEmpresa', config,
                    loadAvisosSucesso,
                    loadAvisosFailed);
        }

        function loadAvisosSucesso(response) {
            $scope.Avisos = response.data.avisos;

            $scope.TotalAvisos = response.data.totalAvisos;
        }

        function loadAvisosFailed(response) {
            notificationService.displayError(response.data);
        }
        // 01---------------------------------------------------------------------------



        // 04---------------Consulta se o fornecedor tem algum pedido na cotação----
        function consultaPedidoCotacaoForn(cotacaoId) {

            var config = {
                params: {
                    cotacaoId: cotacaoId
                }
            };

            apiService.get("/api/cotacao/verificaSeCotacaoForn", config,
                consultaPedidoCotacaoFornCompleted,
                consultaPedidoCotacaoFornFailed);
        }

        function consultaPedidoCotacaoFornCompleted(response) {
            $scope.totalPed = response.data;

            if ($scope.totalPed > 0) {
                SweetAlert.swal({
                    title: "NOVA COTAÇÃO!",
                    text: "CORRA, NÃO PERCA TEMPO, DÊ SEU PREÇO :)",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#87b87f",
                    confirmButtonText: "Ir Agora",
                    cancelButtonText: "Ir Depois ",
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $location.path('/cotacoes');
                        } else {
                            // Não confirmar não faça nada nesse caso
                        }
                    });
            }

        }

        function consultaPedidoCotacaoFornFailed(result) {


        }

        // 04-----------------------------------------------------------------------

        function openDetAvisos(avisoDet) {

            $scope.aviso = avisoDet;
            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/layout/modalDetAvisos.html',
                controller: 'modalDetAvisosCtrl',
                backdrop: 'static',
                scope: $scope,
                size: 'md'
            }).result.then(function ($scope) {

                //console.log("Modal Closed!!!");

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        }

        //05---é chamado pela controller modalDetAvisosCtrl.js-------------
        $rootScope.$on("reloadAvisosForn", function () {
            carregaAvisos();
        });
        //05-----------------------------------------------------------------

        //Chamado por uma controller do fornecedor
        $rootScope.$on('carregaAvisos', function () {
            carregaAvisos();
            $rootScope.$apply();
        });


        //ao clicar no botão fechar do browser
        $(window).on('unload', function (event) {

            membershipService.logout();
        });



        carregaAvisos();

    }

})(angular.module('common.core'));