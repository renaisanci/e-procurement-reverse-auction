(function (app) {
    'use strict';

    app.controller('promocoesCtrl', promocoesCtrl);

    promocoesCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$stateParams', 'storeService', '$modal', '$filter'];

    function promocoesCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $stateParams, storeService, $modal, $filter) {
        $scope.pageClass = 'page-promocoes';

        var url = $location.url();
        localStorage.UrlMembro = url;

        $scope.page = 0;
        $scope.pagesCount = 0;
        
        $scope.Produtos = [];
        $scope.pesquisarProdutoInput = pesquisarProdutoInput;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.cart = storeService.cart;
        $scope.cartPromocoes = storeService.cartPromocoes;
        $scope.openDetProdDialog = openDetProdDialog;
        $scope.useMarcas = {};
        $scope.addQuantidadePromocao = addQuantidadePromocao;
        
        $scope.arrayDiasSemana = [
             { Id: 1, DescSemana: 'Seg' },
             { Id: 2, DescSemana: 'Ter' },
             { Id: 3, DescSemana: 'Qua' },
             { Id: 4, DescSemana: 'Qui' },
             { Id: 5, DescSemana: 'Sex' },
             { Id: 6, DescSemana: 'Sáb' },
             { Id: 7, DescSemana: 'Dom' }
        ];

        //objeto das estrelas do produto
        $scope.isReadOnly = false;

        var uniqueItems = function (data, key) {
            var result = [];

            for (var i = 0; i < data.length; i++) {
                var value = data[i][key];

                if (result.indexOf(value) == -1) {
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
                console.log(newValue.length);
            }
        }, true);

        $scope.count = function (prop, value) {
            return function (el) {
                return el[prop] == value;
            };
        };

        //1-----Carrega membro aba Pesquisar----------------------------------------
        function pesquisarProduto(page) {

            $scope.PaginadorUrl = {
                Pagina: {
                    Ativar: true
                }
            };

            page = page || 0;
            
            var config = {
                params: {
                    page: page,
                    pageSize: 30,
                    categoria: $scope.CategoriaId,
                    subcategoria: $scope.SubCategoriaId,
                    filter: $scope.filtroProdutos
                }
            };

            apiService.get('/api/produtopromocional/pesquisarmembro', config,
                        produtosLoadCompleted,
                        produtosLoadFailed);
        }

        function addQuantidadePromocao(sku, name, price, quantity, campo, imagem, obsFrete, quantidadeMinina, quantidadeEstoque) {

            if (quantity == undefined || quantity === "" || quantity === " " || quantity === 0) {
                notificationService.displayInfo("DIGITE A QUANTIDADE !");

            } else if (quantity < campo.produtoPesq.QtdMinVenda) {
                notificationService.displayInfo("AUMENTE A QUANTIDADE DESTE PRODUTO, POIS ESTÁ ABAIXO DA QUANTIDADE MÍNIMA !");

            } else if (quantity > campo.produtoPesq.QtdProdutos) {
                notificationService.displayInfo("QUANTIDADE ACIMA DO DISPONÍVEL PARA ESTE PRODUTO !");
                
            } else {
                var preco = price.replace(",",".");
                $scope.cartPromocoes.addItemPromocao(sku, name, preco, quantity, imagem, obsFrete, quantidadeMinina, quantidadeEstoque);
                campo.produtoPesq.QtdProdutos = campo.produtoPesq.QtdProdutos - quantity;
                campo.QtdProduto = "";
                notificationService.displaySuccess(name + "<br> Quantidade Adicionada:  <b>" + quantity + "</b>");
            }
        }

        function produtosLoadCompleted(result) {
            
            $scope.produtos = {};

            $scope.Produtos = result.data.Items;
            $scope.produtos = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;



            // Watch the pants that are selected
            $scope.$watch(function () {
                return {
                    produtos: $scope.Produtos,
                    useMarcas: $scope.useMarcas

                }
            }, function (value) {
                var selected;

                $scope.marcasGroup = uniqueItems($scope.produtos, 'Marca');
                var filterAfterMarcas = [];
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
                }
                if (!selected) {
                    filterAfterMarcas = $scope.produtos;
                }
                $scope.FilteredProdutos = filterAfterMarcas;

                $scope.Produtos = filterAfterMarcas;
            }, true);

        }

        function produtosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim------------------------------------------------------------------
        
        //2-----Carrega produto ao pesquisar pelo campo-----------------------------
        function pesquisarProdutoInput(page) {

            $scope.PaginadorUrl = {
                Pagina: {
                    Ativar: false
                }
            };

            $scope.PaginadorInput = {
                Pagina: {
                    Ativar: true
                }
            };

            page = page || 0;



            var config = {
                params: {
                    page: page,
                    pageSize: 6,
                    filter: $scope.filtroProdutos
                }
            };

            apiService.get('/api/produto/pesquisar', config,
                        produtosLoadCompletedInput,
                        produtosLoadFailedInput);


        }

        function produtosLoadCompletedInput(result) {

            $scope.Produtos = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
        }

        function produtosLoadFailedInput(response) {
            notificationService.displayError(response.data);
        }
        //2-----Fim----------------------------------------------------------------
        
        //3-----Abre popup detalhe do produto-------------------------------------
        function openDetProdDialog(prod) {

            //Adiciona nome da semana caso o Fornecedor só atenda a região do membro pelos dias da semana.
            if (prod.Fornecedor.FornecedorPrazoSemanal.length > 0) {
                for (var i = 0; i < prod.Fornecedor.FornecedorPrazoSemanal.length; i++) {
                    for (var j = 0; j < $scope.arrayDiasSemana.length; j++) {
                        if (prod.Fornecedor.FornecedorPrazoSemanal[i].DiaSemana === $scope.arrayDiasSemana[j].Id) {
                            prod.Fornecedor.FornecedorPrazoSemanal[i].DescSemana = $scope.arrayDiasSemana[j].DescSemana;
                            break;
                        }
                    }
                }
            }

            $scope.produtoPesq = prod;

            $scope.close = function () {
                $modalInstance.dismiss('cancel');
            };

            $modal.open({
                templateUrl: 'scripts/SPACliente/promocoes/modalDetPromocao.html',
                controller: 'modalDetPromocaoCtrl',
                scope: $scope
            });


        }
        //3-----------------------------------------------------------------------


        //4----Disponível para pegar no topBarCtrl/Atualiza Qtd em tempo real-----
        $rootScope.$on("pesquisarProdutoPromocao", function (event, data) {
            if (data) {
                for (var i = 0; i < $scope.Produtos.length; i++) {
                    if ($scope.Produtos[i].Id === data.Id) {
                        $scope.Produtos[i].QtdProdutos = data.QtdProdutos;
                    }
                }
            }
        });
        //4-----------------------------------------------------------------------

        pesquisarProduto();

    }

})(angular.module('ECCCliente'));