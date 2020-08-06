
(function (app) {
    'use strict';

    app.controller('modalDetPromocaoCtrl', modalDetPromocaoCtrl);

    modalDetPromocaoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'storeService'];

    function modalDetPromocaoCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, storeService) {

        $scope.close = close;
        $scope.cart = storeService.cart;
        $scope.cartPromocoes = storeService.cartPromocoes;
        $scope.addQuantidadePromocao = addQuantidadePromocao;
        

      

        //1----------Adicionar Quantidade de Produtos Promocionais------------------
        function addQuantidadePromocao(sku, name, price, quantity, campo, imagem, obsFrete) {

            if (imagem !== null && imagem !== undefined && imagem !== "") {
                var separandoImagem = imagem.split("/");

                if (separandoImagem.length == 1) {
                    imagem = $scope.caminhoImg + $scope.produtoPesq.categoria + '/' + $scope.produtoPesq.subcategoria + '/' + imagem;
                }
            }
            if (quantity == undefined || quantity == "" || quantity == " " || quantity == 0) {
                notificationService.displayInfo("DIGITE A QUANTIDADE !");

            } else if (quantity < campo.produtoPesq.QtdMinVenda) {
                notificationService.displayInfo("AUMENTE A QUANTIDADE DESTE PRODUTO, POIS ESTÁ ABAIXO DA QUANTIDADE MÍNIMA !");

            } else if (quantity > campo.produtoPesq.QtdProdutos) {
                notificationService.displayInfo("QUANTIDADE ACIMA DO DISPONÍVEL PARA ESTE PRODUTO !");
                
            }else{
                var preco = price.replace(",", ".");
                $scope.cartPromocoes.addItemPromocao(sku, name, preco, quantity, imagem, obsFrete);
                campo.QtdProduto = "";
                campo.produtoPesq.QtdProdutos = campo.produtoPesq.QtdProdutos - quantity;
                notificationService.displaySuccess(name + "<br> Quantidade Adicionada:  <b>" + quantity + "</b>");

                if ($scope.isMonitor)
                    close();
            }
        }
        //1-------------------------------------------------------------------------

       

        //3---------Fecha Modal----------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //3------------------------------------------





    }

})(angular.module('ECCCliente'));