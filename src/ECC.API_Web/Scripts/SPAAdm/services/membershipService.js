(function (app) {
    'use strict';

    app.factory('membershipService', membershipService);
    
    membershipService.$inject = ['apiService', 'notificationService', '$http', '$base64', '$cookieStore', '$rootScope', '$location'];
    function membershipService(apiService, notificationService, $http, $base64, $cookieStore, $rootScope, $location) {

        var service = {
            login: login,
            register: register,
            saveCredentials: saveCredentials,
            removeCredentials: removeCredentials,
            isUserLoggedIn: isUserLoggedIn,
            enviarEmail: enviarEmail,
            getRecuperaSenha: getRecuperaSenha,
            alterarSenhaUsuario: alterarSenhaUsuario,
            permissaoURL: permissaoURL,
            logout: logout
        }

        function login(user, completed) {
            apiService.post('/api/conta/autenticar', user,
            completed,
            loginFailed);
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

        function register(user, completed) {
            apiService.post('/api/account/register', user,
            completed,
            registrationFailed);
        }

        function saveCredentials(usuario, nomeUsuario) {
            var membershipData = $base64.encode(usuario.UsuarioEmail + ':' + usuario.Senha + ':' + usuario.perfilModulo);

            $rootScope.repository = {
                loggedUser: {
                    username: usuario.UsuarioEmail,
                    authdata: membershipData,
                    nome: nomeUsuario,
                    permissaoPagina: true 
                }
            };

            $http.defaults.headers.common['Authorization'] = 'Basic ' + membershipData;
            $cookieStore.put('repositoryAdmRefresh', $rootScope.repository);

            if (usuario.LembraMe) {
                if (localStorage != null && JSON != null) {
                    localStorage["repositoryAdm"] = JSON.stringify($rootScope.repository);
                }
            }

        }

        function logout(completed) {
            apiService.post('/api/conta/logout', null,
                function (response) {
                    //console.log(response);
                    removeCredentials();
                    completed();
                },
                function (response) {
                    console.error('logout: ', response);
                });
        }

        function removeCredentials() {
            $rootScope.repository = {};
            $cookieStore.remove('repositoryAdmRefresh');
            localStorage.removeItem('repositoryAdm');
            $http.defaults.headers.common.Authorization = '';
        };

        function senhaAlteradaFailed(response) {
            notificationService.displayError(response.data);
        }


        function getRecuperaFailed(response) {
            notificationService.displayError(response.data);
        }

        function loginFailed(response) {
            notificationService.displayError(response.data);
        }

        function enviarEmailFailed(response) {
            notificationService.displayError(response.data);
        }

        function registrationFailed(response) {

            notificationService.displayError('Registration failed. Try again.');
        }

        function isUserLoggedIn() {
            var repUser = localStorage != null ? localStorage["repositoryAdm"] : null;
            if (repUser != 'null' && repUser != undefined) {
                var rep = JSON.parse(repUser);
                $rootScope.repository.loggedUser = rep.loggedUser;
                $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;

            }

            return $rootScope.repository.loggedUser != null;
        }


        function permissaoURL(url, location) {
            
            $rootScope.$location = location;
            apiService.post('/api/conta/permissaourl/' + url, url,
            permissaoURLSucesso,
            permissaoURLFalha);

        }

        function permissaoURLSucesso(request) {
            if (!request.data.temacesso)
                redireciona($rootScope, $rootScope.$location);
        }

        function permissaoURLFalha(response) {
            notificationService.displayError(response.data);
            redireciona($rootScope, $rootScope.$location);
        }

        return service;
    }

    redireciona.$inject = ['$rootScope', '$location'];
    function redireciona($rootScope, $location) {
        $location.path('/');
    }


})(angular.module('common.core'));