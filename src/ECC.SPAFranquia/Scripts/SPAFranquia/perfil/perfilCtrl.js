
(function (app) {
    'use strict';

    app.controller('perfilCtrl', perfilCtrl);

    perfilCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService' ,'$rootScope', 'membershipService'];

    function perfilCtrl($scope, $timeout, apiService, notificationService,  $rootScope, membershipService) {
        $scope.novoUsuario = {}
        $scope.atualizarModelUsuario = atualizarModelUsuario;

        //3-----Retorna Dados-------------------------------
        function carregarUsuario() {
            apiService.get('/api/usuario/perfil', $scope.novoUsuario,
                carregarUsuarioSucesso,
                carregarUsuarioFalha);

        }

        function carregarUsuarioSucesso(response) {

            $scope.emailAnt = response.data.UsuarioEmail;
            $scope.novoUsuario = response.data;
        }

        function carregarUsuarioFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //3 ----------------------------Fim---------------------------------------//

        //3-----Atualiza Usuario-------------------------------
        function atualizarModelUsuario() {


            if ($scope.emailAnt != $scope.novoUsuario.UsuarioEmail)
                membershipService.logout();


            apiService.post('/api/usuario/atualizar', $scope.novoUsuario,
                atualizarUsuarioSucesso,
                atualizarUsuarioFalha);



        }

        function atualizarUsuarioSucesso(response) {
            notificationService.displaySuccess($scope.novoUsuario.UsuarioNome + ' Atualizado com Sucesso.');
            $scope.novoUsuario = response.data;


            if ($scope.emailAnt != response.data.UsuarioEmail)
                $rootScope.$emit("logoutExterno");

        }

        function atualizarUsuarioFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //3 ----------------------------Fim---------------------------------------//

        carregarUsuario();
    }

})(angular.module('ECCFranquia'));