(function (app) {
    'use strict';

    app.controller('topBarCtrl', topBarCtrl);

    topBarCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService', 'Hub', 'SweetAlert', '$modal', 'storeService', '$interval'];

    function topBarCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService, Hub, SweetAlert, $modal, storeService, $interval) {
        $scope.openDetAvisos = openDetAvisos;

        // function isMobile
        // detecta se o useragent e um dispositivo mobile
	//	console.log($rootScope.repository.loggedUser);


        function isMobile() {
            var userAgent = navigator.userAgent.toLowerCase();
            if (userAgent.search(/(android|iPad|iPhone|iPod)/i) != -1)
                return true;
        }

        $scope.cart = storeService.cart;
		$scope.cartPromocoes = storeService.cartPromocoes;
		$scope.ListasComprasGravadas = $scope.ListasComprasGravadas;

        var connection = $.hubConnection(apiService.identificaAmbiente() + '/signalr', { useDefaultPath: false });
        //connection.logging = true;
        var $hub = connection.createHubProxy('Notificacoes');

        $hub.on('notificaFornecedorAceitouMembro', function (nomeFornecedor) {
            SweetAlert.swal({
                title: "SOLICITAÇÃO PARA TRABALHAR COM O FORNECEDOR " + nomeFornecedor + " ACEITA COM SUCESSO!",
                text: "Agora você terá mais chance de economizar..",
                type: "success",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            });
            $scope.$apply();
        });

        $hub.on('pesquisarProdutoPromocao', function (produtoPromocioanal) {
            $rootScope.$emit("pesquisarProdutoPromocao", produtoPromocioanal);
            $rootScope.$apply();
        });

        $hub.on('getProdGroup', function (prodsGroup, cam) {
            //chamar function da controller painelCtrl
            $rootScope.$emit("recebeProdGroup", prodsGroup, cam);
            $rootScope.$apply();
        });

        
        connection.start()
            .done(function () {
                //console.log('connection', connection);
                switch (connection.state) {
                    case $.signalR.connectionState.connecting:
                        break;
                    case $.signalR.connectionState.connected:
                    case $.signalR.connectionState.reconnecting:
                        if ($rootScope.repository.loggedUser)
                            registraMembTokensignalR(connection.id);
                        sessionStorage.TokenSignalRUserMemb = connection.id;
                        break;
                    case $.signalR.connectionState.disconnected:
                        //membershipService.logout();
                        break;
                }
            })
            .fail(function () {
                console.error('fail: ', connection);
            });

        // declaring the hub connection
        //var hub = new Hub('Notificacoes', {
        //    //client side methods




        //    listeners: {
        //'notificaFornecedorAceitouMembro': function (nomeFornecedor) {

        //    SweetAlert.swal({
        //        title: "SOLICITAÇÃO PARA TRABALHAR COM O FORNECEDOR " + nomeFornecedor + " ACEITA COM SUCESSO!",
        //        text: "Agora você terá mais chance de economizar..",
        //        type: "success",
        //        confirmButtonColor: "#DD6B55",
        //        confirmButtonText: "Ok"
        //    });
        //    $scope.$apply();
        //},

        ////Localizado em promocoesCtrl.js
        //'pesquisarProdutoPromocao': function (produtoPromocioanal) {
        //    $rootScope.$emit("pesquisarProdutoPromocao", produtoPromocioanal);
        //    $rootScope.$apply();
        //},



        //'getProdGroup': function (prodsGroup, cam) {
        //    //chamar function da controller painelCtrl
        //    $rootScope.$emit("recebeProdGroup", prodsGroup, cam);
        //    $rootScope.$apply();
        //}
        //    },






        //    stateChanged: function (state) {




        //        switch (state.newState) {
        //            case $.signalR.connectionState.connecting:
        //                //aqui é quando esta conectando no signalR
        //                //console.log(hub);
        //                break;
        //            case $.signalR.connectionState.connected:
        //            case $.signalR.connectionState.reconnecting:
        //                if ($rootScope.repository.loggedUser) {
        //                    registraMembTokensignalR(hub.connection.id);
        //                }
        //                sessionStorage.TokenSignalRUserMemb = hub.connection.id;
        //                //aqui é quando esta conectado no signalR
        //                break;
        //            case $.signalR.connectionState.disconnected:
        //                //hub.connection.id = sessionStorage.tokenSignal;
        //                //aqui é quando esta disconectando no signalR

        //                setTimeout(function () {
        //                    membershipService.logout();
        //                }, 1000);

        //                break;
        //        }
        //    },

        //    rootPath: apiService.identificaAmbiente() + '/signalr'
        //});
        // used to update the UI
        function updateTime() {
            element.text(dateFilter(new Date(), format));
        }
        // 03---------------Registra o tokenSignalR para o usuário logado----
        function registraMembTokensignalR(tokenSignalR) {

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
        // 03---------------------------------------------------------------------------

        // 04--------Carrega as notificações do usuário e da empresa do mesmo-----------

        function carregaAvisos() {
            var config = {
                params: {
                    perfilModulo: 3
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
        // 04---------------------------------------------------------------------------
        

        function openDetAvisos(avisoDet) {
            $scope.aviso = avisoDet;
            $modal.open({
                templateUrl: 'scripts/SPACliente/layout/modalDetAvisos.html',
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
        $rootScope.$on("reloadAvisosMem", function () {
            carregaAvisos();
        });
        //05-----------------------------------------------------------------
 
        //ao clicar no botão fechar do browser
        $(window).on('unload', function (event) {

            membershipService.logout();
            localStorage["atualiza"] = 'ok';
		});

        //================================carrrega lista de compras=============
		function usarListaCompras() {


			//if (localStorage.getItem("listaCompras")!==null)
   //             $scope.ListasComprasGravadas = JSON.parse(localStorage.getItem("listaCompras"));	

            apiService.get('/api/listaCompras/getListaCompras', null,
                 usarListaCompraSucesso,
                 usarListaCompraFailed);
        }

        function usarListaCompraSucesso(response) {
            $scope.ListasComprasGravadas = response.data;      
        }

        function usarListaCompraFailed(response) {
            notificationService.displayError(response.data);
        }

        //=======================================================================================

		$scope.irParaCarrinho = function (item) {

			storeService.cart.items = item;

			$location.path("/shoppingCart");

            apiService.get('/api/listaCompras/listaComprasById/' + item.Id, null,
                irParaCarrinhoSucesso,
                irParaCarrinhoFailed);


			//storeService.cart.items = item;
			//$location.path("/shoppingCart");
			//$rootScope.$emit("reloadCarrinhoPrincipal", {});
        };

        function irParaCarrinhoSucesso(response) {


            storeService.cart.items = response.data.listaCompras;
            localStorage["fornRemPedCot"] = JSON.stringify(response.data.listaRemoveFornecedores);

            $location.path("/shoppingCart");
            $rootScope.$emit("reloadCarrinhoPrincipal", {});

        }

        function irParaCarrinhoFailed(response) {
            notificationService.displayError(response.data);
        }


		$scope.removerListaCompras = function (item) {
			//var index = $scope.ListasComprasGravadas.indexOf(item);
			//$scope.ListasComprasGravadas.splice(index, 1);

			//localStorage.removeItem('listaCompras');

   //         localStorage.setItem("listaCompras", JSON.stringify($scope.ListasComprasGravadas));


          
			localStorage.removeItem('listaCompras');

            apiService.post('/api/listaCompras/listaComprasRemove/' + item.Id, null,
                listaComprasRemoveSucesso,
                listaComprasRemoveFailed);
            
		};


        function listaComprasRemoveSucesso(response) {
            usarListaCompras();
            notificationService.displaySuccess("Lista deletada com sucesso.");
        }

        function listaComprasRemoveFailed(response) {
            notificationService.displayError(response.data);
        }

		//05---é chamado pela controller listaComrpasCtrl.js-------------
		$rootScope.$on("reloadListaCompras", function () {
			usarListaCompras();
		});
        //05-----------------------------------------------------------------

        
		carregaAvisos();
		usarListaCompras();

    }

})(angular.module('common.core'));