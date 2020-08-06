
(function (app) {
    'use strict';

    app.controller('aprovarpromocaoCtrl', aprovarpromocaoCtrl);

    aprovarpromocaoCtrl.$inject = ['$scope', 'apiService','$rootScope', 'notificationService', 'SweetAlert', '$modal'];

    function aprovarpromocaoCtrl($scope, apiService,$rootScope, notificationService, SweetAlert, $modal) {

        $scope.pageClass = 'page-aprovarpromocao';
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.detalhesProdutoPromocional = detalhesProdutoPromocional;
        $scope.atualizarProduto = atualizarProduto;
        $scope.pesquisarCategoria = pesquisarCategoria;
        $scope.pesquisarFornecedor = pesquisarFornecedor;
        $scope.produtos = [];
        $scope.novoProduto = {};
        $scope.novaImagem = {};
     
        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescProduto';
        $scope.reverse = true;


        //1-----Carrega Produtos aba Pesquisar-----------------------
        function pesquisarProduto(page) {

            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 30,
                    categoria: $scope.CategoriaId,
                    subcategoria: $scope.SubCategoriaId,
                    fornecedor: $scope.FornecedorPesqId,
                    filter: $scope.filtroProduto
                }
            };

            apiService.get('/api/produtopromocional/pesquisaradm', config,
                produtoLoadCompleted,
                produtoLoadFailed);
        }

        function produtoLoadCompleted(result) {

            $scope.produtos = result.data.Items;


            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;


            var msg = result.data.Items.length > 1 ? " Produtos Encontrados" : "Produto Encontrado";
            if ($scope.page == 0 && $scope.novoProduto.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function produtoLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----------------------------------------------------------



        //2-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCategoria() {


            apiService.get('/api/produtopromocional/categoria', null,
                categoriaLoadCompleted,
                categoriaLoadFailed);
        }

        function categoriaLoadCompleted(response) {

            var newItem = new function () {
                this.Id = undefined;
                this.DescCategoria = "Categoria ...";
               

            };

            response.data.push(newItem);

            $scope.categorias = response.data;
           
        }

        function categoriaLoadFailed(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2------------------------------------------------------------


        //3-----Carrega Fornecedores para DropDown Fornecedor------------
        function pesquisarFornecedor() {
            apiService.get('/api/fornecedor/getfornecedores', null,
                pesquisarFornecedorLoadCompleted,
                pesquisarFornecedorLoadFailed);
        }

        function pesquisarFornecedorLoadCompleted(response) {

            //var newItem = new function () {
            //    this.Id = undefined;
            //    this.NomeFantasia = "Fornecedor...";

            //};

            //response.data.push(newItem);
            $scope.fornecedores = response.data;

        }

        function pesquisarFornecedorLoadFailed(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2------------------------------------------------------------


        //5------------Preencher DropDown SubCategoria ---------------
        function pesquisarSubCategoria(id) {

            var config = {
                params: {
                    filter: id
                }
            };

            apiService.get('/api/produto/subcategorias', config,
                subCategoriaLoadCompleted,
                subCategoriaLoadFailed);

        }

        function subCategoriaLoadCompleted(response) {
            $scope.DesabilitarSubCategoria = false;
            $scope.subcategorias = response.data;

        }

        function subCategoriaLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //5----------------------------------------------------------


        //7------------Editar Produto--------------------------------
        function detalhesProdutoPromocional(produto) {

            $modal.open({
                templateUrl: 'scripts/SPAAdm/produto/detalhesPromocao.html',
                controller: 'detalhesPromocaoCtrl',
                backdrop: 'static',
                scope: $scope,
                size: ''
            }).result.then(function ($scope) {

                console.log("Modal Closed!!!");

            }, function () {

                console.log("Modal Dismissed!!!");

            });
            
            $scope.novoProduto = produto;
            $scope.novaImagem.CaminhoImagemGrande = $scope.novoProduto.ImagemGrande;

        }
        //7----------------------------------------------------------
        

        //8-----Atualizar Produtos----------------------------------
        function atualizarProduto() {

            apiService.post('/api/produtopromocional/atualizar', $scope.novoProduto,
                atualizarProdutoSucesso,
                atualizarProdutoFalha);
        }

        function atualizarProdutoSucesso(response) {

            notificationService.displaySuccess('Produto atualizado com sucesso!');

           pesquisarProduto();
         
        }

        function atualizarProdutoFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            } else {
                notificationService.displayError(response.statusText);
            }
        }
        //8--------------------Fim----------------------------------


        pesquisarCategoria();
        pesquisarFornecedor();
        pesquisarProduto();
    }

})(angular.module('ECCAdm'));