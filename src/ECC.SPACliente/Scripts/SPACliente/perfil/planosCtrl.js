(function (app) {
    'use strict';

    app.controller('planosCtrl', planosCtrl);

    planosCtrl.$inject = ['$scope', '$rootScope', '$timeout', 'apiService', 'notificationService', 'SweetAlert', '$location', '$modal'];

    function planosCtrl($scope, $rootScope, $timeout, apiService, notificationService, SweetAlert, $location, $modal) {


        $scope.planosMensalidades = [];
        $scope.inserirPlanoMensalidade = inserirPlanoMensalidade;
        $scope.atualizarTipoPagamento = atualizarTipoPagamento;
        $scope.trocarPlano = trocarPlano;
        $scope.tipoBoleto = null;
        $scope.tipoCartao = null;
        $scope.trocarPlanoMembro = false;
        $scope.existePlanoMembro = false;
        var existeCartaoCadastradoMembro = false;
        var planoMembro = {};
        $scope.planoPagamento = {
            IdPano: 0,
            TipoPagamentoId: 0
        };
        $scope.formaPagamento = {};

        //#region [1 - Retorna Planos Mensalidades]
        function carregarPlanosMensalidade() {
            apiService.get('/api/pagamentos/getPlanosMensalidade', null,
                carregarPlanosMensalidadeSucesso,
                carregarPlanosMensalidadeFalha);
        }

        function carregarPlanosMensalidadeSucesso(response) {

            var meuPlano = false;

            if (response.data.length > 0) {

                for (var i = 0; i < response.data.length; i++) {

                    meuPlano = planoMembro !== null && planoMembro.IdPano == response.data[i].Id && planoMembro.MensalidadePaga;

                    var plano = {
                        Id: response.data[i].Id,
                        Descricao: response.data[i].Descricao,
                        QtdMeses: response.data[i].QtdMeses,
                        Valor: response.data[i].Valor,
                        Ativo: response.data[i].Ativo,

                        Class: response.data[i].Id === 1 || response.data[i].Id === 4 ? 'widget-color-green' :
                            response.data[i].Id === 2 || response.data[i].Id === 5  ? 'widget-color-orange' :
                                response.data[i].Id === 3 || response.data[i].Id === 6 ? 'widget-color-blue' : '',

                        ClassButton: response.data[i].Id === 1 || response.data[i].Id === 4 ? 'btn-success' :
                            response.data[i].Id === 2 || response.data[i].Id === 5 ? 'btn-warning' :
                                response.data[i].Id === 3 || response.data[i].Id === 6 ? 'btn-primary' : '',

                        TipoBoleto: planoMembro !== null && planoMembro.IdPano === response.data[i].Id && planoMembro.TipoPagamentoId === 3, // boleto
                        TipoCartao: planoMembro !== null && planoMembro.IdPano === response.data[i].Id && planoMembro.TipoPagamentoId === 1,  //cartão
                        MensalidadePaga: planoMembro !== null && planoMembro.MensalidadePaga,
                        MeuPlano: meuPlano
                    };

                    $scope.planosMensalidades.push(plano);
                }
            }

        }

        function carregarPlanosMensalidadeFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //#endregion [1 - Fim Retorna Planos Mensalidades]


        //#region [2 - Inserir Plano Mensalidade]
        function inserirPlanoMensalidade(plano) {

            //Credito = 1,
            //Boleto = 3

            if (!existeCartaoCadastradoMembro && $scope.planoPagamento.TipoPagamentoId == 1) {

                notificacaoNaoExistecartaoCadastrado(plano);

                return;
            }

            if ($scope.planoPagamento.TipoPagamentoId == 0) {

                notificationService.displayInfo("Selecione o tipo de pagamento!");

            } else {

                SweetAlert.swal({
                    title: "Atenção!",
                    text: "Deseja realmente escolher este plano e forma de pagamento?",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "SIM",
                    cancelButtonText: "NÃO",
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                    function (isConfirm) {

                        if (isConfirm) {

                            // Cartão
                            if ($scope.planoPagamento.TipoPagamentoId === 1) {

                                $modal.open({
                                    templateUrl: 'scripts/SPACliente/pagamento/confirmaCartaoModal.html',
                                    controller: 'confirmaCartaoCtrl',
                                    scope: $scope,
                                    size: 'lg'
                                }).result.then(function ($scope) {
                                    //console.log("Modal Closed!!!");
                                }, function () {
                                    //console.log("Modal Dismissed!!!");
                                });
                            }

                            // Boleto
                            if ($scope.planoPagamento.TipoPagamentoId === 3) {

                                apiService.post('/api/pagamentos/atualizarPlano/' +
                                    $scope.planoPagamento.IdPano +
                                    '/' +
                                    $scope.planoPagamento.TipoPagamentoId +
                                    '/' +
                                    $scope.trocarPlanoMembro,
                                    null,
                                    inserirPlanoMensalidadeSucesso,
                                    inserirPlanoMensalidadeFalha);
                            }
                        }
                    });
            }


        }

        function inserirPlanoMensalidadeSucesso(response) {

            if ($scope.trocarPlanoMembro) {

                SweetAlert.swal({
                    title: "PLANO TROCADO!",
                    text: "Este plano ficará ativo em " + response.data.dataTrocaPlano,
                    type: "success",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK",
                    closeOnConfirm: true

                });

                verificaExisteCartaoCadastradoMembro();

            } else {
                SweetAlert.swal({
                    title: "PLANO CONFIRMADO!",
                    text: "Após o pagamento do boleto, aguarde 3 dias úteis para compensar o pagamento, \n" +
                          "ou envie o comprovante para ''comprovantes@economizaja.com.br'' para liberação imediata.\n"
                        + "Sejá Bem-Vindo a EconomizaJá",
                    type: "success",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK",
                    closeOnConfirm: true

                });

                $location.path('/pagamento/pendente');
            }


        }

        function inserirPlanoMensalidadeFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //#endregion [2 - Fim Inserir Plano Mensalidade]

        //#region [3 - Atualizar Tipo Pagamento]
        function atualizarTipoPagamento(idPag, plano) {

            //Credito = 1,
            //Boleto = 3

            $scope.planoPagamento.TipoPagamentoId = idPag;
            $scope.planoPagamento.IdPano = plano.Id;
        }
        //#endregion [2 - Fim Atualizar Tipo Pagamento]

        //#region [4 - Verifica se existe cartão cadastrado]
        function verificaExisteCartaoCadastradoMembro() {
            apiService.get('/api/cartaocredito/existeCartaoCadastradoMembro', null,
                verificaExisteCartaoCadastradoMembroSucesso,
                verificaExisteCartaoCadastradoMembroFalha);
        }

        function verificaExisteCartaoCadastradoMembroSucesso(response) {
            $scope.planosMensalidades = [];
            $scope.formaPagamento = response.data.cartao;
            existeCartaoCadastradoMembro = response.data.cartao !== null;
            planoMembro = response.data.planoMembro;
            $scope.existePlanoMembro = planoMembro.MensalidadePaga;

            carregarPlanosMensalidade();

        }

        function verificaExisteCartaoCadastradoMembroFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //#endregion [4 - Verifica se existe cartão cadastrado]


        //#region [5 - Evento Reload Cartões Crédito - Pop'up addCartaoCtrl.js]        $rootScope.$on("reloadPlanos", function () {
            verificaExisteCartaoCadastradoMembro();
        });        //#endregion 5 - Evento Reload Cartões Crédito


        // #region [6 - Trocar Plano]
        function trocarPlano() {
            $scope.trocarPlanoMembro = true;

            angular.forEach($scope.planosMensalidades, function (value, key) {
                value.MensalidadePaga = false;
                value.TipoBoleto = false;
                value.TipoCartao = false;
                value.MeuPlano = false;
            });
        }
        //#endregion [6 - Fim Troca de Plano]

        function notificacaoNaoExistecartaoCadastrado(plano) {

            SweetAlert.swal({
                title: "Atenção!",
                text: "Não existe um cartão de crédito cadastrado para escolher está forma de pagamento.\n" +
                    "Cique em ''OK'' para ser direcionado para o cadastro!",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "OK",
                cancelButtonText: "FECHAR",
                closeOnConfirm: true,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {
                        $location.path('/pagamento/formapgto');
                    }
                    else {
                        plano.tipoCartao = false;
                    }
                });

        }


        verificaExisteCartaoCadastradoMembro();
    }

})(angular.module('ECCCliente'));