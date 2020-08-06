(function (app) {
    'use strict';

    app.controller('painelCtrl', painelCtrl);

    painelCtrl.$inject = ['$scope', 'apiService', 'notificationService', 'Hub', '$rootScope'];

    function painelCtrl($scope, apiService, notificationService, Hub, $rootScope) {
        $scope.pageClass = 'page-home';
        $scope.loadingProdG = true;
        $scope.prodAgrupados = {};
        $scope.atualizaFlgCotado = atualizaFlgCotado;

        //02-------------Carrega grid de produtos agrupados dos pedidos que nao foram cotados---------
        function loadPedAgrupados() {
            apiService.get("/api/pedido/totalPedidoGroupAdm", null,
                loadPedAgrupadosCompleted,
                loadPedAgrupadosFailed);
        }
        
        function loadPedAgrupadosFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadPedAgrupadosCompleted(response) {
            $scope.prodAgrupados = response.data.prodsGroup;

            $scope.loadingProdG = false;
        }
        //---------------------------------------------------------------------

        //03-------------Carrega grid de produtos agrupados dos pedidos que nao foram cotados---------
        function atualizaFlgCotado() {
            apiService.get("/api/pedido/atualizaFlgCotado", null,
                atualizaFlgCotadoCompleted,
                atualizaFlgCotadoFailed);
        }

        function atualizaFlgCotadoCompleted(response) {
            notificationService.displaySuccess('Sucesso !!!');
            loadPedAgrupados();
            //chama function de outra controller
            $rootScope.$emit("loadTotalPed", {});
        }

        function atualizaFlgCotadoFailed(response) {
            notificationService.displayError(response.data);
        }
        //---------------------------------------------------------------------

        loadPedAgrupados();

        $rootScope.$on("recebeProdGroup", function (event, data) {
            $scope.prodAgrupados = data;
        });
    }

})(angular.module('ECCAdm'));