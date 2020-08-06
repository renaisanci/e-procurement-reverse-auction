(function (app) {
    'use strict';

    app.controller('recuperasenhaCtrl', recuperasenhaCtrl);

    recuperasenhaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location'];

    function recuperasenhaCtrl($scope, membershipService, notificationService, $rootScope, $location) {
        $scope.pageClass = 'page-login';
        $scope.novaSenha1;
        $scope.novaSenha2;
        $scope.alterarSenha = alterarSenha;
        $scope.desabilitaAlterar = true;
        $scope.usuario = {
            perfilModulo: 1
        };

        function PageLoad() {
            
            var objGet = $location.search();
            
            $scope.objRecuperaSenha = {
                Chave: objGet.Q.toString()
            }

            membershipService.getRecuperaSenha($scope.objRecuperaSenha, pageLoadCompleted);
            
        }

        function alterarSenha() {
            var senha1 = $scope.novaSenha1;
            var senha2 = $scope.novaSenha2;

            if (senha1 == senha2)
            {
                $scope.usuario.senha = senha1;
                membershipService.alterarSenhaUsuario($scope.usuario, senhaAlteradaCompleted)
            }
            else {
                notificationService.displayError('As senhas digitadas não conferem. Tente Novamente.');
            }

        }

        function senhaAlteradaCompleted(result) {
            if (result.data.success) {

                notificationService.displaySuccess('Senha alterada com sucesso.');
                $location.path('/');

            }
        }

        function pageLoadCompleted(result) {
            if (result.data.success) { 
                //membershipService.saveCredentials($scope.usuario, result.data.usuarioNome);
                $scope.usuario.usuarioNome = result.data.usuarioNome;
                $scope.usuario.Id = result.data.usuarioId;
                $scope.usuario.Chave = result.data.usuarioChave;
                $scope.desabilitaAlterar = false;
            }
            else {
                $scope.desabilitaAlterar = true;
                notificationService.displayError('Acesso inválido.');

            }
        }


        PageLoad();
    }

})(angular.module('common.core'));