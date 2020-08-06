(function (app) {
    'use strict';

    app.controller('menuCtrl', menuCtrl);

    menuCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];

    function menuCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {

        $scope.pageClass = 'page-menu';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.pesquisarMenu = pesquisarMenu;
        $scope.inserirMenu = inserirMenu;
        $scope.editarMenu = editarMenu;
        $scope.limpaDados = limpaDados;
        $scope.novoMenu = {};
        $scope.filtroMenu = '';

        $scope.Menus = [];
        $scope.modulos = [];

        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescMenu';
        $scope.reverse = true;

        //0--------Declaracao de todas as abas tela de Menu----
        $scope.tabsMenu = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadMenu: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }

        };
        //0-----------------Fim-------------------------------

        //1-----Carrega Menu aba Pesquisar------------
        function pesquisarMenu(page) {

            page = page || 0;

            $scope.loadingMenu = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    modulo: $scope.novoMenu.ModuloId,
                    filter: $scope.filtroMenu
                }
            };

            apiService.get('api/menu/pesquisar', config,
                        menuLoadCompleted,
                        menuLoadFailed);
        }

        function menuLoadCompleted(result) {

            $scope.Menus = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMenu = false;

            var msg = result.data.Items.length > 1 ? " Menu Encontrados" : " Menu Encontrado";
            if ($scope.page == 0 && $scope.novoMenu.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
        }

        function menuLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim---------------------------------------

        //2-----Carrega Modalidades para DropDown Modalidades------------
        function pesquisarModulos() {

            apiService.get('/api/menu/modulos', null,
                        moduloLoadCompleted,
                        moduloLoadFailed);
        }

        function moduloLoadCompleted(response) {

            var newItem = new function () {
                this.Id = undefined;
                this.DescModulo = "Módulo...";

            };
            response.data.push(newItem);
            $scope.modulos = response.data;

        }

        function moduloLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //2-----Fim-----------------------------------------

        //3-----Editar Menu---------------------------
        function editarMenu(pesqMenu) {

            $scope.novoMenu = pesqMenu;

            //Workflow após selecionar um menu no grid
            $scope.tabsMenu = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadMenu: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            }

        }
        //3-----Fim-----------------------------------------

        //4-----Inserir novo menu------------
        function inserirMenu() {

            if ($scope.novoMenu.Id > 0) {
                atualizarMenu();
            } else {
                inserirMenuModel();
            }

        }

        function inserirMenuModel() {
            apiService.post('/api/menu/inserir', $scope.novoMenu,
            inserirMenuSucesso,
            inserirMenuFalha);
        }

        function inserirMenuSucesso(response) {
            notificationService.displaySuccess('Menu incluído com sucesso.');

            $scope.novoMenu = response.data;

            pesquisarMenu();

            $scope.tabsMenu = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadMenu: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }

        function inserirMenuFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //4----------------------------Fim---------------------------------------

        //5-----Atualizar menu---------------------------------------------------
        function atualizarMenu() {
            apiService.post('/api/menu/atualizar', $scope.novoMenu,
            atualizarMenuSucesso,
            atualizarMenuFalha);
        }

        function atualizarMenuSucesso(response) {
            notificationService.displaySuccess('Menu atualizado com sucesso!');

            habilitaDesabilitaAbaPesquisa();

            limpaDados();
        }

        function atualizarMenuFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else if (response.status == '412') {
                notificationService.displayError(response.data);
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //5---------------------Fim----------------------------
        

        //-----Limpa dados--------------------------------
        function limpaDados() {

            $scope.novoMenu = {};
            $scope.filtroMenu = '';

        }
        //-----Limpa dados--------------------------------

        //-----Habilitar de Desabilitar Abas------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.novoMenu = {};
            $scope.filtroMenu = '';

            $scope.tabsMenu = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadMenu: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };
        }
        //-----Fim-----------------------------------------

        // -------------------- Inicializa Tela ---------------------------------//
        pesquisarModulos();
        pesquisarMenu();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));