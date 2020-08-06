(function (app) {
    'use strict';

    app.controller('menuLateralFornCtrl', menuLateralFornCtrl);

    menuLateralFornCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService'];

    function menuLateralFornCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService) {


        $scope.menuForn = [];

        //1----Carrega menu-------------------------------------------

        function carregaMenuForn() {


            var config = {
                params: {

                    perfilModulo: 3


                }
            };

            if (membershipService.isUserLoggedIn())
                apiService.get('/api/menu/menu', config,
                        carregaMenuFornSucesso,
                        carregaMenuFornFailed);
        }

        function carregaMenuFornSucesso(response) {

            $scope.menuForn = response.data;

        }

        function carregaMenuFornFailed(response) {
            notificationService.displayError(response.data);
        }
        //1--------------------------------------------------------


        //chama 2 vezes pq no firefoex e chrome não carrega o segundo nivel do menu,         
        //depois que mudei para chamar 2 vezes funcionou
        carregaMenuForn();
        carregaMenuForn();


    }

})(angular.module('common.core'));