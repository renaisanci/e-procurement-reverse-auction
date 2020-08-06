
(function (app) {
    'use strict';

    app.controller('aprovacaopromocaoCtrl', aprovacaopromocaoCtrl);

    aprovacaopromocaoCtrl.$inject = ['$scope', 'apiService', '$rootScope', 'notificationService', 'SweetAlert', '$modal'];

    function aprovacaopromocaoCtrl($scope, apiService, $rootScope, notificationService, SweetAlert, $modal) {

        $scope.pageClass = 'page-aprovacaopromocao';
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.detalhesProdutoPromocional = detalhesProdutoPromocional;
        $scope.atualizarProduto = atualizarProduto;
        $scope.pesquisarCategoria = pesquisarCategoria;
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
                    filter: $scope.filtroProduto
                }
            };

            apiService.get('/api/produtopromocional/pesquisarprodfranquia', config,
                produtoLoadCompleted,
                produtoLoadFailed);
        }

        function produtoLoadCompleted(result) {

            $scope.produtos = [];
            var produtosPromocao = result.data.Produtos.Items;
            var produtosFranquiaId = result.data.ProdutosFranquiaId;

            if (produtosPromocao.length > 0) {
                for (var i = 0; i < produtosPromocao.length; i++) {
                    if (produtosFranquiaId.length > 0) {
                        for (var j = 0; j < produtosFranquiaId.length; j++) {
                            if (produtosPromocao[i].Id === produtosFranquiaId[j]) {
                                produtosPromocao[i].AprovacaoFranquia = true;
                                break;
                            }
                        }
                    } else {
                        produtosPromocao[i].AprovacaoFranquia = false;
                    }
                }
            }

            $scope.produtos = produtosPromocao;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;

            var msg = result.data.Items.length > 1 ? " Produtos Encontrados" : "Produto Encontrado";
            if ($scope.page === 0 && $scope.novoProduto.Id === undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
        }

        function produtoLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----------------------------------------------------------
        
        //3-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCategoria() {


            //apiService.get('/api/produtopromocional/categoria', null,
            apiService.get('/api/franquia/categoriasFranquia', null,
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
        //3-----------------------------------------------------------
        
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
                templateUrl: 'scripts/SPAFranquia/promocao/detalhesPromocao.html',
                controller: 'detalhesPromocaoCtrl',
                backdrop: 'static',
                scope: $scope,
                size: ''
            });

            $scope.novoProduto = produto;
            $scope.novaImagem.CaminhoImagemGrande = $scope.novoProduto.ImagemGrande;
        }
        //7----------------------------------------------------------
        
        //11-----Atualizar Produtos----------------------------------
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
        //11--------------------Fim----------------------------------
        
        pesquisarCategoria();
        pesquisarProduto();
    }

})(angular.module('ECCFranquia'));