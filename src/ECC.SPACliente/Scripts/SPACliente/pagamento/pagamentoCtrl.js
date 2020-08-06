(function (app) {
    'use strict';

    app.controller('pagamentoCtrl', pagamentoCtrl);

    pagamentoCtrl.$inject = ['$scope', '$location', '$timeout', 'apiService', 'notificationService', '$modal', 'SweetAlert', '$rootScope', '$stateParams'];

    function pagamentoCtrl($scope, $location, $timeout, apiService, notificationService, $modal, SweetAlert, $rootScope, $stateParams) {

        var paramUrl = $stateParams.id;
        $scope.habilitaPendente = habilitaPendente;
        $scope.habilitaHistorico = habilitaHistorico;
        $scope.habilitaFormaPagamento = habilitaFormaPagamento;
        $scope.loadMensalidadesPendentes = loadMensalidadesPendentes;
        $scope.addCartao = addCartao;
        $scope.removerCartaoCredito = removerCartaoCredito;
        $scope.cancelarPlano = cancelarPlano;

        $scope.mensalidades = [];
        $scope.statusMensalidades = 'pendente';


        //#region [Habilitar Abas]

        $scope.tabsPagamento = {
            tabPendente: {
                tabAtivar: 'active',
                tabHabilitar: true,
                contAtivar: 'tab-pane fade in active',
                contHabilitar: true
            },
            tabHistorico: {
                tabAtivar: '',
                tabHabilitar: true,
                contAtivar: 'tab-pane fade',
                contHabilitar: true
            },
            tabFormaPagamento: {
                tabAtivar: '',
                tabHabilitar: true,
                contAtivar: 'tab-pane fade',
                contHabilitar: true
            }
        };

        function habilitaPendente() {

            $scope.tabsPagamento = {
                tabPendente: {
                    tabAtivar: 'active',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade in active',
                    contHabilitar: true
                },
                tabHistorico: {
                    tabAtivar: '',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade',
                    contHabilitar: true
                },
                tabFormaPagamento: {
                    tabAtivar: '',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade',
                    contHabilitar: true
                }
            };

            $scope.statusMensalidades = 'pendente';
            loadMensalidadesPendentes();

        }

        function habilitaHistorico() {

            $scope.mensalidades = [];

            $scope.tabsPagamento = {
                tabPendente: {
                    tabAtivar: '',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade',
                    contHabilitar: true
                },
                tabHistorico: {
                    tabAtivar: 'active',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade in active',
                    contHabilitar: true
                },
                tabFormaPagamento: {
                    tabAtivar: '',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade',
                    contHabilitar: true
                }
            };

            $scope.statusMensalidades = 'historico';
            loadMensalidadesPendentes();

        }

        function habilitaFormaPagamento() {

            $scope.tabsPagamento = {
                tabPendente: {
                    tabAtivar: '',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade',
                    contHabilitar: true
                },
                tabHistorico: {
                    tabAtivar: '',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade',
                    contHabilitar: true
                },
                tabFormaPagamento: {
                    tabAtivar: 'active',
                    tabHabilitar: true,
                    contAtivar: 'tab-pane fade in active',
                    contHabilitar: true
                }
            };

            loadCartoesCredito();
        }

        //#endregion Habilitar Abas

        //#region [1 - Carrega mensalidades]

        function loadMensalidadesPendentes() {

            apiService.get('/api/pagamentos/mensalidade/' + $scope.statusMensalidades, null,
                mensalidadePendenteLoadCompleted,
                mensalidadePendenteLoadFailed);
        }

        function mensalidadePendenteLoadCompleted(response) {

            $scope.mensalidades = response.data;
            $scope.existeBoleto = false;
            $scope.aguardandoPagamento = false;

            // Credito = 1,
            // Debito = 2,
            // Boleto = 3
            if (response.data.length > 0) {
                for (var i = 0; i < response.data.length; i++) {

                    if (response.data[i].TipoPagamentoId === 3 && !$scope.existeBoleto) {
                        $scope.existeBoleto = true;
                    }

                    if (response.data[i].Status === 3 && !$scope.aguardandoPagamento) {
                        $scope.aguardandoPagamento = true;
                    }

                }
            }

        }

        function mensalidadePendenteLoadFailed(response) {

            notificationService.displayError(response.data);

        }

        //#endregion 1 - Fim Carrega mensalidades

        //#region [2 - Adicionar cartão]

        function addCartao(cartao) {

            $scope.formaPagamento = {};

            if (cartao !== undefined && cartao.Id > 0) {

                $scope.formaPagamento = {

                    Id: cartao.Id,
                    Nome: cartao.Nome,
                    Numero: cartao.Numero,
                    DataVencimento: cartao.DataVencimento,
                    TokenCartaoGerenciaNet: cartao.TokenCartaoGerenciaNet,
                    CartaoBandeiraId: $scope.cartaoBandeiraId,
                    Padrao: cartao.Padrao
                };

            }

            $modal.open({
                templateUrl: 'scripts/SPACliente/pagamento/addCartaoModal.html',
                controller: 'addCartaoCtrl',
                scope: $scope,
                backdrop: 'static',
                size: 'lg'
            }).result.then(function ($scope) {
                //console.log("Modal Closed!!!");
            }, function () {
                //console.log("Modal Dismissed!!!");
            });
        }

        //#endregion 2 - Fim Adicionar cartão        //#region [3 - Remover Cartão]

        function removerCartaoCredito(cartaoId) {

            apiService.post('/api/cartaocredito/removerCartaoCredito/' + cartaoId, null,
                removerCartaoCreditoCompleted,
                removerCartaoCreditoLoadFailed);
        }

        function removerCartaoCreditoCompleted(response) {

            notificationService.displaySuccess("Cartão removido com sucesso!");
            loadCartoesCredito();
        }

        function removerCartaoCreditoLoadFailed(response) {

            notificationService.displayError(response.data);

        }

        //#endregion 3 - Fim Carrega mensalidades        //#region [4 - Carrega Cartões]

        function loadCartoesCredito(param) {

            var cartaoId = param == undefined ? 0 : param.Id;

            apiService.get('/api/cartaocredito/getCartaoCredito/' + cartaoId, null,
                cartoesCreditoCompleted,
                cartoesCreditoFailed);
        }

        function cartoesCreditoCompleted(response) {

            $scope.cartoesCredito = response.data;
            $scope.existeCartaoCredito = $scope.cartoesCredito.length > 0;

        }

        function cartoesCreditoFailed(response) {

            notificationService.displayError(response.data);

        }

        //#endregion 4 - Fim Carrega mensalidades        //#region [5 - Evento Reload Cartões Crédito - Pop'up addCartaoCtrl.js]        $rootScope.$on("reloadCartoesCredito", function () {
            loadCartoesCredito();
        });        //#endregion 5 - Evento Reload Cartões Crédito        //#region [6 - Identifica Ação pela URL]        function identificaAcao() {

            if (paramUrl === "pendente")
                loadMensalidadesPendentes();

            if (paramUrl === "formapgto")
                habilitaFormaPagamento();

        }        //#endregion 6 - Identifica Ação pela URL        //#region [7 - Cancelar Plano]

        function cancelarPlano(planoId) {

            SweetAlert.swal({
                title: "Atenção!",
                text: "Deseja realmente cancelar este plano.",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "SIM",
                cancelButtonText: "NÃO",
                closeOnConfirm: true,
                closeOnCancel: true
            }, function (isConfirm) {

                if (isConfirm) {
                    apiService.post('/api/pagamentos/cancelarPlanoMembro/' + planoId, null,
                        cancelarPlanoCompleted,
                        cancelarPlanoFailed);

                }
            });


        }

        function cancelarPlanoCompleted(response) {

            notificationService.displaySuccess("Plano cancelado com sucesso!");
            loadMensalidadesPendentes();

        }

        function cancelarPlanoFailed(response) {

            notificationService.displayError(response.data);

        }

        //#endregion 7 - Fim Cancelar Plano        identificaAcao();
    }

})(angular.module('ECCCliente'));