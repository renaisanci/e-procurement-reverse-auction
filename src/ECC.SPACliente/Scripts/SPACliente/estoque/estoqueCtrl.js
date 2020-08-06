(function (app) {
    'use strict';

    app.controller('estoqueCtrl', estoqueCtrl);

    estoqueCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$stateParams', 'storeService', '$modal'];

    function estoqueCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $stateParams, storeService, $modal) {

        $scope.pageClass = 'page-estoque';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.pesquisarEstoque = pesquisarEstoque;
        $scope.pesquisarEnderecos = pesquisarEnderecos;
        $scope.pesquisarMembroCategoria = pesquisarMembroCategoria;
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.pesquisarMembro = pesquisarMembro;
        $scope.pesquisarProdutoSelecionado = pesquisarProdutoSelecionado;
        $scope.inserirItemEstoque = inserirItemEstoque;
        $scope.editarItemEstoque = editarItemEstoque;
        $scope.limpaDados = limpaDados;

        $scope.Estoque = [];
        $scope.novoItemEstoque = {};
        $scope.enderecos = [];
        $scope.categorias = [];
        $scope.subcategorias = [];
        $scope.produtos = [];
        $scope.MembroId = '';

        $scope.habilitaCategoria = false;
        $scope.habilitaSubcategoria = false;
        $scope.habilitaProduto = false;

        //0--------Declaracao de todas as abas tela de Estoque--------
        $scope.tabsEstoque = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadEstoque: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },
        };
        //0------------------------Fim--------------------------------

 
        //1-----------Buscar estoque-----------------
        function pesquisarEstoque(page) {

            page = page || 0;

            $scope.loadingEstoque = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.novoItemEstoque.EnderecoId
                }
            };

            apiService.get('/api/estoque/pesquisar', config,
                        estoqueLoadCompleted,
                        funcLoadFailed);
        }

        function estoqueLoadCompleted(result) {

            $scope.Estoque = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingEstoque = false;

            var msg = result.data.Items.length > 1 ? " Produtos Encontrados" : " Produto Encontrado";
            if ($scope.page == 0 && $scope.novoItemEstoque.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
        }
        //1----------------Fim---------------------

        //2------------Pesquisar Enderecos----------------
        function pesquisarEnderecos() {
            apiService.get('/api/estoque/enderecos', null,
                        enderecosLoadComplete,
                        funcLoadFailed);
        }

        function enderecosLoadComplete(response) {
            var newItem = new function () {
                this.Id = undefined;
                this.Endereco = "Endereço...";

            };
            response.data.push(newItem);
            $scope.enderecos = response.data;
        }
        //2--------------------Fim------------------------

        //3---------Pesquisar Categoria Membro-------------
        function pesquisarMembroCategoria() {
            apiService.get('/api/estoque/membrocategoria', null,
                        membroCategoriaLoadComplete,
                        funcLoadFailed);
        }

        function membroCategoriaLoadComplete(response) {
            var newItem = new function () {
                this.Id = undefined;
                this.DescCategoria = "Categoria...";

            };
            response.data.push(newItem);
            $scope.categorias = response.data;
        }
        //3--------------------Fim------------------------

        //4-----------Pesquisar Subcategorias-------------
        function pesquisarSubCategoria(id) {
            $scope.$broadcast('angucomplete-alt:clearInput', 'txtSubcategoria');
            $scope.$broadcast('angucomplete-alt:clearInput', 'txtProduto');

            var config = {
                params: {
                    filter: id
                }
            };

            apiService.get('/api/estoque/subcategorias', config,
                subCategoriaLoadCompleted,
                funcLoadFailed);
        }

        function subCategoriaLoadCompleted(response) {
            $scope.subcategorias = response.data;
        }
        //4--------------------Fim------------------------
 
        //5--------------Pesquisar Produtos---------------
        function pesquisarProduto(item) {
            if (item != undefined) {
                $scope.$broadcast('angucomplete-alt:clearInput', 'txtProduto');
                $scope.novoItemEstoque.subcategoriaId = item.originalObject.Id;

                var config = {
                    params: {
                        filter: $scope.novoItemEstoque.subcategoriaId
                    }
                };

                apiService.get('/api/estoque/produtos', config,
                    produtoLoadCompleted,
                    funcLoadFailed);
            }
        }

        function produtoLoadCompleted(response) {
            $scope.produtos = response.data;
        }
        //5--------------------Fim------------------------

        //6----------Inserir item novo estoque------------
        function inserirItemEstoque() {
            if ($scope.novoItemEstoque.Id > 0) {
                atualizarItemEstoqueModel();
            } else {
                inserirItemEstoqueModel();
            }
        }

        function inserirItemEstoqueModel() {
            if (validaCampos()) {
                $scope.novoItemEstoque.MembroId = $scope.MembroId;
                apiService.post('/api/estoque/inserir', $scope.novoItemEstoque,
                    inserirItemEstoqueSucesso,
                    funcLoadFailed);
            }
        }

        function inserirItemEstoqueSucesso(response) {
            $scope.novoItemEstoque = response.data;

            notificationService.displaySuccess('Produto incluído com sucesso.');

            $scope.tabsEstoque = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadEstoque: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
            };

            pesquisarEstoque();
        }
        //6--------------------Fim------------------------

        //7---------Atualizar item novo estoque-----------
        function atualizarItemEstoqueModel() {
            if (validaCampos()) {
                apiService.post('/api/estoque/atualizar', $scope.novoItemEstoque,
                    atualizarItemEstoqueSucesso,
                    funcLoadFailed);
            }
        }

        function atualizarItemEstoqueSucesso(response) {
            $scope.novoItemEstoque = response.data;

            notificationService.displaySuccess('Produto atualizado com sucesso.');

            $scope.tabsEstoque = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadEstoque: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
            };

            $scope.pesquisarEstoque();
        }
        //7--------------------Fim------------------------

        //8-----------Editar item novo estoque------------
        function editarItemEstoque(pesqEstoque) {
            $scope.novoItemEstoque = pesqEstoque;

            pesquisarProdutoSelecionado(pesqEstoque.ProdutoId);

            $scope.tabsEstoque = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadEstoque: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
            };

            $scope.habilitaCategoria = true;
            $scope.habilitaSubcategoria = true;
            $scope.habilitaProduto = true;
        }
        //8--------------------Fim------------------------

        //9--------------Pesquisar Membro-----------------
        function pesquisarMembro() {
            apiService.get('/api/estoque/membro', null,
                        membroLoadComplete,
                        funcLoadFailed);
        }

        function membroLoadComplete(response) {
            $scope.MembroId = response.data.Id;
        }
        //9--------------------Fim------------------------

        //10-------Pesquisar Produto Selecionado----------
        function pesquisarProdutoSelecionado(produtoId) {
            var config = {
                params: {
                    filter: produtoId
                }
            };

            apiService.get('/api/estoque/produtoSelecionado', config,
                        produtoSelecionadoLoadComplete,
                        funcLoadFailed);
        }

        function produtoSelecionadoLoadComplete(response) {
            $scope.novoItemEstoque.produto = response.data;
        }
        //10--------------------Fim-----------------------

        //-----Limpa dados--------------------------------
        function limpaDados() {
            $scope.habilitaCategoria = false;
            $scope.habilitaSubcategoria = false;
            $scope.habilitaProduto = false;
            $scope.$broadcast('angucomplete-alt:clearInput', 'txtSubcategoria');
            $scope.$broadcast('angucomplete-alt:clearInput', 'txtProduto');
            $scope.novoItemEstoque = '';
            $scope.filtroEstoque = '';

        }
        //-----Fim---------------------------------------

        //-------Valida os campos do formulário----------
        function validaCampos() {

            if ($scope.novoItemEstoque.EnderecoId == undefined) {
                notificationService.displayError('Favor Selecionar campo Endereço.');
                return false;
            }

            if ($scope.novoItemEstoque.produto == undefined) {

                notificationService.displayError('Favor Preencher campo Produto.');
                return false;
            }
            else {
                $scope.novoItemEstoque.ProdutoId = $scope.novoItemEstoque.produto.originalObject.Id;
            }

            if ($scope.novoItemEstoque.MinimoEstoque == undefined) {

                notificationService.displayError('Favor Preencher campo Estoque Mínimo.');
                return false;
            }

            if ($scope.novoItemEstoque.MaximoEstoque == undefined) {

                notificationService.displayError('Favor Preencher campo Estoque Máximo.');
                return false;
            }

            if ($scope.novoItemEstoque.QtdEstoque == undefined) {

                notificationService.displayError('Favor Preencher campo Estoque Atual.');
                return false;
            }

            if ($scope.novoItemEstoque.QtdEstoqueReceber == undefined) {

                notificationService.displayError('Favor Preencher campo Estoque a Receber.');
                return false;
            }

            return true;
        }

        //-----Fim--------------------------------------

        //----------------Habilitar aba pesquisa--------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsEstoque = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadEstoque: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }
        //-------------------------FIM------------------------------
       
        function funcLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        // -------------------- Inicializa Tela ---------------------------------//
        $scope.pesquisarMembro();
        $scope.pesquisarMembroCategoria();
        $scope.pesquisarEnderecos();
        $scope.pesquisarEstoque();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCCliente'));