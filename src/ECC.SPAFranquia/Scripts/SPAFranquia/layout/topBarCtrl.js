(function (app) {
    'use strict';

    app.controller('topBarCtrl', topBarCtrl);

    topBarCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService', 'Hub', 'SweetAlert', '$modal'];

    function topBarCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService, Hub, SweetAlert, $modal) {


        
           /**
        * @function isMobile
        * detecta se o useragent e um dispositivo mobile
        */
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

      

     

        $hub.on('cotacoesAtualizar', function () {
            $rootScope.$emit("cotacoesAtualizar");
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
                            registraFraTokensignalR(connection.id);
                        sessionStorage.TokenSignalRUserFra = connection.id;
                        break;
                    case $.signalR.connectionState.disconnected:
                        break;
                }
            })
            .fail(function () {
                console.error('fail: ', connection);
            });



        ////declaring the hub connection
        //var hub = new Hub('Notificacoes', {
        //    //client side methods
        //    listeners: {
          
        //    },

        //    stateChanged: function (state) {

        //        switch (state.newState) {
        //            case $.signalR.connectionState.connecting:
        //                //aqui é quando esta conectando no signalR
        //                //console.log(hub);
        //                break;
        //            case $.signalR.connectionState.connected:
        //                if ($rootScope.repository.loggedUser) {
        //                    registraFraTokensignalR(hub.connection.id);
        //                }
        //                sessionStorage.TokenSignalRUserFra = hub.connection.id;
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
        function registraFraTokensignalR(tokenSignalR) {


            $scope.data = {
                TokenSignalRUser: tokenSignalR,
                OrigemLogin: isMobile() == true ? 2 : 1
            };

            apiService.post("/api/usuario/atualizarTokenSignalR", $scope.data,
              registraFraTokensignalRCompleted,
              registraFraTokensignalRFailed);
        }

        function registraFraTokensignalRCompleted(response) {
            //console.log(response);
        }

        function registraFraTokensignalRFailed(response) {
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
      

 

        function openDetAvisos(avisoDet) {

            $scope.aviso = avisoDet;
            $modal.open({
                templateUrl: 'scripts/SPAFranquia/layout/modalDetAvisos.html',
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


        //ao clicar no botão fechar do browser
        $(window).on('unload', function (event) {

            membershipService.logout()
        });

        carregaAvisos();

    }

})(angular.module('common.core'));