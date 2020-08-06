(function (app) {
    'use strict';

    app.controller('fabricanteCtrl', fabricanteCtrl);

    fabricanteCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];



    function fabricanteCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {
        $scope.pageClass = 'page-fabricante';
        $scope.inserirFabricante = inserirFabricante;
        $scope.pesquisarFabricante = pesquisarFabricante;
        $scope.editarFabricante = editarFabricante;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.limpaDados = limpaDados;
        
        //0--------Declaracao de todas as abas tela de Fabricante----
        $scope.tabsFabricante = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadFabricante: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

        };
        //0-----------------Fim-------------------------------

        //1-----Inserir nova categoria----------------------------------------
        function inserirFabricante() {
            debugger;
            if ($scope.novoFabricante.ID > 0) {
                atualizarFabricante();
            } else {
                inserirFabricanteModel();
            }

        }

        function inserirFabricanteModel() {
            apiService.post('/api/fabricante/inserir', $scope.novoFabricante,
            inserirFabricanteModelSucesso,
            inserirFabricanteModelFalha);
        }

        function inserirFabricanteModelSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoFabricante = response.data;
            pesquisarFabricante();

            $scope.tabsFabricante = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFabricante: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }

        function inserirFabricanteModelFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //----------------------------Fim---------------------------------------

        //2-----Atualiza Fabricante status sistemas-------------------------------
        function atualizarFabricante() {
            apiService.post('/api/fabricante/atualizar', $scope.novoFabricante,
            atualizarFabricanteSucesso,
            atualizarFabricanteFalha);
        }

        function atualizarFabricanteSucesso(response) {
            notificationService.displaySuccess('Fabricante Atualizado com Sucesso.');
            $scope.novoFabricante = response.data;
            pesquisarFabricante();
            habilitaDesabilitaAbaPesquisa();
        }

        function atualizarFabricanteFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else if (response.status == '412') {
                notificationService.displayError('Fabricante utilizado em algum produto, não é possível ser desativado!')
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //2---------------------Fim--------------------------

        //3-----Carrega Fabricante aba Pesquisar------------
        function pesquisarFabricante(page) {

            page = page || 0;

            $scope.loadingFabricante = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroFabricante
                }
            };

            apiService.get('/api/fabricante/pesquisar', config,
                        FabricanteLoadCompleted,
                        FabricanteLoadFailed);
        }

        function FabricanteLoadCompleted(result) {

            $scope.Fabricante = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingFabricante = false;


            var msg = result.data.Items.length > 1 ? " Fabricantes Encontrados" : "Fabricante  Encontrado";
            if ($scope.page == 0 && $scope.novoFabricante.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function FabricanteLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //3----Fim---------------------------------------

        //4-----Editar dados aba Membro---------------------
        function editarFabricante(FabricantePesq) {
            $scope.novoFabricante = FabricantePesq;

            //Fabricante após selecionar um membro no grid
            $scope.tabsFabricante = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFabricante: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }
        //4---------------------------------Fim---------------------

        //5-----Limpa os dados da tela para insedir o novo Fabricante Status Sistema------------
        function limpaDados() {

            $scope.novoFabricante = '';
            $scope.filtroFabricante = '';
        }
        //5-------------------------------FIM--------------------------------

        //6-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsFabricante = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFabricante: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }
        //6---------------FIM------------------------------

        // -------------------- Inicializa Tela ---------------------------------//

            $scope.pesquisarFabricante();
        
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));