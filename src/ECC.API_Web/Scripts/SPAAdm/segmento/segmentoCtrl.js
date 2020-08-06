(function (app) {
    'use strict';

    app.controller('segmentoCtrl', segmentoCtrl);

    segmentoCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];



    function segmentoCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {
        $scope.pageClass = 'page-segmento';
        $scope.inserirsegmento = inserirsegmento;
        $scope.pesquisarsegmento = pesquisarsegmento;
        $scope.editarsegmento = editarsegmento;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.limpaDados = limpaDados;
        
        //0--------Declaracao de todas as abas tela de segmento----
        $scope.tabssegmento = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadsegmento: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

        };
        //0-----------------Fim-------------------------------

        //1-----Inserir nova categoria----------------------------------------
        function inserirsegmento() {
           
            if ($scope.novosegmento.Id > 0) {
                atualizarsegmento();
            } else {
                inserirsegmentoModel();
            }

        }

        function inserirsegmentoModel() {
            apiService.post('/api/segmento/inserir', $scope.novosegmento,
            inserirsegmentoModelSucesso,
            inserirsegmentoModelFalha);
        }

        function inserirsegmentoModelSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novosegmento = response.data;
            pesquisarsegmento();

            $scope.tabssegmento = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }

        function inserirsegmentoModelFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //----------------------------Fim---------------------------------------

        //2-----Atualiza segmento  sistemas-------------------------------
        function atualizarsegmento() {
            apiService.post('/api/segmento/atualizar', $scope.novosegmento,
            atualizarsegmentoSucesso,
            atualizarsegmentoFalha);
        }

        function atualizarsegmentoSucesso(response) {
            notificationService.displaySuccess('segmento Atualizado com Sucesso.');
            $scope.novosegmento = response.data;
            pesquisarsegmento();
            habilitaDesabilitaAbaPesquisa();
        }

        function atualizarsegmentoFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else if (response.status == '412') {
                notificationService.displayError('segmento utilizado em algum produto, não é possível ser desativado!');
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //2---------------------Fim--------------------------

        //3-----Carrega segmento aba Pesquisar------------
        function pesquisarsegmento(page) {

            page = page || 0;

            $scope.loadingsegmento = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtrosegmento
                }
            };

            apiService.get('/api/segmento/pesquisar', config,
                        segmentoLoadCompleted,
                        segmentoLoadFailed);
        }

        function segmentoLoadCompleted(result) {

            $scope.segmento = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingsegmento = false;


            var msg = result.data.Items.length > 1 ? " segmentos Encontradas" : "segmento  Encontrada";
            if ($scope.page == 0 && $scope.novosegmento.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function segmentoLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //3----Fim---------------------------------------

        //4-----Editar dados aba Membro---------------------
        function editarsegmento(segmentoPesq) {
            $scope.novosegmento = segmentoPesq;

            //segmento após selecionar um membro no grid
            $scope.tabssegmento = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }
        //4---------------------------------Fim---------------------

        //5-----Limpa os dados da tela para insedir o novo segmento  ------------
        function limpaDados() {

            $scope.novosegmento = '';
            $scope.filtrosegmento = '';
            
        }
        //5-------------------------------FIM--------------------------------

        //6-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabssegmento = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }
        //6---------------FIM------------------------------

        // -------------------- Inicializa Tela ---------------------------------//
        $scope.pesquisarsegmento();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));