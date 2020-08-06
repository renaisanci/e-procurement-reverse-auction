
(function (app) {
    'use strict';

    app.controller('modalMotivoPromocaoCanceladaCtrl', modalMotivoPromocaoCanceladaCtrl);

    modalMotivoPromocaoCanceladaCtrl.$inject = ['$scope', '$modalInstance', 'apiService', 'notificationService', 'SweetAlert'];

    function modalMotivoPromocaoCanceladaCtrl($scope, $modalInstance, apiService, notificationService, SweetAlert) {

        $scope.close = close;
        $scope.inserirCancelamentoPromocao = inserirCancelamentoPromocao;
        var aprovacao = null;

        
        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------

        //1-------------Inserir Cancelamento Promocao------------------
        function inserirCancelamentoPromocao() {
            if ($scope.detahePedidoPromocao.DescMotivoCancelamento !== "" &&
                $scope.detahePedidoPromocao.DescMotivoCancelamento !== null) {
                cancelarPromocao($scope.detahePedidoPromocao);
            } else {
                notificationService.displayInfo('Campo motivo cancelamento não pode ser vazio, escreva o motivo para o cancelar!');
                $scope.detahePedidoPromocao.DescMotivoCancelamento = null;
            }
        }
        //1------------------------------------------------------------
        
       
        //2----------------Cancelar Pedido Promocao-------------------
        function cancelarPromocao(pedido) {

            aprovacao = false;

            apiService.post('/api/pedido/aprovarPedidoPromocao/' + aprovacao, pedido,
            cancelarPedidoPromocaoSucesso,
            cancelarPedidoPromocaoFalha);
        }

        function cancelarPedidoPromocaoSucesso(result) {

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

        function cancelarPedidoPromocaoFalha(response) {
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