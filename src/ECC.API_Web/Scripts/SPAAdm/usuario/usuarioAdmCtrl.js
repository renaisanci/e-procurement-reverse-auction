(function (app) {
    'use strict';

    app.controller('usuarioAdmCtrl', usuarioAdmCtrl);

    usuarioAdmCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];



    function usuarioAdmCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {
        $scope.pageClass = 'page-usuarioAdm';
        $scope.booSenha = true;
        $scope.inserirusuarioAdm = inserirusuarioAdm;
        $scope.pesquisarusuarioAdm = pesquisarusuarioAdm;
        $scope.editarusuarioAdm = editarusuarioAdm;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.habilitaDesabilitaAbaCadastro = habilitaDesabilitaAbaCadastro;
        $scope.limpaDados = limpaDados;
        $scope.buscarGrupos = buscarGrupos;
        $scope.gruposUsuarioAdmSel = [];

        //0--------Declaracao de todas as abas tela de usuarioAdm----
        $scope.tabsusuarioAdm = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadusuarioAdm: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

        };
        //0-----------------Fim-------------------------------

        //1-----Inserir nova categoria----------------------------------------
        function inserirusuarioAdm() {

            if ($scope.novousuarioAdm.Id > 0) {
                atualizarrusuarioAdm();
            } else {
                $scope.novousuarioAdm.Senha = 'ABCD1234';
                $scope.novousuarioAdm.ConfirmSenha = 'ABCD1234';
                $scope.novousuarioAdm.PerfilId = 1;
                inserirusuarioAdmModel();
            }

        }

        function inserirusuarioAdmModel() {
            apiService.post('/api/usuario/inserir', $scope.novousuarioAdm,
            inserirusuarioAdmSucesso,
            inserirusuarioAdmFalha);
        }

        function inserirusuarioAdmSucesso(response) {
            $scope.novousuarioAdm = response.data;

            for (var i = 0; i < $scope.gruposUsuarioAdm.length; i++) {
                for (var x = 0; x < $scope.gruposUsuarioAdmSel.length; x++) {
                    if ($scope.gruposUsuarioAdm[i].GrupoId == $scope.gruposUsuarioAdmSel[x]) {
                        $scope.gruposUsuarioAdm[i].Selecionado = true;
                        break;
                    }
                    else {
                        $scope.gruposUsuarioAdm[i].Selecionado = false;
                    }
                }

            }


            apiService.post('/api/usuario/atualizargrupo/' + $scope.novousuarioAdm.Id, $scope.gruposUsuarioAdm,
            inserirGrupoSucesso,
            inserirusuarioAdmFalha);


        }

        function inserirGrupoSucesso(response) {
            notificationService.displaySuccess('Incluído com Sucesso.');
            pesquisarusuarioAdm();
            habilitaDesabilitaAbaPesquisa();

        }


        function inserirusuarioAdmFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //----------------------------Fim---------------------------------------

        //2-----Atualiza Usuário Admin status sistemas-------------------------------
        function atualizarrusuarioAdm() {
            apiService.post('/api/usuario/atualizar', $scope.novousuarioAdm,
            atualizarusuarioAdmSucesso,
            atualizarusuarioAdmFalha);
        }

        function atualizarusuarioAdmSucesso(response) {
            //notificationService.displaySuccess('Usuário Administração Atualizado com Sucesso.');
            $scope.novousuarioAdm = response.data;


            for (var i = 0; i < $scope.gruposUsuarioAdm.length; i++) {
                for (var x = 0; x < $scope.gruposUsuarioAdmSel.length; x++) {
                    if ($scope.gruposUsuarioAdm[i].GrupoId == $scope.gruposUsuarioAdmSel[x]) {
                        $scope.gruposUsuarioAdm[i].Selecionado = true;
                        break;
                    }
                    else {
                        $scope.gruposUsuarioAdm[i].Selecionado = false;
                    }
                    
                }
                
            }


            apiService.post('/api/usuario/atualizargrupo/' + $scope.novousuarioAdm.Id, $scope.gruposUsuarioAdm,
            atualizarGrupoSucesso,
            atualizarusuarioAdmFalha);


        }

        function atualizarGrupoSucesso(response) {
            notificationService.displaySuccess('Usuário Administração Atualizado com Sucesso.');
            pesquisarusuarioAdm();
            habilitaDesabilitaAbaPesquisa();

        }

        function atualizarusuarioAdmFalha(response) {
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
        //2---------------------Fim--------------------------

        //3-----Carrega Usuário aba Pesquisar------------
        function pesquisarusuarioAdm(page) {

            page = page || 0;

            $scope.loadingusuarioAdm = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtrousuarioAdm
                }
            };

            apiService.get('/api/usuario/pesquisar', config,
                        usuarioAdmLoadCompleted,
                        usuarioAdmLoadFailed);
        }

        function usuarioAdmLoadCompleted(result) {

            $scope.usuarioAdm = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingusuarioAdm = false;


            var msg = result.data.Items.length > 1 ? " Usuários Encontrados" : "Usuário de Status Encontrado";
            if ($scope.page == 0 && $scope.novousuarioAdm.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function usuarioAdmLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //3----Fim---------------------------------------

        //4-----Editar dados aba Membro---------------------
        function editarusuarioAdm(usuarioAdmPesq) {
            $scope.novousuarioAdm = usuarioAdmPesq;
            $scope.booSenha = false;
            buscarGrupos();
            //Usuário após selecionar um membro no grid
            $scope.tabsusuarioAdm = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadusuarioAdm: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }
        //4---------------------------------Fim---------------------

        //5-----Limpa os dados da tela para inserir o novo Usuário Admin Sistema------------
        function limpaDados() {

            $scope.novousuarioAdm = '';
            $scope.filtrousuarioAdm = '';
            $scope.gruposUsuarioAdmSel = [];
            $scope.booSenha = true;
            //$scope.gruposUsuarioAdm = '';
        }
        //5-------------------------------FIM--------------------------------

        //6-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {
            $scope.limpaDados();
            $scope.booSenha = true;
            $scope.tabsusuarioAdm = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadusuarioAdm: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

            };
        }
        //6---------------FIM------------------------------

        //7-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaCadastro() {
            buscarGrupos();
            $scope.tabsusuarioAdm = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadusuarioAdm: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

            };
        }
        //7---------------FIM------------------------------

        //8-----Limpa os dados da tela para inserir o novo Usuário Admin Sistema------------
        function buscarGrupos() {


            apiService.post('/api/usuario/buscargrupo', $scope.novousuarioAdm,
            buscarGruposSucesso,
            buscarGruposFalha);
        }

        function buscarGruposSucesso(response) {
            $scope.gruposUsuarioAdm = response.data;
            $scope.gruposUsuarioAdmSel = [];
            for (var i = 0; i < $scope.gruposUsuarioAdm.length; i++) {
                if ($scope.gruposUsuarioAdm[i].Selecionado)
                    $scope.gruposUsuarioAdmSel.push($scope.gruposUsuarioAdm[i].GrupoId);
            }

        }

        function buscarGruposFalha(response) {
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


        //8-------------------------------FIM--------------------------------



        // -------------------- Inicializa Tela ---------------------------------//
        $scope.pesquisarusuarioAdm();
        $scope.buscarGrupos();
        // ----------------------------Fim---------------------------------------//
    }

})(angular.module('ECCAdm'));