(function (app) {
    'use strict';

    app.controller('modalLanceFornecedorCtrl', modalLanceFornecedorCtrl);

    modalLanceFornecedorCtrl.$inject = ['$scope', '$modalInstance', 'apiService'];

    function modalLanceFornecedorCtrl($scope, $uibModalInstance, apiService) {


        $scope.close = close;

        //1----------------Fechar Modal------------------------------
        function close() {

            $uibModalInstance.dismiss();
        };
        //1----------------Fim Modal----------------------------------


        //2-----Carrega todos Status de Pedido -------------------
        function loadHistLancesFornecedorPorPedido() {
            var pedidoId = angular.copy($scope.pPedido.PedidoId);

            apiService.post('/api/historicolance/histLancesFornecedorPorPedido/' + pedidoId, null,
                    loadHistLancesFornecedorPorPedidoSucesso,
                    loadHistLancesFornecedorPorPedidoFalha);
        }

        function loadHistLancesFornecedorPorPedidoSucesso(response) {

            $scope.Pedido = response.data.lancesFornecedorPedido;

        }

        function loadHistLancesFornecedorPorPedidoFalha(response) {
            close();
            notificationService.displayError(response.data);
        }
        //2------------------------fim-----------------------------


        loadHistLancesFornecedorPorPedido();

    }

})(angular.module('ECCCliente'));