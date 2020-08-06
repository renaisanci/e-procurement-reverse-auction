(function (app) {
    'use strict';

    app.controller('fornecedorCtrl', fornecedorCtrl);

    fornecedorCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'SweetAlert', '$modal'];

    function fornecedorCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, SweetAlert, $modal) {

        $scope.pageClass = 'page-fornecedor';

        $scope.pesquisarNovoFornecedor = pesquisarNovoFornecedor;
        $scope.detalhesFornecedor = detalhesFornecedor;
        $scope.habilitaDesabilitaAbas = habilitaDesabilitaAbas;
        $scope.habilitaDesabilitaAbasRespostaAceiteFornecedor = habilitaDesabilitaAbasRespostaAceiteFornecedor;
        $scope.cancelarNovoFornecedor = cancelarNovoFornecedor;
        $scope.membroSolicitaFornecedor = membroSolicitaFornecedor;
        $scope.pesquisarRespostaFornecedor = pesquisarRespostaFornecedor;
        $scope.detalhesRecusaFornecedor = detalhesRecusaFornecedor;
        $scope.membroCancelaFornecedor = membroCancelaFornecedor;

        $scope.novoFornecedor = [];
        $scope.respostaFornecedor = [];
        var resultadoRecusa = [];
        var resultadoMembroFornecedor = [];
        var membroFornecedor = [];

        $scope.arrayDiasSemana = [
        { Id: 1, DescSemana: 'Seg' },
        { Id: 2, DescSemana: 'Ter' },
        { Id: 3, DescSemana: 'Qua' },
        { Id: 4, DescSemana: 'Qui' },
        { Id: 5, DescSemana: 'Sex' },
        { Id: 6, DescSemana: 'Sáb' },
        { Id: 7, DescSemana: 'Dom' }
        ];


        //0--------Declaracao de todas as abas de tela de novo fornecedor--------
        $scope.tabsFornecedor = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadFornecedor: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            },
            tabRespostaAceiteFornecedor: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }
        };
        //0------------------------Fim--------------------------------

        function habilitaDesabilitaAbas() {
            $scope.tabsFornecedor = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabRespostaAceiteFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }

        function habilitaDesabilitaAbasRespostaAceiteFornecedor() {
            $scope.tabsFornecedor = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabRespostaAceiteFornecedor: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };

            pesquisarRespostaFornecedor();
        }

        function cancelarNovoFornecedor() {
            pesquisarNovoFornecedor(0);

            $scope.tabsFornecedor = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                },
                tabRespostaAceiteFornecedor: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }

        function detalhesFornecedor(fornecedor) {

            $scope.novoSolicitaFornecedor = fornecedor;

            $scope.tabsFornecedor = {
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
                }
            };
        }


        //1-------------------Pesquisar Fornecedor------------------------
        function pesquisarNovoFornecedor(page) {

            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtronovofornecedor
                }
            };

            apiService.get('/api/membro/pesquisarfornecedor', config,
                fornecedorLoadCompleted,
                fornecedorLoadFailed);
        }

        function fornecedorLoadCompleted(result) {

            $scope.novoFornecedor = result.data.Items;

            if ($scope.novoFornecedor.length > 0) {
                for (var i = 0; i < $scope.novoFornecedor.length; i++) {
                    if ($scope.novoFornecedor[i].FornecedorPrazoSemanal.length > 0) {
                        for (var j = 0; j < $scope.novoFornecedor[i].FornecedorPrazoSemanal.length; j++) {
                            for (var k = 0; k < $scope.arrayDiasSemana.length; k++) {
                                if ($scope.novoFornecedor[i].FornecedorPrazoSemanal[j].DiaSemana === $scope.arrayDiasSemana[k].Id) {
                                    $scope.novoFornecedor[i].FornecedorPrazoSemanal[j].DescDiaSemana = $scope.arrayDiasSemana[k].DescSemana;
                                }
                            }
                        }
                    }
                }
            }

          

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
        }

        function fornecedorLoadFailed(result) {

            notificationService.displayError(result.data.ExceptionMessage);
        }
        //1--------------------------------------------------------------


        //2--------------------Membro Solicita Fornecedor-------------------
        function membroSolicitaFornecedor() {            

            var fornecedor = $scope.novoSolicitaFornecedor;

            apiService.post('/api/membro/inserirMembroFornecedorTelaMembro/' + 0 + '/' + fornecedor.FornecedorId, null,
                inserirMembroFornecedorsSucesso,
                inserirMembroFornecedorsFalha);
        }

        function inserirMembroFornecedorsSucesso(result) {

            SweetAlert.swal({
                title: "SOLICITAÇÃO ENVIADA COM SUCESSO!",
                text: "Aguarde a resposta do fornecedor, você será notificado por E-MAIL e SMS.",
                type: "warning",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            }, function () {

                pesquisarNovoFornecedor(0);

                $scope.tabsFornecedor = {
                    tabPesquisar: {
                        tabAtivar: "active",
                        tabhabilitar: true,
                        contentAtivar: "tab-pane fade in active",
                        contentHabilitar: true
                    },
                    tabCadFornecedor: {
                        tabAtivar: "",
                        tabhabilitar: false,
                        contentAtivar: "tab-pane fade",
                        contentHabilitar: false
                    },
                    tabRespostaAceiteFornecedor: {
                        tabAtivar: "",
                        tabhabilitar: true,
                        contentAtivar: "tab-pane fade",
                        contentHabilitar: true
                    }
                }
            });
        }

        function inserirMembroFornecedorsFalha(result) {
            if (result.status === 304) {
                SweetAlert.swal({
                    title: "ATENÇÃO!",
                    text:"Já existe uma solicitação pendente de aprovação para este fonecedor, aguarde sua aprovação!",
                    type: "warning",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Ok",
                    closeOnConfirm: true
                });
            } else {
                notificationService.displayError(result.statusText);
            }

           
        }
        //2-----------------------------------------------------------------


        //2--------------------Membro Solicita cancelamento Fornecedor-------------------
        function membroCancelaFornecedor() {

            var fornecedor = $scope.novoSolicitaFornecedor;

            SweetAlert.swal({
                title: "ATENÇÃO",
                text: "Ao cancelar este fornecedor ele não irá mais receber seus pedidos, deseja confirmar?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "SIM!",
                cancelButtonText: "NÃO",
                closeOnConfirm: false,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {
                        apiService.post('/api/membro/cancelarMembroFornecedorTelaMembro/' + fornecedor.FornecedorId, null,
                            cancelarMembroFornecedorsSucesso,
                            cancelarMembroFornecedorsFalha);
                    }
                });
        }

        function cancelarMembroFornecedorsSucesso(result) {

            SweetAlert.swal({
                title: "CANCELAMENTO EFETUADO COM SUCESO!",
                text: "O fornecedor não receberá mais os seus pedidos.\nCaso queira adicioná-lo novamente,\nserá necessário efetuar uma nova solicitação para trabalhar com o mesmo!",
                type: "warning",
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Ok"
            }, function () {

                pesquisarNovoFornecedor(0);

                $scope.tabsFornecedor = {
                    tabPesquisar: {
                        tabAtivar: "active",
                        tabhabilitar: true,
                        contentAtivar: "tab-pane fade in active",
                        contentHabilitar: true
                    },
                    tabCadFornecedor: {
                        tabAtivar: "",
                        tabhabilitar: false,
                        contentAtivar: "tab-pane fade",
                        contentHabilitar: false
                    },
                    tabRespostaAceiteFornecedor: {
                        tabAtivar: "",
                        tabhabilitar: true,
                        contentAtivar: "tab-pane fade",
                        contentHabilitar: true
                    }
                }
            });
        }

        function cancelarMembroFornecedorsFalha(result) {
            notificationService.displayError('Erro ao efetuar o cancelamaneto para o fornecedor!');
        }
        //2-----------------------------------------------------------------


        //3-------------Carregar Recusas dos Fornecedores-------------------
        function pesquisarRespostaFornecedor(page) {

            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtrorespostafornecedor
                }
            };

            apiService.get('/api/membro/pesquisarespostafornecedor', config,
                pesquisarRespostaFornecedorLoadCompleted,
                pesquisarRespostaFornecedorLoadFailed);

        }

        function pesquisarRespostaFornecedorLoadCompleted(result) {

            resultadoRecusa = result.data.Items;

            for (var i = 0; i < resultadoRecusa.length; i++) {
                var observacao = resultadoRecusa[i].Observacao;

                if (observacao === "Aceito") {
                    $scope.Status = "Aceito";
                } else if (observacao === "Aguardando") {
                    $scope.Status = "Aguardando";
                } else {
                    $scope.Status = "Recusado";
                }
                resultadoRecusa[i].Status = $scope.Status;
            }

            $scope.respostaFornecedor = resultadoRecusa;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
        }

        function pesquisarRespostaFornecedorLoadFailed(result) {

            notificationService.displayError('Erro ao pesquisar as respostas dos fornecedores para as suas solicitações!');
        }


        function pesquisarNovoMembroSolicitaFornecedor() {


            apiService.get('/api/membro/pesquisarmembrofornecedor', null,
                membroSolicitaLoadCompleted,
                membroSolicitaLoadFailed);
        }

        function membroSolicitaLoadCompleted(result) {

            resultadoMembroFornecedor = result.data.Items;

            for (var i = 0; i < resultadoRecusa.length; i++) {

                membroFornecedor.push(resultadoRecusa[i]);
            }

            for (var j = 0; j < resultadoMembroFornecedor.length; j++) {

                membroFornecedor.push(resultadoMembroFornecedor[j]);
            }

        }

        function membroSolicitaLoadFailed(result) {

            notificationService.displayError('Erro ao carregar fornecedores');
        }
        //3------------------------------------------------------------


        //4----------------Detalhes Recusa Fornecedor-----------------
        function detalhesRecusaFornecedor(fornecedor) {

            $scope.Fornecedor = fornecedor;

            $modal.open({
                animation: true,
                templateUrl: 'scripts/SPACliente/fornecedor/modalDetalhesFornecedor.html',
                controller: 'modalDetalhesFornecedorCtrl',
                scope: $scope,
                size: 'lg'
            });
        }
        //4-----------------------------------------------------------
        
        pesquisarRespostaFornecedor();
        pesquisarNovoMembroSolicitaFornecedor();
        pesquisarNovoFornecedor(0);
    }

})(angular.module('ECCCliente'));