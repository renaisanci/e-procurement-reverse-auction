
(function (app) {
    'use strict';

    app.controller('detalhesPromocaoCtrl', detalhesPromocaoCtrl);

    detalhesPromocaoCtrl.$inject = ['$scope', '$modalInstance', '$rootScope', 'apiService', 'notificationService', 'SweetAlert', '$filter'];

    function detalhesPromocaoCtrl($scope, $modalInstance,$rootScope, apiService, notificationService, SweetAlert, $filter) {

        $scope.close = close;
        $scope.salvarAprovacao = salvarAprovacao;
        $scope.cancelarPromocao = cancelarPromocao;
        $scope.salvarMotivoCancelamento = salvarMotivoCancelamento;
        $scope.botaoSalvarPromocao = true;
        $scope.botaoCancelarPromocao = true;
        $scope.botaoSalvarMotivoCancel = false;
        $scope.divDescMotivoCancelamento = false;


        //1---------Fecha Modal--------------------------
        function close() {
            $modalInstance.dismiss();
        }
        //1----------------------------------------------


        //2---------Salvar Aprovação---------------------
        function salvarAprovacao() {

            //inserir aqui api para a aprovação da promoção

            $scope.novoProduto.Fornecedor = null;

            apiService.post('/api/produtopromocional/salvarAprovacaoPromocao/' + true, $scope.novoProduto,
                salvarAprovacaoLoadCompleted,
                salvarAprovacaoLoadFailed);

            function salvarAprovacaoLoadCompleted(result) {

                notificationService.displaySuccess('Promoção aprovada com sucesso!');
                close();

                for (var i = 0; i < $scope.produtos.length; i++) {

                    if ($scope.produtos[i].Id === result.data.Id) {
                        $scope.produtos[i] = result.data;
                    }
                }
              
                $scope.pesquisarCategoria();
                $rootScope.$emit("loadTotalProdutoPromocao", {});
            }

            function salvarAprovacaoLoadFailed(result) {

                notificationService.displayError('Erro ao aprovar Promoção!\n' + result.data);
            }

        }
        //2----------------------------------------------
        

        //3---------Cancelar Promoção---------------------
        function cancelarPromocao() {


            $scope.botaoSalvarPromocao = false;
            $scope.botaoCancelarPromocao = false;
            $scope.botaoSalvarMotivoCancel = true;
            $scope.divDescMotivoCancelamento = true;
        }
        //3-----------------------------------------------


        //4---------Salvar Motivo Cancelamento---------------------
        function salvarMotivoCancelamento() {

            if ($scope.novoProduto.DescMotivoCancelamento != undefined) {

                $scope.novoProduto.Fornecedor = null;

                //inserir aqui api para o motivo do cancelamento da promoção
                apiService.post('/api/produtopromocional/salvarAprovacaoPromocao/' + false, $scope.novoProduto,
                    salvarMotivoCancelamentoAprovacaoLoadCompleted,
                    salvarMotivoCancelamentoAprovacaoLoadFailed);

                function salvarMotivoCancelamentoAprovacaoLoadCompleted(result) {

                    $scope.pesquisarProduto();

                    notificationService.displaySuccess('Promoção cancelada com sucesso!');

                    $scope.botaoSalvarPromocao = true;
                    $scope.botaoCancelarPromocao = true;
                    $scope.botaoSalvarMotivoCancel = false;
                    $scope.divDescMotivoCancelamento = false;
                    close();
                }

                function salvarMotivoCancelamentoAprovacaoLoadFailed(result) {

                    notificationService.displayError('Erro ao cancelar Promoção!\n' + result.data);
                }




                //método localizado em aprovarpromocaoCtrl.js
                //$scope.pesquisarProduto();
            } else {

                notificationService.displayError('Descrever o motivo para o cancelamento desta oferta!');
            }



        }
        //4----------------------------------------------

        
        //5---------Verifica data de Vencimento---------------------
        function verificaDataVencimento() {

            var data = new Date();
            //var dia = data.getDate();
            //var mes = data.getMonth() + 1;
            //var ano = data.getFullYear();
            //var datanova = dia + "/" + mes + "/" + ano;
            var dataBanco = $scope.novoProduto.FimPromocao;

            //var data1 = parseInt(datanova.split("/")[0].toString() + datanova.split("/")[1].toString() + datanova.split("/")[2].toString());
            //var data2 = parseInt(dataBanco.split("/")[0].toString() + dataBanco.split("/")[1].toString() + dataBanco.split("/")[2].toString());

            if (data >= dataBanco) {

                $scope.botaoSalvarPromocao = false;
                $scope.botaoCancelarPromocao = false;
                $scope.botaoSalvarMotivoCancel = false;

            } else if (data >= dataBanco && $scope.novoProduto.PromoAtivo) {

                $scope.botaoSalvarPromocao = true;
                $scope.botaoCancelarPromocao = true;
            }


            if ($scope.novoProduto.PromoAtivo && $scope.novoProduto.Aprovado) {

                $scope.botaoSalvarPromocao = false;
                $scope.botaoCancelarPromocao = false;
                $scope.botaoSalvarMotivoCancel = false;
            }
        }
        //5---------Verifica data de Vencimento---------------------

        verificaDataVencimento();
    }

})(angular.module('ECCAdm'));