
(function (app) {
    'use strict';

    app.controller('detProdutoCtrl', detProdutoCtrl);

    detProdutoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'storeService'];

    function detProdutoCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, storeService) {

        $scope.close = close;
        $scope.cart = storeService.cart;
        $scope.addQuantidade = addQuantidade;

        function addQuantidade(sku, name, price, quantity, campo, imagem) {

            if (imagem != null && imagem != undefined && imagem != "") {
                var separandoImagem = imagem.split("/");

                if (separandoImagem.length == 1) {
                    imagem = $scope.caminhoImg + $scope.produtoPesq.categoria + '/' + $scope.produtoPesq.subcategoria + '/' + imagem;
                }
            }

            if (quantity == undefined || quantity == "" || quantity == " " || quantity == 0) {
                notificationService.displayInfo("DIGITE A QUANTIDADE !");
            } else {
                $scope.cart.addItem(sku, name, price, quantity, imagem);
                campo.QtdProduto = "";
                notificationService.displaySuccess(name + "<br> Quantidade Adicionada:  <b>" + quantity + "</b>");

                if ($scope.isMonitor)
                    close();
            }
        }

        //1---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------



        $scope.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };

    }

})(angular.module('ECCCliente'));