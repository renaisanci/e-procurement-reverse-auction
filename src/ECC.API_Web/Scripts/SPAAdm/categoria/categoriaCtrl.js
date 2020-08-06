(function (app) {
    'use strict';

    app.controller('categoriaCtrl', categoriaCtrl);

    categoriaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'SweetAlert', 'admUtilService'];

    function categoriaCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, SweetAlert, admUtilService) {

        $scope.pageClass = 'page-categoria';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.habilitaDesabilitaAbaCategoria = habilitaDesabilitaAbaCategoria;
        $scope.habilitaDesabilitaAbaSegmento = habilitaDesabilitaAbaSegmento;
        $scope.inserirCategoriaSegmentos = inserirCategoriaSegmentos;
        $scope.novaCategoria = {};
        $scope.pesquisarCategoria = pesquisarCategoria;
        $scope.inserirCategoria = inserirCategoria;
        $scope.limpaDados = limpaDados;
        $scope.editarCategoria = editarCategoria;
        $scope.editarCategoriaSegmento = editarCategoriaSegmento;
        $scope.categorias = [];
        $scope.segmentos = [];

        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescCategoria';
        $scope.reverse = true;


        //0--------Declaracao de todas as abas tela de Categoria----
        $scope.tabsCategoria = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadCategoria: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },
            tabCadsegmento: {
                tabAtivar: "",
                tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            }
        };

        //-----Limpa dados--------------------------------------------------------
        function limpaDados() {

            $scope.novaCategoria = '';
            $scope.filtroCategoria = '';

        }
        //-----Limpa dados--------------------------------------------------------



        //1-----Carrega Categorias aba Pesquisar----------------------------------
        function pesquisarCategoria(page) {
            page = page || 0;

            $scope.loadingCategoria = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroCategoria
                }
            };

            apiService.get('/api/produto/categorias', config,
                        categoriaLoadCompleted,
                        categoriaLoadFailed);
        }

        function categoriaLoadCompleted(result) {

            $scope.categorias = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingCategoria = false;


            var msg = result.data.Items.length > 1 ? " Categorias Encontradas" : "Categoria Encontrada";
            if ($scope.page == 0 && $scope.novaCategoria.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function categoriaLoadFailed(response) {

            notificationService.displayError(response.data);
        }
        //1-----Fim---------------------------------------------------------------
        

        //2-----Editar Categorias-------------------------------------------------
        function editarCategoria(pesqCategoria) {

            $scope.novaCategoria = pesqCategoria;

            //Workflow após selecionar um membro no grid
            $scope.tabsCategoria = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadCategoria: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };

        }
        //2-----Fim---------------------------------------------------------------


        //3-----Atualizar Categoria-----------------------------------------------
        function atualizarCategoria() {
            apiService.post('/api/categoria/atualizar', $scope.novaCategoria,
            atualizarCategoriaSucesso,
            atualizarCategoriaFalha);
        }

        function atualizarCategoriaSucesso(response) {
            notificationService.displaySuccess('Categoria atualizada com sucesso!');
            $scope.novaCategoria = response.data;
            pesquisarCategoria();

            $scope.tabsCategoria = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };
        }

        function atualizarCategoriaFalha(response) {
            console.log(response);
            if (response.status == '400') {
                if (response.data.DescAtivo === "Ativo") {
                    $scope.novaCategoria.Ativo = true;

                    notificationService.displayInfo('Categoria relacionada com usuário, retire o relacionamento para desativar a mesma!');
                }
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //3---------------------Fim----------------------------------------------


        //4-----Inserir nova categoria ------------------------------------------
        function inserirCategoria() {

            if ($scope.novaCategoria.Id > 0) {
                atualizarCategoria();
            } else {
                inserirCategoriaModel();
            }

        }

        function inserirCategoriaModel() {
            apiService.post('/api/categoria/inserir', $scope.novaCategoria,
            inserirCategoriaSucesso,
            inserirCategoriaFalha);
        }

        function inserirCategoriaSucesso(response) {

            notificationService.displaySuccess('Categoria incluída com sucesso.');

            $scope.novaCategoria = response.data;

            pesquisarCategoria();

            $scope.tabsCategoria = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };
        }

        function inserirCategoriaFalha(response) {

            if (response.status == '400')

                for (var i = 0; i < response.data.length; i++) {

                    notificationService.displayInfo(response.data[i]);
                }

            else if (response.status == '304') {

                notificationService.displayError('Já existe uma categoria com este nome!');

            } else {
            
                notificationService.displayError(response.statusText);
            }
            
        }
        //4----------------------------Fim---------------------------------------
        

        //5-----Habilitar de Desabilitar Abas------------------------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.novaCategoria = '';
            $scope.filtroCategoria = '';

            $scope.tabsCategoria = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };
        }

        function habilitaDesabilitaAbaCategoria() {
            $scope.tabsCategoria = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadCategoria: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadsegmento: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };
        }

        function habilitaDesabilitaAbaSegmento() {
            loadSegmentos()
            $scope.tabsCategoria = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabCadCategoria: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabCadsegmento: {
                    tabAtivar: "active",
                    tabhabilitar: $scope.novaCategoria.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };
        }
        //5-----Fim-------------------------------------------------------------


        //6-----Carrega Segmentos --
        function loadSegmentos() {

            apiService.get('/api/segmento/loadsegmentos', null,
                    loadSegmentosSucesso,
                    loadSegmentosFailed);
        }

        function loadSegmentosSucesso(response) {
            $scope.segmentos = response.data;
            loadCategoriaSegmento();


        }

        function loadSegmentosFailed(response) {
            notificationService.displayError(response.data);
        }
        //6------------------------fim-----------------------------

        //7-----Carrega categoria fornecedor --
        function loadCategoriaSegmento() {

            var config = {
                params: {
                    CategoriaId: $scope.novaCategoria.Id
                }
            };


            apiService.get('/api/categoria/categoriaSegmento', config,
                  loadCategoriaSegmentoSucesso,
                  loadCategoriaSegmentoFailed);
        }

        function loadCategoriaSegmentoSucesso(response) {

            var categoriaSegmentos = response.data;
            for (var i = 0; i < $scope.segmentos.length; i++) {
                for (var j = 0; j < categoriaSegmentos.length; j++) {

                    if ($scope.segmentos[i].Id == categoriaSegmentos[j].Id) {
                        $scope.segmentos[i].Relacionado = true;
                        $scope.segmentos[i].selected = true;
                    }
                }

            }
        }

        function loadCategoriaSegmentoFailed(response) {
            notificationService.displayError(response.data);
        }
        //7------------------------fim-----------------------------

        //8-----Relaciona categoria de produtos ao fornecedor----------
        function inserirCategoriaSegmentos() {

            $scope.segmentoCategorias = [];
            angular.forEach($scope.segmentos, function (segmentoPesq) {
                if (segmentoPesq.selected) $scope.segmentoCategorias.push(segmentoPesq.Id);
            });

            apiService.post('/api/categoria/inserirCategoriaSegmento/' + $scope.novaCategoria.Id, $scope.segmentoCategorias,
                inserirCategoriaSegmentosSucesso,
                inserirCategoriaSegmentosFalha);
        }


        function inserirCategoriaSegmentosSucesso(result) {

            if (result.data.success)
                notificationService.displaySuccess('Segmento relacionado com sucesso.');
            //loadFornecedorCategoria();
            loadCategoriaSegmento();

        }

        function inserirCategoriaSegmentosFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //8------------------------fim-----------------------------

        
        //9-----Editar Categorias---------------------------
        function editarCategoriaSegmento(pesqCategoria) {

            $scope.novaCategoria = pesqCategoria;
            
            habilitaDesabilitaAbaSegmento();

        }
        //9------------------------fim-------------------------

        //checkedall-------------------------------------------------
        $scope.checkAll = admUtilService.checkBoxAll;
        //-----------------------------------------------------------


        $scope.pesquisarCategoria();

    }

})(angular.module('ECCAdm'));