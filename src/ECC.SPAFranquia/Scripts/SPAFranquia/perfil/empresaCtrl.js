
(function (app) {
    'use strict';

    app.controller('empresaCtrl', empresaCtrl);

    empresaCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService'];

    function empresaCtrl($scope, $timeout, apiService, notificationService) {

        $scope.novaFranquia = {};

        $scope.atualizarFranquia = atualizarFranquia;

        //-----Carregar dados da Franquia------------

        function carregarFranquia() {
            apiService.get('/api/franquia/perfil', null,
                carregarFranquiaSucesso,
                carregarFranquiaFalha);
        }

        function carregarFranquiaSucesso(response) {
            $scope.novaFranquia = response.data;

        }

        function carregarFranquiaFalha(response) {
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Carregar dados do Fornecedor------------



        //-----Atualiza dados Franquia---------------------

        function atualizarFranquia() {
            $scope.novaFranquia.Cnpj = $scope.novaFranquia.Cnpj.split(".").join("").split("/").join("").split("-").join("");
            $scope.novaFranquia.DddTelComl = $scope.novaFranquia.DddTelComl.split("(").join("").split(")").join("");
            $scope.novaFranquia.TelefoneComl = $scope.novaFranquia.TelefoneComl.split("-").join("");
            $scope.novaFranquia.DddCel = $scope.novaFranquia.DddCel.split("(").join("").split(")").join("");
            $scope.novaFranquia.Celular = $scope.novaFranquia.Celular.split("-").join("");

            apiService.post('/api/franquia/atualizarfranquia', $scope.novaFranquia,
                atualizarFranquiaSucesso,
                atualizarFranquiaFalha);

        }

        function atualizarFranquiaSucesso(response) {
            $scope.novaFranquia = response.data;
            notificationService.displaySuccess($scope.novaFranquia.RazaoSocial + ' Atualizado com Sucesso.');
        }

        function atualizarFranquiaFalha(response) {
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Atualiza dados Franquia---------------------


        carregarFranquia();
    }

})(angular.module('ECCFranquia'));