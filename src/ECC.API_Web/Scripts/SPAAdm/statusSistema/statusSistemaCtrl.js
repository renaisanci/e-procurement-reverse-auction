(function (app) {
    'use strict';

    app.controller('statusSistemaCtrl', statusSistemaCtrl);

    statusSistemaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'SweetAlert'];

    function statusSistemaCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, SweetAlert) {
        $scope.pageClass = 'page-statusSistema';
        $scope.inserirStatusSistema = inserirStatusSistema;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.pesquisarStatusSistema = pesquisarStatusSistema;
        $scope.editarStatusSistema = editarStatusSistema;
        $scope.deletarStatusSistema = deletarStatusSistema;
        $scope.limpaDados = limpaDados;
        $scope.workflowstatus = [];
        $scope.novoStatusSistema = [];

        //0--------Declaracao de todas as abas tela de Status de Sistema----
        $scope.tabsStatusSistema = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadStatusSistema: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },
        };
        //0-----------------Fim-------------------------------

        //1-----Insere novo Status Sistema aba Cadastro------------
        function inserirStatusSistema() {

            if ($scope.novoStatusSistema.WorkflowStatusId > 0) {
                if ($scope.novoStatusSistema.Id > 0) {
                    atualizarStatusSistema();
                } else {
                    inserirStatusSistemaModel();
                }
            }
            else {
                notificationService.displayError("Selecione o workflow do Status.");
            }
        }

        function inserirStatusSistemaModel() {

            apiService.post('/api/StatusSistema/inserir', $scope.novoStatusSistema,
            inserirStatusSistemaSucesso,
            inserirStatusSistemaFalha);
        }

        function inserirStatusSistemaSucesso(response) {
            notificationService.displaySuccess(' Incluído com Sucesso.');
            $scope.novoStatusSistema = response.data;
            pesquisarStatusSistema();

            $scope.tabsStatusSistema = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadStatusSistema: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
            };
        }

        function inserirStatusSistemaFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //1----------------------------Fim---------------------------------------

        //2-----Atualiza dados membro aba Membro------------
        function atualizarStatusSistema() {
            apiService.post('/api/StatusSistema/atualizar', $scope.novoStatusSistema,
            atualizarStatusSistemaSucesso,
            atualizarStatusSistemaFalha);

        }

        function atualizarStatusSistemaSucesso(response) {
            notificationService.displaySuccess($scope.novoStatusSistema.DescStatus + ' Atualizado com Sucesso.');
            $scope.novoStatusSistema = response.data;
            pesquisarStatusSistema();
            habilitaDesabilitaAbaPesquisa();

        }

        function atualizarStatusSistemaFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2---------------------Fim--------------------------

        //3-----Carrega dropdown WorkflowStatus --
        function workflowstatus() {
            apiService.get('/api/statusSistema/workflowstatus', null,
            workflowstatusLoadCompleted,
            workflowstatusLoadFailed);
        }

        function workflowstatusLoadCompleted(response) {


            var newItem = new function () {
                this.Id = undefined;
                this.DescWorkslowStatus = "Workflow Status...";

            };
            response.data.push(newItem);
            $scope.workflowstatus = response.data;
        }

        function workflowstatusLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //3------------------------fim-----------------------------

        //4-----Limpa os dados da tela para insedir o novo Status Sistema------------
        function limpaDados() {

            $scope.novoStatusSistema = '';
            $scope.filtroStatusSistema = '';
        }
        //4-------------------------------FIM--------------------------------

        //5-----Carrega Status aba Pesquisar------------
        function pesquisarStatusSistema(page) {

            page = page || 0;

            $scope.loadingStatusSistema = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroStatusSistema
                }
            };

            apiService.get('/api/StatusSistema/pesquisar', config,
                        statusSistemaLoadCompleted,
                        statusSistemaLoadFailed);
        }

        function statusSistemaLoadCompleted(result) {

            $scope.StatusSistema = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingStatusSistema = false;


            var msg = result.data.Items.length > 1 ? " Status Encontrados" : "Status Encontrado";
            if ($scope.page == 0 && $scope.novoStatusSistema.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function statusSistemaLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //5----Fim---------------------------------------

        //6-----Editar dados aba Membro---------------------
        function editarStatusSistema(statusSistemaPesq) {

            $scope.novoStatusSistema = statusSistemaPesq;

            //Workflow após selecionar um membro no grid
            $scope.tabsStatusSistema = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadStatusSistema: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }
        //6---------------------------------Fim---------------------

        //7-----Atualiza dados membro aba Membro------------
        function deletarStatusSistema(statusSistemaPesq) {

            SweetAlert.swal({
                title: "Você Tem Certeza?",
                text: "Deseja deletar " + statusSistemaPesq.DescStatus + " ?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok",
                cancelButtonText: "Cancelar",
                closeOnConfirm: true,
                closeOnCancel: true
            },
                        function (isConfirm) {
                            if (isConfirm) {
                                apiService.post('/api/StatusSistema/deletar', statusSistemaPesq,
                    deletarSistemaSucesso,
                    deletarStatusSistemaFalha);
                            } else {
                                // Não confirmar não faça nada nesse caso
                            }
                        });



        }

        function deletarSistemaSucesso(response) {
            notificationService.displaySuccess('Registro Deletado com Sucesso.');
            $scope.novoStatusSistema = ''
            pesquisarStatusSistema();

        }

        function deletarStatusSistemaFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //7---------------------Fim--------------------------

        //8-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsStatusSistema = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadStatusSistema: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }
        //8---------------FIM------------------------------

        // -------------------- Inicializa Tela ---------------------------------//
        workflowstatus();
        $scope.pesquisarStatusSistema();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));