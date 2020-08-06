(function (app) {
    'use strict';

    app.controller('menuLateralCtrl', menuLateralCtrl);

    menuLateralCtrl.$inject = ['$scope', 'membershipService', '$routeParams', 'notificationService', '$rootScope', '$location', 'apiService', 'storeService', 'SweetAlert', '$route'];

    function menuLateralCtrl($scope, membershipService, $routeParams, notificationService, $rootScope, $location, apiService, storeService, SweetAlert, $route) {


        $scope.carregaCategoriaProduto = carregaCategoriaProduto;
        $scope.categoriasMembro = [];
        $scope.menuCli = [];
        $scope.exibirMenus = exibirMenus;
        $scope.cart = storeService.cart;
        $scope.cartPromocoes = storeService.cartPromocoes;

        //variaveis para controlar a exibição do menu de categorias e gerencial
        $scope.exibiMenuCategoria = true;
        $scope.exibiMenuGerencial = false;
        $scope.habilitaMenCategoria = true;
        if (sessionStorage["exibiMenuCategoria"] != undefined || sessionStorage["exibiMenuCategoria"] != null) {
            $scope.exibiMenuCategoria = JSON.parse(sessionStorage["exibiMenuCategoria"]);
            $scope.exibiMenuGerencial = JSON.parse(sessionStorage["exibiMenuGerencial"]);
        }


        //1----Carrega todas as categorias dos Produtos amarrada ao membro logado----

        function carregaCategoriaProduto() {

            if (membershipService.isUserLoggedIn())
                apiService.get('/api/produto/membroCategoriasMenu', null,
                        loadMembroCategoriaSucesso,
                        loadMembroCategoriaFailed);
        }

        function loadMembroCategoriaSucesso(response) {

            $scope.categoriasMembro = response.data;

        }

        function loadMembroCategoriaFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Carrega menu-------------------------------------------
        


        //2-----------------------------------------------------------
        function carregaMenuCli() {


            var config = {
                params: {
                    perfilModulo: 2
                }
            };

            if (membershipService.isUserLoggedIn())
                apiService.get('/api/menu/menu', config,
                        carregaMenuFornSucesso,
                        carregaMenuFornFailed);
        }

        function carregaMenuFornSucesso(response) {

            $scope.menuCli = response.data;

        }

        function carregaMenuFornFailed(response) {
            notificationService.displayError(response.data);
        }
        //2--------------------------------------------------------


        //3--------------------------------------------------------
        function exibirMenus(flgCat, flgGer) {


            sessionStorage["exibiMenuCategoria"] = flgCat;
            sessionStorage["exibiMenuGerencial"] = flgGer;

            $scope.exibiMenuCategoria = flgCat;
            $scope.exibiMenuGerencial = flgGer;


        }
        //3--------------------------------------------------------
        

        carregaCategoriaProduto();
        carregaMenuCli();
    }

})(angular.module('common.core'));