(function (app) {
    'use strict';

    app.controller('marcaCtrl', marcaCtrl);

    marcaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];



    function marcaCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {
        $scope.pageClass = 'page-marca';
        $scope.inserirMarca = inserirMarca;
        $scope.pesquisarMarca = pesquisarMarca;
        $scope.editarMarca = editarMarca;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.limpaDados = limpaDados;
        
        //0--------Declaracao de todas as abas tela de Marca----
        $scope.tabsMarca = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadMarca: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

        };
        //0-----------------Fim-------------------------------

        //1-----Inserir nova categoria----------------------------------------
        function inserirMarca() {
           
            if ($scope.novoMarca.Id > 0) {
                atualizarMarca();
            } else {
                inserirMarcaModel();
            }

        }

        function inserirMarcaModel() {
            apiService.post('/api/Marca/inserir', $scope.novoMarca,
            inserirMarcaModelSucesso,
            inserirMarcaModelFalha);
        }

        function inserirMarcaModelSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoMarca = response.data;
            pesquisarMarca();

            $scope.tabsMarca = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadMarca: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }

		function inserirMarcaModelFalha(response) {

			console.log(response.data[0]);

			if (response.status == '400') {
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			}
			else {
				notificationService.displayError(response.statusText);
			}
        }
        //----------------------------Fim---------------------------------------

        //2-----Atualiza Marca  sistemas-------------------------------
        function atualizarMarca() {
            apiService.post('/api/Marca/atualizar', $scope.novoMarca,
            atualizarMarcaSucesso,
            atualizarMarcaFalha);
        }

        function atualizarMarcaSucesso(response) {
            notificationService.displaySuccess('Marca Atualizado com Sucesso.');
            $scope.novoMarca = response.data;
            pesquisarMarca();
            habilitaDesabilitaAbaPesquisa();
        }

        function atualizarMarcaFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else if (response.status == '412') {
                notificationService.displayError('Marca utilizado em algum produto, não é possível ser desativado!');
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //2---------------------Fim--------------------------

        //3-----Carrega Marca aba Pesquisar------------
        function pesquisarMarca(page) {

            page = page || 0;

            $scope.loadingMarca = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroMarca
                }
            };

            apiService.get('/api/marca/pesquisar', config,
                        MarcaLoadCompleted,
                        MarcaLoadFailed);
        }

        function MarcaLoadCompleted(result) {

            $scope.Marca = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMarca = false;


            var msg = result.data.Items.length > 1 ? " Marcas Encontradas" : "Marca  Encontrada";
            if ($scope.page == 0 && $scope.novoMarca.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

		function MarcaLoadFailed(response) {


			console.log(response.status);

			if (response.status == '400') {



			for (var i = 0; i < response.data.length; i++) {
				notificationService.displayInfo(response.data[i]);
			}

		}else {
				notificationService.displayError(response.statusText);
			}
        }
        //3----Fim---------------------------------------

        //4-----Editar dados aba Membro---------------------
        function editarMarca(MarcaPesq) {
            $scope.novoMarca = MarcaPesq;

            //Marca após selecionar um membro no grid
            $scope.tabsMarca = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadMarca: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }
        //4---------------------------------Fim---------------------

        //5-----Limpa os dados da tela para insedir o novo Marca  ------------
        function limpaDados() {

            $scope.novoMarca = '';
            $scope.filtroMarca = '';
            
        }
        //5-------------------------------FIM--------------------------------

        //6-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsMarca = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadMarca: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }
        //6---------------FIM------------------------------

        // -------------------- Inicializa Tela ---------------------------------//
        $scope.pesquisarMarca();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));