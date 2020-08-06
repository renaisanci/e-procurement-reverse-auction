(function (app) {
    'use strict';
    app.controller('unidadeMedidaCtrl', unidadeMedidaCtrl);
    unidadeMedidaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];

    function unidadeMedidaCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {

        $scope.pageClass = 'page-unidadeMedida';

        $scope.inserirUnidadeMedida = inserirUnidadeMedida;
        $scope.atualizarUnidadeMedida = atualizarUnidadeMedida;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.inserirUnidadeMedidaModel = inserirUnidadeMedidaModel;
        $scope.pesquisarUnidadeMedida = pesquisarUnidadeMedida;
        $scope.editarUnidadeMedida = editarUnidadeMedida;
        $scope.limpaDados = limpaDados;
        //0 ------- Declaracao de todas as abas --//
        $scope.tabsUnidadeMedida = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },


            tabCadUnidadeMedida: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }
        };

        //0 ------ Fim --//

        //1-----Habilita e desabilita abas após clicar em pesquisar--
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsUnidadeMedida = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },


                tabCadUnidadeMedida: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            }
        }
        //1 ----------------------------Fim---------------------------------------//

        //2 ------------------------Inserir ------------------------------------//
        function inserirUnidadeMedida() {
            if ($scope.novoUnidadeMedida.Id > 0) {
                atualizarUnidadeMedida();

            } else {
                inserirUnidadeMedidaModel();
            }
        }

        function inserirUnidadeMedidaModel() {
            apiService.post('/api/unidadeMedida/inserir', $scope.novoUnidadeMedida,
                inserirUnidadeMedidaSucesso,
                inserirUnidadeMedidaFalha);
        }

        function inserirUnidadeMedidaSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoUnidadeMedida = response.data;
            pesquisarUnidadeMedida();
            habilitaDesabilitaAbaPesquisa()
        }

        function inserirUnidadeMedidaFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //2 ----------------------------Fim---------------------------------------//


        //3-----Atualiza Workflow status sistemas-------------------------------
        function atualizarUnidadeMedida() {
            apiService.post('/api/unidadeMedida/atualizar', $scope.novoUnidadeMedida,
                atualizarUnidadeMedidaSucesso,
                atualizarUnidadeMedidaFalha);

        }

        function atualizarUnidadeMedidaSucesso(response) {
            notificationService.displaySuccess($scope.novoUnidadeMedida.DescUnidadeMedida + ' Atualizado com Sucesso.');
            $scope.novoUnidadeMedida = response.data;
            pesquisarUnidadeMedida();
            habilitaDesabilitaAbaPesquisa();
        }

        function atualizarUnidadeMedidaFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //3 ----------------------------Fim---------------------------------------//

        //4 --------------------- Pesquisa Unidade Medida ------------------------//
        function pesquisarUnidadeMedida(page) {

            page = page || 0;

            $scope.loadingUnidadeMedidas = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroUnidadeMedidas
                }
            };

            apiService.get('/api/unidadeMedida/pesquisar', config,
                        unidadeMedidaLoadCompleted,
                        unidadeMedidaLoadFailed);
        }

        function unidadeMedidaLoadCompleted(result) {

            $scope.UnidadesMedidas = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingUnidadeMedidas = false;

            // if ($scope.filtroMembros && $scope.filtroMembros.length) {
            var msg = result.data.Items.length > 1 ? " Registros Encontrados" : "Registro Encontrado";
            if ($scope.page == 0 && $scope.novoUnidadeMedida.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function unidadeMedidaLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //4 ----------------------------Fim---------------------------------------//

        //5 ----------------------- Editar Unidade Medida ------------------------//
        function editarUnidadeMedida(unidadePesq) {
            $scope.novoUnidadeMedida = unidadePesq;

            $scope.tabsUnidadeMedida = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabCadUnidadeMedida: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };

        }
        //5 ----------------------------Fim---------------------------------------//

        //6 ---------------------- Limpa Dados -----------------------------------//
        function limpaDados() {

            $scope.novoUnidadeMedida = '';
            $scope.filtroUnidadeMedidas = '';

        }
        //6 ----------------------------Fim---------------------------------------//


        // -------------------- Inicializa Tela ---------------------------------//
        $scope.pesquisarUnidadeMedida();
        // ----------------------------Fim---------------------------------------//
    }


})(angular.module('ECCAdm'));