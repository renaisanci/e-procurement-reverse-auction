(function (app) {
    'use strict';

    app.controller('subcategoriaCtrl', subcategoriaCtrl);

    subcategoriaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'SweetAlert'];

    function subcategoriaCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, SweetAlert) {

        $scope.pageClass = 'page-subcategoria';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.inserirSubCategoria = inserirSubCategoria;
        $scope.editarSubCategoria = editarSubCategoria;
        $scope.limpaDados = limpaDados;
        $scope.novaSubCategoria = {};


        $scope.subcategorias = [];
        $scope.categorias = [];


        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescCategoria';
        $scope.reverse = true;


        //0--------Declaracao de todas as abas tela SubCategoria----
        $scope.tabsSubCategoria = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabSubCadCategoria: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }
        };



        //1-----Carrega Categorias aba Pesquisar------------
        function pesquisarSubCategoria(page) {
            page = page || 0;

            $scope.loadingSubCategoria = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroSubCategoria
                }
            };

            apiService.get('/api/subcategoria/pesquisar', config,
                        subCategoriaLoadCompleted,
                        subCategoriaLoadFailed);
        }

        function subCategoriaLoadCompleted(result) {

            $scope.subcategorias = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingSubCategoria = false;


            var msg = result.data.Items.length > 1 ? " Sub-Categorias Encontradas" : " Sub-Categoria Encontrada";
            if ($scope.page == 0 && $scope.novaSubCategoria.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function subCategoriaLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1-----Fim-----------------------------------------


        //2-----Editar SubCategorias---------------------------
        function editarSubCategoria(pesqSubCategoria) {

            $scope.novaSubCategoria = pesqSubCategoria;

            //Workflow após selecionar um membro no grid
            $scope.tabsSubCategoria = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabSubCadCategoria: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };

        }
        //2-----Fim-----------------------------------------


        //2-----Atualizar SubCategoria---------------------------
        function atualizarSubCategoria() {
            apiService.post('/api/subcategoria/atualizar', $scope.novaSubCategoria,
            atualizarSubCategoriaSucesso,
            atualizarSubCategoriaFalha);
        }

        function atualizarSubCategoriaSucesso(response) {
            notificationService.displaySuccess('Sub-Categoria atualizada com sucesso!');
            $scope.novaSubCategoria = response.data;

            
            $scope.tabsSubCategoria = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabSubCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
            limpaDados();
        }

        function atualizarSubCategoriaFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else if (response.status == '412') {
                notificationService.displayError(response.data)
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //3---------------------Fim----------------------------


        //4-----Inserir nova categoria ------------
        function inserirSubCategoria() {

            if ($scope.novaSubCategoria.Id > 0) {
                atualizarSubCategoria();
            } else {
                inserirSubCategoriaModel();
            }

        }

        function inserirSubCategoriaModel() {
            apiService.post('/api/subcategoria/inserir', $scope.novaSubCategoria,
            inserirSubCategoriaSucesso,
            inserirSubCategoriaFalha);
        }

        function inserirSubCategoriaSucesso(response) {
            notificationService.displaySuccess('sub-Categoria incluída com sucesso.');

            $scope.novaSubCategoria = response.data;

            pesquisarSubCategoria();
         
            $scope.tabsSubCategoria = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabSubCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };

            
        }

        function inserirSubCategoriaFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //4----------------------------Fim---------------------------------------



        //5-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCategoria() {


            apiService.get('/api/subcategoria/categoria', null,
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
            notificationService.displayError(response.data);
        }
        //5-----Fim-----------------------------------------

        //-----Limpa dados--------------------------------
        function limpaDados() {

            $scope.novaSubCategoria = {};
            $scope.filtroSubCategoria = '';
            
        }
        //-----Limpa dados--------------------------------


        //-----Habilitar de Desabilitar Abas------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.novaSubCategoria = '';
            $scope.filtroSubCategoria = '';


            $scope.tabsSubCategoria = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabSubCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }
        //-----Fim-----------------------------------------

        pesquisarCategoria();
        $scope.pesquisarSubCategoria();
     
    }

})(angular.module('ECCAdm'));