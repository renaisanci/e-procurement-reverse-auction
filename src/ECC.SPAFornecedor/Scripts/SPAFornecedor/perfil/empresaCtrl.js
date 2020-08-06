
(function (app) {
    'use strict';

    app.controller('empresaCtrl', empresaCtrl);

    empresaCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', 'fornecedorUtilService'];

    function empresaCtrl($scope, $timeout, apiService, notificationService, fornecedorUtilService) {

        $scope.novoFornecedor = {};
        $scope.logradouros = [];
        $scope.estados = [];
        $scope.cidades = [];
        $scope.bairros = [];
        $scope.formaPagtos = [];

        $scope.loadCidadeChange = loadCidade;
        $scope.loadBairroChange = loadBairro;
        $scope.loadEnderecoCep = loadEnderecoCep;
        $scope.atualizarFornecedor = atualizarFornecedor;
        $scope.openDatePicker = openDatePicker;

        //-----Carregar dados do Fornecedor------------

        function carregarFornecedor() {
            apiService.get('/api/fornecedor/perfil', null,
                carregarMembroSucesso,
                carregarMembroFalha);
        }

        function carregarMembroSucesso(response) {

            $scope.novoFornecedor = response.data;

            loadCidade($scope.novoFornecedor.Endereco.EstadoId);

            loadBairro($scope.novoFornecedor.Endereco.CidadeId);

            checaFormaPagto($scope.novoFornecedor.FormaPagtos);
        }

        function carregarMembroFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Carregar dados do Fornecedor------------

        //-----Atualiza dados Fornecedor aba Membro------------

        function atualizarFornecedor() {
            $scope.novoFornecedor.Cnpj = $scope.novoFornecedor.Cnpj.split(".").join("").split("/").join("").split("-").join("");
            $scope.novoFornecedor.DddTelComl = $scope.novoFornecedor.DddTelComl.split("(").join("").split(")").join("");
            $scope.novoFornecedor.TelefoneComl = $scope.novoFornecedor.TelefoneComl.split("-").join("");
            $scope.novoFornecedor.DddCel = $scope.novoFornecedor.DddCel.split("(").join("").split(")").join("");
            $scope.novoFornecedor.Celular = $scope.novoFornecedor.Celular.split("-").join("");
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
                    formaPagto.Desconto = '';
                }


            });

            angular.forEach($scope.formaPagtosParcelado, function (formaPagto) {
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
            checaFormaPagto($scope.novoFornecedor.FormaPagtos);

        }

        function atualizarFornecedorFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Atualiza dados membro aba Membro------------

        //-----Date Piker------------

        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.opened = true;
        };

        $scope.datepicker = {};
        $scope.format = 'dd/MM/yyyy';
        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };

        //-----Date Piker------------

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

        //4-----Carrega dropdown Estado--
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


                if (response.data.Endereco == "" || response.data.Endereco == null) {
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
            $scope.cidades = response.data;
        }

        function loadCidadeFailed(response) {
            notificationService.displayError(response.data);
        }
        //6------------------------fim-----------------------------

        //7-----Carrega dropdown Bairro --
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
            $scope.formaPagtosAvista = response.data.formaPagtosAvistaVM;
            $scope.formaPagtosParcelado = response.data.formaPagtosParceladoVM;
        }

        function loadFormaPagtoFailed(response) {
            notificationService.displayError(response.data);
        }
        //8------------------------fim-----------------------------

        //10-----Chaca formas de pagamentos ja cadastrada----------------------
        function checaFormaPagto(formaPagtoCadastrada) {

            for (var i = 0; i < $scope.formaPagtosAvista.length; i++) {
                for (var j = 0; j < formaPagtoCadastrada.length; j++) {
                    if ($scope.formaPagtosAvista[i].Id === formaPagtoCadastrada[j].FormaPagtoId &&
                        formaPagtoCadastrada[j].Ativo)
                    {
                        $scope.formaPagtosAvista[i].Relacionado = true;
                        $scope.formaPagtosAvista[i].selected = true;
                        $scope.formaPagtosAvista[i].Desconto = formaPagtoCadastrada[j].Desconto;
                    }
                }
            }

            for (var k = 0; k < $scope.formaPagtosParcelado.length; k++) {
                for (var x = 0; x < formaPagtoCadastrada.length; x++) {
                    if ($scope.formaPagtosParcelado[k].Id === formaPagtoCadastrada[x].FormaPagtoId &&
                        formaPagtoCadastrada[x].Ativo)
                    {
                        $scope.formaPagtosParcelado[k].Relacionado = true;
                        $scope.formaPagtosParcelado[k].selected = true;
                        $scope.formaPagtosParcelado[k].VlFormaPagto = formaPagtoCadastrada[x].VlFormaPagto;
                        $scope.formaPagtosParcelado[k].QtdParcelas = formaPagtoCadastrada[x].QtdParcelas;
                        $scope.formaPagtosParcelado[k].ValorMinParcela = formaPagtoCadastrada[x].ValorMinParcela;
                        $scope.formaPagtosParcelado[k].ValorMinParcelaPedido = formaPagtoCadastrada[x].ValorMinParcelaPedido;
                    }
                }
            }
        }
        //10----------------------------Fim----------------------------------------

        loadLogradouro();
        loadEstado();
        loadFormaPagto();
        carregarFornecedor();
    }

})(angular.module('ECCFornecedor'));