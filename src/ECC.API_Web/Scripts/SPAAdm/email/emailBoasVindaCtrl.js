(function (app) {
    'use strict';

    app.controller('emailBoasVindaCtrl', emailBoasVindaCtrl);

    emailBoasVindaCtrl.$inject = ['$scope', 'membershipService', '$modal', 'notificationService', '$rootScope', '$location', 'apiService', 'admUtilService'];

    function emailBoasVindaCtrl($scope, membershipService, $modal, notificationService, $rootScope, $location, apiService, admUtilService) {
        $scope.pageClass = 'page-EmailBoasVinda';
        $scope.pesquisarMembro = pesquisarMembro;
        $scope.pesquisarFornecedor = pesquisarFornecedor;
        $scope.habilitaDesabilitaAbaPesquisaM = habilitaDesabilitaAbaPesquisaM;
        $scope.habilitaDesabilitaAbaPesquisaF = habilitaDesabilitaAbaPesquisaF;
        $scope.habilitaDesabilitaAbaPesquisaMembroPf = habilitaDesabilitaAbaPesquisaMembroPf;
        $scope.enviarEmailMembro = enviarEmailMembro;
        $scope.enviarEmailFornecedor = enviarEmailFornecedor;

        $scope.page = 0;
        $scope.pagesCount = 0;


        //0--------Declaracao de todas as abas tela de membros----
        $scope.tabs = {
            tabPesquisarM: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },

            tabPesquisarMPF: {
                tabAtivar: " ",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },



            tabPesquisarF: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }


        };
        //0-----------------Fim-------------------------------



        //1-----Carrega membro aba Pesquisar------------
        function pesquisarMembro(page) {

            page = page || 0;

            $scope.loadingMembros = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 200,
                    filter: $scope.filtroMembros
                }
            };

            apiService.get('/api/membro/pesquisar', config,
                membrosLoadCompleted,
                membrosLoadFailed);
        }

        function membrosLoadCompleted(result) {
            $scope.Membros = [];



            var membros = result.data.Items;

            for (var i = 0; i < membros.length; i++) {

                if (membros[i].Usuario == null && membros[i].Ativo !== false) {

                    $scope.Membros.push(membros[i]);
                }

            }

            console.log($scope.Membros);

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMembros = false;

            // if ($scope.filtroMembros && $scope.filtroMembros.length) {
            //var msg = result.data.Items.length > 1 ? " Membros Encontrados" : "Membro Encontrado";

            //notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function membrosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim---------------------------------------


        //2-----Carrega Fornecedor aba Pesquisar------------
        function pesquisarFornecedor(page) {

            page = page || 0;
            var config = {
                params: {
                    page: page,
                    pageSize: 200,
                    filter: $scope.filtroFornecedor
                }
            };

            apiService.get('/api/fornecedor/pesquisar', config,
                fornecedoresLoadSucesso,
                fornecedoresLoadFailed);
        }

        function fornecedoresLoadSucesso(result) {
            $scope.Fornecedores = [];

            var fornecedores = result.data.Items;

            for (var f = 0; f < fornecedores.length; f++) {

                if (fornecedores[f].Usuario == null && fornecedores[f].Ativo !== false) {

                    $scope.Fornecedores.push(fornecedores[f]);
                }
            }

            $scope.pagina = result.data.Page;
            $scope.paginasContador = result.data.TotalPages;
            $scope.totalContador = result.data.TotalCount;

        }

        function fornecedoresLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //2----Fim---------------------------------------

        //3---------Habilita Desabilita Abas Membro-----------
        function habilitaDesabilitaAbaPesquisaM() {

            $scope.tabs = {
                tabPesquisarM: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabPesquisarMPF: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPesquisarF: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }


            };

            pesquisarMembro();
        }
        //3--------------------------------------------------

        //4---------Habilita Desabilita Aba Fornecedor-----------
        function habilitaDesabilitaAbaPesquisaF() {

            $scope.tabs = {
                tabPesquisarM: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabPesquisarMPF: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabPesquisarF: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }


            };

            pesquisarFornecedor();
        }
        //4------------------------------------------------------

        //5-----------Enviar Email Membro-----------------------
        function enviarEmailMembro(membro) {

            apiService.post('/api/email/enviaEmailMembro/' + membro.Id, null,
                loadEnviarEmailMembroCompleted,
                loadEnviarEmailMembroFailed);

            function loadEnviarEmailMembroCompleted(result) {

                notificationService.displaySuccess('E-mail enviado com sucesso!');

                pesquisarMembro();
            }

            function loadEnviarEmailMembroFailed(result) {

                notificationService.displayError(result.data);
            }

        }
        //5-----------------------------------------------------

        //6-----------Enviar Email Membro-----------------------
        function enviarEmailFornecedor(fornecedor) {

            apiService.post('/api/email/enviaEmailFornecedor/' + fornecedor.Id, null,
                loadEnviarEmailFornecedorCompleted,
                loadEnviarEmailFornecedorFailed);

            function loadEnviarEmailFornecedorCompleted(result) {

                notificationService.displaySuccess('E-mail enviado com sucesso!');

                pesquisarFornecedor();
            }

            function loadEnviarEmailFornecedorFailed(result) {

                notificationService.displayError('Erro ao enviar E-mail!');
            }

        }
        //6-----------------------------------------------------

        //8-----Carrega membro PF aba Pesquisar------------
        function pesquisarMembroPf(page) {

            page = page || 0;



            var config = {
                params: {
                    page: page,
                    pageSize: 200,
                    filter: $scope.filtroMembros
                }
            };

            apiService.get('/api/membro/pesquisarPf', config,
                membrosPfLoadCompleted,
                membrosPfLoadFailed);
        }

        function membrosPfLoadCompleted(result) {
            $scope.MembrosPf = [];



            var membrosPf = result.data.Items;

            for (var i = 0; i < membrosPf.length; i++) {

                if (membrosPf[i].Usuario == null && membrosPf[i].Ativo !== false) {

                    $scope.MembrosPf.push(membrosPf[i]);
                }

            }


            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;


        }

        function membrosPfLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //8----Fim---------------------------------------

        //4---------Habilita Desabilita Aba Fornecedor-----------
        function habilitaDesabilitaAbaPesquisaMembroPf() {

            $scope.tabs = {
                tabPesquisarM: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabPesquisarF: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabPesquisarMPF: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }


            };

            pesquisarMembroPf();
        }
        //4------------------------------------------------------

        $scope.pesquisarMembro();


    }

})(angular.module('ECCAdm'));