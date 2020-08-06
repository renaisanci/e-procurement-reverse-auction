(function (app) {
    'use strict';

    app.controller('rootCtrl', rootCtrl);

    rootCtrl.$inject = ['$scope', '$location', 'membershipService', '$rootScope', '$modal'];
    function rootCtrl($scope, $location, membershipService, $rootScope, $modal) {

        $scope.userData = {};
        $scope.userData.displayUserInfo = displayUserInfo;
        $scope.logout = logout;
        $scope.alterarSenhaLogado = alterarSenhaLogado;
		$scope.anoFoot = new Date().getFullYear();

        function alterarSenhaLogado() {
            $modal.open({
                templateUrl: 'scripts/SPAAdm/login/alteraSenha.html',
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
            }
        }

        function logout() {
            membershipService.logout(function () {
                $location.path("/login");
                $scope.userData.displayUserInfo();
            });
        }

        $scope.userData.displayUserInfo();
    }

})(angular.module('ECCAdm'));