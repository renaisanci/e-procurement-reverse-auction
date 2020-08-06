
(function (app) {
    'use strict';

    app.controller('modalMotivoPedidoCanceladoCtrl', modalMotivoPedidoCanceladoCtrl);

    modalMotivoPedidoCanceladoCtrl.$inject = ['$scope', '$modalInstance', 'apiService', 'notificationService', 'SweetAlert'];

    function modalMotivoPedidoCanceladoCtrl($scope, $modalInstance, apiService, notificationService, SweetAlert) {

        $scope.close = close;
        $scope.inserirCancelamentoPedido = inserirCancelamentoPedido;
        var aprovacao = null;

        
        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------

        //1-------------Inserir Cancelamento Promocao------------------
        function inserirCancelamentoPedido() {
            if ($scope.detahePedido.DescMotivoCancelamento !== "" &&
                $scope.detahePedido.DescMotivoCancelamento !== null) {
                cancelarPedido($scope.detahePedido);
            } else {
                notificationService.displayInfo('Campo motivo cancelamento não pode ser vazio, escreva o motivo para o cancelar!');
                $scope.detahePedido.DescMotivoCancelamento = null;
            }
        }
        //1------------------------------------------------------------
        
       
        //2----------------Cancelar Pedido Promocao-------------------
        function cancelarPedido(pedido) {
            aprovacao = false;
            var pedidoCancela = {

                PedidoId: pedido.PedidoId,
                DescMotivoCancelamento: pedido.DescMotivoCancelamento

            };


            apiService.post('/api/cotacao/aprovarPedido/' + aprovacao, pedidoCancela,
                cancelarPedidoLoadCompleted,
                cancelarPedidoLoadFailed);
        }

        function cancelarPedidoLoadCompleted(result) {

            close();

            if (result.data.success) {

                SweetAlert.swal({
                    title: "PEDIDO CANCELADO COM SUCESSO!",
                    text: "O membro será avisado sobre o cancelamento deste pedido.", //VALIDAR FRASE
                    type: "success",
                    html: true
                },
                    function () {
                       
                        $scope.loadCotacao();
                    });
            }

        }

        function cancelarPedidoLoadFailed(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2----------------------------------------------------------


    }

})(angular.module('ECCFornecedor'));