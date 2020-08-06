
(function (app) {
    'use strict';
    app.controller('dataCotacaoPedidoCtrl', dataCotacaoPedidoCtrl);

    dataCotacaoPedidoCtrl.$inject = ['$scope', 'apiService', '$rootScope', 'notificationService', 'SweetAlert', '$modal'];

    function dataCotacaoPedidoCtrl($scope, apiService, $rootScope, notificationService, SweetAlert, $modal) {

        $scope.pageClass = 'page-DataCotacao';

        $scope.inserirSemana = inserirSemana;
        var semana = [];
        $scope.arrayDiasSemana = [
            { Id: 1, DescDiaSemana: 'Segunda' },
            { Id: 2, DescDiaSemana: 'Terça' },
            { Id: 3, DescDiaSemana: 'Quarta' },
            { Id: 4, DescDiaSemana: 'Quinta' },
            { Id: 5, DescDiaSemana: 'Sexta' },
            { Id: 6, DescDiaSemana: 'Sábado' }
        ];


        //01-----Grava Dias da semana para cotar pedidos da franquia -------
        function inserirSemana() {

            $scope.semanasSect = [];

            if ($scope.arrayDiasSemana.length > 0) {

                angular.forEach($scope.arrayDiasSemana, function (semana) {
                    if (semana.selected) {
                        $scope.semanasSect.push(semana.Id);
                    }
                });

                apiService.post('/api/franquia/inserirSemanaCotacao/' + $scope.pedidoDataObrigatorio, $scope.semanasSect,
                    inserirSemanaSucesso,
                    inserirSemanaFalha);
            }
        }

        function inserirSemanaSucesso(result) {

            notificationService.displaySuccess("Gravado com sucesso");
            $scope.arrayDiasSemana = [
          { Id: 1, DescDiaSemana: 'Segunda' },
          { Id: 2, DescDiaSemana: 'Terça' },
          { Id: 3, DescDiaSemana: 'Quarta' },
          { Id: 4, DescDiaSemana: 'Quinta' },
          { Id: 5, DescDiaSemana: 'Sexta' },
          { Id: 6, DescDiaSemana: 'Sábado' }
            ];

            semana = $scope.semanasSect;
            $scope.pedidoDataObrigatorio = result.data.PedidoDataObrigatorio;

            //Marca os dias da Semana já cadastrados pela Franquia
            for (var i = 0; i < semana.length; i++) {
                for (var j = 0; j < $scope.arrayDiasSemana.length; j++) {
                    if (semana[i] === $scope.arrayDiasSemana[j].Id) {
                        $scope.arrayDiasSemana[j].selected = true;
                        $scope.arrayDiasSemana[j].checado = true;
                    }
                }

            }


        }

        function inserirSemanaFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //01----------------------------------------------------------------

        //02-----Carrega dias da Semana Franquia----------------------------
        function carregaDiasSemanaFranquia() {

            apiService.get('/api/franquia/getDataFranquiaAdm', null,
                loadCarregaDiasSemanaCompleted,
                loadCarregaDiasSemanaFailed);
        }

        function loadCarregaDiasSemanaCompleted(result) {

            semana = result.data.diasCotacao;
            $scope.pedidoDataObrigatorio = result.data.PedidoDataObrigatorio;

            //Marca os dias da Semana já cadastrados pela Franquia
            for (var i = 0; i < semana.length; i++) {
                for (var j = 0; j < $scope.arrayDiasSemana.length; j++) {
                    if (semana[i] === $scope.arrayDiasSemana[j].Id) {
                        $scope.arrayDiasSemana[j].selected = true;
                        $scope.arrayDiasSemana[j].checado = true;
                    }
                }

            }

        }

        function loadCarregaDiasSemanaFailed(result) {
            notificationService.displayError(result.data);
        }

        //02-----------------------------------------------------------------

        carregaDiasSemanaFranquia();


    }

})(angular.module('ECCFranquia'));