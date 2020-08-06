(function (app) {
    'use strict';

    app.controller('usuarioCtrl', usuarioCtrl);

    usuarioCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService'];

    function usuarioCtrl($scope, $modalInstance, $timeout, apiService, notificationService) {

        $scope.cancel = cancel;
        $scope.inserirUsuario = inserirUsuario;
        $scope.habilitarSenhaAtual = false;

        //    $scope.novoUsuario = {
        //        Senha: "",
        //        ConfirmSenha: "",
        //        Ativo:true

        //};


        //1---------Fecha Modal---------------------
        function cancel() {
            $modalInstance.dismiss();
        }
        //------------------------------------------




        //2---------Insere ou Atualiza Usuario------
        function inserirUsuario() {
            if ($scope.novoUsuario.Id > 0) {
                atualizarModelUsuario();

            } else {
                inserirModelUsuario();
            }
        }

        //------------------------------------------



        //2---------Insere  Usuario----------------
        function inserirModelUsuario() {

            if ($scope.novoUsuario.ckbUsurMaster.checked)
                $scope.novoUsuario.FlgMaster = true;

            apiService.post('/api/usuario/inserir', $scope.novoUsuario,
                inserirModelUsuarioSucesso,
                inserirModelUsuarioFalha);
        }

        function inserirModelUsuarioSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoUsuario = response.data;

        }

        function inserirModelUsuarioFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }


        //------------------------------------------



        //3-----Atualiza Usuario-------------------------------
        function atualizarModelUsuario() {
            apiService.post('/api/usuario/atualizar', $scope.novoUsuario,
                atualizarUsuarioSucesso,
                atualizarUsuarioFalha);

        }

        function atualizarUsuarioSucesso(response) {
            notificationService.displaySuccess($scope.novoUsuario.UsuarioNome + ' Atualizado com Sucesso.');
            $scope.novoUsuario = response.data;

        }

        function atualizarUsuarioFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //3 ----------------------------Fim---------------------------------------//



    }

})(angular.module('ECCAdm'));