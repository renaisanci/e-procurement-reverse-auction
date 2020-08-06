(function (app) {
    'use strict';

    app.controller('modalPagtoPromocaoCtrl', modalPagtoPromocaoCtrl);

    modalPagtoPromocaoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$location', 'SweetAlert', '$filter'];

    function modalPagtoPromocaoCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $location, SweetAlert, $filter) {


		console.log($scope);

        $scope.close = close;
        $scope.inserirPedidoPromocao = inserirPedidoPromocao;

        //Array de produtos promocionais da controller shoppingCartCtrl.js
        var itensPed = angular.copy($scope.novoArrayPedPromocao);


        //1---------Inserir Pedido Promocional---------------------------------------------------------------
        function inserirPedidoPromocao() {

            //Array de produtos promocionais da controller shoppingCartCtrl.js
            itensPed = angular.copy($scope.novoArrayPedPromocao);
            var arrayProdPromocao = [];
            var arrayQtdAcimaPromocao = [];

            if (itensPed.length > 0) {
                for (var m = 0; m < itensPed.length; m++) {
                    if (itensPed[m].FormaPagtoId === undefined || itensPed[m].FormaPagtoId === null) {
                        notificationService.displayInfo('Selecione uma forma de pagamento para o produto "' + itensPed[m].name + '" .');
                        return;
                    }
                }
            }

            if ($scope.ItensPromocionais.length > 0) {
                for (var i = 0; i < $scope.ItensPromocionais.length; i++) {
                    for (var j = 0; j < itensPed.length; j++) {
                        if (itensPed[j].sku === $scope.ItensPromocionais[i].Id) {
                            if (itensPed[j].quantity > $scope.ItensPromocionais[i].QtdProdutos) {
                                itensPed[j].FornecedorId = $scope.ItensPromocionais[i].FornecedorId;
                                arrayQtdAcimaPromocao.push(itensPed[j]);
                            } else {
                                itensPed[j].FornecedorId = $scope.ItensPromocionais[i].FornecedorId;
                                arrayProdPromocao.push(itensPed[j]);
                            }
                        }
                    }
                }
            }

            if (arrayQtdAcimaPromocao.length > 0) {
                for (var k = 0; k < arrayQtdAcimaPromocao.length; k++) {
                    notificationService.displayInfo('DIMINUA A QUANTIDADE PARA O PRODUTO <b>' + arrayQtdAcimaPromocao[k].name.toUpperCase() + ' </b> POIS ESTÁ ACIMA DO ESTOQUE !');
                }

            } else {
                apiService.post('/api/pedido/inserirPedidoPromocional/' + $scope.EnderecoPadrao.Id, arrayProdPromocao,
                       inserirPedidoPromocaoLoadCompleted,
                       inserirPedidoPromocaoLoadFailed);
            }
        }

        function inserirPedidoPromocaoLoadCompleted(result) {

            var pedidosGerados = result.data.pedidosGerados;
            var pedidosNaoGerados = result.data.pedidosNaoGerados;
            var pedidosForaValidade = result.data.pedidosForaValidade;

            if (pedidosNaoGerados.length > 0) {

                $scope.cartPromocoes.clearItemsPromocao();
                //$scope.EnderecoPadraoPromocao = {};
                close();

                for (var x = 0; x < pedidosNaoGerados.length; x++) {
                    notificationService.displayInfo("QUANTIDADE INDISPONÍVEL PARA " + "(" + pedidosNaoGerados[x].name + ")");
                }

            }

            if (pedidosForaValidade.length > 0) {

                $scope.cartPromocoes.clearItemsPromocao();
                //$scope.EnderecoPadraoPromocao = {};
                close();

                for (var i = 0; i < pedidosForaValidade.length; i++) {
                    notificationService.displayInfo("PROMOÇÃO EXPIROU PARA O PRODUTO " + "(" + pedidosForaValidade[i].name + ")");
                }
            }

            if (pedidosGerados.length > 0) {

                $scope.cartPromocoes.clearItemsPromocao();
                close();

                for (var g = 0; g < pedidosGerados.length; g++) {
                    notificationService.displaySuccess("Pedido de número " + "(" + pedidosGerados[g].PedidoId + ") gerado com sucesso.");
                }
            }

            $location.path('/promocoes');
        }

        function inserirPedidoPromocaoLoadFailed(result) {
            notificationService.displayError(result.data);
        }
        //1--------------------------------------------------------------------------------------------------

        //2----------Calcula Valor Total do Pedido------------------------------------------------------------
        function calcularValorTotalProdutos() {
            if ($scope.ItensPromocionais.length > 0) {
                for (var x = 0; x < $scope.ItensPromocionais.length; x++) {
                    for (var z = 0; z < itensPed.length; z++) {
                        if (itensPed[z].sku === $scope.ItensPromocionais[x].Id) {
                            var precoPromocao = $scope.ItensPromocionais[x].PrecoPromocao.replace(",", ".");
                            $scope.ItensPromocionais[x].Valor = precoPromocao * itensPed[z].quantity;
                            $scope.ItensPromocionais[x].ValorPedDesconto = precoPromocao * itensPed[z].quantity;
                            break;
                        }
                    }
                }
            }
        }
        //2---------------------------------------------------------------------------------------------------

        //2---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss();
        }
        //2------------------------------------------

        calcularValorTotalProdutos();
    }

})(angular.module('ECCCliente'));