(function (app) {
    'use strict';

    app.controller('workflowStatusCtrl', workflowStatusCtrl);

    workflowStatusCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];



    function workflowStatusCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {
        $scope.pageClass = 'page-workflowStatus';
        $scope.inserirWorkStatusSistema = inserirWorkStatusSistema;
        $scope.pesquisarWorkflowStatus = pesquisarWorkflowStatus;
        $scope.editarWorkflowStatus = editarWorkflowStatus;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.limpaDados = limpaDados;

        //0--------Declaracao de todas as abas tela de WorkflowStatus----
        $scope.tabsWorkflowStatus = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadWorkflowStatus: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

        };
        //0-----------------Fim-------------------------------

        //1-----Inserir nova categoria----------------------------------------
        function inserirWorkStatusSistema() {

            if ($scope.novoWorkFlowStatusSistema.Id > 0) {
                atualizarrWorkStatusSistema();
            } else {
                inserirWorkStatusSistemaModel();
            }

        }

        function inserirWorkStatusSistemaModel() {
            apiService.post('/api/workflowstatus/inserir', $scope.novoWorkFlowStatusSistema,
            inserirWorkStatusSistemaSucesso,
            inserirWorkStatusSistemaFalha);
        }

        function inserirWorkStatusSistemaSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoWorkFlowStatusSistema = response.data;
            pesquisarWorkflowStatus();

            $scope.tabsWorkflowStatus = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadWorkflowStatus: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }

        function inserirWorkStatusSistemaFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //----------------------------Fim---------------------------------------

        //2-----Atualiza Workflow status sistemas-------------------------------
        function atualizarrWorkStatusSistema() {
            apiService.post('/api/workflowstatus/atualizar', $scope.novoWorkFlowStatusSistema,
            atualizarWorkflowStatusSucesso,
            atualizarWorkflowStatusFalha);
        }

        function atualizarWorkflowStatusSucesso(response) {
            notificationService.displaySuccess('Workflow Status de Sistema Atualizado com Sucesso.');
            $scope.novoWorkFlowStatusSistema = response.data;
            pesquisarWorkflowStatus();
            habilitaDesabilitaAbaPesquisa();
        }

        function atualizarWorkflowStatusFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else if (response.status == '412') {
                notificationService.displayError('Workflow utilizado em algum status, não é possível ser desativado!')
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //2---------------------Fim--------------------------

        //3-----Carrega Workflow aba Pesquisar------------
        function pesquisarWorkflowStatus(page) {

            page = page || 0;

            $scope.loadingWorkflowStatus = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroWorkflowStatus
                }
            };

            apiService.get('/api/workflowstatus/pesquisar', config,
                        workFlowLoadCompleted,
                        workFlowLoadFailed);
        }

        function workFlowLoadCompleted(result) {

            $scope.WorkflowStatus = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingWorkflowStatus = false;


            var msg = result.data.Items.length > 1 ? " Workflow de Status Encontrados" : "Workflow de Status Encontrado";
            if ($scope.page == 0 && $scope.novoWorkFlowStatusSistema.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function workFlowLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //3----Fim---------------------------------------

        //4-----Editar dados aba Membro---------------------
        function editarWorkflowStatus(workflowstatusPesq) {
            $scope.novoWorkFlowStatusSistema = workflowstatusPesq;

            //Workflow após selecionar um membro no grid
            $scope.tabsWorkflowStatus = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadWorkflowStatus: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }
        //4---------------------------------Fim---------------------

        //5-----Limpa os dados da tela para insedir o novo Workflow Status Sistema------------
        function limpaDados() {

            $scope.novoWorkFlowStatusSistema = '';
            $scope.filtroWorkflowStatus = '';
        }
        //5-------------------------------FIM--------------------------------

        //6-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsWorkflowStatus = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadWorkflowStatus: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }
        //6---------------FIM------------------------------

        // -------------------- Inicializa Tela ---------------------------------//
        $scope.pesquisarWorkflowStatus();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));