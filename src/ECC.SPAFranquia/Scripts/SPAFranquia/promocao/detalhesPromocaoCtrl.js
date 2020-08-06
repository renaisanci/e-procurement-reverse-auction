(function (app) {
    'use strict';

    app.controller('detalhesPromocaoCtrl', detalhesPromocaoCtrl);

    detalhesPromocaoCtrl.$inject = ['$scope', '$modalInstance', '$rootScope', 'apiService', 'notificationService'];

    function detalhesPromocaoCtrl($scope, $modalInstance, $rootScope, apiService, notificationService) {

        $scope.close = close;
        $scope.salvarAprovacao = salvarAprovacao;
        $scope.cancelarPromocao = cancelarPromocao;
        
        //1---------Fecha Modal--------------------------
        function close() {
            $modalInstance.dismiss();
        }
        //1----------------------------------------------

        //2---------Salvar Aprovação---------------------
        function salvarAprovacao() {

            $scope.novoProduto.Fornecedor = null;

            apiService.post('/api/produtopromocional/aprovaCancelaPromocaoFranquia/' + true, $scope.novoProduto,
                salvarAprovacaoLoadCompleted,
                salvarAprovacaoLoadFailed);
        }

        function salvarAprovacaoLoadCompleted(result) {

            notificationService.displaySuccess('Promoção aprovada com sucesso!');
            close();

            $scope.pesquisarCategoria();
            $scope.pesquisarProduto();
        }

        function salvarAprovacaoLoadFailed(result) {

            notificationService.displayError('Erro ao aprovar Promoção!\n' + result.data);
        }
        //2----------------------------------------------

        //3---------Cancelar Promoção---------------------
        function cancelarPromocao() {

            $scope.novoProduto.Fornecedor = null;

            apiService.post('/api/produtopromocional/aprovaCancelaPromocaoFranquia/' + false, $scope.novoProduto,
                salvarMotivoCancelamentoAprovacaoLoadCompleted,
                salvarMotivoCancelamentoAprovacaoLoadFailed);
        }

        function salvarMotivoCancelamentoAprovacaoLoadCompleted(result) {
            close();
            $scope.pesquisarProduto();      //método da da controller aprovacaopromocaoCtrl.js
            $scope.pesquisarCategoria();    //método da da controller aprovacaopromocaoCtrl.js

            notificationService.displaySuccess('Promoção cancelada com sucesso!');
        }

        function salvarMotivoCancelamentoAprovacaoLoadFailed(result) {
            notificationService.displayError('Erro ao cancelar Promoção!\n' + result.data);
        }
        //3-----------------------------------------------


    }

})(angular.module('ECCFranquia'));