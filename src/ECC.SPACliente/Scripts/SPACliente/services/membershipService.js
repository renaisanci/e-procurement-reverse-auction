(function (app) {
    'use strict';

    app.factory('membershipService', membershipService);

    membershipService.$inject = ['apiService', 'notificationService', '$http', '$base64', '$cookieStore', '$rootScope'];

    function membershipService(apiService, notificationService, $http, $base64, $cookieStore, $rootScope) {

        var service = {
            login: login,
            register: register,
            saveCredentials: saveCredentials,
            removeCredentials: removeCredentials,
            isUserLoggedIn: isUserLoggedIn,
            enviarEmail: enviarEmail,
            getRecuperaSenha: getRecuperaSenha,
            alterarSenhaUsuario: alterarSenhaUsuario,
            logout: logout
        };

        function login(user, completed) {

          
            apiService.post('/api/conta/autenticar', user,completed,loginFailed);
        }

        function register(user, completed) {
            apiService.post('/api/account/register', user, completed, registrationFailed);
        }

 

		function saveCredentials(usuario, nomeUsuario, faturaVenceu, tipoPessoaPfPj) {
            var membershipData = $base64.encode(usuario.UsuarioEmail + ':' + usuario.Senha + ':' + usuario.perfilModulo);


            $rootScope.repository = {
                loggedUser: {
                    username: usuario.UsuarioEmail,
                    authdata: membershipData,
                    nome: nomeUsuario,
					faturaMensalidade: faturaVenceu,
					tipoPessoa: tipoPessoaPfPj
                }
			};


			

            $http.defaults.headers.common['Authorization'] = 'Basic ' + membershipData;
            $cookieStore.put('repositoryCliRefresh', $rootScope.repository);

            if (usuario.LembraMe) {
                if (localStorage != null && JSON != null) {
                    localStorage["repositoryCli"] = JSON.stringify($rootScope.repository);
                }
            }

           
        }

        function logout(completed) {
            apiService.post('/api/conta/logout', null,
                function (response) {
                    console.log(response);
                    removeCredentials();
                    completed();
                },
                function (response) {
                    console.error('logout: ', response);
                });
        }

        function removeCredentials() {
            $rootScope.repository = {};
            $cookieStore.remove('repositoryCliRefresh');
            localStorage.removeItem('repositoryCli');
            $http.defaults.headers.common.Authorization = '';
        }

        function enviarEmail(user, completedEmail) {
            apiService.post('/api/conta/recuperarsenha', user,
                completedEmail,
                enviarEmailFailed);
        }

        function getRecuperaSenha(objRecuperaSenha, getRecuperaSenhaCompleted) {
            apiService.post('/api/conta/getrecuperarsenha', objRecuperaSenha,
                getRecuperaSenhaCompleted,
                getRecuperaFailed);
        }

        function alterarSenhaUsuario(user, senhaAlteradaCompleted) {
            apiService.post('/api/conta/alterarsenhausuario', user,
                senhaAlteradaCompleted,
                senhaAlteradaFailed);
        }

        function senhaAlteradaFailed(response) {
            notificationService.displayError(response.data);
        }


        function getRecuperaFailed(response) {
            notificationService.displayError(response.data);
        }


        function enviarEmailFailed(response) {
            notificationService.displayError(response.data);
        }


        function loginFailed(response) {
            notificationService.displayError(response.data);
        }

        function registrationFailed(response) {

            notificationService.displayError('Registration failed. Try again.');
        }

        function isUserLoggedIn() {

            var repUser = localStorage != null ? localStorage["repositoryCli"] : null;
            if (repUser != 'null' && repUser != undefined) {
                var rep = JSON.parse(repUser);
                $rootScope.repository.loggedUser = rep.loggedUser;
                $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;

            }



            return $rootScope.repository.loggedUser != null;
        }

        return service;
    }



})(angular.module('common.core'));