
(function (app) {
    'use strict';

    app.controller('aprovacaopromocaoCtrl', aprovacaopromocaoCtrl);

    aprovacaopromocaoCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', '$stateParams', 'SweetAlert', '$modal'];

    function aprovacaopromocaoCtrl($scope, $timeout, apiService, notificationService, $stateParams, SweetAlert, $modal) {

        $scope.pageClass = 'page-aprovacaopromocao';
        $scope.aprovarPedidoPromocao = aprovarPedidoPromocao;
        $scope.cancelarPedidoPromocao = cancelarPedidoPromocao;
        $scope.openDialogDetalhesPedido = openDialogDetalhesPedido;
        $scope.loadCotacao = loadCotacao;
        var aprovacao = null;


        //1------------------Carrega cotacao ------------------------
        function loadCotacao() {
            apiService.get('/api/cotacao/cotacaoPedidosAprovar', null,
                loadCotacaoSucesso,
                loadCotacaoFalha);
        }

        function loadCotacaoSucesso(response) {

            var resultadoPromocoes = response.data.PedidosPromocao;

            for (var m = 0; m < resultadoPromocoes.length; m++) {

                var objQuantidadeItensPromocao = {
                    QtdItens: 0,
                    FormaPagamento: ''
                };
                for (var n = 0; n < resultadoPromocoes[m].Itens.length; n++) {
                    objQuantidadeItensPromocao.QtdItens = objQuantidadeItensPromocao.QtdItens + resultadoPromocoes[m].Itens[n].quantity;
                   
                   var descontoItemPedido = resultadoPromocoes[m].Itens[n].Desconto;
                    var pagamentoPromocaoId = resultadoPromocoes[m].Itens[n].FormaPagtoId;
                    var fornecedorPromocao = resultadoPromocoes[m].Itens[n].Fornecedor;

                    for (var o = 0; o < fornecedorPromocao.FormasPagamento.length; o++) {
                        if (fornecedorPromocao.FormasPagamento[o].Id === pagamentoPromocaoId) {
                            objQuantidadeItensPromocao.FormaPagamento = fornecedorPromocao.FormasPagamento[o].DescFormaPagto;
                            for (var g = 0; g < fornecedorPromocao.FormaPagtos.length; g++) {
                                if (fornecedorPromocao.FormaPagtos[g].FormaPagtoId === pagamentoPromocaoId) {
                                    var desconto = (resultadoPromocoes[m].ValorTotal * descontoItemPedido) / 100;
                                    objQuantidadeItensPromocao.ValorTotalAvista = resultadoPromocoes[m].ValorTotal - desconto;
                                }
                            }

                        }
                    }
                }
                resultadoPromocoes[m].Quantidade = objQuantidadeItensPromocao;
            }

            $scope.Promocoes = resultadoPromocoes;

            if (($scope.Promocoes == undefined || $scope.Promocoes == null || $scope.Promocoes.length < 1)) {
                SweetAlert.swal({
                    title: "NÃO EXISTEM PEDIDOS À SEREM APROVADOS!",
                    text: "Por favor aguarde, até que existam novos pedidos para aprovação.", //VALIDAR FRASE
                    type: "success",
                    html: true
                });
            }
        }

        function loadCotacaoFalha(response) {
            notificationService.displayError(response.data);
        }
        //1------------------------fim-------------------------------


        //5-------------Aprovar Pedido Promoção----------------------
        function aprovarPedidoPromocao(pedido) {
            aprovacao = true;

            SweetAlert.swal({
                title: "ATENÇÃO",
                text: "Tem certeza que deseja aprovar este Pedido!",
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
                    apiService.post('/api/pedido/aprovarPedidoPromocao/' + aprovacao, pedido,
                        aprovarPedidoPromocaoSucesso,
                        aprovarPedidoPromocaoFalha);
                }
            });

        }

        function aprovarPedidoPromocaoSucesso(result) {

            if (result.data.success) {

                SweetAlert.swal({
                    title: "SEU PEDIDO FOI APROVADO COM SUCESSO!",
                    text: "Verifique em pedidos gerados", //VALIDAR FRASE
                    type: "success",
                    html: true
                },
                    function () {
                        loadCotacao();
                    });
            }

        }

        function aprovarPedidoPromocaoFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //5-----------------Fim--------------------------------------


        //6-------------Cancelar Pedido Promoção----------------------
        function cancelarPedidoPromocao(pedido) {

          
            $scope.pedidoPromocao = pedido;

            SweetAlert.swal({
                title: "ATENÇÃO",
                text: "Tem certeza que deseja cancelar este Pedido!",
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
                    openDialogMotivoPromocaoCancelado(pedido);
                }
            });
        }
        //6-----------------Fim---------------------------------------
        
        
        function openDialogDetalhesPedido(pedido) {
            $scope.detahePedidoGerado = pedido;
            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/pedidos/modalDetPedidoGerado.html',
                controller: 'modalDetPedidoGeradoCtrl',
                scope: $scope,
                size: 'lg'
            });
        }


        function openDialogMotivoPromocaoCancelado(pedido) {
            $scope.detahePedidoPromocao = pedido;
            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/pedidos/modalMotivoPromocaoCancelada.html',
                controller: 'modalMotivoPromocaoCanceladaCtrl',
                scope: $scope,
                size: 'lg'
            });
        }


        loadCotacao();

    }

})(angular.module('ECCFornecedor'));