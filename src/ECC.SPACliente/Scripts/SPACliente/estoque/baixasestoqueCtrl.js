(function (app) {
    'use strict';

    app.controller('baixasestoqueCtrl', baixasestoqueCtrl);

    baixasestoqueCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$stateParams', 'storeService', '$modal', 'SweetAlert'];

    function baixasestoqueCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $stateParams, storeService, $modal, SweetAlert) {

        $scope.pageClass = 'page-baixasestoque';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.pesquisarEstoque = pesquisarEstoque;
        $scope.pesquisarEnderecos = pesquisarEnderecos;
        $scope.editarItemEstoque = editarItemEstoque;
        $scope.salvarItemEstoque = salvarItemEstoque;
        $scope.limpaDados = limpaDados;

        $scope.Estoque = [];
        $scope.novoItemEstoque = {};
        $scope.enderecos = [];

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

        //3-----------Editar item novo estoque------------
        function editarItemEstoque(pesqEstoque) {
            $scope.novoItemEstoque = pesqEstoque;

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
        }
        //3--------------------Fim------------------------

        //4--------------Salvar item estoque--------------
        function salvarItemEstoque() {
            if ($scope.novoItemEstoque.Id > 0) {
                atualizarItemEstoque();
            }
        }

        function atualizarItemEstoque() {
            if (validaCampos()) {
                apiService.post('/api/estoque/atualizar', $scope.novoItemEstoque,
                    atualizarItemEstoqueSucesso,
                    funcLoadFailed);
            }
        }

        function atualizarItemEstoqueSucesso(response) {
            $scope.novoItemEstoque = response.data;

            if ($scope.novoItemEstoque.MinimoEstoque > $scope.novoItemEstoque.QtdEstoque) {
                SweetAlert.swal({
                    title: "URGENTE",
                    text: "O produto " + $scope.novoItemEstoque.DescProduto + " está abaixo do estoque mínimo, faça um novo pedido!",
                    type: "warning"
                });
            }

            limpaDados();

            notificationService.displaySuccess('Produto atualizado com sucesso.');

            $scope.pesquisarEstoque();

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
        //4--------------------Fim------------------------

        //-------Valida os campos do formulário----------
        function validaCampos() {

            if ($scope.novoItemEstoque.QtdEstoque == undefined) {
                notificationService.displayError('Favor Preencher campo Estoque Atual.');
            }
            return true;
        }

        //-----Fim--------------------------------------

        //-----Limpa dados--------------------------------
        function limpaDados() {
            $scope.novoItemEstoque = '';
            $scope.filtroEstoque = '';

        }
        //-----Fim---------------------------------------

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
        $scope.pesquisarEstoque();
        $scope.pesquisarEnderecos();
        // ----------------------------Fim---------------------------------------//

    }
})(angular.module('ECCCliente'));