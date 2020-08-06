(function (app) {
    'use strict';

    app.controller('menuLateralAdmCtrl', menuLateralAdmCtrl);

    menuLateralAdmCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService'];

    function menuLateralAdmCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService) {
 

        $scope.menuAdm = [];
 
        //1----Carrega menu-------------------------------------------

        function carregaMenuAdm() {

 
            var config = {
                params: {
                perfilModulo:1
                }
            };

            if (membershipService.isUserLoggedIn())
                apiService.get('/api/menu/menu', config,
                        carregaMenuAdmSucesso,
                        carregaMenuAdmFailed);
        }

        function carregaMenuAdmSucesso(response) {

            $scope.menuAdm = response.data;

        }

        function carregaMenuAdmFailed(response) {
            notificationService.displayError(response.data);
        }
        //1--------------------------------------------------------

        carregaMenuAdm();

    }

})(angular.module('common.core'));