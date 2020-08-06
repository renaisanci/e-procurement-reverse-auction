(function (app) {
    'use strict';

    app.controller('empresaCtrl', empresaCtrl);

    empresaCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService'];

    function empresaCtrl($scope, $timeout, apiService, notificationService) {
        $scope.novoMembro = {};
        $scope.atualizarMembro = atualizarMembro;
        $scope.openDatePicker = openDatePicker;

        //-----Carregar dados do Membro------------

        function carregarMembro() {
            apiService.get('/api/membro/perfil', null,
            carregarMembroSucesso,
            carregarMembroFalha);
        }

        function carregarMembroSucesso(response) {
			$scope.novoMembro = response.data;



			//TODO tratar quando o tipo de pessoa for PF agora

        }

        function carregarMembroFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Carregar dados do Membro------------

        //-----Atualiza dados membro aba Membro------------

        function atualizarMembro() {
            $scope.novoMembro.Cnpj = $scope.novoMembro.Cnpj.split(".").join("").split("/").join("").split("-").join("");
            apiService.post('/api/membro/atualizar', $scope.novoMembro,
            atualizarMembroSucesso,
            atualizarMembroFalha);
        }

        function atualizarMembroSucesso(response) {
            $scope.novoMembro = response.data;
        }

        function atualizarMembroFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Atualiza dados membro aba Membro------------

        //-----Date Piker------------

        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.opened = true;
        };

        $scope.datepicker = {};
        $scope.format = 'dd/MM/yyyy';
        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };

        //-----Date Piker------------

        carregarMembro();
    }

})(angular.module('ECCCliente'));