(function (app) {
    'use strict';

    app.controller('cadendmembroCtrl', cadendmembroCtrl);

    cadendmembroCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$upload', 'SweetAlert'];

    function cadendmembroCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $upload, SweetAlert) {

        $scope.pageClass = 'page-cadendmembro';

     
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.loadLogradouro = loadLogradouro;
        $scope.habilitaDesabilitaAbaCad = habilitaDesabilitaAbaCad;
        $scope.loadCidadeChange = loadCidade;
        $scope.loadEnderecoCep = loadEnderecoCep;
        $scope.loadBairroChange = loadBairro;
        $scope.limpaDadosEndereco = limpaDadosEndereco;
        $scope.inserirEndereco = inserirEndereco;
        $scope.pesquisarEndereco = pesquisarEndereco;
        $scope.enderecoPadraoMembro = enderecoPadraoMembro;
        $scope.levaParaAbaAlteraEndereco = levaParaAbaAlteraEndereco;
        $scope.checarEnderecosAtivos = checarEnderecosAtivos;

        $scope.estados = [];
        $scope.cidades = [];
        $scope.bairros = [];
        $scope.logradouros = [];
        $scope.HorarioEntrega = [];
        $scope.novoEndereco = {};
        $scope.entrega = {};

        var idEndereco = 0;

 


        //0--------Declaracao de todas as abas tela de Status de Sistema----
        $scope.tabsProduto = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadProduto: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }

        };

        //------Ao clicar no botão editar leva para a Aba Endereço com os campos preenchidos--------
        function levaParaAbaAlteraEndereco(usuario) {

            loadCidade(usuario.EstadoId);

            if (usuario.CidadeId > 0) {
                loadBairro(usuario.CidadeId);
            }

            $scope.novoEndereco = usuario;

            idEndereco = $scope.novoEndereco.Id;

            carregaPeriodoEntrega();

            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };

        }


        //-----Habilitar de Desabilitar Abas------------
        function habilitaDesabilitaAbaCad() {

            carregaPeriodoEntrega();
            $scope.habilitaddlCidade = true;
            $scope.habilitaddlBairro = true;

            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };
        }



        //-----Habilitar de Desabilitar Abas------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
            $scope.novoEndereco = {};

            pesquisarEndereco();
        }

        //-----Fim-----------------------------------------


        //1-----Carrega dropdown Logradouro --
        function loadLogradouro() {
            apiService.get('/api/endereco/logradouro', null,
            logradouroLoadCompleted,
            logradourLoadFailed);
        }

        function logradouroLoadCompleted(response) {
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
        //1------------------------fim-----------------------------


        //2-----Carrega dropdown Cidade --
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
            $scope.habilitaddlBairro = true;
        }


        function loadCidadeFailed(response) {
            notificationService.displayError(response.data);
        }
        //2------------------------fim-----------------------------


        //3-----Carrega dropdown Estado--
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
        //3------------------------fim-----------------------------


        //4-----Busca Endereço pelo cep--
        function loadEnderecoCep(cep) {
         
            if (cep.length == 9) {
               limpaDadosEndereco();
                var cepEndereco = cep.replace(/-/g, '').trim();
                var config = {
                    params: {
                        cep: cepEndereco.trim()
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


                if (response.data.Bairro == null || response.data.Endereco ==null)
                    notificationService.displayInfo('ENDEREÇO NÃO ENCONTRADO FAVOR DIGITAR !');


                $scope.novoEndereco = response.data;
            }

        }


        function loadEnderecoCepFailed(response) {
            notificationService.displayError(response.data);
        }
        //4------------------------fim-----------------------------



        //5-----Carrega dropdown Bairro --
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
        }

        function loadBairroFailed(response) {
            notificationService.displayError(response.data);
        }
        //5------------------------fim-----------------------------



        //6----Limpas dados do endereço para inserir um novo------------
        function limpaDadosEndereco() {

            var cepTemp = $scope.novoEndereco.Cep;
            $scope.novoEndereco = {};
            $scope.novoEndereco.Cep= cepTemp;

            idEndereco = 0;
            $scope.entrega = {};
            $scope.habilitaddlCidade = true;
            $scope.habilitaddlBairro = true;
        }
        //6-------------------Fim----------------------------------------



        //7----------Cadastro de Novo Endereço-----
        function inserirEndereco() {
            $scope.novoEndereco.Id = idEndereco;
            if ($scope.novoEndereco.Id > 0) {



                if (validarEndereco()) {
                    if ($scope.novoEndereco.Ativo) {
                        atualizarEndereco();
                    }

                    checarEnderecosAtivos($scope.novoEndereco);
                }

            } else {
                if (validarEndereco()) {

                    inserirEnderecoModel();
                }
            }
        }


        function inserirEnderecoModel() {
            $scope.novoEndereco.Cep = $scope.novoEndereco.Cep.replace(/-/g, '').trim();


            apiService.post('/api/usuario/inserirendereco', $scope.novoEndereco,
            inserirEnderecoSucesso,
            inserirEnderecoFalha);
        }


        function inserirEnderecoSucesso(response) {
            notificationService.displaySuccess('Novo Endereço Incluído com Sucesso.');
            $scope.novoEndereco = {};
            habilitaDesabilitaAbaPesquisa();
        }


        function inserirEnderecoFalha(response) {

            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //7----------------------------------------

        //8----Atuallizar endereco membro----------------
        function atualizarEndereco() {
            $scope.novoEndereco.Cep = $scope.novoEndereco.Cep.replace(/-/g, '').trim();

            apiService.post('/api/endereco/atualizar', $scope.novoEndereco,
            atualizarEnderecoSucesso,
            atualizarEnderecoFalha);
        }


        function atualizarEnderecoSucesso(response) {
            notificationService.displaySuccess('Endereço Atualizado com Sucesso.');
            $scope.novoEndereco = {};
            habilitaDesabilitaAbaPesquisa();
        }


        function atualizarEnderecoFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //8----------------------------------------


        //9-----Carrega enderecos membro------------
        function pesquisarEndereco() {

            var config = {
                params: {
                    filter: $scope.filtroEndereco
                }
            };

            apiService.get('/api/usuario/enderecoMembro', config,
                        enderecosLoadCompleted,
                        enderecosLoadFailed);
        }


        function enderecosLoadCompleted(response) {

            $scope.Enderecos = response.data;

            $scope.loadingEnderecos = false;
        }


        function enderecosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //9----Fim---------------------------------------



        //10-----Carrega enderecos padrao membro------------
        function enderecoPadraoMembro(enderecoUsuario) {

            apiService.post('/api/usuario/atualizarEndPadrao', enderecoUsuario,
                        enderecosCompleted,
                        enderecosFailed);
        }


        function enderecosCompleted(response) {

            //notificationService.displaySuccess('Endereço adicionado como padrão !');

            if (response.data.EnderecoPadrao) {
                $scope.checked = 'checked';
            }

            pesquisarEndereco();

        }


        function enderecosFailed(response) {
            notificationService.displayError(response.data);
        }
        //10----Fim---------------------------------------



        //11-------------Carrega Período Entrega-------------------------
        function carregaPeriodoEntrega() {

            apiService.get('/api/endereco/periodoentrega', null,
            periodoentregaLoadCompleted,
            periodoentregaLoadFailed);

            function periodoentregaLoadCompleted(result) {
                $scope.HorarioEntrega = result.data;
            }

            function periodoentregaLoadFailed(result) {

                notificationService.displayError('Erro ao carregar períodos!');
            }
        }

        //11-------------FIM---------------------------------------


        //12------------Checar Endereços Ativos--------------------------
        function checarEnderecosAtivos(endereco) {
            if (!endereco.Ativo) {
                var cont = 0;
                var objEnderecoPadrao = {};
                if ($scope.Enderecos.length === 1) {
                    SweetAlert.swal("Atenção!", "Não poderá desativar este endereço, " +
                        "pois ele é o unico cadastrado para a entrega dos seus pedidos!", "warning");
                } else {
                    for (var i = 0; i < $scope.Enderecos.length; i++) {
                        if ($scope.Enderecos[i].Id !== endereco.Id && $scope.Enderecos[i].Ativo && cont === 0) {
                            cont++;
                            objEnderecoPadrao = $scope.Enderecos[i];
                        }
                    }
                    if (cont === 0 && !endereco.Ativo) {
                        SweetAlert.swal("Atenção!", "Não poderá desativar este endereço enquanto não cadastrar " +
                            "ou ativar outro endereço para entrega !", "warning");
                        endereco.Ativo = true;
                    } else {

                        //Chama a função para adicionar o próximo endereço como padrão.
                        enderecoPadraoMembro(objEnderecoPadrao);

                        atualizarEndereco();
                    }
                }
            }
        }
        //12-------------------------------------------------------------


        //13------------------Valida campos de endereço-----------------
        function validarEndereco() {

            if ($scope.novoEndereco.Cep == null) {
                notificationService.displayInfo('Inserir um Cep!');
                return false;
            }

            if ($scope.novoEndereco.LogradouroId == null) {
                notificationService.displayInfo('Selecione um logradouro para o endereço!');
                return false;
            }

            if ($scope.novoEndereco.Endereco == null) {
                notificationService.displayInfo('Inserir um endereço!');
                return false;
            }

            if ($scope.novoEndereco.NumEndereco === "") {
                notificationService.displayInfo('Inserir um número para o endereço!');
                return false;
            }

            if ($scope.novoEndereco.EstadoId == null) {
                notificationService.displayInfo('Selecione um estado para o endereço!');
                return false;
            }

            if ($scope.novoEndereco.CidadeId == null) {
                notificationService.displayInfo('Selecione uma cidade para o endereço!');
                return false;
            }

            if ($scope.novoEndereco.BairroId == null) {
                notificationService.displayInfo('Selecione um bairro para o endereço!');
                return false;
            }

            if ($scope.novoEndereco.PeriodoEntrega == null) {
                notificationService.displayInfo('Selecione um período de entrega!');
                return false;
            }


            return true;
        }
        //13------------------------------------------------------------

        habilitaDesabilitaAbaPesquisa();
        loadLogradouro();
        loadEstado();

        pesquisarEndereco();

    }

})(angular.module('ECCCliente'));