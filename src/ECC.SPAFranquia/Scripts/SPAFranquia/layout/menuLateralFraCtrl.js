(function (app) {
    'use strict';

    app.controller('menuLateralFraCtrl', menuLateralFraCtrl);

    menuLateralFraCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService'];

    function menuLateralFraCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService) {


        $scope.menuFra = [];

        //1----Carrega menu-------------------------------------------

        function carregaMenuFra() {


            var config = {
                params: {

                    perfilModulo: 4


                }
            };

            if (membershipService.isUserLoggedIn())
                apiService.get('/api/menu/menu', config,
                        carregaMenuFraSucesso,
                        carregaMenuFraFailed);
        }

        function carregaMenuFraSucesso(response) {

            $scope.menuFra = response.data;

        }

        function carregaMenuFraFailed(response) {
            notificationService.displayError(response.data);
        }
        //1--------------------------------------------------------


         //chama 2 vezes pq no firefoex e chrome não carrega o segundo nivel do menu,
        //depois que mudei para chamar 2 vezes funcionou
        carregaMenuFra();
        carregaMenuFra();

    }

})(angular.module('common.core'));