(function (app) {
    'use strict';

    app.controller('produtoCtrl', produtoCtrl);

    produtoCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$stateParams', 'storeService', '$modal'];

    function produtoCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $stateParams, storeService, $modal) {
        $scope.pageClass = 'page-produto';

        sessionStorage.carregaCategoriaProduto = $stateParams.id;

        $scope.selectCustomer = selectCustomer;

        var url = $location.url();
        localStorage.UrlMembro = url;

        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.ambiente = apiService.identificaAmbiente();

        $scope.Produtos = [];
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.inputText = inputText;
        $scope.cart = storeService.cart;
        $scope.cartPromocoes = storeService.cartPromocoes;
        $scope.openDetProdDialog = openDetProdDialog;
        $scope.useMarcas = {};
        $scope.useUnidadeMedidas = {};
        $scope.useCategorias = {};
        $scope.addQuantidade = addQuantidade;

        //objeto das estrelas do produto
        $scope.isReadOnly = false;

        var uniqueItems = function (data, key) {
            var result = [];

            for (var i = 0; i < data.length; i++) {
                var value = data[i][key];

                if (result.indexOf(value) === -1) {
                    result.push(value);
                }

            }
            return result;
        };

        app.filter('count', function () {
            return function (collection, key) {
                var out = "test";
                for (var i = 0; i < collection.length; i++) {
                    //console.log(collection[i].pants);
                    //var out = myApp.filter('filter')(collection[i].pants, "42", true);
                }
                return out;
            }
        });

        app.filter('groupBy', function () {
            return function (collection, key) {
                if (collection === null) return;
                return uniqueItems(collection, key);
            };
        });

        $scope.$watch('filtered', function (newValue) {
            if (angular.isArray(newValue)) {
                //console.log(newValue.length);
            }
        }, true);

        $scope.count = function (prop, value) {
            return function (el) {
                return el[prop] === value;
            };
        };

        //1-----Carrega membro aba Pesquisar------------



        $scope.pesquisarProdutoPaginacao = function pesquisarProdutoPaginacao(page, filtroPaginacao) {

            $scope.PaginadorUrl = {
                Pagina: {
                    Ativar: true
                }
            };

            page = page || 0;



            var config = {
                params: {
                    page: page,
                    pageSize: 20,
                    subcategoria: sessionStorage.carregaCategoriaProduto,
                    filter: filtroPaginacao
                }
            };
            $scope.Produtos = undefined;
            apiService.get('/api/produto/pesquisar', config,
                produtosLoadCompleted,
                produtosLoadFailed);

        };


        function pesquisarProduto(page) {

            $scope.PaginadorUrl = {
                Pagina: {
                    Ativar: true
                }
            };

            page = page || 0;


            if ($scope.filtroProdutos == '') {


                $scope.filtroProdutos = $scope.filtroProdutosPaginacao;
            } else {

                $scope.filtroProdutosPaginacao = $scope.filtroProdutos;
            }




            $scope.filtroProdutosPaginacao = $scope.filtroProdutos;

            var config = {
                params: {
                    page: page,
                    pageSize: 20,
                    subcategoria: sessionStorage.carregaCategoriaProduto,
                    filter: $scope.filtroProdutos
                }
            };
            $scope.Produtos = undefined;
            apiService.get('/api/produto/pesquisar', config,
                produtosLoadCompleted,
                produtosLoadFailed);

        }

        function addQuantidade(sku, name, price, quantity, campo, imagem) {

            if (quantity == undefined || quantity == "" || quantity == " " || quantity == 0) {
                notificationService.displayInfo("DIGITE A QUANTIDADE !");

            } else {

                $scope.cart.addItem(sku, name, price, quantity, imagem);
                campo.QtdProduto = "";
                notificationService.displaySuccess(name + "<br> Quantidade Adicionada:  <b>" + quantity + "</b>");
            }
        }

        function produtosLoadCompleted(result) {

            $scope.filtroProdutos = '';
           
            $scope.produtos = {};

            $scope.Produtos = result.data.Items;
            // console.log($scope.Produtos);

            $scope.produtos = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;

            // Watch the pants that are selected
            $scope.$watch(function () {
                return {
                    produtos: $scope.Produtos,
                    useMarcas: $scope.useMarcas,
                    useUnidadeMedidas: $scope.useUnidadeMedidas,
                    useCategorias: $scope.useCategorias
                }
            }, function (value) {
                var selected;

                $scope.marcasGroup = uniqueItems($scope.produtos, 'Marca');
                $scope.UnidadeMedidasGroup = uniqueItems($scope.produtos, 'UnidadeMedida');
                $scope.CategoriasGroup = uniqueItems($scope.produtos, 'DescCategoria');


                // console.log($scope.CategoriasGroup);

                var filterAfterMarcas = [];
                var filterAfterUnidadeMedidas = [];
                var filterAfterCategorias = [];
                selected = false;

                for (var j in $scope.produtos) {
                    var p = $scope.produtos[j];
                    for (var i in $scope.useMarcas) {
                        if ($scope.useMarcas[i]) {
                            selected = true;
                            if (i == p.Marca) {
                                filterAfterMarcas.push(p);
                                break;
                            }
                        }
                    }



                    for (var ii in $scope.useUnidadeMedidas) {
                        if ($scope.useUnidadeMedidas[ii]) {
                            selected = true;
                            if (ii == p.UnidadeMedida) {
                                filterAfterUnidadeMedidas.push(p);
                                break;
                            }
                        }
                    }

                    // console.log($scope.useCategorias);
                    // console.log(p);

                    for (var j in $scope.useCategorias) {
                        if ($scope.useCategorias[j]) {
                            selected = true;
                            if (j == p.DescCategoria) {


                                filterAfterCategorias.push(p);
                                break;
                            }
                        }
                    }
                }

                if (!selected) {
                    filterAfterMarcas = $scope.produtos;
                }

                if (!selected) {
                    filterAfterUnidadeMedidas = $scope.produtos;
                }

                if (!selected) {
                    filterAfterCategorias = $scope.produtos;
                }



                $scope.FilteredProdutos = filterAfterMarcas;

                $scope.FilteredProdutosUni = filterAfterUnidadeMedidas;

                $scope.FilteredCategorias = filterAfterCategorias;


                if (filterAfterMarcas)
                    $scope.Produtos = filterAfterMarcas;


                if (filterAfterUnidadeMedidas.length > 0)
                    $scope.Produtos = filterAfterUnidadeMedidas;


                if (filterAfterCategorias.length > 0)
                    $scope.Produtos = filterAfterCategorias;


            }, true);

        }

        function produtosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim---------------------------------------



        //2-----Abre popup detalhe do produto----------------------------------
        function openDetProdDialog(prod) {

            $scope.produtoPesq = prod;

            $scope.close = function () {
                $modalInstance.dismiss('cancel');
            };

            $modal.open({
                templateUrl: 'scripts/SPACliente/produto/detProduto.html',
                controller: 'detProdutoCtrl',
                scope: $scope
            }).result.then(function ($scope) {

                //console.log("Modal Closed!!!");

            }, function () {

                //console.log("Modal Dismissed!!!");
            });

        }
        //---------------------------------------------------------------------------------------


        function selectCustomer($item) {
            if ($item) {
                $scope.filtroProdutos = $item.originalObject.DescProduto;
                pesquisarProduto();
            }
        }

        function inputText(text) {
            $scope.filtroProdutos = text;
        }



        function removeAccents(value) {
            return value
                .replace(/á/g, 'a')
                .replace(/à/g, 'a')
                .replace(/ã/g, 'a')
                .replace(/ç/g, 'c')
                .replace(/é/g, 'e')
                .replace(/ê/g, 'e')
                .replace(/í/g, 'i')
                .replace(/ó/g, 'o')
                .replace(/õ/g, 'o')
                .replace(/ú/g, 'u')
                .replace(/ô/g, 'o')
                .replace(/â/g, 'a')
                ;
        }

        $scope.ignoreAccents = function (item) {
            if (!$scope.filtroProdutos)
                return true;
            var textDescProduto = removeAccents(item.DescProduto.toLowerCase());
            var textDescCategoria = removeAccents(item.DescCategoria.toLowerCase());
            var textDescMarca = removeAccents(item.Marca.toLowerCase());
            var search = removeAccents($scope.filtroProdutos.toLowerCase());
            return textDescProduto.indexOf(search) > -1 || textDescCategoria.indexOf(search) > -1 || textDescMarca.indexOf(search) > -1;
        };



        pesquisarProduto();

    }

})(angular.module('ECCCliente'));