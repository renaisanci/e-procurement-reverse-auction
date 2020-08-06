
(function (app) {
    'use strict';

    app.controller('pagamentoCtrl', pagamentoCtrl);

    pagamentoCtrl.$inject = ['$scope', '$location', '$timeout', 'apiService', 'notificationService', 'SweetAlert'];

    function pagamentoCtrl($scope, $location, $timeout, apiService, notificationService, SweetAlert) {

        $scope.Fatura = {};
        $scope.FaturaAnterior = {};
        $scope.FaturaAtual = {};
        $scope.FaturaProxima = {};
        $scope.gerarBoletoShow = false;

        $scope.habilitaAnterior = habilitaAnterior;
        $scope.habilitaAtual = habilitaAtual;
        $scope.habilitaProxima = habilitaProxima;
        $scope.gerarBoleto = gerarBoleto;

        $scope.tabsFatura = {
            tabAnterior: {
                tabAtivar: '',
                tabHabilitar: false,
                contAtivar: 'tab-pane fade',
                contHabilitar: true
            },
            tabAtual: {
                tabAtivar: '',
                tabHabilitar: false,
                contAtivar: 'tab-pane fade',
                contHabilitar: true
            },
            tabProxima: {
                tabAtivar: '',
                tabHabilitar: false,
                contAtivar: 'tab-pane fade',
                contHabilitar: true
            }
        };

        function habilitaAnterior() {
            $scope.Fatura = $scope.FaturaAnterior;
            $scope.gerarBoletoShow = $scope.Fatura.StatusRecebimento == 2;
        }

        function habilitaAtual() {
            $scope.Fatura = $scope.FaturaAtual;
            $scope.gerarBoletoShow = $scope.Fatura.StatusRecebimento == 2;
        }

        function habilitaProxima() {
            $scope.Fatura = $scope.FaturaProxima;
            $scope.gerarBoletoShow = $scope.Fatura.StatusRecebimento == 2;
        }

        //-----Carregar Fatura------------

        function carregarFatura() {
            apiService.get('/api/pagamento/fatura', null,
                carregarFaturaSucesso,
                carregarFaturaFalha);
        }

        function carregarFaturaSucesso(response) {
            $scope.FaturaAnterior = response.data.Anterior;
            $scope.FaturaAtual = response.data.Atual;
            $scope.FaturaProxima = response.data.Proxima;

            $scope.tabsFatura.tabAnterior.tabHabilitar = $scope.FaturaAnterior != undefined;
            $scope.tabsFatura.tabAtual.tabHabilitar = $scope.FaturaAtual != undefined;
            $scope.tabsFatura.tabProxima.tabHabilitar = $scope.FaturaProxima != undefined;

            if ($scope.FaturaAtual != undefined && $scope.FaturaAtual.DtVencimento < Date.now()) {
                habilitaAtual();
                $scope.tabsFatura.tabAtual.tabAtivar = 'active';
                $scope.tabsFatura.tabAtual.contAtivar = 'tab-pane fade in active';
            } else if ($scope.FaturaProxima != undefined) {
                habilitaProxima();
                $scope.tabsFatura.tabProxima.tabAtivar = 'active';
                $scope.tabsFatura.tabProxima.contAtivar = 'tab-pane fade in active';
            } else {
                SweetAlert.swal({
                    title: "NENHUMA FATURA FOI GERADA!",
                    text: "É necessário ter pedidos aprovados para poder gerar comissões.", //VALIDAR FRASE
                    type: "success",
                    html: true
                });
            }
        }

        function carregarFaturaFalha(response) {
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //-----Carregar Fatura------------

        //-----Gerar Boleto------------

        function gerarBoleto(fatura) {
            
        }

        //-----Gerar Boleto------------

        carregarFatura();
    }

})(angular.module('ECCFranquia'));