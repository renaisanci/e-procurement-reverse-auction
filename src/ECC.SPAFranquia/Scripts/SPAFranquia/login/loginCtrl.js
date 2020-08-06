﻿(function (app) {
    'use strict';

    app.controller('loginCtrl', loginCtrl);

    loginCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'Hub', 'apiService',  'SweetAlert'];

    function loginCtrl($scope, membershipService, notificationService, $rootScope, $location, Hub, apiService,  SweetAlert) {


        var isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification);


        if (isSafari) {
            SweetAlert.swal({
                title: "Desculpe :( browser safari é incompatível",
                text: "Tente acessar com navegador Opera !!!",
                type: "warning",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            },


            function (isConfirm) {
                if (isConfirm) {
                    window.location.replace("http://www.opera.com/pt-br/mobile/mini/ios");
                }



            });



        }


        //caso o usuario clique no botão voltar do BROWSER SEJA ELE QUAL FOR NÓS VERIFICAMOS 
        //SE JA EXISTE UM LOGIN ATIVO E NAO DEIXA CARREGAR A TELA DE LOGIN NOVAMENTE
        if ($rootScope.repository.loggedUser)
            $location.path('/');



        $scope.pageClass = 'page-login';
        $scope.login = login;
        $scope.enviarEmail = enviarEmail;
        $scope.habTrocaSenha = false;
        $scope.alterarSenha = alterarSenha;
        $scope.usuario = {
            perfilModulo: 4
        };
        $scope.usuarioAcesso = {
            perfilModulo: 4
        };


        function login() {
            membershipService.login($scope.usuario, loginCompleted);
        }

        function loginCompleted(result) {
            if (result.data.success) {

                if (result.data.usuarioFlgTrocaSenha) {

                    membershipService.saveCredentials($scope.usuario, result.data.usuarioNome, result.data.faturaVenceu);
                    notificationService.displaySuccess("Olá " + result.data.usuarioNome);
                    $scope.userData.displayUserInfo();

                    registraFraTokensignalR();
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
        function registraFraTokensignalR() {

            $scope.data = {

                TokenSignalRUser: sessionStorage.TokenSignalRUserFra
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

})(angular.module('ECCFranquia'));