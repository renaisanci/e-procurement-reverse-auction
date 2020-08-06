
(function (app) {
    'use strict';

    app.controller('franqueadosCtrl', franqueadosCtrl);

    franqueadosCtrl.$inject = ['$scope', 'apiService', '$rootScope', 'notificationService', 'SweetAlert', '$modal'];

    function franqueadosCtrl($scope, apiService, $rootScope, notificationService, SweetAlert, $modal) {

        $scope.pageClass = 'page-franqueados';
        $scope.pesquisarFranqueado = pesquisarFranqueado;
        $scope.detalhesFranqueado = detalhesFranqueado;
        $scope.pesquisarCidades = pesquisarCidades;
        $scope.Estados = [];
        $scope.Cidades = [];
        $scope.Franqueados = [];
        $scope.habilitaCategoria = true;

        //1-----Carrega Franqueados aba Pesquisar--------------------
        function pesquisarFranqueado(page) {

            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    cidadeId: $scope.CidadeId,
                    filter: $scope.filtroFranqueado
                }
            };

            apiService.get('/api/franquia/carregaMembrosFranquia', config,
                franqueadoLoadCompleted,
                franqueadoLoadFailed);
        }

        function franqueadoLoadCompleted(result) {

            $scope.Franqueados = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
        }

        function franqueadoLoadFailed(response) {

            notificationService.displayError(response.data);
        }
        //1----------------------------------------------------------


        //2-----Carrega Categorias para DropDown Categoria-----------
        function pesquisarEstados() {

            apiService.get('/api/endereco/estado', null,
                estadosLoadCompleted,
                estadosLoadFailed);
        }

        function estadosLoadCompleted(response) {

            var newItem = new function () {
                this.Id = undefined;
                this.Uf = "Selecione ...";
            };

            response.data.push(newItem);

            $scope.Estados = response.data;

        }

        function estadosLoadFailed(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2-----------------------------------------------------------
        

        //3-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCidades(estado) {

            var config = {
                params: {
                    estadoId: estado
                }
            };

            apiService.get('/api/endereco/cidade', config,
                cidadesLoadCompleted,
                cidadesLoadFailed);
        }

        function cidadesLoadCompleted(response) {

            $scope.habilitaCategoria = false;

            $scope.Cidades = response.data;

        }

        function cidadesLoadFailed(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //3-----------------------------------------------------------
        

        //4------------Editar Produto--------------------------------
        function detalhesFranqueado(franqueado) {

            $modal.open({
                templateUrl: 'scripts/SPAFranquia/promocao/detalhesPromocao.html',
                controller: 'detalhesPromocaoCtrl',
                backdrop: 'static',
                scope: $scope,
                size: ''
            });

            $scope.novoFranqueado = franqueado;
        }
        //4----------------------------------------------------------
        
        pesquisarFranqueado();
        pesquisarEstados();
    }

})(angular.module('ECCFranquia'));