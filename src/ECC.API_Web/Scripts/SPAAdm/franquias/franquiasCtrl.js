(function (app) {
    'use strict';

    app.controller('franquiasCtrl', franquiasCtrl);

    franquiasCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService'];

    function franquiasCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService) {

        $scope.pageClass = 'page-franquias';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.inserirFranquia = inserirFranquia;
        $scope.editarFranquia = editarFranquia;
        $scope.limpaDados = limpaDados;
        $scope.loadEnderecoCep = loadEnderecoCep;
        $scope.loadBairroChange = loadBairro;
        $scope.openDatePicker = openDatePicker;
        
        $scope.Franquias = [];
        $scope.novaFranquia = {};
        $scope.mostraBairroDll = true;
        $scope.mostraBairroInput = false;
        $scope.format = 'dd/MM/yyyy';
        $scope.datepicker = {};

        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };
        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.opened = true;
        };

        //0--------Declaracao de todas as abas tela de Franquia----
        $scope.tabsFranquia = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadFranquia: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }
        };
        //0-----------------Fim-------------------------------

        //1-----Inserir nova Franquia-------------------------------------
        function inserirFranquia() {

            $scope.novaFranquia.Endereco = $scope.novoEndereco;

            if ($scope.novaFranquia.FranquiaId > 0 && validarDadosFranquia()) {
                atualizarFranquia();
            } else if (validarDadosFranquia()) {
                inserirFranquiaModel();
            }

        }

        function inserirFranquiaModel() {
            apiService.post('/api/franquia/inserirfranquia', $scope.novaFranquia,
                inserirFranquiaModelSucesso,
                inserirFranquiaModelFalha);
        }

        function inserirFranquiaModelSucesso(response) {
            $scope.novaFranquia = response.data;
            notificationService.displaySuccess('Franquia incluída com sucesso.');
            pesquisarFranquias();

            $scope.tabsFranquia = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFranquia: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };

        }

        function inserirFranquiaModelFalha(response) {
            notificationService.displayError(response.data);
        }
        //----------------------------Fim---------------------------------------

        //2-----Atualiza Franquias----------------------------------------------
        function atualizarFranquia() {
            apiService.post('/api/franquia/atualizarfranquia', $scope.novaFranquia,
                atualizarFranquiaSucesso,
                atualizarFranquiaFalha);
        }

        function atualizarFranquiaSucesso(response) {
            $scope.novaFranquia = response.data;
            notificationService.displaySuccess('Franquia atualizada com sucesso.');
        }

        function atualizarFranquiaFalha(response) {
            notificationService.displayError(response.data);
        }
        //2---------------------Fim----------------------------------------------


        //3-----Carrega Franquias aba Pesquisar------------
        function pesquisarFranquias(page) {

            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroFranquia
                }
            };

            apiService.get('/api/franquia/getFranquias', config,
                franquiasLoadCompleted,
                franquiasLoadFailed);
        }

        function franquiasLoadCompleted(result) {

            $scope.Franquias = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;

        }

        function franquiasLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //3----Fim---------------------------------------



        //4-----Editar dados Franquia------------------------------
        function editarFranquia(franquia) {

            $scope.novaFranquia = franquia;
            if (franquia.Endereco === null) {
                loadLogradouro();
                loadEstado();
            } else {
                $scope.novoEndereco = franquia.Endereco;
                loadLogradouro();
                loadEstado();
                loadCidade(franquia.Endereco.EstadoId);
                loadBairro(franquia.Endereco.CidadeId);
            }

            $scope.tabsFranquia = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFranquia: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };

        }
        //4---------------------------------Fim---------------------


        //5-----Limpa os dados da Franquia--------------------
        function limpaDados() {

            $scope.novaFranquia = {};
            $scope.novoEndereco = {};
            $scope.filtroFranquia = '';

        }
        //5-------------------------------FIM--------------------------------

        //6-----Habilitar aba pesquisa --------------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsFranquia = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFranquia: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }
        //6---------------FIM------------------------------


        //7-----Busca Endereço pelo cep-----------------------------------
        function loadEnderecoCep(cep) {
            if (cep.length === 8) {

                var config = {
                    params: {
                        cep: cep.trim()
                    }
                };
                apiService.get('/api/endereco/enderecoCep', config,
                    loadEnderecoCepCompleted,
                    loadEnderecoCepFailed);
            }
        }

        function loadEnderecoCepCompleted(response) {
          
            if (response.data != null) {
                if (response.data.EstadoId > 0) {
                    loadCidade(response.data.EstadoId);
                }
                if (response.data.CidadeId > 0) {
                    loadBairro(response.data.CidadeId);
                }

                var periodoEntregaEnd = $scope.novoEndereco.PeriodoEntrega;
                var idEndereco = $scope.novoEndereco.Id;
                var enderecoSelecPadrao = $scope.novoEndereco.EnderecoPadrao;
                var endAtivo = $scope.novoEndereco.Ativo;
                var obsHorario = $scope.novoEndereco.DescHorarioEntrega;

                if ($scope.novoEndereco.Id === "" || $scope.novoEndereco.Id == undefined) {
                    if (response.data.Endereco === "" || response.data.Endereco == null) {
                        notificationService.displayInfo("CEP NÃO ENCONTRADO FAVOR DIGITAR O ENDEREÇO MANUALMENTE !");
                    } else {
                        var idEnd = $scope.novoEndereco.Id;

                        $scope.novoEndereco = response.data;

                        $scope.novoEndereco.Id = idEnd;

                        $scope.novoEndereco.PeriodoEntrega = periodoEntregaEnd;

                        $scope.novoEndereco.DescHorarioEntrega = obsHorario;
                    }

                } else {

                    if (response.data.Endereco === "" || response.data.Endereco == null) {
                        notificationService.displayInfo("CEP NÃO ENCONTRADO FAVOR DIGITAR O ENDEREÇO MANUALMENTE !");
                    } else {
                        $scope.novoEndereco = response.data;
                        $scope.novoEndereco.Id = idEndereco;
                        $scope.novoEndereco.Ativo = endAtivo;
                        $scope.novoEndereco.EnderecoPadrao = enderecoSelecPadrao;
                        $scope.novoEndereco.PeriodoEntrega = periodoEntregaEnd;
                        $scope.novoEndereco.DescHorarioEntrega = obsHorario;
                    }
                }
            }

        }

        function loadEnderecoCepFailed(response) {
            notificationService.displayError(response.data);
        }
        //7------------------------fim-----------------------------


        //8-----Carrega dropdown Cidade ---------------------------------
        function loadCidade(estadoId) {

            var config = {
                params: {
                    EstadoId: estadoId
                }
            };
            apiService.get('/api/endereco/cidade', config,
                loadCidadeCompleted,
                loadCidadeFailed);
        }

        function loadCidadeCompleted(response) {


            $scope.cidades = response.data;
            $scope.habilitaddlCidade = false;
        }

        function loadCidadeFailed(response) {
            notificationService.displayError(response.data);
        }
        //8------------------------fim-----------------------------



        //9-----Carrega dropdown Bairro ------------------------------
        function loadBairro(cidadeId) {

            var config = {
                params: {
                    CidadeId: cidadeId
                }
            };
            apiService.get('/api/endereco/bairro', config,
                loadBairroCompleted,
                loadBairroFailed);
        }

        function loadBairroCompleted(response) {

            $scope.bairros = response.data;
            $scope.habilitaddlBairro = false;


            if (response.data === "") {
                $scope.mostraBairroDll = false;
                $scope.mostraBairroInput = true;
            } else {

                $scope.mostraBairroDll = true;
                $scope.mostraBairroInput = false;
            }
        }

        function loadBairroFailed(response) {
            notificationService.displayError(response.data);
        }
        //9------------------------fim-----------------------------


        //10-----Carrega dropdown Estado-----------------------------
        function loadEstado() {
            apiService.get('/api/endereco/estado', null,
                loadEstadoCompleted,
                loadEstadoFailed);
        }

        function loadEstadoCompleted(response) {


            var newItem = new function () {
                this.Id = undefined;
                this.DescEstado = "Estado...";

            };
            response.data.push(newItem);
            $scope.estados = response.data;
        }

        function loadEstadoFailed(response) {
            notificationService.displayError(response.data);
        }
        //10------------------------fim-----------------------------


        //11-----Carrega dropdown Logradouro --------------------------
        function loadLogradouro() {
            apiService.get('/api/endereco/logradouro', null,
                logradourLoadCompleted,
                logradourLoadFailed);
        }

        function logradourLoadCompleted(response) {


            var newItem = new function () {
                this.Id = undefined;
                this.DescLogradouro = "Logradouro...";

            };
            response.data.push(newItem);
            $scope.logradouros = response.data;
        }

        function logradourLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //11------------------------fim-----------------------------

        //12------------Validar Endereço-------------------------------
        function validarDadosFranquia() {

            if ($scope.novaFranquia.RazaoSocial === "") {
                notificationService.displayInfo('Digite a razão social.');
                return false;
            }

            if ($scope.novaFranquia.NomeFantasia === "") {
                notificationService.displayInfo('Digite o nome Fantasia.');
                return false;
            }

            if ($scope.novaFranquia.Cnpj === "") {
                notificationService.displayInfo('Digite o Cnpj.');
                return false;
            }

            if ($scope.novaFranquia.Descricao === "") {
                notificationService.displayInfo('Digite uma descrição para franquia.');
                return false;
            }
            
            if ($scope.novaFranquia.Endereco.Cep == null) {
                notificationService.displayInfo('Inserir um Cep!');
                return false;
            }

            if ($scope.novaFranquia.Endereco.LogradouroId == null) {
                notificationService.displayInfo('Selecione um logradouro para o endereço!');
                return false;
            }

            if ($scope.novaFranquia.Endereco.Endereco == null) {
                notificationService.displayInfo('Inserir um endereço!');
                return false;
            }

            if ($scope.novaFranquia.Endereco.NumEndereco === "") {
                notificationService.displayInfo('Inserir um número para o endereço!');
                return false;
            }

            if ($scope.novaFranquia.Endereco.EstadoId == null) {
                notificationService.displayInfo('Selecione um estado para o endereço!');
                return false;
            }

            if ($scope.novaFranquia.Endereco.CidadeId == null) {
                notificationService.displayInfo('Selecione uma cidade para o endereço!');
                return false;
            }

            if ($scope.novaFranquia.Endereco.BairroId == null) {
                notificationService.displayInfo('Selecione um bairro para o endereço!');
                return false;
            }
            
            if ($scope.novaFranquia.DddTelComl === "") {
                notificationService.displayInfo('Digite um DDD para o telefone.');
                return false;
            }

            if ($scope.novaFranquia.TelefoneComl === "") {
                notificationService.displayInfo('Digite um telefone.');
                return false;
            }

            if ($scope.novaFranquia.DddCel === "") {
                notificationService.displayInfo('Digite um DDD para o celular.');
                return false;
            }

            if ($scope.novaFranquia.Celular === "") {
                notificationService.displayInfo('Digite um celular.');
                return false;
            }

            if ($scope.novaFranquia.Contato === "") {
                notificationService.displayInfo('Digite o nome de um contato.');
                return false;
            }

            return true;
        }
        //12-----------------------------------------------------------


        pesquisarFranquias();
        loadEstado();
        loadLogradouro();
    }

})(angular.module('ECCAdm'));