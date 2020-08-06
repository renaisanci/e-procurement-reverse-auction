(function (app) {
    'use strict';

    app.controller('membroCtrl', membroCtrl);

    membroCtrl.$inject = ['$scope', 'membershipService', '$modal', 'notificationService', '$rootScope', '$location', 'apiService', 'admUtilService'];

    function membroCtrl($scope, membershipService, $modal, notificationService, $rootScope, $location, apiService, admUtilService) {
        $scope.pageClass = 'page-membro';
        $scope.inserirMembro = inserirMembro;
        $scope.pesquisarMembro = pesquisarMembro;
        $scope.limpaDados = limpaDados;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.habilitaDesabilitaAbaEndereco = habilitaDesabilitaAbaEndereco;
        $scope.habilitaDesabilitaAbaMembro = habilitaDesabilitaAbaMembro;


        $scope.habilitaDesabilitaAbaPesquisaPF = habilitaDesabilitaAbaPesquisaPF;
        $scope.editarMembro = editarMembro;
        $scope.novoMembro = { Ativo: true };
        $scope.logradouros = [];
        $scope.estados = [];
        $scope.cidades = [];
        $scope.bairros = [];
        $scope.loadEnderecoCep = loadEnderecoCep;
        $scope.novoEndereco = { Ativo: true };
        $scope.inserirEndereco = inserirEndereco;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.loadCidadeChange = loadCidade;
        $scope.loadBairroChange = loadBairro;
        $scope.habilitaddlCidade = true;
        $scope.habilitaddlBairro = true;
        $scope.limpaDadosEndereco = limpaDadosEndereco;
        $scope.editarEndereco = editarEndereco;
        $scope.categorias = [];
        $scope.habilitaDesabilitaAbaRelatProduto = habilitaDesabilitaAbaRelatProduto;
        $scope.inserirMembroCategoriasProduto = inserirMembroCategoriasProduto;
        $scope.modelContainer = [];
        $scope.inserirMembroFornecedors = inserirMembroFornecedors;
        $scope.habilitaDesabilitaAbaRelatForn = habilitaDesabilitaAbaRelatForn;
        $scope.fornecedoresRegiao = [];
        $scope.openEditDialog = openEditDialog;
        $scope.openDatePicker = openDatePicker;
        $scope.openDatePickerFimGratuidade = openDatePickerFimGratuidade;
        $scope.atualizaParaEndPadrao = atualizaParaEndPadrao;
        $scope.subcategorias = [];
        $scope.usuariosMembro = [];
        $scope.loadCategoriaPorSegmento = loadCategoriaPorSegmento;
        $scope.filtroCategoriaSegmento = 0;
        $scope.HorarioEntrega = [];
        $scope.FranquiaMembro = [];
        $scope.entrega = {};
        $scope.habilitaDesabilitaAbaUsuMembro = habilitaDesabilitaAbaUsuMembro;
        $scope.mostraBairroDll = true;
        $scope.mostraBairroInput = false;
        $scope.openDatePickerNascimento = openDatePickerNascimento;

        $scope.sexos = [


            { Id: 1, DescSexo: "Masculino" },
            { Id: 2, DescSexo: "Feminino" },
            { Id: 3, DescSexo: "Outros" }
        ];

        var idEndereco = 0;

        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };

        $scope.subCategoriaSelecionada = '';
        $scope.membroCategoria = '';

        $scope.novoMembroDemanda = {};
        $scope.pesquisarMembroDemanda = pesquisarMembroDemanda;
        $scope.editarMembroDemanda = editarMembroDemanda;
        $scope.openEditDialogMembroDemanda = openEditDialogMembroDemanda;
        $scope.habilitaDesabilitaAbaMembroDemanda = habilitaDesabilitaAbaMembroDemanda;
        $scope.loadMembroCategoriaId = loadMembroCategoriaId;

        $scope.tipoPessoa = "pj";





        $scope.mostraDadosTipoPessoa = mostraDadosTipoPessoa;

        function mostraDadosTipoPessoa(tipoPessoa) {


        }



        function openEditDialog(membroParam) {
            $scope.novoUsuario = membroParam;
            $modal.open({
                templateUrl: 'Scripts/SPAAdm/usuario/usuarioModal.html',
                controller: 'usuarioCtrl',

                scope: $scope
            }).result.then(function ($scope) {
                //clearSearch();
            }, function () {
            });
        }

        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.opened = true;
        };
        $scope.datepicker = {};
        $scope.format = 'dd/MM/yyyy';


        function openDatePickerFimGratuidade($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepickerFimGratuidade.opened = true;
        };
        $scope.datepickerFimGratuidade = {};
        $scope.formatFimGratuidade = 'dd/MM/yyyy';



        function openDatePickerNascimento($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepickerNascimento.opened = true;
        };
        $scope.datepickerNascimento = {};
        $scope.formatNascimento = 'dd/MM/yyyy';

        //0--------Declaracao de todas as abas tela de membros----
        $scope.tabsMembro = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },

            tabPesquisarPF: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },



            tabCadMembro: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

            tabCadEndMembro: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            },

            tabRelProduto: {
                tabAtivar: " ",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            },
            tabRelFornecedor: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            },

            tabMembroDemanda: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            },

            tabUsuMembro: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            }

        };
        //0-----------------Fim-------------------------------




        //1-----Carrega membro PF aba Pesquisar------------
        function pesquisarMembroPF(page) {

            page = page || 0;



            var config = {
                params: {
                    page: page,
                    pageSize: 40,
                    filter: $scope.filtroMembros
                }
            };

            apiService.get('/api/membro/pesquisarPf', config,
                membrosPFLoadCompleted,
                membrosPFLoadFailed);
        }

        function membrosPFLoadCompleted(result) {

            $scope.MembrosPf = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;


            // if ($scope.filtroMembros && $scope.filtroMembros.length) {
            var msg = result.data.Items.length > 1 ? " Membros Encontrados" : "Membro Encontrado";
            if ($scope.page === 0 && $scope.novoMembro.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function membrosPFLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim---------------------------------------



        //1-----Carrega membro aba Pesquisar------------
        function pesquisarMembro(page) {

            page = page || 0;

            $scope.loadingMembros = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 40,
                    filter: $scope.filtroMembros
                }
            };

            apiService.get('/api/membro/pesquisar', config,
                membrosLoadCompleted,
                membrosLoadFailed);
        }

        function membrosLoadCompleted(result) {

            $scope.Membros = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMembros = false;

            // if ($scope.filtroMembros && $scope.filtroMembros.length) {
            var msg = result.data.Items.length > 1 ? " Membros Encontrados" : "Membro Encontrado";
            if ($scope.page === 0 && $scope.novoMembro.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);
            // }

        }

        function membrosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim---------------------------------------

        //2-----Insere novo membro aba Membro------------
        function inserirMembro() {


            console.log($scope.novoMembro.Cnpj);

            if ($scope.novoMembro.Cnpj != undefined)
                $scope.novoMembro.Cnpj = $scope.novoMembro.Cnpj.split(".").join("").split("/").join("").split("-").join("");



            if ($scope.novoMembro.Cpf != undefined)
                $scope.novoMembro.Cpf = $scope.novoMembro.Cpf.split(".").join("").split("/").join("").split("-").join("");

            $scope.novoMembro.DddTelComl = $scope.novoMembro.DddTelComl.split("(").join("").split(")").join("");

            $scope.novoMembro.TelefoneComl = $scope.novoMembro.TelefoneComl.split("-").join("");

            $scope.novoMembro.DddCel = $scope.novoMembro.DddCel.split("(").join("").split(")").join("");

            $scope.novoMembro.Celular = $scope.novoMembro.Celular.split("-").join("");

            if ($scope.novoMembro.Id > 0) {
                atualizarMembro();
            } else {
                inserirMembroModel();
            }
        }

        function inserirMembroModel() {



            if ($scope.tipoPessoa == "pj") {



                apiService.post('/api/membro/inserir', $scope.novoMembro,
                    inserirMembroSucesso,
                    inserirMembroFalha);
            } else {



                apiService.post('/api/membro/inserirPf', $scope.novoMembro,
                    inserirMembroSucesso,
                    inserirMembroFalha);
            }

        }

        function inserirMembroSucesso(response) {


            if ($scope.tipoPessoa == "pj") {
                notificationService.displaySuccess($scope.novoMembro.RazaoSocial + ' Incluído com Sucesso.');
            } else {

                notificationService.displaySuccess($scope.novoMembro.Nome + ' Incluído com Sucesso.');
            }


            $scope.novoMembro = response.data;

            $scope.filtroMembros = response.data.Cnpj;
            pesquisarMembro();

            $scope.tabsMembro = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPesquisarPF: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadMembro: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },


                tabCadEndMembro: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabUsuMembro: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true


                }
            };

            loadLogradouro();
            loadEstado();

        }

        function inserirMembroFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2---------------------Fim--------------------------

        //3-----Limpa os dados da tela para insedir o novo membro------------
        function limpaDados() {

            $scope.novoMembro = {
                Ativo: true
            };
            $scope.filtroMembros = '';

            //habilita as abas de relacionar fornecedor e 
            //relacionar produto após o cadastro um nono membro
            $scope.tabsMembro.tabRelProduto.tabhabilitar = false;
            $scope.tabsMembro.tabRelProduto.contentHabilitar = false;

            $scope.tabsMembro.tabRelFornecedor.tabhabilitar = false;
            $scope.tabsMembro.tabRelFornecedor.contentHabilitar = false;

            $scope.tabsMembro.tabCadEndMembro.tabhabilitar = false;
            $scope.tabsMembro.tabCadEndMembro.contentHabilitar = false;

            $scope.tabsMembro.tabUsuMembro.tabhabilitar = false;
            $scope.tabsMembro.tabUsuMembro.contentHabilitar = false;

            $scope.tabsMembro.tabMembroDemanda.tabhabilitar = false;
            $scope.tabsMembro.tabMembroDemanda.contentHabilitar = false;
        }


        //4-----Atualiza dados membro aba Membro------------
        function atualizarMembro() {



            if ($scope.tipoPessoa == "pj") {

                $scope.novoMembro.Cnpj = $scope.novoMembro.Cnpj.split(".").join("").split("/").join("").split("-").join("");
                apiService.post('/api/membro/atualizar', $scope.novoMembro,
                    atualizarMembroSucesso,
                    atualizarMembroFalha);
            } else {


                $scope.novoMembro.Cpf = $scope.novoMembro.Cpf.split(".").join("").split("/").join("").split("-").join("");
                apiService.post('/api/membro/atualizarPf', $scope.novoMembro,
                    atualizarMembroSucesso,
                    atualizarMembroFalha);

            }
        }

        function atualizarMembroSucesso(response) {
            if ($scope.tipoPessoa == "pj") {
                notificationService.displaySuccess($scope.novoMembro.RazaoSocial + ' Atualizado com Sucesso.');

            } else {
                notificationService.displaySuccess($scope.novoMembro.Nome + ' Atualizado com Sucesso.');
            }


            $scope.novoMembro = response.data;
            $scope.filtroMembros = response.data.Cnpj;
            pesquisarMembro();


            $scope.tabsMembro = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabPesquisarPF: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadMembro: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },


                tabCadEndMembro: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabUsuMembro: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true


                }
            };

        }

        function atualizarMembroFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //4---------------------Fim--------------------------

        //5-----Editar dados aba Membro---------------------
        function editarMembro(membroPesq, pessoa) {



            $scope.tipoPessoa = pessoa;



            $scope.novoMembro = membroPesq;
            loadLogradouro();
            loadEstado();

            //habilita as abas de relacionar fornecedor e relacionar 
            //produto após selecionar um membro no grid
            $scope.tabsMembro = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPesquisarPF: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabCadMembro: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadEndMembro: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabUsuMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }
        //5---------------------------------Fim---------------------


        function habilitaDesabilitaAbaPesquisaPF() {

            pesquisarMembroPF();

            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabPesquisarPF: {

                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabUsuMembro: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };


        }




        //6-----Habilita e desabilita abas após clicar em pesquisar--
        function habilitaDesabilitaAbaPesquisa() {
            $scope.novoMembro = '';
            $scope.filtroMembros = '';
            $scope.novoEndereco = {};

            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true

                },

                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabUsuMembro: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };


        }
        //6-----Fim---------------------

        //7-----Carrega dropdown Logradouro --
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
        //7------------------------fim-----------------------------

        //8-----Carrega dropdown Estado--
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
        //8------------------------fim-----------------------------

        //9-----Busca Endereço pelo cep--
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



                if ($scope.novoEndereco.Id == "" || $scope.novoEndereco.Id == undefined) {




                    if (response.data.Endereco == "" || response.data.Endereco == null) {
                        notificationService.displayInfo("CEP NÃO ENCONTRADO FAVOR DIGITAR O ENDEREÇO MANUALMENTE !");

                    } else {



                        var idEnd = $scope.novoEndereco.Id;

                        $scope.novoEndereco = response.data;

                        $scope.novoEndereco.Id = idEnd;

                        $scope.novoEndereco.PeriodoEntrega = periodoEntregaEnd;

                        $scope.novoEndereco.DescHorarioEntrega = obsHorario;
                    }

                } else {


                    if (response.data.Endereco == "" || response.data.Endereco == null) {
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
        //9------------------------fim-----------------------------

        //10------------------------------------------
        function habilitaDesabilitaAbaEndereco() {


            pesquisarEnderecoMembro();
            carregaPeriodoEntrega();

            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },

                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabUsuMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }
        //10-----Fim---------------------

        //11----------Cadastro de Novo Endereço-----
        function inserirEndereco() {
            $scope.novoEndereco.Id = idEndereco;
            if ($scope.novoEndereco.Id > 0) {
                atualizarEndereco();
            } else {
                inserirEnderecoModel();
            }
        }

        function inserirEnderecoModel() {

            if (validarEndereco()) {

                $scope.novoEndereco.Cep = $scope.novoEndereco.Cep.replace(/-/g, '').trim();

                $scope.novoEndereco.PessoaId = $scope.novoMembro.PessoaId;

                apiService.post('/api/endereco/inserir', $scope.novoEndereco,
                    inserirEnderecoSucesso,
                    inserirEnderecoFalha);
            }
        }

        function inserirEnderecoSucesso(response) {

            notificationService.displaySuccess('Novo Endereço Incluído com Sucesso.');
            //$scope.novoEndereco = response.data;
            $scope.novoEndereco = {};
            pesquisarEnderecoMembro();
        }

        function inserirEnderecoFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

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

        //11----------------------------------------


        //12----Atuallizar endereco membro----------------
        function atualizarEndereco() {

            if (validarEndereco()) {
                $scope.novoEndereco.Cep = $scope.novoEndereco.Cep.replace(/-/g, '').trim();
                apiService.post('/api/endereco/atualizar', $scope.novoEndereco,
                    atualizarEnderecoSucesso,
                    atualizarEnderecoFalha);
            }
        }

        function atualizarEnderecoSucesso(response) {
            notificationService.displaySuccess('Endereço Atualizado com Sucesso.');
            //$scope.novoEndereco = response.data;
            $scope.novoEndereco = {};
            pesquisarEnderecoMembro();
        }

        function atualizarEnderecoFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //12-------------------------------------------


        //13-----Carrega dropdown Cidade --
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
        //13------------------------fim-----------------------------



        //14-----Carrega dropdown Bairro --
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
        //14------------------------fim-----------------------------

        //15-----Carrega enderecos membro------------
        function pesquisarEnderecoMembro(page) {

            page = page || 0;

            $scope.loadingEnderecos = true;

            var config = {
                params: {
                    pessoaId: $scope.novoMembro.PessoaId,
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroEndereco
                }
            };

            apiService.get('/api/endereco/enderecoMembro', config,
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
        //15----Fim---------------------------------------

        //16-----Confirma endereço padrão------------
        //quem vai decidir o endereco padrão é o cliente na tela dele
        //$scope.EnderecoPadrao = function () {

        //    var msg = "já existe um endereço padrão, deseja substituir por esse?";
        //    if ($scope.novoEndereco.Id > 0 && $scope.novoEndereco.EnderecoPadrao) {
        //        msg = "Este endereço deixará de ser o padrão?";

        //    }
        //    SweetAlert.swal({
        //            title: "Você Tem Certeza?",
        //            text: msg,
        //            type: "warning",
        //            showCancelButton: true,
        //            confirmButtonColor: "#DD6B55",
        //            confirmButtonText: "Ok",
        //            cancelButtonText: "Cancelar",
        //            closeOnConfirm: true,
        //            closeOnCancel: true
        //        },
        //        function(isConfirm) {
        //            if (isConfirm) {
        //                notificationService.displaySuccess('Endereço será substituído como padrão.');
        //                $scope.novoEndereco.EnderecoPadrao = true;
        //            } else {
        //                $scope.novoEndereco.EnderecoPadrao = false;
        //            }
        //        });

        //}
        //16-------------------Fim---------------------------------

        //17----Limpas dados do endereço para inserir um novo------------
        function limpaDadosEndereco() {

            $scope.novoEndereco = '';
            idEndereco = 0;
            $scope.entrega = {};

            $scope.habilitaddlCidade = true;
            $scope.habilitaddlBairro = true;


        }
        //17-------------------Fim----------------------------------------

        //18-----Editar endereco selecionado---------------------
        function editarEndereco(enderecoPesq) {
            $scope.novoEndereco = enderecoPesq;
            idEndereco = $scope.novoEndereco.Id;

            loadCidade(enderecoPesq.EstadoId);

            loadBairro(enderecoPesq.CidadeId);

        }
        //18---------------------------------Fim---------------------

        //19------------------------------------------
        function habilitaDesabilitaAbaMembro() {

            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: $scope.novoMembro.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoMembro.Id > 0 ? true : false
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: $scope.novoMembro.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoMembro.Id > 0 ? true : false
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: $scope.novoMembro.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoMembro.Id > 0 ? true : false
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: $scope.novoMembro.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoMembro.Id > 0 ? true : false
                },

                tabUsuMembro: {
                    tabAtivar: " ",
                    tabhabilitar: $scope.novoMembro.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoMembro.Id > 0 ? true : false
                }
            };


        }
        //19-----Fim---------------------

        //20------------------------------------------
        function habilitaDesabilitaAbaRelatProduto() {

            if ($scope.novoMembro.Id > 0) {
                $scope.filtroCategoriaSegmento = 0;
                loadCategoria();

                //TODO
            }
            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },

                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabUsuMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }

        //20-----Fim---------------------

        //21-----Carrega categorias --
        function loadCategoria() {

            apiService.get('/api/produto/categoria', null,
                loadCategoriaSucesso,
                loadCategoriaFailed);
        }

        function loadCategoriaSucesso(response) {

            $scope.categorias = response.data;
            loadMembroCategoria();

        }

        function loadCategoriaFailed(response) {
            notificationService.displayError(response.data);
        }
        //21------------------------fim-----------------------------

        //22-----Carrega categoria Membro --
        function loadMembroCategoria() {

            var config = {
                params: {
                    membroId: $scope.novoMembro.Id

                }
            };


            apiService.get('/api/membro/membroCategorias', config,
                loadMembroCategoriaSucesso,
                loadMembroCategoriaFailed);
        }

        function loadMembroCategoriaSucesso(response) {

            var membroCategoria = response.data;
            for (var i = 0; i < $scope.categorias.length; i++) {
                for (var j = 0; j < membroCategoria.length; j++) {

                    if ($scope.categorias[i].Id == membroCategoria[j].Id) {
                        $scope.categorias[i].Relacionado = true;
                        $scope.categorias[i].selected = true;
                        break;
                    }
                }
                $scope.categorias[i].visivel = false;
                if ($scope.categoriasSeg == undefined || $scope.categoriasSeg.length == 0 || $scope.filtroCategoriaSegmento == 0) {
                    $scope.categorias[i].visivel = true;
                }
                else {
                    for (var j = 0; j < $scope.categoriasSeg.length; j++) {

                        if ($scope.categorias[i].Id == $scope.categoriasSeg[j].Id) {
                            $scope.categorias[i].visivel = true;
                            break;
                        }
                    }
                }

            }
        }

        function loadMembroCategoriaFailed(response) {
            notificationService.displayError(response.data);
        }
        //22------------------------fim-----------------------------

        //23-----Relaciona categoria de produtos ao membro----------
        function inserirMembroCategoriasProduto() {

            $scope.membroCategorias = [];
            angular.forEach($scope.categorias, function (categoriaPesq) {
                if (categoriaPesq.selected)
                    $scope.membroCategorias.push(categoriaPesq.Id);

            });

            apiService.post('/api/membro/inserirMembroCategoria/' + $scope.novoMembro.Id, $scope.membroCategorias,
                membroCategoriasProdutoSucesso,
                membroCategoriasProdutoFalha);
        }

        function membroCategoriasProdutoSucesso(result) {

            if (result.data.success)
                notificationService.displaySuccess('Categoria relacionada com sucesso.');
            loadMembroCategoria();

        }

        function membroCategoriasProdutoFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //23------------------------fim-----------------------------

        //24-----Relaciona Membro a fornecedores----------
        function inserirMembroFornecedors() {

            $scope.membroFornecedores = [];
            $scope.membroFornecedoresAdd = [];
            $scope.membroFornecedoresDel = [];


            angular.forEach($scope.fornecedoresRegiao, function (fornecedorPesq) {             

                if (fornecedorPesq.selected && !fornecedorPesq.Relacionado)
                    $scope.membroFornecedoresAdd.push(fornecedorPesq.Id);


                if (!fornecedorPesq.selected && fornecedorPesq.Relacionado)
                    $scope.membroFornecedoresDel.push(fornecedorPesq.Id);

            });

            var membroFornecedoresAdm = {
                MembroId: $scope.novoMembro.Id,
                FornecedoresAdd: $scope.membroFornecedoresAdd,
                FornecedoresDel: $scope.membroFornecedoresDel
            };

            apiService.post('/api/membro/inserirMembroFornecedor', membroFornecedoresAdm,
                inserirMembroFornecedorsSucesso,
                inserirMembroFornecedorsFalha);
        }
        
        function inserirMembroFornecedorsSucesso(result) {
            if (result.data.success)
                notificationService.displaySuccess('Fornecedor relacionada com sucesso.');
            loadMembroFornecedor();

        }

        function inserirMembroFornecedorsFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //24------------------------fim-----------------------------

        //25------------------------------------------
        function habilitaDesabilitaAbaRelatForn() {

            if ($scope.novoMembro.Id > 0)
                loadFornecedoresRegiaoMembro();
            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabUsuMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }
        //25-----Fim---------------------


        //25.5------------- Carrega categorias ----------------
        function loadCategoriaPorSegmento(idSeg) {
            if (idSeg != 0) {
                var config = {
                    params: {
                        segmentoId: $scope.filtroCategoriaSegmento
                    }
                };

                apiService.get('/api/produto/categoriaporsegmento', config,
                    loadCategoriaPorSegmentoSucesso,
                    loadCategoriaPorSegmentoFailed);
            }
            else {
                $scope.categoriasSeg = [];
                //loadFornecedorCategoria();
                loadMembroCategoria();
            }
        }

        function loadCategoriaPorSegmentoSucesso(response) {
            $scope.categoriasSeg = response.data;
            //loadFornecedorCategoria();
            loadMembroCategoria();
        }

        function loadCategoriaPorSegmentoFailed(response) {
            notificationService.displayError(response.data);
        }
        //25.5------------------------fim-----------------------------


        //26-----Carrega categoria Membro --
        function loadFornecedoresRegiaoMembro() {

            var config = {
                params: {
                    membroId: $scope.novoMembro.Id

                }
            };


            apiService.get('/api/membro/fornecedorRegiaoMembro', config,
                loadFornecedoresRegiaoMembroSucesso,
                loadFornecedoresRegiaoMembroFalha);
        }

        function loadFornecedoresRegiaoMembroSucesso(response) {
            $scope.fornecedoresRegiao = response.data;
            loadMembroFornecedor();
        }

        function loadFornecedoresRegiaoMembroFalha(response) {
            notificationService.displayError(response.data);
        }
        //26------------------------fim-----------------------------

        //27-----Carrega categoria Membro --
        function loadMembroFornecedor() {

            var config = {
                params: {
                    membroId: $scope.novoMembro.Id
                }
            };


            apiService.get('/api/membro/membroFornecedores', config,
                loadMembroFornecedorSucesso,
                loadMembroFornecedorFalha);
        }

        function loadMembroFornecedorSucesso(response) {

            var membroFornecedor = response.data;
            for (var i = 0; i < $scope.fornecedoresRegiao.length; i++) {
                for (var j = 0; j < membroFornecedor.length; j++) {

                    if ($scope.fornecedoresRegiao[i].Id == membroFornecedor[j].FornecedorId) {
                        $scope.fornecedoresRegiao[i].Relacionado = true;
                        $scope.fornecedoresRegiao[i].selected = true;
                        break;
                    }
                }

            }
        }

        function loadMembroFornecedorFalha(response) {
            notificationService.displayError(response.data);
        }
        //27------------------------fim-----------------------------

        //28------------------------------------------
        function habilitaDesabilitaAbaUsuMembro() {

            if ($scope.novoMembro.Id > 0)
                loadUsuarioMembro();
            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },

                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabUsuMembro: {

                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true


                }
            };
        }

        //28-----Fim---------------------

        //29-----Carrega usuário Membro --
        function loadUsuarioMembro() {

            var config = {
                params: {
                    pessoaId: $scope.novoMembro.PessoaId

                }
            };


            apiService.get('/api/usuario/usuarios', config,
                loadUsuarioMembroSucesso,
                loadUsuarioMembroFalha);
        }

        function loadUsuarioMembroSucesso(response) {

            $scope.usuariosMembro = response.data;

        }

        function loadUsuarioMembroFalha(response) {
            notificationService.displayError(response.data);
        }
        //29------------------------fim-----------------------------


        //31------------------------------------------
        function habilitaDesabilitaAbaMembroDemanda() {

            if ($scope.novoMembro.Id > 0) {
                pesquisarCategoria();
                $scope.pesquisarMembroDemanda();
            }

            $scope.tabsMembro = {
                tabPesquisar: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabPesquisarPF: {

                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true

                },
                tabCadMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabCadEndMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRelFornecedor: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabMembroDemanda: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabUsuMembro: {
                    tabAtivar: " ",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


        }

        //31-----Fim---------------------


        //32-----Carrega Segmentos --
        function loadSegmentos() {

            apiService.get('/api/segmento/loadsegmentos', null,
                loadSegmentosSucesso,
                loadSegmentosFailed);
        }

        function loadSegmentosSucesso(response) {
            $scope.segmentos = response.data;
            //loadCategoriaSegmento();

        }

        function loadSegmentosFailed(response) {
            notificationService.displayError(response.data);
        }
        //32------------------------fim-----------------------------



        //----------------------------------Altera endereco padrão-------------
        function atualizaParaEndPadrao(endereco) {

            endereco.Cep = endereco.Cep.replace(/-/g, '').trim();
            endereco.EnderecoPadrao = true;
            apiService.post('/api/endereco/atualizarEndPadrao', endereco,
                atualizarEnderecoPSucesso,
                atualizarEnderecoPFalha);

        }

        function atualizarEnderecoPSucesso(response) {

            notificationService.displaySuccess('Endereço Padrão Atualizado com Sucesso.');


            pesquisarEnderecoMembro();

        }

        function atualizarEnderecoPFalha(response) {

            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //checkedall-------------------------------------------------
        $scope.checkAll = admUtilService.checkBoxAll;
        //-----------------------------------------------------------


        //1-----Carrega Categorias aba Pesquisar------------

        function pesquisarMembroDemanda(page) {
            page = page || 0;

            $scope.loadingMembroDemanda = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    membroId: $scope.novoMembro.Id
                }
            };

            apiService.get('/api/membroDemanda/pesquisar', config,
                membroDemandaLoadCompleted,
                membroDemandaLoadFailed);
        }

        function membroDemandaLoadCompleted(result) {

            $scope.demandas = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMembroDemanda = false;

            var msg = result.data.Items.length > 1 ? " Demandas Encontradas" : " Demanda Encontrada";
            if ($scope.page == 0 && $scope.novoMembroDemanda.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function membroDemandaLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //2-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCategoria() {


            apiService.get('/api/subcategoria/categoria', null,
                categoriaLoadCompleted,
                categoriaLoadFailed);
        }

        function categoriaLoadCompleted(response) {

            var newItem = new function () {
                this.Id = undefined;
                this.DescCategoria = "Categoria ...";

            };
            response.data.push(newItem);
            $scope.categorias = response.data;

        }

        function categoriaLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function openEditDialogMembroDemanda(membroDemandaParam) {
            $scope.novoMembroDemanda = membroDemandaParam;

            $modal.open({
                templateUrl: 'Scripts/SPAAdm/demanda/demandaModal.html',
                controller: 'demandaCtrl',
                scope: $scope
            }).result.then(function ($scope) {
            }, function () {
                $scope.pesquisarMembroDemanda();
            });
        }

        //2-----FIM--------------------------------------------------


        function editarMembroDemanda(pesqMembroDemanda) {
            $scope.novoMenu = pesqMembroDemanda;

            $scope.menuSubCategoria = $scope.pesquisarMembroDemanda.filter(function (obj) {
                return obj.subcategoriaId == pesqMembroDemanda.Id;
            });
        }

        function loadMembroCategoriaId(categoriaId) {
            if (categoriaId != undefined)
                membroCategoriaId.value = categoriaId;
            else
                membroCategoriaId.value = 0;
        }


        //3-------------Carrega Período Entrega-------------------------
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

        //3-------------Carrega Período Entrega-------------------------


        //-------------Carrega franquias---------------------------------
        function carregaFranquias() {

            apiService.get('/api/franquia/carregaFranquiasMembro', null,
                carregaFranquiasLoadCompleted,
                loadFranquiasLoadFailed);
        }

        function carregaFranquiasLoadCompleted(result) {
            $scope.FranquiaMembro = result.data;
        }

        function loadFranquiasLoadFailed(result) {

            notificationService.displayError('Erro ao carregar combo de franquias!');
        }

        //--------------------------------------------------------------

        pesquisarMembro();
        loadSegmentos();
        carregaFranquias();
    }

})(angular.module('ECCAdm'));