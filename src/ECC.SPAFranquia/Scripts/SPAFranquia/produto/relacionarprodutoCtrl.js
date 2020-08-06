
(function (app) {
    'use strict';

    app.controller('relacionarprodutoCtrl', relacionarprodutoCtrl);

    relacionarprodutoCtrl.$inject = ['$scope', 'apiService', '$rootScope', 'notificationService'];

    function relacionarprodutoCtrl($scope, apiService, $rootScope, notificationService) {

        $scope.pageClass = 'page-relacionarproduto';


        $scope.pesquisarProduto = pesquisarProduto;
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.inserirDeletarProdutosFranquia = inserirDeletarProdutosFranquia;
        $scope.checkBoxAll = checkBoxAll;

        $scope.Produtos = [];
        $scope.ProdutosFranquia = [];
        var idProdutosInserir = [];
        var idProdutosDeletar = [];
        $scope.DesabilitarSubCategoria = true;
        $scope.allReg = false;


        //1-----Carrega Produtos aba Pesquisar--------------------------
        function pesquisarProduto(page) {
            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    categoria: $scope.CategoriaId,
                    subcategoria: $scope.SubCategoriaId,
                    filter: $scope.filtroProduto
                }
            };

            apiService.get('/api/produto/pesquisar', config,
                produtoLoadCompleted,
                produtoLoadFailed);
        }

        function produtoLoadCompleted(result) {

            $scope.Produtos = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;

            carregaProdutosFranquia();
        }

        function produtoLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //1-----Fim----------------------------------------------------


        //2-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCategoria() {

            apiService.get('/api/franquia/categoriasFranquia', null,
                categoriaLoadCompleted,
                categoriaLoadFailed);
        }

        function categoriaLoadCompleted(response) {

            $scope.categorias = response.data;
         

        }

        function categoriaLoadFailed(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //2-----Fim---------------------------------------------------


        //3------------Preencher DropDown SubCategoria ---------------
        function pesquisarSubCategoria(id) {

            $scope.SubCategoriaId = 0;

            if (id !== undefined) {
                var config = {
                    params: {
                        filter: id
                    }
                };

                apiService.get('/api/franquia/subcategoriasFranquia', config,
                    subCategoriaLoadCompleted,
                    subCategoriaLoadFailed);
            } else {
                $scope.DesabilitarSubCategoria = true;
                $scope.subcategorias = [];
            }
        }

        function subCategoriaLoadCompleted(response) {
            $scope.DesabilitarSubCategoria = false;
            $scope.subcategorias = response.data;
        }

        function subCategoriaLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //3----------------------------------------------------------


        //4------------Carrega Produtos Franquia---------------------
        function carregaProdutosFranquia() {
            apiService.get('/api/franquia/carregaProdutosFranquia', null,
                loadCarregaFranquiasProdutoCompleted,
                loadCarregaFranquiasProdutoFailed);
        }

        function loadCarregaFranquiasProdutoCompleted(result) {

            var produtoFranquia = result.data;
            var contador = 0;
            $scope.allReg = false;

            for (var i = 0; i < $scope.Produtos.length; i++) {
                for (var j = 0; j < produtoFranquia.length; j++) {
                    if ($scope.Produtos[i].Id === produtoFranquia[j]) {
                        $scope.Produtos[i].selected = true;
                        $scope.Produtos[i].Relacionado = true;
                        contador++;
                    }
                }
                if ($scope.Produtos[i].selected === undefined) {
                    $scope.Produtos[i].selected = false;
                    $scope.Produtos[i].Relacionado = false;
                }
            }

            if (contador === $scope.Produtos.length && $scope.Produtos.length > 0) {
                $scope.allReg = true;
            }
        }

        function loadCarregaFranquiasProdutoFailed(result) {
            notificationService.displayError(result.data);
        }

        //4-----------------------------------------------------------
        

        //5------------Inserir e Deletar Produtos Franquia -------------
        function checkBoxAll(itens, chk) {

            idProdutosInserir = [];
            idProdutosDeletar = [];

            if (chk) {
                chk = true;
            } else {
                chk = false;
            }

            angular.forEach(itens, function (item) {
                item.selected = chk;
            });

            angular.forEach(itens, function (item) {

                if (item.Relacionado) {
                    if (!item.selected) {
                        idProdutosDeletar.push(item.Id);
                    }
                }

                if (!item.Relacionado) {
                    if (item.selected) {
                        idProdutosInserir.push(item.Id);
                    }
                }
            });

            var arrays = idProdutosDeletar.concat(idProdutosInserir);

            inserirDeletarProdutosFranquia(arrays);
        }
        
        function inserirDeletarProdutosFranquia(produtos) {
            idProdutosInserir = [];
            idProdutosDeletar = [];

            apiService.post('/api/franquia/inserirDeletarProdutosFranquia', produtos,
                loadInserirDeletarProdutosFranquiaCompleted,
                loadInserirDeletarProdutosFranquiaFailed);
        }

        function loadInserirDeletarProdutosFranquiaCompleted(result) {
            notificationService.displaySuccess('Alterado com sucesso!');
            $scope.allReg = false;
            var contador = 0;
            var produtoFranquia = result.data.produtosFranquia;

            for (var i = 0; i < $scope.Produtos.length; i++) {
                for (var j = 0; j < produtoFranquia.length; j++) {
                    if ($scope.Produtos[i].Id === produtoFranquia[j]) {
                        $scope.Produtos[i].selected = true;
                        $scope.Produtos[i].Relacionado = true;
                        contador++;
                    }
                }
                if ($scope.Produtos[i].selected === undefined || $scope.Produtos[i].selected === false) {
                    $scope.Produtos[i].selected = false;
                    $scope.Produtos[i].Relacionado = false;
                }
            }

            if (contador === $scope.Produtos.length) {
                $scope.allReg = true;
            }
        }

        function loadInserirDeletarProdutosFranquiaFailed(result) {
            notificationService.displayError(result.data);
        }
        //5--------------------------------------------------------------
        

        pesquisarCategoria();

    }

})(angular.module('ECCFranquia'));