(function (app) {
    'use strict';

    app.controller('grupoPermissaoCtrl', grupoPermissaoCtrl);

    grupoPermissaoCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'fileUploadService', '$upload', 'SweetAlert'];

    function grupoPermissaoCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, fileUploadService, $upload, SweetAlert) {

        $scope.pageClass = 'page-grupoPermissao';
        $scope.pesquisarGrupos = pesquisarGrupos;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.habilitaDesabilitaAbaCadastro = habilitaDesabilitaAbaCadastro;
        $scope.inserirGrupo = inserirGrupo;
        $scope.editarGrupo = editarGrupo;
        $scope.editarPermissaoGrupo = editarPermissaoGrupo;
        $scope.editarPermissao = editarPermissao;
        $scope.limpaDados = limpaDados;
        $scope.menuPermissaoGrupo = [];

        //0--------Declaracao de todas as abas tela de grupoPermissao----
        $scope.tabsGrupo = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadGrupo: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },
            tabCadPermissao: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            }
        };

        //1-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {
            $scope.limpaDados();
            $scope.tabsGrupo = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadGrupo: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadPermissao: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }
        //1---------------FIM------------------------------

        //2-----Inserir nova categoria----------------------------------------
        function inserirGrupo() {

            if ($scope.novoGrupo.Id > 0) {
                atualizarGrupo();
            } else {
                inserirGrupoModel();
            }

        }

        function inserirGrupoModel() {
            apiService.post('/api/usuariogrupo/inserir', $scope.novoGrupo,
            inserirGrupoSucesso,
            inserirGrupoFalha);
        }

        function inserirGrupoSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            $scope.novoGrupo = response.data;
            pesquisarGrupos();

        }

        function inserirGrupoFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2----------------------------Fim---------------------------------------

        //3-----Atualiza Workflow status sistemas-------------------------------
        function atualizarGrupo() {
            apiService.post('/api/usuariogrupo/atualizar', $scope.novoGrupo,
            atualizarGrupoSucesso,
            atualizarGrupoFalha);
        }

        function atualizarGrupoSucesso(response) {
            notificationService.displaySuccess('Grupo Atualizado com Sucesso.');
            $scope.novoGrupo = response.data;
            pesquisarGrupos();

        }

        function atualizarGrupoFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }
        //3---------------------Fim--------------------------



        //4-----Carrega Workflow aba Pesquisar------------
        function pesquisarGrupos(page) {

            page = page || 0;

            $scope.loadingGrupos = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 100,
                    filter: $scope.filtroGrupo
                }
            };

            apiService.get('/api/usuariogrupo/pesquisar', config,
                        pesquisarGruposLoadCompleted,
                        pesquisarGruposLoadFailed);
        }

        function pesquisarGruposLoadCompleted(result) {

            $scope.Grupos = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingGrupos = false;

            $scope.habilitaDesabilitaAbaPesquisa();

            var msg = result.data.Items.length > 1 ? " Grupos Encontrados" : "Grupo Encontrado";
            if ($scope.page == 0 && $scope.novoGrupo.item.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);


        }

        function pesquisarGruposLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //4----Fim---------------------------------------


        //5-----Editar dados aba Grupo---------------------
        function editarGrupo(grupoPesq) {
            $scope.novoGrupo = grupoPesq;

            $scope.tabsGrupo = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadGrupo: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadPermissao: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }
        //5---------------------------------Fim---------------------


        //6-----Editar dados aba Permissao---------------------
        function editarPermissaoGrupo(grupoPesq) {
            $scope.novoGrupo = grupoPesq;
            loadMenu(grupoPesq.Id);

            $scope.tabsGrupo = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadGrupo: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadPermissao: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };


        }
        //6---------------------------------Fim---------------------



        //7-----Carrega categorias --
        function loadMenu(grupoId) {
            if ($scope.menuPermissaoGrupo.length == 0) {
                var config = {
                    params: {
                        idgrupo: grupoId,
                        modulo: 1
                    }
                };
                apiService.get('/api/usuariogrupo/getmenu', config,
                        loadMenuSucesso,
                        loadMenuFailed);
            }
        }

        function loadMenuSucesso(response) {
            $scope.menuPermissaoGrupo = response.data;
            //loadMembroCategoria();
        }

        function loadMenuFailed(response) {
            notificationService.displayError(response.data);
        }
        //7------------------------fim-----------------------------

        //8-----Carrega categorias --
        function editarPermissao() {

            apiService.post('/api/usuariogrupo/atualizarpermissao/' + $scope.novoGrupo.Id, $scope.menuPermissaoGrupo,
            atualizarMenuPermissaoSucesso,
            atualizarMenuPermissaoFalha);
        }

        function atualizarMenuPermissaoSucesso(response) {
            notificationService.displaySuccess('Permissões atualizadas para o grupo.');
            //$scope.pesquisarGrupos();

        }

        function atualizarMenuPermissaoFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            }
            else {
                notificationService.displayError(response.statusText);
            }
        }


        //8-----Carrega categorias --

        //9 ------------------- LimpaDados ----------------------
        function limpaDados() {
            $scope.novoGrupo = '';
            $scope.menuPermissaoGrupo = [];
            $scope.filtroGrupo = '';
            $scope.filtroMenuPermissao = '';
        }
        //9 ---------------------- FIM -------------------------


        //10 -------------- Habilita Haba Cadastro ------------------------

        function habilitaDesabilitaAbaCadastro() {

            $scope.tabsGrupo = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadGrupo: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadPermissao: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novoGrupo.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            }

        }

        //10 ------------------------ FIM ---------------------------------


        // -------------------- Inicializa Tela ---------------------------------//
        pesquisarGrupos();
        // ----------------------------Fim---------------------------------------//


    }


})(angular.module('ECCAdm'));