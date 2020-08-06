
(function (app) {
    'use strict';

    app.controller('modalDetPedidoGeradoCtrl', modalDetPedidoGeradoCtrl);

    modalDetPedidoGeradoCtrl.$inject = ['$scope', '$modalInstance', 'apiService', 'notificationService', '$location'];

    function modalDetPedidoGeradoCtrl($scope, $modalInstance, apiService, notificationService, $location) {

        $scope.close = close;
        $scope.printPedido = printPedido;

        $scope.arrayDiasSemana = [
            { Id: 1, DescSemana: 'Seg' },
            { Id: 2, DescSemana: 'Ter' },
            { Id: 3, DescSemana: 'Qua' },
            { Id: 4, DescSemana: 'Qui' },
            { Id: 5, DescSemana: 'Sex' },
            { Id: 6, DescSemana: 'Sáb' },
            { Id: 7, DescSemana: 'Dom' }
        ];


        console.log($scope.detahePedidoGerado);

        function carregarFormaPagamento() {

            var detPedido = $scope.detahePedidoGerado;
            var qtdItens = detPedido.Itens.length - 1;

            if (qtdItens >= 0) {
                var formasPag = detPedido.Itens[qtdItens].Fornecedor.FormasPagamento;
                var cont = 0;

                //Mostra a forma de pagamento do pedido
                for (var i = 0; i < formasPag.length; i++) {
                    for (var j = 0; j < detPedido.Itens.length; j++) {
                        if (detPedido.Itens[j].FormaPagtoId === formasPag[i].Id) {
                            if (cont < 1) {
                                cont++;

                                $scope.pagamento = {
                                    descFormaPagamento: formasPag[i].DescFormaPagto,
                                    desconto: detPedido.Itens[j].Desconto,
                                    Avista: formasPag[i].Avista
                                };
                            }
                        }
                    }

                }

                if ($scope.detahePedidoGerado.FornecedorPrazoSemanal !== null) {
                    for (var k = 0; k < $scope.detahePedidoGerado.FornecedorPrazoSemanal.length; k++) {
                        for (var l = 0; l < $scope.arrayDiasSemana.length; l++) {
                            if ($scope.detahePedidoGerado.FornecedorPrazoSemanal[k].DiaSemana === $scope.arrayDiasSemana[l].Id) {
                                $scope.detahePedidoGerado.FornecedorPrazoSemanal[k].DescSemana = $scope.arrayDiasSemana[l].DescSemana;
                            }
                        }
                    }
                }
            }
        }

        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------

        //---------Imprimir Pedido---------------------
        function printPedido(divName) {

            var printContents = document.getElementById(divName).innerHTML;
            var popupWin = window.open('', '_blank', '');
            popupWin.document.open();
            popupWin.document.write('<html><head>' +
                '<link rel="stylesheet" type="text/css" href="'+ identificaAmbiente()+'/Content/css/bootstrap.min.css"/>' +
                '</head> <body style="width: 102%;font-size:8px !important;margin: 2mm 0 2mm 0" onload="window.print()">' + printContents + '</body></html> ');
            popupWin.document.close();
        }
        //------------------------------------------

        function identificaAmbiente() {
            if ($location.absUrl().indexOf("localhost") > 0) {
                return window.location.protocol + "//localhost:3327";
            } else if ($location.absUrl().indexOf("/dev") > 0) {
                return window.location.protocol + "//devfornecedor.economizaja.com.br";
            } else if ($location.absUrl().indexOf("/hom") > 0) {
                return window.location.protocol + "//homfornecedor.economizaja.com.br";
            } else {
                //	return window.location.protocol + "//adm.economizaja.com.br";

                return "https://fornecedor.economizaja.com.br";
            }
        }

        carregarFormaPagamento();
    }

})(angular.module('ECCFornecedor'));