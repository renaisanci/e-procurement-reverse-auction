(function (app) {
    'use strict';

    app.controller('loginCtrl', loginCtrl);

    loginCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];

    function loginCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {

        //caso o usuario clique no botão voltar do BROWSER SEJA ELE QUAL FOR NÓS VERIFICAS 
        //SE JA EXISTE UM LOGIN ATIVO E NAO DEIXA CARREGAR A TELA DE LOGIN NOVAMENTE
        if ($rootScope.repository.loggedUser)
            $location.path('/');


        $scope.pageClass = 'page-login';
        $scope.login = login;
        $scope.enviarEmail = enviarEmail;
        $scope.habTrocaSenha = false;
        $scope.alterarSenha = alterarSenha;
        $scope.usuario = {
            perfilModulo: 1
        };
        $scope.usuarioAcesso = {
            perfilModulo: 1
        };


        function login() {
            membershipService.login($scope.usuario, loginCompleted);
        }

        function loginCompleted(result) {
            if (result.data.success) {

                if (result.data.usuarioFlgTrocaSenha)
                { 
                    membershipService.saveCredentials($scope.usuario, result.data.usuarioNome);


                    notificationService.displaySuccess("Olá " + result.data.usuarioNome);
                    $scope.userData.displayUserInfo();
                    registraTokensignalR();
                    if ($rootScope.previousState && $rootScope.previousState != "/login")
                        $location.path($rootScope.previousState);
                    else
                        $location.path('/');
                }
                else {
                    $scope.habTrocaSenha = true;
                    $scope.usuario.Chave = result.data.usuarioChave;
                    $scope.usuario.Nome = result.data.usuarioNome;
                    $scope.usuario.Id = result.data.usuarioId;
                }
            }
            else {
                notificationService.displayError('Login sem sucesso. Tente Novamente.');
            }
        }

        function enviarEmail() {
            
            membershipService.enviarEmail($scope.usuario, enviarEmailCompleted);
        }


        function pageLoadCompleted(result) {
            if (result.data.success) {

                //membershipService.saveCredentials($scope.usuario, result.data.usuarioNome);

                notificationService.displaySuccess("Olá " + result.data.usuarioNome);
                $scope.userData.displayUserInfo();




                if ($rootScope.previousState && $rootScope.previousState != "/login")
                    $location.path($rootScope.previousState);
                else
                    $location.path('/');
            }
            else {
                notificationService.displayError('Login sem sucesso. Tente Novamente.');
            }
        }


        function enviarEmailCompleted(result) {
            if (result.data.success) {

                //membershipService.saveCredentials($scope.usuario, result.data.usuarioEmail);

                notificationService.displaySuccess("Email enviado para " + result.data.usuarioEmail);
            }
            else {
                notificationService.displayError('Email não cadastrado.');
            }
        }


        function alterarSenha() {
            var senha1 = $scope.novaSenha1;
            var senha2 = $scope.novaSenha2;

            if (senha1 == senha2) {
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
                //$location.path('/');
                $scope.usuarioAcesso.Senha = $scope.usuario.senha;
                $scope.usuarioAcesso.Nome = $scope.usuario.Nome;
                $scope.usuarioAcesso.UsuarioEmail = $scope.usuario.UsuarioEmail;
                
                membershipService.saveCredentials($scope.usuarioAcesso, $scope.usuario.Nome);

                notificationService.displaySuccess("Olá " + $scope.usuario.Nome);
                $scope.userData.displayUserInfo();
                
                $location.path('/');

            }
        }


        // 03---------------Registra o tokenSignalR para o usuário logado----
        function registraTokensignalR( ) {

            $scope.data = {

                TokenSignalRUser: sessionStorage.TokenSignalRUser
            };

            apiService.post("/api/usuario/atualizarTokenSignalR", $scope.data,
              registraTokensignalRCompleted,
              registraTokensignalRFailed);
        }

        function registraTokensignalRCompleted(response) {

        }

        function registraTokensignalRFailed(result) {


        }
        // 03----------------------------------------------------------------


    }

})(angular.module('common.core'));