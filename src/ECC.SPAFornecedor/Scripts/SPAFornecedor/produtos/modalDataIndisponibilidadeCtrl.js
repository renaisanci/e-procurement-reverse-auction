
(function (app) {
    'use strict';

    app.controller('modalDataIndisponibilidadeCtrl', modalDataIndisponibilidadeCtrl);

    modalDataIndisponibilidadeCtrl.$inject = ['$scope', '$modalInstance', 'notificationService', 'items'];

    function modalDataIndisponibilidadeCtrl($scope, $modalInstance, notificationService, items) {

        $scope.salvarDataDisponibilidadeProduto = salvarDataDisponibilidadeProduto;
        $scope.habilitaDesabilitaDatas = habilitaDesabilitaDatas;
        $scope.close = close;
        var produtosDisponiveis = items;

        $scope.disponibilidadeProd.InicioIndisponibilidade = undefined;
        $scope.disponibilidadeProd.FimIndisponibilidade = undefined;
        $scope.disponibilidadeProd.IndisponivelPermanente = false;
        $scope.habilitaDatas = false;

        //0---------Fecha Modal--------------------------------
        function close() {
            if ($scope.disponibilidadeProd.InicioIndisponibilidade == undefined || $scope.disponibilidadeProd.FimIndisponibilidade == undefined) {
                $scope.inserirDeletarDisponibilidadeProd(true, produtosDisponiveis);
            }
            $modalInstance.dismiss('cancel');
        }
        //0----------------------------------------------------


        //0---------Fecha Modal--------------------------------
        function fecharSalvar() {
            $modalInstance.dismiss('cancel');
        }
        //0----------------------------------------------------


        //1------------Salvar Indisponibilidade de Produto------
        function salvarDataDisponibilidadeProduto() {

            if (validarDataIndisponibilidadeProduto()) {

                  produtosDisponiveis.InicioIndisponibilidade = $scope.disponibilidadeProd.InicioIndisponibilidade;
                  produtosDisponiveis.FimIndisponibilidade = $scope.disponibilidadeProd.FimIndisponibilidade;
                  produtosDisponiveis.IndisponivelPermanente = $scope.disponibilidadeProd.IndisponivelPermanente;

                  $scope.inserirDeletarDisponibilidadeProd(true, produtosDisponiveis);
                  fecharSalvar();
            }
        }
        //1-----------------------------------------------------



        //2------Validadar Datas--------------------------------
        function validarDataIndisponibilidadeProduto() {

            if (!$scope.disponibilidadeProd.IndisponivelPermanente) {
                if ($scope.disponibilidadeProd.InicioIndisponibilidade > $scope.disponibilidadeProd.FimIndisponibilidade) {
                    notificationService.displayInfo('Data Início não pode ser maior que Data Fim !');
                    return false;
                } else if ($scope.disponibilidadeProd.FimIndisponibilidade < $scope.disponibilidadeProd.InicioIndisponibilidade) {
                    notificationService.displayInfo('Data Fim não pode ser maior que Data Início !');
                    return false;
                } else if ($scope.disponibilidadeProd.InicioIndisponibilidade == undefined) {
                    notificationService.displayInfo('Data Início não pode ser vazia !');
                    return false;
                } else if ($scope.disponibilidadeProd.FimIndisponibilidade == undefined) {
                    notificationService.displayInfo('Data Fim não pode ser vazia !');
                    return false;
                }
            }
            return true;
        }
        //2-----------------------------------------------------

        //3------- 
        function habilitaDesabilitaDatas() {
            if ($scope.disponibilidadeProd.IndisponivelPermanente) {
                $scope.disponibilidadeProd.InicioIndisponibilidade = undefined;
                $scope.disponibilidadeProd.FimIndisponibilidade = undefined;
            }
            $scope.habilitaDatas = $scope.disponibilidadeProd.IndisponivelPermanente;
        }
        //3------------------------------------------------------

    }

})(angular.module('ECCFornecedor'));