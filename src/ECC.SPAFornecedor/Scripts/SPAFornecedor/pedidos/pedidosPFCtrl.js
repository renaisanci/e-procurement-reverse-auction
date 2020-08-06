
(function (app) {
    'use strict';

	app.controller('pedidosPFCtrl', pedidosPFCtrl);

	pedidosPFCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', '$stateParams', 'SweetAlert', '$modal', '$rootScope'];

	function pedidosPFCtrl($scope, $timeout, apiService, notificationService, $stateParams, SweetAlert, $modal, $rootScope) {

        $scope.pageClass = 'page-pedidoPF';
        $scope.aprovarPedido = aprovarPedido;
        $scope.openPedidoMapa = openPedidoMapa;
        $scope.cancelarPedido = cancelarPedido;
        $scope.openDialogDetalhesPedido = openDialogDetalhesPedido;
        $scope.loadCotacao = loadCotacao;
        $scope.existeTaxa = false;
        var aprovacao = null;


        //1-----Carrega cotacao -----------------------------------
        function loadCotacao() {

            apiService.get('/api/cotacao/cotacaoPedidosAprovarPf', null,
                loadCotacaoSucesso,
                loadCotacaoFalha);
        }

        function loadCotacaoSucesso(response) {

            var cotacoes = response.data.Cotacoes;

            for (var i = 0; i < cotacoes.length; i++) {
                for (var j = 0; j < cotacoes[i].Pedidos.length; j++) {

                    var objQuantidadeItens = {
                        QtdItens: 0,
                        FormaPagamento: '',
                        ValorTotalAvista: 0
                    };
                    
                    var desconto = cotacoes[i].Pedidos[j].Itens[0].Desconto;
                    objQuantidadeItens.ValorTotalAvista = cotacoes[i].Pedidos[j].ValorTotal -
                        (cotacoes[i].Pedidos[j].ValorTotal * desconto / 100);

                    for (var k = 0; k < cotacoes[i].Pedidos[j].Itens.length; k++) {

                        objQuantidadeItens.QtdItens = objQuantidadeItens.QtdItens + cotacoes[i].Pedidos[j].Itens[k].quantity;
                        var pagamentoid = cotacoes[i].Pedidos[j].Itens[k].FormaPagtoId;
                        var fornecedor = cotacoes[i].Pedidos[j].Itens[k].Fornecedor;

                        //Verifica se existe Taxa de Entrega para cada pedido
                        if (!$scope.existeTaxa && cotacoes[i].Pedidos[j].Itens[k].TaxaEntrega > 0) {
                            $scope.existeTaxa = true;
                        }

                        for (var l = 0; l < fornecedor.FormasPagamento.length; l++) {
                            if (fornecedor.FormasPagamento[l].Id === pagamentoid) {
                                objQuantidadeItens.FormaPagamento = fornecedor.FormasPagamento[l].DescFormaPagto;
                            }
                        }
                    }
                    cotacoes[i].Pedidos[j].Quantidade = objQuantidadeItens;
                }
            }

            $scope.cotacoes = cotacoes;

            if ($scope.cotacoes == undefined || $scope.cotacoes == null || $scope.cotacoes.length < 1) {
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
        //1------------------------fim-----------------------------
        

        //2-------------Aprovar Pedido-----------------------------
        function aprovarPedido(pedido) {

            aprovacao = true;

 
            var pedidoAprove = {

                PedidoId: pedido.PedidoId,
                DescMotivoCancelamento: pedido.DescMotivoCancelamento

            };


            SweetAlert.swal({
                title: "Atenção!",
                text: "Deseja realmente aprovar este pedido?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55", confirmButtonText: "SIM",
                cancelButtonText: "NÃO",
                closeOnConfirm: false,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {
                        apiService.post('/api/cotacao/aprovarPedido/' + aprovacao, pedidoAprove,
                            aprovarPedidoSucesso,
                            aprovarPedidoFalha);
                    }
                });
        }

        function aprovarPedidoSucesso(result) {

            if (result.data.success) {
                SweetAlert.swal({
                    title: "APROVADO COM SUCESSO. INFORME O MEMBRO QUANDO DESPACHAR O PEDIDO",
                    text: "Vá no Menu Pedidos Gerados, informe o mebro que despachou o pedido. ", //VALIDAR FRASE
                    type: "success",
                    html: true
                },
                    function () {
                        loadCotacao();
                        //Chama método localizado em topBarCtrl.js para carregar os avisos novamente
                        $rootScope.$emit("carregaAvisos",{});
                    });
            }

        }

        function aprovarPedidoFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //2-----------------Fim------------------------------------
        

        //3-------------Cancelar Pedido----------------------------
        function cancelarPedido(pedido) {

            SweetAlert.swal({
                title: "ATENÇÃO",
                text: "Tem certeza que deseja cancelar este pedido?",
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
                         openDialogMotivoPedidoCancelado(pedido);
                     }
                 });
        }
        //3-----------------Fim------------------------------------
        

        //4-----Abre popup do mapa----------------------------------
        function openPedidoMapa(idPedido) {
            $scope.idPedido = idPedido;
            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/pedidos/pedidoMapa.html',
                controller: 'pedidoMapaCtrl',
                scope: $scope,
                size: 'lg-mapa'
            }).result.then(function ($scope) {
                //console.log("Modal Closed!!!");
            }, function () {
                //console.log("Modal Dismissed!!!");
            });
        }
        //4-----Abre popup do mapa----------------------------------


        function openDialogDetalhesPedido(pedido) {

            $scope.detahePedidoGerado = pedido;

            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/pedidos/modalDetPedidoGerado.html',
                controller: 'modalDetPedidoGeradoCtrl',
                scope: $scope,
                size: 'lg'
            });
        }

        function openDialogMotivoPedidoCancelado(pedido) {
            $scope.detahePedido = pedido;
            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/pedidos/modalMotivoPedidoCancelado.html',
                controller: 'modalMotivoPedidoCanceladoCtrl',
                scope: $scope,
                size: 'lg'
            });
        }


        loadCotacao();
    }

})(angular.module('ECCFornecedor'));