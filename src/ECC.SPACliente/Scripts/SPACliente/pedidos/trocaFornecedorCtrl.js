
(function (app) {
    'use strict';

    app.controller('trocaFornecedorCtrl', trocaFornecedorCtrl);

    trocaFornecedorCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'SweetAlert', 'utilService', '$rootScope', '$filter'];

    function trocaFornecedorCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, SweetAlert, utilService, $rootScope, $filter) {

        $scope.clModalTrocaFornItem = clModalTrocaFornItem;
        $scope.salvaTrocaFornecedorDoItemCancelado = salvaTrocaFornecedorDoItemCancelado;
        $scope.idFormPagtoQuandoFornNaoAparece = 0;
        $scope.formaPagamento = formaPagamento;

        //1---------Fecha Modal-----------------------------------------
        function clModalTrocaFornItem() {
            $modalInstance.dismiss();
        }
        //1---------Fim------------------------------------------------


        //#region [ Carrega todos preços do item selecionado ]

        function loadFornecedorQueDeuPreco() {



            var itemProdutoId = angular.copy($scope.ItemProdutoId);
            var flgOutraMarcaTroca = angular.copy($scope.flgOutraMarcaTrocaItem);
            var pedidoId = angular.copy($scope.pedido.PedidoId);




            apiService.post('/api/trocaItemFornecedor/trocaFornecedorItem/' + pedidoId + '/' + itemProdutoId + '/' + flgOutraMarcaTroca, null,
                loadFornecedorQueDeuPrecoSucesso,
                loadFornecedorQueDeuPrecoFalha);
        }

        function loadFornecedorQueDeuPrecoSucesso(response) {

            $scope.listPrecoItemFornecedor = response.data.listPrecoItemFornecedor;


        }

        function loadFornecedorQueDeuPrecoFalha(response) {

            notificationService.displayError(response.data);
        }

        //#endregion



        $scope.trocaFornDoItem = function salvaTrocaFornecedorDoItem(dados) {
            var params = {

                Observacao: dados.Observacao,
                ItemId: angular.copy($scope.ItemIdTroca),
                FornecedorId: dados.FornecedorId,
                ValorItemFornecedor: dados.PrecoNegociadoUnit
            };
            apiService.post('/api/TrocaItemFornecedor/salvaTrocaFornecedor/', params,
                salvaTrocaFornecedorItemSucesso,
                salvaTrocaFornecedorItemFalha);

        };

        function salvaTrocaFornecedorItemSucesso(result) {

            if (result.data.success) {
                notificationService.displaySuccess("Mudanças do fornecedor do item realizada com sucesso!");
                clModalTrocaFornItem();
                $scope.recarregaPedido();
            }
        }

        function salvaTrocaFornecedorItemFalha(result) {
            notificationService.displayError(result.data);
        }


        //#region [ Troca de item cancelado para outro Fornecedor ]

        function salvaTrocaFornecedorDoItemCancelado(dados) {

            var params = {

                Observacao: dados.Observacao,
                ItemId: angular.copy($scope.ItemIdTroca),
                FornecedorId: dados.FornecedorId,
                ValorItemFornecedor: dados.PrecoNegociadoUnit,
                FornecedorPagtoIdChosen: $scope.idFormPagtoQuandoFornNaoAparece
            };

            apiService.post('/api/TrocaItemFornecedor/salvaTrocaMembroItemFornecedor/', params,
                salvaTrocaFornecedorSucesso,
                salvaTrocaFornecedorFalha);

        };

        function salvaTrocaFornecedorSucesso(response) {

            if (response.data.success) {
                notificationService.displaySuccess("Mudanças do fornecedor do item realizada com sucesso!");
                clModalTrocaFornItem();
                $scope.recarregaPedido();
            }
        }

        function salvaTrocaFornecedorFalha(response) {


            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayWarning(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //#endregion


        function formaPagamento(FormPagto, Item) {

            $scope.idFormPagtoQuandoFornNaoAparece = FormPagto.FormaPagtoId;
        }

        loadFornecedorQueDeuPreco();
    }

})(angular.module('ECCCliente'));