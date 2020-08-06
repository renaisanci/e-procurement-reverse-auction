
(function (app) {
    'use strict';

    app.controller('fornecedorCtrl', fornecedorCtrl);

    fornecedorCtrl.$inject = ['$scope', 'apiService', '$rootScope', 'notificationService', '$modal'];

    function fornecedorCtrl($scope, apiService, $rootScope, notificationService, $modal) {

        $scope.pageClass = 'page-fornecedor';

        $scope.pesquisarNovoFornecedor = pesquisarNovoFornecedor;
        $scope.detalhesFornecedor = detalhesFornecedor;
        $scope.franquiaSolicitaFornecedor = franquiaSolicitaFornecedor;
        $scope.loadRegiao = loadRegiao;
        $scope.loadCidadePorRegiao = loadCidadePorRegiao;
        $scope.close = close;

        $scope.estados = [];
        $scope.fornCidades = [];
        $scope.regioes = [];
        $scope.novoFornecedor = [];
        $scope.fornecedores = [];
        $scope.cidades = [];
        var idFornecedoresInserir = [];
        var idFornecedoresDeletar = [];
        $scope.allReg = false;
        
        $scope.arrayDiasSemana = [
            { Id: 1, DescSemana: 'Seg' },
            { Id: 2, DescSemana: 'Ter' },
            { Id: 3, DescSemana: 'Qua' },
            { Id: 4, DescSemana: 'Qui' },
            { Id: 5, DescSemana: 'Sex' },
            { Id: 6, DescSemana: 'Sáb' },
            { Id: 7, DescSemana: 'Dom' }
        ];

        //1-------------------Pesquisar Fornecedor-------------------------
        function pesquisarNovoFornecedor(page) {

            page = page || 0;
            $scope.CidadeId = $scope.CidadeId || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    cidadeId: $scope.CidadeId,
                    filter: $scope.filtronovofornecedor
                }
            };

            apiService.get('/api/franquia/carregaFornecedoresFranquia', config,
                fornecedorLoadCompleted,
                fornecedorLoadFailed);
        }

        function fornecedorLoadCompleted(result) {

            $scope.fornecedores = result.data.Items;
            var contador = 0;

            if ($scope.fornecedores.length > 0) {
                for (var i = 0; i < $scope.fornecedores.length; i++) {
                    if ($scope.fornecedores[i].FornecedorPrazoSemanal.length > 0) {
                        for (var j = 0; j < $scope.fornecedores[i].FornecedorPrazoSemanal.length; j++) {
                            for (var k = 0; k < $scope.arrayDiasSemana.length; k++) {
                                if ($scope.fornecedores[i].FornecedorPrazoSemanal[j].DiaSemana === $scope.arrayDiasSemana[k].Id) {
                                    $scope.fornecedores[i].FornecedorPrazoSemanal[j].DescDiaSemana = $scope.arrayDiasSemana[k].DescSemana;
                                }
                            }
                        }
                    }

                    if ($scope.fornecedores[i].TrabalhaFranquia) {
                        $scope.fornecedores[i].selected = true;
                        $scope.fornecedores[i].Relacionado = true;
                        contador++;
                    } else {
                        $scope.fornecedores[i].selected = false;
                        $scope.fornecedores[i].Relacionado = false;
                    }
                }
            }

            if ($scope.fornecedores.length === contador && $scope.fornecedores.length > 0) {
                $scope.allReg = true;
            } else {
                $scope.allReg = false;
            }

            $scope.pagina = result.data.Page;
            $scope.pagesContador = result.data.TotalPages;
            $scope.totalPaginas = result.data.TotalCount;
        }

        function fornecedorLoadFailed(result) {

            notificationService.displayError(result.data.ExceptionMessage);
        }
        //1----------------------------------------------------------------


        //2--------------------Membro Solicita Fornecedor-------------------
        function franquiaSolicitaFornecedor(fornecedores) {

            apiService.post('/api/franquia/inserirDeletarFornecedorFranquia', fornecedores,
                inserirFranquiaFornecedorsSucesso,
                inserirFranquiaFornecedorFalha);
        }

        function inserirFranquiaFornecedorsSucesso(result) {
            pesquisarNovoFornecedor();
        }

        function inserirFranquiaFornecedorFalha(result) {
            notificationService.displayError(result.data);
        }
        //2-----------------------------------------------------------------


        //3-----------------Carregar Estados--------------------------------
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
        //3-----------------------------------------------------------------


        //4-----Carrega dropdown Região --
        function loadRegiao(estadoId) {

            loadCidade(estadoId);

            var config = {
                params: {
                    EstadoId: estadoId
                }
            };

            apiService.get('/api/endereco/regiao', config,
                    loadRegiaoCompleted,
                    loadRegiaoFailed);


        }

        function loadRegiaoCompleted(response) {

            $scope.regioes = response.data;

        }

        function loadRegiaoFailed(response) {
            notificationService.displayError(response.data);
        }
        //4------------------------fim-------------------------------------


        //5---------------Carrega dropdown Cidade---------------------------
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
        //5-----------------------------------------------------------------


        //6-------------Carrega lista Cidades por Região--------------------
        function loadCidadePorRegiao(estadoId, regiaoId) {

            if (regiaoId == undefined) {
                regiaoId = 0;
            }

            var config = {
                params: {
                    EstadoId: estadoId,
                    RegiaoId: regiaoId
                }
            };

            apiService.get('/api/endereco/cidadeporregiao', config,
                    loadCidadePorRegiaoCompleted,
                    loadCidadePorRegiaoFailed);
        }

        function loadCidadePorRegiaoCompleted(response) {

            $scope.cidades = response.data;
        }

        function loadCidadePorRegiaoFailed(response) {
            notificationService.displayError(response.data);
        }
        //6------------------------fim-------------------------------------
        
        //7------------Detalhes do Fornecedor-------------------------------
        function detalhesFornecedor(fornecedor) {

           $scope.modal = $modal.open({
                templateUrl: 'scripts/SPAFranquia/fornecedor/modalDetalhesFornecedor.html',
                controller: '',
                //backdrop: 'static',
                scope: $scope,
                size: ''
            });

            $scope.fornecedor = fornecedor;

        }
        //7-----------------------------------------------------------------

        //--------------------Fechar Modal----------------------------------
        function close() {
            $scope.modal.close();
        }
        //------------------------------------------------------------------

        //checkedall-------------------------------------------------------
        $scope.checkBoxAll = function (itens, chk) {

            idFornecedoresInserir = [];
            idFornecedoresDeletar = [];

            if (chk) {
                chk = true;
            } else {
                chk = false;
            }

            angular.forEach(itens, function (item) {
                item.selected = chk;
            });

            angular.forEach(itens, function (item) {

                if (item.Relacionado && !item.selected) {
                    idFornecedoresDeletar.push(item.FornecedorId);
                }
                if (!item.Relacionado && item.selected) {
                    idFornecedoresInserir.push(item.FornecedorId);

                }
            });

            var arrays = idFornecedoresDeletar.concat(idFornecedoresInserir);

            franquiaSolicitaFornecedor(arrays);

        }
        //-----------------------------------------------------------------

        loadEstado();
        pesquisarNovoFornecedor();
    }

})(angular.module('ECCFranquia'));