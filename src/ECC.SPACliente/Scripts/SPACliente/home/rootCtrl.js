﻿(function (app) {
    'use strict';

    app.controller('rootCtrl', rootCtrl);

    rootCtrl.$inject = ['$scope', '$location', 'membershipService', '$rootScope', '$modal'];
    function rootCtrl($scope, $location, membershipService, $rootScope, $modal) {

        $scope.userData = {};
        $scope.userData.displayUserInfo = displayUserInfo;
		$scope.logout = logout;
		$scope.anoFoot = new Date().getFullYear();
     
        $scope.alterarSenhaLogado = alterarSenhaLogado;
        
        function alterarSenhaLogado() {
            $modal.open({
                templateUrl: 'scripts/SPACliente/login/alteraSenha.html',
                controller: 'alteraSenhaCtrl',
                size: 'sm',
                scope: $scope
            }).result.then(function ($scope) {

                atualizaCredenciais($scope);

            }, function () {
            });
        }

        function atualizaCredenciais() {
        }

        function displayUserInfo() {
            $scope.userData.isUserLoggedIn = membershipService.isUserLoggedIn();
            if ($scope.userData.isUserLoggedIn) {
				$scope.username = $rootScope.repository.loggedUser.nome;
				$scope.tipoPessoa = $rootScope.repository.loggedUser.tipoPessoa;
            }
        }

        function logout() {
            membershipService.logout(function () {
                $location.path("/login");
                $scope.userData.displayUserInfo();
            });
        }

        $scope.userData.displayUserInfo();

        $rootScope.$on("logoutExterno", function (event, data, cam) {

            logout();
            $scope.userData.isUserLoggedIn = false;

		});


		$rootScope.$on("logoutExternoError", function (event, data, cam) {


			$scope.userData.isUserLoggedIn = false;
			membershipService.removeCredentials();
			$location.path('/login');

		});
    

      
    }

})(angular.module('ECCCliente'));