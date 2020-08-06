(function (app) {
    'use strict';

    app.controller('topBarCtrl', topBarCtrl);

    topBarCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService', 'Hub'];

    function topBarCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService, Hub) {

        $scope.totalPed = 0;
        $scope.totalAprovaPromocao = 0;

        var connection = $.hubConnection('/signalr', { useDefaultPath: false });
        //connection.logging = true;
        var $hub = connection.createHubProxy('Notificacoes');
        
        $hub.on('getTotalPed',
            function (totalPed) {
                $scope.totalPed = totalPed;
                $rootScope.$apply();
            });

        $hub.on('aprovarPromocao',
            function (totalAprovaPromocao) {
                $scope.totalAprovaPromocao = totalAprovaPromocao;
                $rootScope.$apply();
            });

        $hub.on('getProdGroup', function (prodsGroup) {
            //chamar function da controller indexCtrl
            $rootScope.$emit("recebeProdGroup", prodsGroup);
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
                            registraTokensignalR(connection.id);
                        sessionStorage.TokenSignalRUserForn = connection.id;
                        break;
                    case $.signalR.connectionState.disconnected:
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
        //        'getTotalPed': function(totalPed) {
        //            $scope.totalPed = totalPed;
        //            $rootScope.$apply();
        //        },


        //        'aprovarPromocao': function (totalAprovaPromocao) {
        //            $scope.totalAprovaPromocao = totalAprovaPromocao;
        //            $rootScope.$apply();
        //        },

        //        'getProdGroup': function(prodsGroup) {

        //            //chamar function da controller indexCtrl
        //            $rootScope.$emit("recebeProdGroup", prodsGroup);

        //            $rootScope.$apply();
        //        }
        //    },


        //    stateChanged: function(state) {
        //        switch (state.newState) {
        //            case $.signalR.connectionState.connecting:                 
        //                //aqui é quando esta conectando no signalR
        //                //console.log(hub);
        //                break;
        //            case $.signalR.connectionState.connected:
        //                if ($rootScope.repository.loggedUser) {
        //                    registraTokensignalR(hub.connection.id);
        //                }
        //                sessionStorage.TokenSignalRUser = hub.connection.id;                      
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
        //    }

        //});


        //01---------------Carrega o total de pedidos para ser cotado----
        function loadTotalPed() {

            apiService.get("/api/pedido/totalPedido", null,
                loadTotalPedCompleted,
                loadTotalPedFailed);
        }

        function loadTotalPedFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadTotalPedCompleted(result) {

            $scope.totalPed = result.data.totalPed;
        }
        //01----------------------------------------------------------------


        if ($rootScope.repository.loggedUser) {
            loadTotalPed();
            loadTotalProdutoPromocao();
        }

        //02---------------é chamado pela controller topBarCtrl----
        $rootScope.$on("loadTotalPed", function () {
            $scope.loadTotalPed = loadTotalPed();

        });

        //02---------------Carrega o total de pedidos para ser cotado----




        // 03---------------Registra o tokenSignalR para o usuário logado----
        function registraTokensignalR(tokenSignalR) {


            $scope.data = {

                TokenSignalRUser: tokenSignalR


            };



            apiService.post("/api/usuario/atualizarTokenSignalR", $scope.data,
                registraTokensignalRCompleted,
                registraTokensignalRFailed);
        }

        function registraTokensignalRCompleted(response) {

        }

        function registraTokensignalRFailed(result) {


        }
        // 03----------------------------------------------------------------


        //04---------------Carrega total de Promoções para aprovação---------
        function loadTotalProdutoPromocao() {

            apiService.get("/api/produtopromocional/totalProdutoPromocao", null,
                loadTotalProdutoPromocaoCompleted,
                loadTotalProdutoPromocaoFailed);
        }

        function loadTotalProdutoPromocaoCompleted(result) {
            $scope.totalAprovaPromocao = result.data;
        }

        function loadTotalProdutoPromocaoFailed(result) {
            notificationService.displayError(result.data);
        }
        //04-----------------------------------------------------------------



        //05---é chamado pela controller detalhespromocaoCtrl.js-------------
        $rootScope.$on("loadTotalProdutoPromocao", function () {
            $scope.loadTotalProdutoPromocao = loadTotalProdutoPromocao();

        });
        //05-----------------------------------------------------------------



        //ao clicar no botão fechar do browser
        $(window).on('unload', function (event) {

            membershipService.logout()
        });

    }

})(angular.module('common.core'));