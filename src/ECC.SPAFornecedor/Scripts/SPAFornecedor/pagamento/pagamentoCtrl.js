(function (app) {
    'use strict';

    app.controller('pagamentoCtrl', pagamentoCtrl);

    pagamentoCtrl.$inject = ['$scope', '$location', '$timeout', 'apiService', 'notificationService', 'SweetAlert', '$modal', '$sce'];

    function pagamentoCtrl($scope, $location, $timeout, apiService, notificationService, SweetAlert, $modal, $sce) {

        $scope.Fatura = {};
        $scope.FaturaAnterior = {};
        $scope.FaturaAtual = {};

        $scope.gerarBoletoShow = false;
        $scope.imprimirBoletoShow = false;
        $scope.habilitaAnterior = habilitaAnterior;
        $scope.habilitaAtual = habilitaAtual;
        $scope.gerarBoleto = gerarBoleto;
        
        $scope.tabsFatura = {
            tabAnterior: {
                tabAtivar: '',
                tabHabilitar: false,
                contAtivar: 'tab-pane fade',
                contHabilitar: false
            },
            tabAtual: {
                tabAtivar: '',
                tabHabilitar: false,
                contAtivar: 'tab-pane fade',
                contHabilitar: false
            },
            tabBoleto: {
                tabAtivar: '',
                tabHabilitar: false,
                contAtivar: 'tab-pane fade',
                contHabilitar: false
            }
        };
        

        function habilitaAnterior() {


            $scope.tabsFatura.tabAnterior.tabHabilitar = true;
            $scope.tabsFatura.tabAnterior.contHabilitar = true;
            $scope.tabsFatura.tabAnterior.tabAtivar = 'active';
            $scope.tabsFatura.tabAnterior.contAtivar = 'tab-pane fade in active';

            $scope.tabsFatura.tabAtual.tabAtivar = '';
            $scope.tabsFatura.tabAtual.contAtivar = 'tab-pane fade';

            $scope.tabsFatura.tabBoleto.tabAtivar = '';
            $scope.tabsFatura.tabBoleto.contAtivar = 'tab-pane fade';

            $scope.Fatura = $scope.FaturaAnterior;
            $scope.imprimirBoletoShow = $scope.Fatura.StatusRecebimento === 3;
            var dataHoje = new Date();

            if (dataHoje > $scope.dataVencimentoAnterior && $scope.Fatura.StatusRecebimento !== 3) {
                $scope.imprimirBoletoShow = false;
                $scope.gerarBoletoShow = true;
            }

        }

        function habilitaAtual() {

           

            $scope.tabsFatura.tabAtual.tabHabilitar = true;
            $scope.tabsFatura.tabAtual.contHabilitar = true;
            $scope.tabsFatura.tabAtual.tabAtivar = 'active';
            $scope.tabsFatura.tabAtual.contAtivar = 'tab-pane fade in active';

            $scope.tabsFatura.tabAnterior.tabAtivar = '';
            $scope.tabsFatura.tabAnterior.contAtivar = 'tab-pane fade';

            $scope.tabsFatura.tabBoleto.tabAtivar = '';
            $scope.tabsFatura.tabBoleto.contAtivar = 'tab-pane fade';

            $scope.Fatura = $scope.FaturaAtual;
            $scope.gerarBoletoShow = $scope.Fatura.StatusRecebimento === 2;
            $scope.imprimirBoletoShow = false;
        }

       
        //-----Carregar Fatura-----------------------
        function carregarFatura() {
            apiService.get('/api/pagamentos/fatura', null,
                carregarFaturaSucesso,
                carregarFaturaFalha);
        }

        function carregarFaturaSucesso(response) {

            $scope.FaturaAnterior = response.data.Anterior;
            $scope.FaturaAtual = response.data.Atual;

            var dataHoje = new Date();
            var dataVencimentoAtual = new Date();

            if ($scope.FaturaAnterior != null) {
                $scope.dataVencimentoAnterior = new Date($scope.FaturaAnterior.DtVencimento);
            }

            if ($scope.FaturaAtual !== null) {
                dataVencimentoAtual = new Date($scope.FaturaAtual.DtVencimento);
            }

            $scope.tabsFatura.tabAnterior.tabHabilitar = $scope.FaturaAnterior != undefined;
            $scope.tabsFatura.tabAnterior.contHabilitar = $scope.FaturaAnterior != undefined;

            $scope.tabsFatura.tabAtual.tabHabilitar = $scope.FaturaAtual != undefined;
            $scope.tabsFatura.tabAtual.contHabilitar = $scope.FaturaAtual != undefined;



            if ($scope.FaturaAtual != undefined && dataVencimentoAtual > dataHoje) {
                habilitaAtual();
                $scope.tabsFatura.tabAtual.tabAtivar = 'active';
                $scope.tabsFatura.tabAtual.contAtivar = 'tab-pane fade in active';
                $scope.tabsFatura.tabAnterior.tabAtivar = '';
                $scope.tabsFatura.tabAnterior.contAtivar = 'tab-pane fade';

            } else if ($scope.FaturaAnterior != undefined) {
                habilitaAnterior();
                $scope.tabsFatura.tabAnterior.tabAtivar = 'active';
                $scope.tabsFatura.tabAnterior.contAtivar = 'tab-pane fade in active';
                $scope.tabsFatura.tabAtual.tabAtivar = '';
                $scope.tabsFatura.tabAtual.contAtivar = 'tab-pane fade';

            } else {
                SweetAlert.swal({
                    title: "NENHUMA FATURA FOI GERADA!",
                    text: "É necessário ter pedidos aprovados para poder gerar comissões.", //VALIDAR FRASE
                    type: "warning",
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
        //-----Carregar Fatura-----------------------
        

        //-----Gerar Boleto--------------------------
        function gerarBoleto(fatura) {

            apiService.post('/api/pagamentos/gerarboletofornecedor', fatura,
                gerarBoletoLoadCompleted,
                gerarBoletoLoadFailed);
        }

        function gerarBoletoLoadCompleted(result) {

            notificationService.displaySuccess('Boleto gerado com sucesso!');
        }

        function gerarBoletoLoadFailed(result) {

            notificationService.displayError(result.data);

        }
        //-----Gerar Boleto--------------------------
        

        carregarFatura();
    }

})(angular.module('ECCFornecedor'));