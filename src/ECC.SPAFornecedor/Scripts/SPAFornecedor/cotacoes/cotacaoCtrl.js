(function (app) {
    'use strict';

    app.controller('cotacaoCtrl', cotacaoCtrl);

    cotacaoCtrl.$inject = ['$scope', '$rootScope', '$timeout', 'apiService', 'notificationService', '$stateParams', 'SweetAlert', 'Hub', '$location', '$interval','fornecedorUtilService'];

    function cotacaoCtrl($scope, $rootScope, $timeout, apiService, notificationService, $stateParams, SweetAlert, Hub, $location, $interval, fornecedorUtilService) {

        $scope.pageClass = 'page-cotacao';
        $scope.gravarOferta = gravarOferta;
        $scope.refreshPrecos = refreshPrecos;
        // $scope.DtFechamento = undefined;    
        $scope.tempoFinal = tempoFinal;
        $scope.BloquearBotaoEnviarOferta = false;
        $scope.exibirEnviar = false;
        //var promise = $interval(loadCotacao, 480000);


        //pegar data do servidor. aqui esta pegando a data local do computador
        //$scope.dateCotaIni = new Date();
        if ($stateParams.cotacaoSelect != null) {
            sessionStorage.setItem("DtFechamento", $stateParams.cotacaoSelect.DtFechamento);
            sessionStorage.setItem("CotacaoId", $stateParams.cotacaoSelect.CotacaoId);
        }

        $scope.CotacaoId = parseInt(sessionStorage.getItem("CotacaoId"));


        //1-----Carrega cotacao -----
        function loadCotacao() {
            apiService.get('/api/cotacao/CotacaoProdGroup/' + $scope.CotacaoId, null,
                loadCotacaoSucesso,
                loadCotacaoFalha);
        }

        function loadCotacaoSucesso(response) {
            $scope.cotacaoPeds = response.data.cotacaoProdsGroup;
            //console.log($scope.cotacaoPeds);

            var faturaPendente = response.data.faturaFornecedorPendente;
            $scope.DtFechamento = sessionStorage.getItem("DtFechamento");
            $scope.dateCotaNow = response.data.dateCotaNow;

            if (faturaPendente) {
                SweetAlert.swal({
                    title: "ATENÇÃO!",
                    text: "Infelizmente você não poderá participar " +
                        "desta cotação, pois existe fatura pendente de pagamento!\n\n" +
                        "Clique em ''OK'' e imprima o boleto!",
                    type: "warning",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK"
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $location.path('/pagamento');
                        }
                    });
            }
        }

        function loadCotacaoFalha(response) {
            notificationService.displayError(response.data);
        }
        //1------------------------fim-----------------------------

        //16-----Grava oferta dada pelo fornecedor----------
        function gravarOferta() {


            $scope.exibirEnviar = true;

            $scope.itemCotacao = [];
            $scope.itemCotacaoZero = [];
            var precoZero = true;

            angular.forEach($scope.cotacaoPeds, function (cotacaoPed) {

                if ((cotacaoPed.Observacao == null || cotacaoPed.Observacao == undefined || cotacaoPed.Observacao == '' || cotacaoPed.Observacao == ' ') && cotacaoPed.FlgOutraMarca == true && cotacaoPed.PrecoNegociadoUnit !== "0,00") {

                    SweetAlert.swal({
                        title: "Coloque a Marca ou Cor para os itens que permitem Outra Marca",
                        text: "Para os Itens que você pode colocar outra marca, é obrigatório especificar a marca ou cor.",
                        type: "warning",
                        html: true
                    });


                    precoZero = false;
                    $scope.exibirEnviar = false;

                    return;

                } else {

                    if (cotacaoPed.PrecoNegociadoUnit !== "0,00") {
                        $scope.itemCotacao.push(cotacaoPed);


                    } else {
                        $scope.itemCotacaoZero.push(cotacaoPed);
                        if ($scope.itemCotacaoZero.length === $scope.cotacaoPeds.length) {

                            SweetAlert.swal({
                                title: "VOCÊ ESQUECEU DE DAR SEU PREÇO :(",
                                text: "Para enviar seu preço, você precisa preencher ao menos o valor de um produto.",
                                type: "warning",
                                html: true
                            });
                            precoZero = false;
                            $scope.exibirEnviar = false;
                        }
                    }
                }
            });

            if (precoZero)
                apiService.post('/api/cotacao/gravarPrecoForn/' + $scope.CotacaoId, $scope.itemCotacao,
                    gravarOfertaSucesso,
                    gravarOfertaFalha);
        }

        function gravarOfertaSucesso(result) {
            $scope.exibirEnviar = false;
            if (result.data.success) {


                SweetAlert.swal({
                    title: "SEU PREÇO FOI REGISTRADA COM SUCESSO, OBRIGADO!",
                    text: "Fique à vontade para alterar O seu preço enquanto a cotação estiver aberta.",
                    type: "success",
                    html: true
                });                

                $rootScope.$emit("reloadAvisosForn", {});

            }
            if (result.data.cotacaoEncerrada) {
                tempoFinal();
            }

            refreshPrecos();
        }

        function gravarOfertaFalha(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //16------------------------fim-----------------------------

        //------------------Atualização em tempo Real---------------
        $rootScope.$on("cotacaoNovoPreco", function (event, cotacaoId, dtFechamento, cotacaoPeds, dtIniCota) {

            if (parseInt(cotacaoId) !== parseInt($scope.CotacaoId)) return;

            //$scope.cotacaoPeds = cotacaoPeds;

            for (var i = 0; i < cotacaoPeds.length; i++) {
                $scope.cotacaoPeds[i].DescProduto = cotacaoPeds[i].DescProduto;
                $scope.cotacaoPeds[i].indMaisPrecoIgual = cotacaoPeds[i].indMaisPrecoIgual;
                $scope.cotacaoPeds[i].indPrecoIgual = cotacaoPeds[i].indPrecoIgual;
                $scope.cotacaoPeds[i].menorPreco = cotacaoPeds[i].menorPreco;
                $scope.cotacaoPeds[i].Observacao = cotacaoPeds[i].Observacao;
                $scope.cotacaoPeds[i].ProdutoId = cotacaoPeds[i].ProdutoId;
                $scope.cotacaoPeds[i].qtd = cotacaoPeds[i].qtd;
                $scope.cotacaoPeds[i].FlgOutraMarca = cotacaoPeds[i].FlgOutraMarca;
            }

            sessionStorage.setItem("DtFechamento", dtFechamento);
            $scope.DtFechamento = dtFechamento;
            $scope.dateCotaNow = dtIniCota;

            $scope.itemCotacao = [];
            $scope.itemCotacaoZero = [];
        });
        //------------------Atualização em tempo Real---------------

        function tempoFinal() {
            if ($rootScope.repository.loggedUser) {

                $scope.BloquearBotaoEnviarOferta = true;

                SweetAlert.swal({
                    title: "PERÍODO DA COTAÇÃO ENCERRADO!",
                    text: "Infelizmente o prazo para dar lances esgotou.",
                    type: "warning",
                    html: true
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $location.path('/cotacoes');
                        }
                    });
            }
        }


        function refreshPrecos() {
            loadCotacao();
        };




        $scope.verificaPreco = function verificaPreco(dadosCotacao) {

            if (dadosCotacao.PrecoNegociadoUnit != "0,00") {


                var precoUnit = fornecedorUtilService.moedaDecimal(dadosCotacao.PrecoNegociadoUnit);
               
                //calcula percentual
                var percentual = ((dadosCotacao.PrecoMedioMercado - precoUnit) /  dadosCotacao.PrecoMedioMercado) * 100;

                //maior q 75% avisa o usuário
                if (percentual > 70) {

                     
                    SweetAlert.swal({
                        title: "PREÇO MUITO ABAIXO DO VALOR MÉDIO DE MERCADO! TEM CERTEZA ?",
                        text: "Confira se o preço do item  (" + dadosCotacao.DescProduto +") bate com a descrição do produto. O valor terá que ser honrado de acordo com nossa política e termo de uso.",
                        type: "warning", 
                        html: true
                    });

                }
            }
          


        };

        loadCotacao();

        //$scope.timeInMs = 0;

        //var refreshCotacaoMinutos = function () {
        //	loadCotacao();
        //};

        //$timeout(refreshCotacaoMinutos, 12000);


        //$scope.$on('$destroy', function () {
        //    if (promise)
        //        $interval.cancel(promise);
        //});
    }

})(angular.module('ECCFornecedor'));