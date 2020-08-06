(function (app) {
    'use strict';

    app.controller('fornecedorCtrl', fornecedorCtrl);

    fornecedorCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$modal', 'admUtilService', 'SweetAlert'];

    function fornecedorCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $modal, admUtilService, SweetAlert) {
        $scope.pageClass = 'page-fornecedor';
        $scope.pesquisarFornecedor = pesquisarFornecedor;
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.habilitaDesabilitaAbaFornecedor = habilitaDesabilitaAbaFornecedor;
        $scope.novoFornecedor = {};
        $scope.logradouros = [];
        $scope.estados = [];
        $scope.cidades = [];
        $scope.bairros = [];
        $scope.loadCidadePorRegiaoChange = loadCidadePorRegiao;
        $scope.loadCidadePorRegiaoFornecedorChange = loadCidadePorRegiaoFornecedor;
        $scope.loadCidadePorRegiaoFornecedorSemanal = loadCidadePorRegiaoFornecedorSemanal;
        $scope.loadRegiaoChange = loadRegiao;
        $scope.loadBairroChange = loadBairro;
        $scope.loadEnderecoCep = loadEnderecoCep;
        $scope.habilitaddlCidade = true;
        $scope.habilitaddlBairro = true;
        $scope.formaPagtosAvista = [];
        $scope.formaPagtosParcelados = [];
        $scope.inserirFornecedor = inserirFornecedor;
        $scope.editarFornecedor = editarFornecedor;
        $scope.habilitaDesabilitaAbaRelaCategoria = habilitaDesabilitaAbaRelaCategoria;
        $scope.categorias = [];
        $scope.inserirFornecedorCategoriasProduto = inserirFornecedorCategoriasProduto;
        $scope.regiaoForn = [];
        $scope.inserirDeletarRegiaoFornecedor = inserirDeletarRegiaoFornecedor;
        $scope.habilitaDesabilitaAbaUsuForn = habilitaDesabilitaAbaUsuForn;
        $scope.habilitaDesabilitaAbaRegForn = habilitaDesabilitaAbaRegForn;
        $scope.habilitaDesabilitaAbaPrazForn = habilitaDesabilitaAbaPrazForn;
        $scope.inserirPrazoDiasSemana = inserirPrazoDiasSemana;
        $scope.limpaArrayPrazoSemanaChanged = limpaArrayPrazoSemanaChanged;
        $scope.openEditDialog = openEditDialog;
        $scope.allReg = false;
        $scope.atualizaPrazoFornecedor = atualizaPrazoFornecedor;
        $scope.loadCategoriaPorSegmento = loadCategoriaPorSegmento;
        $scope.categoriasSeg = [];
        $scope.fornRegiaoCidades = [];
        $scope.regioes = [];
        $scope.filtroCategoriaSegmento = 0;
        $scope.mostraBairroDll = true;
        $scope.mostraBairroInput = false;
        $scope.habilitaCategoria = true;
        $scope.checado = false;
        $scope.habilitaDesabilitaComboPrazoSemana = habilitaDesabilitaComboPrazoSemana;
        $scope.habilitaDesabilitaComboPrazoCorrido = habilitaDesabilitaComboPrazoCorrido;
        $scope.novoFornecedor.Endereco = {};
        $scope.escolhaPrazo = {};
        $scope.arrayPrazoSemana = [];
        $scope.novoPrazoFornSemanal = {};
        $scope.novoPrazoForn = {};
        $scope.fornRegiaoCidadesSemanal = [];
        $scope.fornecedorRemoveRegiao = [];
        $scope.fornecedorRegiao = [];

        $scope.openDatePicker = openDatePicker;
        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };

        function openEditDialog(fornParam) {
            $scope.novoUsuario = fornParam;
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

        app.filter('groupBy', function () {
            return _.memoize(function (items, field) {
                return _.groupBy(items, field);
            }
            );
        });


        $scope.arrayDiasSemana = [
            { Id: 1, DescDiaSemana: 'Segunda' },
            { Id: 2, DescDiaSemana: 'Terça' },
            { Id: 3, DescDiaSemana: 'Quarta' },
            { Id: 4, DescDiaSemana: 'Quinta' },
            { Id: 5, DescDiaSemana: 'Sexta' },
            { Id: 6, DescDiaSemana: 'Sábado' },
            { Id: 7, DescDiaSemana: 'Domingo' }
        ];

        //0--------Declaracao de todas as abas tela de fornecedores----
        $scope.tabsFornecedores = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadFornecedor: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },

            tabRelProduto: {
                tabAtivar: " ",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            }

        };
        //0-----------------Fim----------------------------------------



        //1-----Carrega membro aba Pesquisar------------
        function pesquisarFornecedor(page) {

            page = page || 0;
            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroFornecedor
                }
            };

            apiService.get('/api/fornecedor/pesquisar', config,
                fornecedoresLoadSucesso,
                fornecedoresLoadFailed);
        }

        function fornecedoresLoadSucesso(result) {

            $scope.Fornecedores = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;



            var msg = result.data.Items.length > 1 ? " Fornecedores Encontrados" : "Fornecedor Encontrado";
            if ($scope.page == 0 && $scope.novoFornecedor.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function fornecedoresLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //1----Fim---------------------------------------



        //2-----Habilita e desabilita abas após clicar em pesquisar--
        function habilitaDesabilitaAbaPesquisa() {
            $scope.novoFornecedor = '';
            $scope.fornCidades = [];
            $scope.regioesForn = [];
            $scope.novoPrazoForn = {};
            $scope.novaRegiaoForn = {};
            limpaFormaPagto();
            $scope.filtro = '';

            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },


                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };


        }
        //2-----Fim--------------------------------------------------


        //2.5 -----Habilita e desabilita abas após clicar em Fornecedor--
        function habilitaDesabilitaAbaFornecedor() {

            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: " ",
                    tabhabilitar: $scope.novoFornecedor.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoFornecedor.Id > 0 ? true : false
                },


                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novoFornecedor.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoFornecedor.Id > 0 ? true : false
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novoFornecedor.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoFornecedor.Id > 0 ? true : false
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: $scope.novoFornecedor.Id > 0 ? true : false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: $scope.novoFornecedor.Id > 0 ? true : false
                }
            };


        }

        //2.5 -----Fim---------------------------------------------------


        //3-----Carrega dropdown Logradouro --
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
        //3------------------------fim-----------------------------


        //4-----Carrega dropdown Estado----------------------------
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
            ;
            response.data.push(newItem);
            $scope.estados = response.data;
        }

        function loadEstadoFailed(response) {
            notificationService.displayError(response.data);
        }
        //4------------------------fim-----------------------------


        //5-----Busca Endereço pelo cep----------------------------
        function loadEnderecoCep(cep) {

            if (cep.length == 8) {

                var config = {
                    params: {
                        cep: cep
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


                if (response.data.Endereco === "" || response.data.Endereco == null) {
                    notificationService.displayInfo("CEP NÃO ENCONTRADO FAVOR DIGITAR O ENDEREÇO MANUALMENTE !");

                } else {

                    var idEndForn = $scope.novoFornecedor.Endereco.Id;

                    $scope.novoFornecedor.Endereco = response.data;

                    $scope.novoFornecedor.Endereco.Id = idEndForn;
                }
            }



        }

        function loadEnderecoCepFailed(response) {
            notificationService.displayError(response.data);
        }
        //5------------------------fim-----------------------------



        //6-----Carrega dropdown Cidade --
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


            //estados
            //regioes
            $scope.cidades = response.data;
            $scope.habilitaddlCidade = false;


        }

        function loadCidadeFailed(response) {
            notificationService.displayError(response.data);
        }
        //6------------------------fim-----------------------------


        //7-----Carrega dropdown Bairro --------------------------
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

            if (response.data == "") {
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
        //7------------------------fim-----------------------------


        //8-----Carrega Formas de pagamentos ---------------------
        function loadFormaPagto() {


            apiService.get('/api/fornecedor/formaPagto', null,
                loadFormaPagtoSucesso,
                loadFormaPagtoFailed);
        }

        function loadFormaPagtoSucesso(response) {

            //$scope.formaPagtos = response.data.formaPagtosAvistaVM;
            var pagtoAvista = response.data.formaPagtosAvistaVM;

            $scope.formaPagtosAvista = pagtoAvista;
            $scope.formaPagtosParcelados = response.data.formaPagtosParceladoVM;

        }

        function loadFormaPagtoFailed(response) {
            notificationService.displayError(response.data);
        }
        //8------------------------fim-----------------------------


        //9-----Insere novo membro aba Membro-----------------------
        function inserirFornecedor() {

            $scope.novoFornecedor.Cnpj = $scope.novoFornecedor.Cnpj.split(".").join("").split("/").join("").split("-").join("");
            $scope.novoFornecedor.DddTelComl = $scope.novoFornecedor.DddTelComl.split("(").join("").split(")").join("");
            $scope.novoFornecedor.TelefoneComl = $scope.novoFornecedor.TelefoneComl.split("-").join("");
            $scope.novoFornecedor.DddCel = $scope.novoFornecedor.DddCel.split("(").join("").split(")").join("");
            $scope.novoFornecedor.Celular = $scope.novoFornecedor.Celular.split("-").join("");


            if ($scope.novoFornecedor.Id > 0) {
                atualizarFornecedor();
            } else {
                inserirFornecedorModel();
            }

        }

        function inserirFornecedorModel() {

            $scope.novoFornecedor.FormaPagtos = [];

            $scope.novoFornecedor.FormaPagtos = [];

            angular.forEach($scope.formaPagtosAvista, function (formaPagto) {
                if (formaPagto.selected) {
                    if (formaPagto.Desconto === null || formaPagto.Desconto === undefined) {
                        formaPagto.Desconto = 0;
                    }

                    var objFormaPagtoAvista = {
                        FormaPagtoId: formaPagto.Id,
                        FornecedorId: $scope.novoFornecedor.Id,
                        Desconto: formaPagto.Desconto
                    };

                    $scope.novoFornecedor.FormaPagtos.push(objFormaPagtoAvista);
                } else {
                    formaPagto.Desconto = 0;
                }


            });

            angular.forEach($scope.formaPagtosParcelados, function (formaPagto) {
                if (formaPagto.selected) {
                    if (formaPagto.Desconto === null || formaPagto.Desconto === undefined) {
                        formaPagto.Desconto = 0;
                    }
                    var objFormaPagtoAvista = {
                        FormaPagtoId: formaPagto.Id,
                        FornecedorId: $scope.novoFornecedor.Id,
                        Desconto: formaPagto.Desconto,
                        VlFormaPagto: formaPagto.VlFormaPagto
                    };
                    $scope.novoFornecedor.FormaPagtos.push(objFormaPagtoAvista);
                } else {
                    formaPagto.Desconto = 0;
                }
            });
            if ($scope.novoFornecedor.FormaPagtos.length > 0) {

                apiService.post('/api/fornecedor/inserir', $scope.novoFornecedor,
                    inserirFornecedorSucesso,
                    inserirFornecedorFalha);
            } else {
                notificationService.displayInfo('Selecione ao menos uma forma de pagamento.');
            }
        }

        function inserirFornecedorSucesso(response) {

            loadBairro($scope.novoFornecedor.Endereco.CidadeId);
            notificationService.displaySuccess($scope.novoFornecedor.RazaoSocial + ' Incluído com Sucesso.');
            $scope.novoFornecedor = response.data;

            $scope.mostraBairroDll = true;
            $scope.mostraBairroInput = false;

            $scope.filtroFornecedor = response.data.Cnpj;
            checaFormaPagto($scope.novoFornecedor.FormaPagtos);
            pesquisarFornecedor();


            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
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
                }
                ,
                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };

        }

        function inserirFornecedorFalha(response) {
			console.log(response);



            if (response.status === "400")
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //----------------------------Fim---------------------------

        //10-----Chaca formas de pagamentos ja cadastrada-----------
        function checaFormaPagto(formaPagtoCadastrada) {

            for (var i = 0; i < $scope.formaPagtosAvista.length; i++) {
                for (var j = 0; j < formaPagtoCadastrada.length; j++) {

                    if ($scope.formaPagtosAvista[i].Id === formaPagtoCadastrada[j].FormaPagtoId &&
                        formaPagtoCadastrada[j].Ativo) {

                        $scope.formaPagtosAvista[i].Relacionado = true;
                        $scope.formaPagtosAvista[i].selected = true;
                        $scope.formaPagtosAvista[i].Desconto = formaPagtoCadastrada[j].Desconto;
                    }

                }
            }

            for (var m = 0; m < $scope.formaPagtosParcelados.length; m++) {
                for (var x = 0; x < formaPagtoCadastrada.length; x++) {
                    if ($scope.formaPagtosParcelados[m].Id === formaPagtoCadastrada[x].FormaPagtoId &&
                        formaPagtoCadastrada[x].Ativo) {

                        $scope.formaPagtosParcelados[m].Relacionado = true;
                        $scope.formaPagtosParcelados[m].selected = true;
                        $scope.formaPagtosParcelados[m].VlFormaPagto = formaPagtoCadastrada[x].VlFormaPagto;
                        $scope.formaPagtosParcelados[m].ValorMinParcela = formaPagtoCadastrada[x].ValorMinParcela;
                        $scope.formaPagtosParcelados[m].ValorMinParcelaPedido = formaPagtoCadastrada[x].ValorMinParcelaPedido;
                    }

                }
            }
        }
        //10----------------------------Fim-------------------------


        //11-----Editar dados aba fornecedor---------------------
        function editarFornecedor(fornecedorPesq) {
            $scope.novoFornecedor = fornecedorPesq;
            checaFormaPagto($scope.novoFornecedor.FormaPagtos);
            carregaRegiaoPrazo(fornecedorPesq.Id);
            loadLogradouro();
            loadEstado();
            loadCidade($scope.novoFornecedor.Endereco.EstadoId);
            loadBairro($scope.novoFornecedor.Endereco.CidadeId);
            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
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


                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };


        }
        //11---------------------------------Fim---------------------


        //12-----Atualiza dados membro aba Membro------------
        function atualizarFornecedor() {
            $scope.novoFornecedor.Cnpj = $scope.novoFornecedor.Cnpj.split(".").join("").split("/").join("").split("-").join("");

            $scope.novoFornecedor.FormaPagtos = [];

            angular.forEach($scope.formaPagtosAvista, function (formaPagto) {
                if (formaPagto.selected) {
                    if (formaPagto.Desconto === null || formaPagto.Desconto === undefined) {
                        formaPagto.Desconto = 0;
                    }

                    var objFormaPagtoAvista = {
                        FormaPagtoId: formaPagto.Id,
                        FornecedorId: $scope.novoFornecedor.Id,
                        Desconto: formaPagto.Desconto
                    };

                    $scope.novoFornecedor.FormaPagtos.push(objFormaPagtoAvista);
                } else {
                    formaPagto.Desconto = 0;
                }


            });

            angular.forEach($scope.formaPagtosParcelados, function (formaPagto) {
                if (formaPagto.selected) {
                    if (formaPagto.Desconto === null || formaPagto.Desconto === undefined) {
                        formaPagto.Desconto = 0;
                    }
                    var objFormaPagtoAvista = {
                        FormaPagtoId: formaPagto.Id,
                        FornecedorId: $scope.novoFornecedor.Id,
                        Desconto: formaPagto.Desconto,
                        VlFormaPagto: formaPagto.VlFormaPagto,
                        ValorMinParcela: formaPagto.ValorMinParcela,
                        ValorMinParcelaPedido: formaPagto.ValorMinParcelaPedido
                    };
                    $scope.novoFornecedor.FormaPagtos.push(objFormaPagtoAvista);
                } else {
                    formaPagto.Desconto = 0;
                }
            });


            if ($scope.novoFornecedor.FormaPagtos.length > 0) {

                $scope.novoFornecedor.Usuario = null;
                apiService.post('/api/fornecedor/atualizar', $scope.novoFornecedor,
                    atualizarFornecedorSucesso,
                    atualizarFornecedorFalha);
            } else {

                notificationService.displayInfo('Selecione ao menos uma forma de pagamento.');
            }
        }

        function atualizarFornecedorSucesso(response) {
            notificationService.displaySuccess($scope.novoFornecedor.RazaoSocial + ' Atualizado com Sucesso.');
            $scope.novoFornecedor = response.data;

            pesquisarFornecedor();
            $scope.filtroFornecedor = response.data.Cnpj;
            checaFormaPagto($scope.novoFornecedor.FormaPagtos);

            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
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
                }

                ,


                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };
        }

        function atualizarFornecedorFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //12---------------------Fim--------------------------


        //13----------habilitaDesabilitaAbaRelaCategoria----
        function habilitaDesabilitaAbaRelaCategoria() {

            $scope.filtroCategoriaSegmento = 0;
            loadCategoria();

            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
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

                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };


        }
        //13----------------- Fim ---------------------------


        //14------------- Carrega categorias ----------------
        function loadCategoria() {
            apiService.get('/api/produto/categoria', null,
                loadCategoriaSucesso,
                loadCategoriaFailed);
        }

        function loadCategoriaSucesso(response) {
            $scope.categorias = response.data;
            loadFornecedorCategoria();
        }

        function loadCategoriaFailed(response) {
            notificationService.displayError(response.data);
        }
        //14------------------------fim-----------------------------

        //14.5------------- Carrega categorias ----------------
        function loadCategoriaPorSegmento(idSeg) {
            if (idSeg !== 0) {
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
                loadFornecedorCategoria();
            }
        }

        function loadCategoriaPorSegmentoSucesso(response) {
            $scope.categoriasSeg = response.data;
            //$scope.habilitaCategoria = false;

            loadFornecedorCategoria();
        }

        function loadCategoriaPorSegmentoFailed(response) {
            notificationService.displayError(response.data);
        }
        //14.5------------------------fim-----------------------------


        //15-----Carrega categoria fornecedor --
        function loadFornecedorCategoria() {
            var config = {
                params: {
                    fornecedorId: $scope.novoFornecedor.Id
                }
            };


            apiService.get('/api/fornecedor/fornecedorCategorias', config,
                loadFornecedorCategoriaSucesso,
                loadFornecedorCategoriaFailed);
        }

        function loadFornecedorCategoriaSucesso(response) {

            var fornCategoria = response.data;
            for (var i = 0; i < $scope.categorias.length; i++) {
                for (var j = 0; j < fornCategoria.length; j++) {

                    if ($scope.categorias[i].Id == fornCategoria[j].Id) {
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

        function loadFornecedorCategoriaFailed(response) {
            notificationService.displayError(response.data);
        }
        //15------------------------fim-----------------------------


        //16-----Relaciona categoria de produtos ao fornecedor----------
        function inserirFornecedorCategoriasProduto() {

            $scope.fornecedorCategorias = [];
            angular.forEach($scope.categorias, function (categoriaPesq) {
                if (categoriaPesq.selected) $scope.fornecedorCategorias.push(categoriaPesq.Id);
            });

            apiService.post('/api/fornecedor/inserirFornecedorCategoria/' + $scope.novoFornecedor.Id, $scope.fornecedorCategorias,
                fornecedorCategoriasProdutoSucesso,
                fornecedorCategoriasProdutoFalha);
        }

        function fornecedorCategoriasProdutoSucesso(result) {

            if (result.data.success)
                notificationService.displaySuccess('Categoria relacionada com sucesso.');
            loadFornecedorCategoria();

        }

        function fornecedorCategoriasProdutoFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //16------------------------fim-----------------------------


        //17----------habilitaDesabilitaAbaRelaCategoria----

        function habilitaDesabilitaAbaUsuForn() {

            if ($scope.novoFornecedor.Id > 0)
                loadUsuarioForn();
            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabUsuForn: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };


        }
        //17-----------------Fim----------------------------


        //18-----Carrega usuário Fornecedor --
        function loadUsuarioForn() {

            var config = {
                params: {
                    pessoaId: $scope.novoFornecedor.PessoaId
                }
            };

            apiService.get('/api/usuario/usuarios', config,
                loadUsuarioFornSucesso,
                loadUsuarioFornFalha);
        }

        function loadUsuarioFornSucesso(response) {

            $scope.usuariosForn = response.data;

        }

        function loadUsuarioFornFalha(response) {
            notificationService.displayError(response.data);
        }
        //18------------------------fim-----------------------------


        //20-----Limap forma de pagamento----------------------
        function limpaFormaPagto() {

            for (var i = 0; i < $scope.formaPagtosAvista.length; i++) {
                $scope.formaPagtosAvista[i].Relacionado = false;
                $scope.formaPagtosAvista[i].Desconto = '';
            }

            for (var x = 0; x < $scope.formaPagtosParcelados.length; x++) {
                $scope.formaPagtosParcelados[x].Relacionado = false;
                $scope.formaPagtosParcelados[x].VlFormaPagto = '';
                $scope.formaPagtosParcelados[x].ValorMinParcela = '';
                $scope.formaPagtosParcelados[x].ValorMinParcelaPedido = '';

            }

        }
        //20----------------------------Fim-----------------------------------

        //21----------habilitaDesabilitaAbaRegiao----

        function habilitaDesabilitaAbaRegForn() {

            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRegForn: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };


        }
        //21-----------------Fim----------------------------

        //22-----Carrega Regiao fornecedor --
        function loadFornecedorRegiao() {

            var config = {
                params: {
                    fornecedorId: $scope.novoFornecedor.Id
                }
            };

            apiService.get('/api/fornecedor/fornecedorregioes', config,
                loadFornecedorRegiaoSucesso,
                loadFornecedorRegiaoFailed);
        }

        function loadFornecedorRegiaoSucesso(response) {
            var diasSemana = 1;
            var dias = 2;

            var fornReg = response.data.fornecedoresRegiao;
            $scope.cidadesCadastradas = [];


            for (var i = 0; i < $scope.fornCidades.length; i++) {
                var contador = 0;
                for (var j = 0; j < fornReg.length; j++) {
                    if ($scope.fornCidades[i].Id === fornReg[j].CidadeId) {

                        $scope.fornCidades[i].selected = true;
                        $scope.fornCidades[i].VlPedMinRegiao = fornReg[j].VlPedMinRegiao.replace('R$', '');
                        $scope.fornCidades[i].TaxaEntrega = fornReg[j].TaxaEntrega.replace('R$', '');
                        $scope.fornCidades[i].Cif = fornReg[j].Cif;

                        if (fornReg[j].TipoPrazo === dias) {
                            $scope.fornCidades[i].TipoPrazo = dias;
                        } else {
                            $scope.fornCidades[i].TipoPrazo = diasSemana;
                        }

                        $scope.cidadesCadastradas.push($scope.fornCidades[i]);
                    } else {
                        contador++;
                    }
                }
                if (fornReg.length === contador) {
                    $scope.fornCidades[i].TipoPrazo = 0;
                }
            }
        }

        function loadFornecedorRegiaoFailed(response) {
            notificationService.displayError(response.data);
        }
        //22------------------------fim-----------------------------


        //23-----Carrega dropdown Região --
        function loadRegiao(estadoId) {

            if (estadoId !== undefined && estadoId !== 0) {

                $scope.fornRegiaoCidadesSemanal = [];
                $scope.fornRegiaoCidades = [];
                $scope.fornCidades = [];
                $scope.novoPrazoFornSemanal.RegiaoId = undefined;
                $scope.novoPrazoFornSemanal.CidadeId = undefined;
                $scope.novoPrazoForn.RegiaoId = undefined;
                $scope.novoPrazoForn.CidadeId = undefined;
                $scope.filtroRegiao = "";
                $scope.regioes = [];
                limpaArrayPrazoSemanaChanged();

                loadCidadePorRegiaoFornecedorSemanal(estadoId, 0);
                loadCidadePorRegiaoFornecedor(estadoId, 0);
                loadCidadePorRegiao(0, estadoId, 0);

                var config = {
                    params: {
                        EstadoId: estadoId
                    }
                };

                apiService.get('/api/endereco/regiao', config,
                    loadRegiaoCompleted,
                    loadRegiaoFailed);
            } else {

                $scope.fornRegiaoCidadesSemanal = [];
                $scope.fornRegiaoCidades = [];
                $scope.fornCidades = [];
                $scope.novoPrazoFornSemanal.RegiaoId = undefined;
                $scope.novoPrazoFornSemanal.CidadeId = undefined;
                $scope.novoPrazoForn.RegiaoId = undefined;
                $scope.novoPrazoForn.CidadeId = undefined;
                $scope.filtroRegiao = "";
                $scope.regioes = [];
                limpaArrayPrazoSemanaChanged();
                $scope.page = 0;
                $scope.pagesCount = 0;
                $scope.totalCount = 0;


            }
        }

        function loadRegiaoCompleted(response) {

            $scope.regioes = response.data;
            $scope.fornRegiaoCidades = [];
            $scope.novoPrazoForn.Prazo = '';

            $scope.habilitaddlCidade = false;
        }

        function loadRegiaoFailed(response) {
            notificationService.displayError(response.data);
        }
        //23------------------------fim-----------------------------


        //24-----Carrega lista Cidade ------------------------------
        function loadCidadePorRegiao(page, estadoId, regiaoId) {

            page = page || 0;

            if (regiaoId == undefined) {
                regiaoId = 0;
            }

            var config = {
                params: {
                    page: page,
                    pageSize: 100,
                    EstadoId: estadoId,
                    RegiaoId: regiaoId,
                    filter: $scope.filtroRegiao
                }
            };

            apiService.get('/api/endereco/cidadeporregiaopaginada', config,
                loadCidadePorRegiaoCompleted,
                loadCidadePorRegiaoFailed);
        }

        function loadCidadePorRegiaoCompleted(response) {

            $scope.fornCidades = response.data.Items;
            loadFornecedorRegiao();

            $scope.page = response.data.Page;
            $scope.pagesCount = response.data.TotalPages;
            $scope.totalCount = response.data.TotalCount;
        }

        function loadCidadePorRegiaoFailed(response) {
            notificationService.displayError(response.data);
        }
        //24------------------------fim-----------------------------


        //25-----Cadastro de Regiao Fornecedor -------------------------------

        function inserirDeletarRegiaoFornecedor() {

            var objetoRegiao = {};

            angular.forEach($scope.fornCidades, function (item, chave) {

                if (!item.selected && item.selected !== undefined) {

                    if ($scope.cidadesCadastradas.length > 0) {
                        for (var y = 0; y < $scope.cidadesCadastradas.length; y++) {
                            if ($scope.cidadesCadastradas[y].Id === item.Id) {
                                objetoRegiao = {
                                    CidadeId: item.Id,
                                    VlPedMinRegiao: item.VlPedMinRegiao.replace('R$', ''),
                                    TipoPrazo: item.TipoPrazo,
                                    TaxaEntrega: item.TaxaEntrega,
                                    Cif: item.Cif,
                                    InserirRegiao: false
                                };
                                $scope.fornecedorRegiao.push(objetoRegiao);
                            }
                        }
                    }
                    else {
                        objetoRegiao = {
                            CidadeId: item.Id,
                            VlPedMinRegiao: item.VlPedMinRegiao.replace('R$', ''),
                            TipoPrazo: item.TipoPrazo,
                            TaxaEntrega: item.TaxaEntrega,
                            Cif: item.Cif,
                            InserirRegiao: true
                        };
                        $scope.fornecedorRegiao.push(objetoRegiao);
                    }
                }
                else {
                    var contador = 0;
                    if ($scope.cidadesCadastradas.length > 0 && item.selected !== undefined) {
                        for (var x = 0; x < $scope.cidadesCadastradas.length; x++) {
                            if (!$scope.cidadesCadastradas[x].Id === item.Id) {
                                objetoRegiao = {
                                    CidadeId: item.Id,
                                    VlPedMinRegiao: item.VlPedMinRegiao.replace('R$', ''),
                                    TipoPrazo: item.TipoPrazo,
                                    TaxaEntrega: item.TaxaEntrega,
                                    Cif: item.Cif,
                                    InserirRegiao: true
                                };
                                $scope.fornecedorRegiao.push(objetoRegiao);
                            } else {
                                contador++;
                            }
                        }

                        if ($scope.cidadesCadastradas.length === contador) {
                            objetoRegiao = {
                                CidadeId: item.Id,
                                VlPedMinRegiao: item.VlPedMinRegiao.replace('R$', ''),
                                TipoPrazo: item.TipoPrazo,
                                TaxaEntrega: item.TaxaEntrega,
                                Cif: item.Cif,
                                InserirRegiao: true
                            };
                            $scope.fornecedorRegiao.push(objetoRegiao);
                        }
                    }
                    else {
                        if (item.selected !== undefined) {
                            objetoRegiao = {
                                CidadeId: item.Id,
                                VlPedMinRegiao: item.VlPedMinRegiao.replace('R$', ''),
                                TipoPrazo: item.TipoPrazo,
                                TaxaEntrega: item.TaxaEntrega,
                                Cif: item.Cif,
                                InserirRegiao: true
                            };
                            $scope.fornecedorRegiao.push(objetoRegiao);
                        }
                    }
                }
            });

            if ($scope.fornecedorRegiao.length > 0) {
                inserirRegiaoFornecedor();
            }

        }

        function inserirRegiaoFornecedor() {

            $scope.novoRegiaoFornecedor = {};

            $scope.novoRegiaoFornecedor.fornecedorId = $scope.novoFornecedor.Id;
            $scope.novoRegiaoFornecedor.regiaoId = $scope.novaRegiaoForn.RegiaoId;
            $scope.novoRegiaoFornecedor.dadosRegiaoFornecedores = $scope.fornecedorRegiao;

            apiService.post('/api/fornecedor/inserirregiao', $scope.novoRegiaoFornecedor,
                inserirRegiaoFornecedorSucesso,
                inserirRegiaoFornecedorFalha);

            function inserirRegiaoFornecedorSucesso(response) {
                $scope.regioesForn = response.data;
                loadCidadePorRegiao($scope.page, $scope.novaRegiaoForn.EstadoId, $scope.novaRegiaoForn.RegiaoId);
                $scope.allReg = false;
                //$scope.novaRegiaoForn = {};
                $scope.novoRegiaoFornecedor = {};
                //$scope.filtroRegiao = '';
                $scope.fornecedorRegiao = [];
                notificationService.displaySuccess('Região atualizada com sucesso!');

            }

            function inserirRegiaoFornecedorFalha(response) {
                console.log(response);
                if (response.status === '400')
                    for (var i = 0; i < response.data.length; i++) {
                        notificationService.displayInfo(response.data[i]);
                    }
                else
                    notificationService.displayError(response.statusText);
            }

        }

        //25 -----------------------------------------------------------------


        //26----------habilitaDesabilitaAbaRegiao----
        function habilitaDesabilitaAbaPrazForn() {

            carregaRegiaoPrazo($scope.novoFornecedor.Id);

            carregaAbaPrazoEntrega();

            $scope.tabsFornecedores = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRelProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },


                tabUsuForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabRegForn: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },

                tabPrazForn: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }

            };


        }
        //26-----------------Fim----------------------------------------------


        //27-----Atualiza prazo Fornecedor -----------------------------------
        function atualizaPrazoFornecedor(diasSemanas, diasCorridos) {

            if ($scope.arrayPrazoSemana.length > 0 && diasSemanas) {

                apiService.post('/api/fornecedor/inserirprazosemana', $scope.arrayPrazoSemana,
                    inserirPrazoSemanaSucesso,
                    inserirPrazoSemanaFalha);

                function inserirPrazoSemanaSucesso(response) {

                    carregaRegiaoPrazo($scope.novoFornecedor.Id);
                    limpaPrazoSemanal();
                    SweetAlert.swal("Sucesso!", "Prazo de entrega por dia da semana atualizado com sucesso!", "success");

                }

                function inserirPrazoSemanaFalha(response) {
                    console.log(response);
                    if (response.status === '400')
                        for (var i = 0; i < response.data.length; i++) {
                            notificationService.displayInfo(response.data[i]);
                        }

                    else if (response.status == '412') {
                        notificationService.displayError('Nenhuma cidade cadastrada para a Região selecionada.');
                    }
                    else {
                        notificationService.displayError(response.statusText);
                    }
                }

            }

            if (diasCorridos) {

                $scope.novoPrazoFornecedor = {};

                if ($scope.novoPrazoForn.RegiaoId == undefined) {
                    $scope.novoPrazoForn.RegiaoId = 0;
                }

                $scope.novoPrazoFornecedor.fornecedorId = $scope.novoFornecedor.Id;
                $scope.novoPrazoFornecedor.regiaoId = $scope.novoPrazoForn.RegiaoId;
                $scope.novoPrazoFornecedor.cidadeId = $scope.novoPrazoForn.CidadeId;
                $scope.novoPrazoFornecedor.Prazo = $scope.novoPrazoForn.Prazo;

                apiService.post('/api/fornecedor/atualizaprazo', $scope.novoPrazoFornecedor,
                    atualizaPrazoFornecedorSucesso,
                    atualizaPrazoFornecedorFalha);

                function atualizaPrazoFornecedorSucesso(response) {

                    $scope.regioesForn = response.data;
                    $scope.allReg = false;
                    SweetAlert.swal("Sucesso!", "Prazo de entrega por dias corridos atualizados com sucesso!", "success");
                    limpaPrazoSemanal();
                    carregaRegiaoPrazo($scope.novoFornecedor.Id);
                }

                function atualizaPrazoFornecedorFalha(response) {
                    console.log(response);
                    if (response.status === '400')
                        for (var i = 0; i < response.data.length; i++) {
                            notificationService.displayInfo(response.data[i]);
                        }

                    else if (response.status == '412') {
                        notificationService.displayError('Nenhuma cidade cadastrada para a Região selecionada.')
                    }
                    else {
                        notificationService.displayError(response.statusText);
                    }
                }


            }
        }
        //27 -----------------------------------------------------------------


        //28-----Carrega Segmentos -------------------------------------------
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
        //28------------------------fim---------------------------------------


        //checkedall----------------------------------------------------------
        $scope.checkAll = admUtilService.checkBoxAll;
        //--------------------------------------------------------------------


        //29-------------Carrega Região Prazos--------------------------------
        function carregaRegiaoPrazo(idFornecedor) {

            apiService.get('/api/endereco/carregaRegiaoPrazo/' + idFornecedor, null,
                carregaRegiaoPrazoLoadCompleted,
                carregaRegiaoPrazoLoadFailed);
        }

        function carregaRegiaoPrazoLoadCompleted(result) {

            $scope.CidadesCorridosSemanal = result.data.cidadesVM;
            $scope.regioesForn = result.data.regiaoPrazoFornecedorVM;
            carregaPrazoSemanal(result.data.regiaoSemanalVM);

        }

        function carregaRegiaoPrazoLoadFailed(result) {

            notificationService.displayError(result.data);
        }
        //29------------------------------------------------------------------



        //30-----Carrega lista Cidade por Região------------------------------
        function loadCidadePorRegiaoFornecedor(estadoId, regiaoId) {

            if (regiaoId == undefined) {
                regiaoId = 0;
            }

            var config = {
                params: {
                    EstadoId: estadoId,
                    RegiaoId: regiaoId,
                    FornecedorId: $scope.novoFornecedor.Id
                }
            };
            apiService.get('/api/endereco/fornecedorRegiaoPrazo', config,
                loadCidadePorRegiaoFornecedorCompleted,
                loadCidadePorRegiaoFornecedorFailed);
        }

        function loadCidadePorRegiaoFornecedorCompleted(response) {

            $scope.fornRegiaoCidades = response.data;

            if ($scope.fornRegiaoCidades.length === 0) {
                $scope.novoPrazoForn.Prazo = '';
            }

            loadFornecedorRegiao();
        }

        function loadCidadePorRegiaoFornecedorFailed(response) {
            notificationService.displayError(response.data);
        }


        function loadCidadePorRegiaoFornecedorSemanal(estadoId, regiaoId) {
            $scope.novoPrazoFornSemanal.CidadeId = undefined;
            if (regiaoId == undefined) {
                regiaoId = 0;
            }

            var config = {
                params: {
                    EstadoId: estadoId,
                    RegiaoId: regiaoId,
                    FornecedorId: $scope.novoFornecedor.Id
                }
            };

            apiService.get('/api/endereco/fornecedorRegiaoPrazoSemanal', config,
                loadCidadePorRegiaoFornecedorSemanalCompleted,
                loadCidadePorRegiaoFornecedorSemanalFailed);
        }

        function loadCidadePorRegiaoFornecedorSemanalCompleted(response) {
            $scope.fornRegiaoCidadesSemanal = response.data;
        }

        function loadCidadePorRegiaoFornecedorSemanalFailed(response) {
            notificationService.displayError(response.data);
        }

        //30------------------------fim---------------------------------------


        //31----------Mostra Checks Prazos------------------------------------
        function habilitaDesabilitaComboPrazoSemana() {
            $scope.escolhaPrazo.DiasCorridos = false;
            $scope.escolhaPrazo.DiasSemana = true;

        }

        function habilitaDesabilitaComboPrazoCorrido() {
            $scope.escolhaPrazo.DiasSemana = false;
            $scope.escolhaPrazo.DiasCorridos = true;

        }

        function carregaAbaPrazoEntrega() {

            if ($scope.arrayFornecedorPrazoSemanal.length > 0) {
                habilitaDesabilitaComboPrazoSemana();
                $scope.novoPrazoFornSemanal = {};
                $scope.novoPrazoForn = {};
                $scope.regioes = [];
                $scope.fornRegiaoCidadesSemanal = [];
                loadEstado();
            } else {
                if ($scope.regioesForn.length > 0) {
                    habilitaDesabilitaComboPrazoCorrido();
                    $scope.novoPrazoFornSemanal = {};
                    $scope.novoPrazoForn = {};
                    $scope.regioes = [];
                    $scope.fornRegiaoCidadesSemanal = [];
                    loadEstado();
                } else {
                    habilitaDesabilitaComboPrazoSemana();
                    $scope.novoPrazoFornSemanal = {};
                    $scope.novoPrazoForn = {};
                    $scope.regioes = [];
                    $scope.fornRegiaoCidadesSemanal = [];
                    loadEstado();
                }

            }
        }
        //31------------------------------------------------------------------


        //32------------------------------------------------------------------
        function inserirPrazoDiasSemana(semanaId, premissa) {

            var contador = 0;

            if ($scope.novoPrazoFornSemanal === undefined || $scope.novoPrazoFornSemanal.CidadeId === undefined) {
                notificationService.displayInfo('Selecione uma Cidade !');
                limpaArrayPrazoSemanaChanged();

            } else {

                var objPrazoSemana = {
                    FornecedorId: $scope.novoFornecedor.Id,
                    CidadeId: $scope.novoPrazoFornSemanal.CidadeId,
                    DiaSemana: semanaId
                };

                if (premissa) {

                    if ($scope.arrayPrazoSemana.length === 0) {
                        $scope.arrayPrazoSemana.push(objPrazoSemana);
                    } else {

                        for (var i = 0; i < $scope.arrayPrazoSemana.length; i++) {

                            for (var j = 0; j < $scope.arrayPrazoSemana.length; j++) {

                                if ($scope.arrayPrazoSemana[j].DiaSemana === objPrazoSemana.DiaSemana) {
                                    contador++;
                                }
                            }

                            if ($scope.arrayPrazoSemana[i].FornecedorId === objPrazoSemana.FornecedorId &&
                                $scope.arrayPrazoSemana[i].CidadeId === objPrazoSemana.CidadeId && contador === 0) {

                                $scope.arrayPrazoSemana.push(objPrazoSemana);
                            }
                        }
                    }
                } else {

                    for (var k = 0; k < $scope.arrayPrazoSemana.length; k++) {

                        if ($scope.arrayPrazoSemana[k].DiaSemana === objPrazoSemana.DiaSemana) {

                            var index = $scope.arrayPrazoSemana.indexOf($scope.arrayPrazoSemana[k]);
                            $scope.arrayPrazoSemana.splice(index, 1);
                        }
                    }
                }
            }


        }

        function carregaPrazoSemanal(prazos) {

            $scope.PrazoSemanal = prazos;
            $scope.arrayFornecedorPrazoSemanal = [];

            for (var k = 0; k < $scope.PrazoSemanal.length; k++) {

                for (var l = 0; l < $scope.PrazoSemanal[k].DiaSemana.length; l++) {

                    for (var j = 0; j < $scope.arrayDiasSemana.length; j++) {

                        if ($scope.PrazoSemanal[k].DiaSemana[l] === $scope.arrayDiasSemana[j].Id) {

                            var objPrazoSemana = {
                                DescDiaSemana: $scope.arrayDiasSemana[j].DescDiaSemana,
                                CidadeId: $scope.PrazoSemanal[k].IdCidade,
                                DiaSemana: $scope.arrayDiasSemana[j].Id
                            };

                            $scope.arrayFornecedorPrazoSemanal.push(objPrazoSemana);
                        }
                    }

                }
            }



        }

        //32------------------------------------------------------------------


        //33----------------Limpando Arrays Prazo Semanal---------------------
        function limpaPrazoSemanal() {
            $scope.novoPrazoForn = {};
            $scope.novoPrazoFornSemanal = {};
            $scope.arrayPrazoSemana = [];
            $scope.estados = [];
            $scope.regioes = [];
            $scope.fornRegiaoCidades = [];
            $scope.fornRegiaoCidadesSemanal = [];
            loadEstado();
            $scope.arrayDiasSemana = [];
            $scope.arrayDiasSemana = [
                { Id: 1, DescDiaSemana: 'Segunda' },
                { Id: 2, DescDiaSemana: 'Terça' },
                { Id: 3, DescDiaSemana: 'Quarta' },
                { Id: 4, DescDiaSemana: 'Quinta' },
                { Id: 5, DescDiaSemana: 'Sexta' },
                { Id: 6, DescDiaSemana: 'Sábado' },
                { Id: 7, DescDiaSemana: 'Domingo' }
            ];
        }

        function limpaArrayPrazoSemanaChanged() {
            $scope.arrayPrazoSemana = [];
            $scope.arrayDiasSemana = [];
            $scope.arrayDiasSemana = [
                { Id: 1, DescDiaSemana: 'Segunda' },
                { Id: 2, DescDiaSemana: 'Terça' },
                { Id: 3, DescDiaSemana: 'Quarta' },
                { Id: 4, DescDiaSemana: 'Quinta' },
                { Id: 5, DescDiaSemana: 'Sexta' },
                { Id: 6, DescDiaSemana: 'Sábado' },
                { Id: 7, DescDiaSemana: 'Domingo' }
            ];
        }
        //33------------------------------------------------------------------

        pesquisarFornecedor();
        loadLogradouro();
        loadEstado();
        loadFormaPagto();
        loadSegmentos();

    }

})(angular.module('ECCAdm'));
