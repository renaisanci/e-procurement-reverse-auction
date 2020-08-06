(function (app) {
    'use strict';

    app.controller('detPedidoCtrl', detPedidoCtrl);

    detPedidoCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'SweetAlert', 'utilService', '$rootScope', '$filter', '$modal'];

    function detPedidoCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, SweetAlert, utilService, $rootScope, $filter, $modal) {

        $scope.close = close;
        $scope.indexSelecionado = -1;
        $scope.aprovarPedido = aprovarPedido;
        $scope.cancelarPedido = cancelarPedido;
        $scope.validarPedido = validarPedido;
        $scope.formaPagamento = formaPagamento;
        $scope.aprovacaoItemPedido = aprovacaoItemPedido;

        $scope.aprovacaoReprovacaoItensPedido = aprovacaoReprovacaoItensPedido;

        $scope.aumentaQuantidadeItens = aumentaQuantidadeItens;
        $scope.openModalTrocaFornecedor = openModalTrocaFornecedor;
        $scope.verificaValorPedMin = verificaValorPedMin;
        $scope.itensSubTotalFornecedor = [];
        $scope.confirmed = null;
        $scope.Frete = [];
        $scope.exibirMensagemValPedMinimo = false;
        $scope.exibirColunaObservacaoFornecedor = false;
        $scope.exibirColunaObservacaoEntregaFornecedor = false;
        $scope.aprovacaoTodosItensPedido = false;
        $scope.totalItensPedidoParaFornecedor = 0;
        $scope.idFornePassouMouse = [];
        $scope.IdFornecedorQuePassouMouse = 0;


        $scope.setarIndexSelecionado = function (indexItemPedido) {
            if ($scope.indexSelecionado == indexItemPedido)
                $scope.indexSelecionado = undefined;
            else
                $scope.indexSelecionado = indexItemPedido;
        };

        //Array da controller meusPedidosCtrl.js
        $scope.arrayFornecedores = angular.copy($scope.somaSubTotalFornecedores);

        // Recupera o StatusId do Pedido
        var statusId = $scope.pedido.StatusId;

        // Vem como true de meusPedidosCtrl.js quando existe algum item com observação
        $scope.exibirColunaObservacaoItemPedido = $scope.pedido.existeItensObservacao;

        $scope.abrirIndexSelecionado = function (indexItemPedido) {
            var retorno = false;

            if ($scope.indexSelecionado == indexItemPedido)
                retorno = true;

            return retorno;
        };

        $scope.arrayDiasSemana = [
            { Id: 1, DescSemana: 'Seg' },
            { Id: 2, DescSemana: 'Ter' },
            { Id: 3, DescSemana: 'Qua' },
            { Id: 4, DescSemana: 'Qui' },
            { Id: 5, DescSemana: 'Sex' },
            { Id: 6, DescSemana: 'Sáb' },
            { Id: 7, DescSemana: 'Dom' }
        ];

        $scope.Frete = [
            {
                Transportadora: 'Oxiteno S/A. Ind. E Com.',
                FormaPagto: 'Á Vista',
                PrazoEntrega: '3 dias',
                Valor: 35.50,
                selected: false
            }, {
                Transportadora: 'Electrolux do Brasil S/A.',
                FormaPagto: 'Boleto',
                PrazoEntrega: '5 dias',
                Valor: 27.13,
                selected: false
            },
            {
                Transportadora: 'Patrus – Transportes Urgentes',
                FormaPagto: 'Á Vista',
                PrazoEntrega: '2 dias',
                Valor: 18.77,
                selected: false
            }
        ];

        //1---------Aprovar Pedido-------------------------------------
        function aprovarPedido() {

            if (validarPedido()) {
                var itensPedido = $scope.pedido.Itens;



                SweetAlert.swal({
                    title: "Tem certeza que vai APROVAR o Pedido!",
                    text: "Após a aprovação, não será possivel cancelar!",
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
                            apiService.post('/api/pedido/aprovar',
                                itensPedido,
                                aprovarPedidoSucesso,
                                aprovarPedidoFalha);
                        }
                    });



            }

        }

        function aprovarPedidoSucesso(response) {

            //fecha modal de detalhes do pedido
            close();

            SweetAlert.swal({
                title: "PEDIDO APROVADO COM SUCESSO!",
                text: "Seu pedido será encaminhado para aprovação do fornecedor!",
                type: "success"
            });

            $scope.pedido = response.data;
            $scope.loadPedidosMembro();
            $modalInstance.dismiss();


            $rootScope.$emit("reloadAvisosMem", {});
        }

        function aprovarPedidoFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //1-----------------Fim-----------------------------------------

        //2--------------Inseri forma de pagamento----------------------
        function formaPagamento(pagamento, fornecedorId, pedido, confirmado) {
            var itensPedidos = pedido.Itens;
            var percentualDesconto = 0;
            $scope.quantidadeItens = 0;
            var cont = 0;

            // Verifica a forma de pagamento com valor do pedido por fornecedor...
            for (var n = 0; n < $scope.somaSubTotalFornecedores.length; n++) {

                if ($scope.somaSubTotalFornecedores[n].Fornecedor.Id == fornecedorId) {
                    var formaPagto = $scope.somaSubTotalFornecedores[n].Fornecedor.FormaPagtos;
                    for (var x = 0; x < formaPagto.length; x++) {
                        if (formaPagto[x].FormaPagtoId == pagamento.Id) {

                            var valorPed = utilService.moedaDecimal(formaPagto[x].VlFormaPagto);
                            var QtdParcelas = pagamento.QtdParcelas;
                            var valorMinParcela = utilService.moedaDecimal(formaPagto[x].ValorMinParcela);
                            var valorMinParcelaPedido = utilService.moedaDecimal(formaPagto[x].ValorMinParcelaPedido);

                            var resultadoParcelas = $scope.somaSubTotalFornecedores[n].subtotalFornecedor / QtdParcelas;

                            // Verificando se valor do pedido está acima do valor mínimo da forma de pagamento
                            if (valorMinParcelaPedido > 0) {

                                if (valorMinParcelaPedido > $scope.somaSubTotalFornecedores[n].subtotalFornecedor) {

                                    confirmado.confirmed = false;
                                    $scope.somaSubTotalFornecedores[n].FormaPagtoId = null;
                                    notificationService.displayInfo('Para escolher esta forma de pagamento o valor do pedido deve estar acima de ' + $filter('currency')(valorMinParcelaPedido));

                                    break;
                                }
                            }

                            // Verificando se valor da parcela está acima do valor mínimo de parcela para está forma de pagamento
                            if (valorMinParcela > resultadoParcelas) {

                                confirmado.confirmed = false;
                                $scope.somaSubTotalFornecedores[n].FormaPagtoId = null;
                                notificationService.displayInfo('Para escolher esta forma de pagamento o valor da parcela deve ser igual ou acima de ' + $filter('currency')(valorMinParcela));

                                break;
                            }

                            if ($scope.somaSubTotalFornecedores[n].subtotalFornecedor < valorPed && cont == 0) {
                                var valorPedido = $filter('currency')(valorPed);
                                cont++;
                                confirmado.confirmed = false;
                                $scope.somaSubTotalFornecedores[n].FormaPagtoId = null;
                                notificationService.displayInfo('Para escolher está forma de pagamento, o valor de pedido deve ser no mínimo ' + valorPedido);

                                break;
                            }
                        }
                    }
                }
            }

            //#region Inserir Forma Pagamento e Calcular Desconto Fornecedores

            if (cont == 0) {

                for (var i = 0; i < itensPedidos.length; i++) {
                    if (itensPedidos[i].FornecedorId == fornecedorId) {
                        var formaPagtos = itensPedidos[i].Fornecedor.FormaPagtos;

                        for (var k = 0; k < formaPagtos.length; k++) {
                            if (formaPagtos[k].FormaPagtoId == pagamento.Id) {
                                itensPedidos[i].Desconto = formaPagtos[k].Desconto;
                            }
                        }
                        itensPedidos[i].FormaPagtoId = pagamento.Id;
                    }

                    if (itensPedidos[i].AprovacaoMembro)
                        $scope.quantidadeItens += itensPedidos[i].quantity;
                }
            }

            for (var r = 0; r < $scope.somaSubTotalFornecedores.length; r++) {

                $scope.somaSubTotalFornecedores[r].subtotalFornecedor = 0;

                for (var f = 0; f < itensPedidos.length; f++) {

                    if ($scope.somaSubTotalFornecedores[r].FornecedorId == itensPedidos[f].FornecedorId && itensPedidos[f].AprovacaoMembro) {

                        $scope.somaSubTotalFornecedores[r].subtotalFornecedor += itensPedidos[f].SubTotal;
                        percentualDesconto = itensPedidos[f].Desconto;
                    }

                }
                var descontoItens = $scope.somaSubTotalFornecedores[r].subtotalFornecedor * percentualDesconto / 100;
                $scope.somaSubTotalFornecedores[r].subtotalFornecedor = $scope.somaSubTotalFornecedores[r].subtotalFornecedor - descontoItens;

            }

            $scope.subtotal = 0;
            for (var j = 0; j < $scope.somaSubTotalFornecedores.length; j++) {
                $scope.subtotal += $scope.somaSubTotalFornecedores[j].subtotalFornecedor;
            }

            //#endregion

        }
        //2-------------------Fim---------------------------------------

        //3------------------Valida Pedido------------------------------
        function validarPedido() {

            //Recupera os itens do pedido da controller meusPedidosCtrl
            var itensPedido = angular.copy($scope.pedido.Itens);

            for (var i = 0; i < $scope.somaSubTotalFornecedores.length; i++) {

                if ($scope.somaSubTotalFornecedores[i].FormaPagtoId == null) {

                    var cont = 0;
                    $scope.somaSubTotalFornecedores[i].Itens = [];

                    for (var j = 0; j < itensPedido.length; j++) {
                        if ($scope.somaSubTotalFornecedores[i].FornecedorId == itensPedido[j].FornecedorId) {
                            $scope.somaSubTotalFornecedores[i].Itens.push(itensPedido[j]);
                        }
                    }

                    for (var q = 0; q < $scope.somaSubTotalFornecedores[i].Itens.length; q++) {
                        if (!$scope.somaSubTotalFornecedores[i].Itens[q].AprovacaoMembro) {
                            cont++;
                        }
                    }

                    if ($scope.somaSubTotalFornecedores[i].Itens.length == cont) {
                        $scope.somaSubTotalFornecedores[i].PodeAprovar = true;
                    } else {
                        $scope.somaSubTotalFornecedores[i].PodeAprovar = false;
                    }

                } else {
                    $scope.somaSubTotalFornecedores[i].PodeAprovar = true;
                }
            }

            for (var k = 0; k < $scope.somaSubTotalFornecedores.length; k++) {

                if ($scope.somaSubTotalFornecedores[k].PodeAprovar == false) {

                    SweetAlert.swal({
                        title: "FORMA DE PAGAMENTO NÃO SELECIONADA!",
                        text: "Selecione a forma de pagamento para todos os fornecedores!",
                        type: "error"
                    });

                    return false;
                }

                if ($scope.somaSubTotalFornecedores[k].Fornecedor.TaxaEntrega > 0)
                    $scope.somaSubTotalFornecedores[k].TaxaEntrega = $scope.somaSubTotalFornecedores[k].Fornecedor.TaxaEntrega;


                //if (!$scope.somaSubTotalFornecedores[k].Fornecedor.Cif) {

                //	SweetAlert.swal({
                //		//title: "SELECIONE UM FRETE OU TAXA DE ENTREGA!",
                //		title: "ATENÇÃO!",
                //		text: "Selecione a Taxa de Entrega para o Fornecedor!",
                //		//text: "Selecione algum frete ou uma a Taxa de Entrega para o Fornecedor!",
                //		type: "error"
                //	});

                //	return false;
                //} else {
                //	$scope.somaSubTotalFornecedores[k].TaxaEntrega = $scope.somaSubTotalFornecedores[k].Fornecedor.TaxaEntrega;
                //}
            }

            return true;
        }
        //3---------------------Fim-------------------------------------

        //4---------Fecha Modal-----------------------------------------
        function close() {


            $modalInstance.dismiss();
        }
        //4-------------------------------------------------------------

        //5----------------Cancelar item Pedido-------------------------
        function aprovacaoItemPedido(item) {

            if (item.AprovacaoMembro) {

                for (var i = 0; i < $scope.somaSubTotalFornecedores.length; i++) {
                    if ($scope.somaSubTotalFornecedores[i].FornecedorId == item.FornecedorId) {
                        $scope.somaSubTotalFornecedores[i].subtotalFornecedor += item.SubTotal;
                    }
                }

                $scope.subtotal += item.SubTotal;
                $scope.quantidadeItens += item.quantity;
                $scope.pedido.QtdItem = $scope.quantidadeItens;

            } else {

                for (var t = 0; t < $scope.somaSubTotalFornecedores.length; t++) {

                    if ($scope.somaSubTotalFornecedores[t].FornecedorId == item.FornecedorId) {

                        $scope.somaSubTotalFornecedores[t].subtotalFornecedor -= item.SubTotal;

                        var pedValorMin = $scope.somaSubTotalFornecedores[t].Fornecedor.VlPedidoMin.replace(",", ".");

                        if ($scope.somaSubTotalFornecedores[t].subtotalFornecedor < pedValorMin && $scope.qtdItens > 1) {

                            var exibirMensagemValPedMin = parseFloat(pedValorMin) >
                                $scope.somaSubTotalFornecedores[t].subtotalFornecedor &&
                                $scope.somaSubTotalFornecedores[t].Fornecedor.TaxaEntrega > 0;

                            var mensagemValorPedMinNaoAtingiu = exibirMensagemValPedMin
                                ?
                                "Os produtos comprados com o fornecedor ''" +
                                $scope.somaSubTotalFornecedores[t].Fornecedor.NomeFantasia +
                                "'' não atingiu o ''valor de pedido mínimo''\n" +
                                "Será cobrado a taxa de entrega no valor de " +
                                $filter('currency')($scope.somaSubTotalFornecedores[t].Fornecedor.TaxaEntrega) +
                                "\nOu adicione mais itens para chegar ao valor de pedido mínimo."
                                :
                                "Os produtos comprados com o fornecedor ''" +
                                $scope.somaSubTotalFornecedores[t].Fornecedor.NomeFantasia +
                                "'' não atingiu o ''valor de pedido mínimo''\n" +
                                " Adicione mais itens para chegar ao valor de pedido mínimo.\n" +
                                " Caso contrário ficará ao critério do fornecedor aprovar ou não.\n" +
                                " Fique atento aos seus e-mails e sms para ter mais informações sobre o andamento deste pedido.";

                            SweetAlert.swal({
                                title: "ATENÇÃO!",
                                text: mensagemValorPedMinNaoAtingiu,
                                type: "warning"
                            });
                        }
                    }
                }

                $scope.subtotal -= item.SubTotal;
                $scope.quantidadeItens -= item.quantity;
                $scope.pedido.QtdItem = $scope.quantidadeItens;

            }

            apiService.post('/api/pedido/atualizaItemPedidoMembro/' + item.AprovacaoMembro, item,
                aprovacaoItemPedidoLoadCompleted,
                aprovacaoItemPedidoLoadFailed
            );

            function aprovacaoItemPedidoLoadCompleted(result) {

                $scope.qtdItens = result.data.QtdItens;
                $scope.totalItensPedido = result.data.TotalItensPedido;

                if (item.AprovacaoMembro) {
                    notificationService.displayInfo('Item aprovado!');
                } else {
                    notificationService.displayInfo('Item não aprovado!');
                    if ($scope.totalItensPedido == 0) {
                        SweetAlert.swal({
                            title: "ATENÇÃO!",
                            text: "Nenhum item do pedido foi aprovado, clique no botão ''Cancelar Pedido'' para finalizar.\n" +
                                "Caso contrário aprove ao menos um produto e clique em ''Aprovar Pedido''.",
                            type: "warning"
                        });
                    }
                }

                $scope.aprovacaoTodosItensPedido = result.data.TotalItensPedido == $scope.pedido.Itens.length;

            }

            function aprovacaoItemPedidoLoadFailed(result) {
                notificationService.displayError('Erro ao atualizar a aprovação do item!');
            }


        }
        //5---------------------Fim-------------------------------------

        //6----------------Aumenta Quantidade de Itens------------------
        function aumentaQuantidadeItens(item, quantidade, pedido) {

            if (quantidade > 0) {

                var itensPedido = pedido.Itens;
                var percentualDesconto = item.Desconto;
                $scope.subtotal = 0;
                $scope.quantidadeItens = 0;

                if (quantidade < item.primeiraQuantidade) {
                    notificationService.displayInfo('Quantidade deve ser maior do que ' + item.primeiraQuantidade + ' !');

                } else {

                    item.quantity = quantidade;
                    item.SubTotal = item.PrecoNegociadoUnit * item.quantity;

                    for (var i = 0; i < $scope.somaSubTotalFornecedores.length; i++) {

                        $scope.somaSubTotalFornecedores[i].subtotalFornecedor = 0;

                        for (var j = 0; j < itensPedido.length; j++) {

                            if ($scope.somaSubTotalFornecedores[i].FornecedorId == itensPedido[j].FornecedorId) {

                                $scope.somaSubTotalFornecedores[i].subtotalFornecedor += itensPedido[j].SubTotal;
                            }

                        }

                        var desconto = $scope.somaSubTotalFornecedores[i].subtotalFornecedor * percentualDesconto / 100;
                        $scope.somaSubTotalFornecedores[i].subtotalFornecedor = $scope.somaSubTotalFornecedores[i].subtotalFornecedor - desconto;
                        $scope.subtotal += $scope.somaSubTotalFornecedores[i].subtotalFornecedor;
                    }

                    for (var k = 0; k < itensPedido.length; k++) {
                        if (item.sku == itensPedido[k].sku) {
                            itensPedido[k].quantity = quantidade;
                        }

                        $scope.quantidadeItens += itensPedido[k].quantity;

                    }

                }
            } else {
                notificationService.displayInfo('Digite uma quantidade para este produto!');
            }


        }
        //6---------------------Fim-------------------------------------

        //7-------------Verifica Valor Pedido por Fornecedor------------
        function verificaValorPedMin(item, pedidoStatus) {
            for (var x = 0; x < $scope.somaSubTotalFornecedores.length; x++) {
                if ($scope.somaSubTotalFornecedores[x].FornecedorId == item.FornecedorId) {
                    var valorPedMin = item.Fornecedor.VlPedidoMin.replace(",", ".");
                    if ($scope.somaSubTotalFornecedores[x].subtotalFornecedor < valorPedMin && item.AprovacaoMembro && pedidoStatus == 23) {
                        return true;
                    }

                }
            }
            return false;
        }
        //7---------------------Fim-------------------------------------

        //8------------Carrega os Prazos por dia Semana caso exista-----
        function carregaPrazoSemanalCalculaSubtotalFornecedor() {

            if (statusId > 23) {
                $scope.subtotal = 0;
            }

            for (var i = 0; i < $scope.somaSubTotalFornecedores.length; i++) {


                //Mostra descontos das formas de pagamento caso exista
                for (var o = 0; o < $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento.length; o++) {


                    for (var p = 0; p < $scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos.length; p++) {
                        if ($scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento[o].Id ==
                            $scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos[p].FormaPagtoId) {

                            //Adiciona os descontos para as formas de pagamento para este membro
                            if ($scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos[p].Desconto > 0) {
                                var desconto = $scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos[p].Desconto;
                                $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento[o].Desconto = desconto;
                            }

                            //Retirar as formas de pagamento que estão desativadas e o status do pedido esteja para aprovação do membro
                            if ($scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos[p].Ativo == false && statusId == 23) {
                                var formaPagamento = $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento[o];
                                var index = $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento.indexOf(formaPagamento);
                                $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento.splice(index, 1);
                            }

                        }
                    }
                }


                //Mostra somente as formas de pagamentos parcelados com o valor acima do pedido do mesmo
                for (var m = 0; m < $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento.length; m++) {
                    if ($scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento[m].Avista == false) {

                        for (var n = 0; n < $scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos.length; n++) {

                            if ($scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos[n].FormaPagtoId ==
                                $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento[m].Id) {

                                var pedValorMin = utilService.moedaDecimal($scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos[n].VlFormaPagto);
                                if ($scope.somaSubTotalFornecedores[i].subtotalFornecedor < pedValorMin) {
                                    var indice = $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento.indexOf($scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento[m]);
                                    $scope.somaSubTotalFornecedores[i].Fornecedor.FormasPagamento.splice(indice, 1);
                                }
                            }
                        }
                    }
                }


                //Calcula o desconto do pedido por fornecedor de acordo com a forma de pagamento
                if (statusId > 23) {

                    var descontoFormaPagto = 0;
                    var valorDescSubtotalFornecedor = 0;
                    var formaPagtos = $scope.somaSubTotalFornecedores[i].Fornecedor.FormaPagtos;
                    var idFormaPagtoEscolhida = $scope.somaSubTotalFornecedores[i].FormaPagtoId;
                    for (var l = 0; l < formaPagtos.length; l++) {
                        if (idFormaPagtoEscolhida !== null) {
                            if (formaPagtos[l].FormaPagtoId == idFormaPagtoEscolhida) {
                                descontoFormaPagto = $scope.somaSubTotalFornecedores[i].Desconto;
                                valorDescSubtotalFornecedor = ($scope.somaSubTotalFornecedores[i].subtotalFornecedor * descontoFormaPagto) / 100;
                                $scope.somaSubTotalFornecedores[i].subtotalFornecedor = $scope.somaSubTotalFornecedores[i].subtotalFornecedor - valorDescSubtotalFornecedor;
                                $scope.subtotal += $scope.somaSubTotalFornecedores[i].subtotalFornecedor;
                            }
                        }

                    }
                }

                //Pega os dias da Semana de cada Fornecedor, caso exista
                if ($scope.somaSubTotalFornecedores[i].Fornecedor.FornecedorPrazoSemanal.length > 0) {
                    for (var j = 0; j < $scope.somaSubTotalFornecedores[i].Fornecedor.FornecedorPrazoSemanal.length; j++) {
                        for (var k = 0; k < $scope.arrayDiasSemana.length; k++) {
                            if ($scope.somaSubTotalFornecedores[i].Fornecedor.FornecedorPrazoSemanal[j].DiaSemana == $scope.arrayDiasSemana[k].Id) {
                                $scope.somaSubTotalFornecedores[i].Fornecedor.FornecedorPrazoSemanal[j].DescDiaSemana = $scope.arrayDiasSemana[k].DescSemana;
                                break;
                            }
                        }
                    }
                }

                // Condição para adicionar Taxa de Entrega caso o valor de pedido mínimo não for alcançada
                $scope.somaSubTotalFornecedores[i].Fornecedor.exibirTaxaEntrega = false;

                var condicaoOne = parseFloat($scope.somaSubTotalFornecedores[i].Fornecedor.VlPedidoMin) >
                    $scope.somaSubTotalFornecedores[i].subtotalFornecedor;

                var condicaoTwo = $scope.somaSubTotalFornecedores[i].Fornecedor.TaxaEntrega > 0;


                if (condicaoOne && condicaoTwo) {
                    $scope.exibirMensagemValPedMinimo = true;
                    $scope.somaSubTotalFornecedores[i].Fornecedor.exibirTaxaEntrega = true;
                }

                if ($scope.somaSubTotalFornecedores[i].Fornecedor.Observacao !== null)
                    if ($scope.somaSubTotalFornecedores[i].Fornecedor.Observacao.length > 0)
                        $scope.exibirColunaObservacaoFornecedor = true;

                if ($scope.somaSubTotalFornecedores[i].Fornecedor.ObservacaoEntrega !== null)
                    if ($scope.somaSubTotalFornecedores[i].Fornecedor.ObservacaoEntrega.length > 0)
                        $scope.exibirColunaObservacaoEntregaFornecedor = true;


                verificaTodosItensAprovadosMembro();

            }
        }
        //8-------------------------------------------------------------

        //9------------Cancelar Pedido----------------------------------
        function cancelarPedido() {
            if ($scope.totalItensPedido == 0) {

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
                            apiService.post('/api/pedido/cancelarPedido/' + $scope.pedido.PedidoId, null,
                                cancelarPedidoSucesso,
                                cancelarPedidoFalha);
                        }
                    });
            }
        }

        function cancelarPedidoSucesso(result) {
            close();
            SweetAlert.swal("Sucesso!", "Seu pedido foi cancelado com sucesso!", "success");
            $scope.pedido = result.data;
            $scope.loadPedidosMembro();
        }

        function cancelarPedidoFalha(result) {
            notificationService.displayError(result.data);
        }
        //9-------------------------------------------------------------

        //10--------------Limpa Forma Pagamento------------------------
        function limpaFormaPagamentoFornecedor() {
            if ($scope.somaSubTotalFornecedores.length > 0 && $scope.StatusId == 23) {
                for (var i = 0; i < $scope.somaSubTotalFornecedores.length; i++) {
                    $scope.somaSubTotalFornecedores[i].FormaPagtoId = null;
                }
            }
        }
        //10-----------------------------------------------------------

        //------------Abre Modal Trocar fornecedor do item-------------------------
        $scope.openTrocaFornecedorItem = function openTrocaFornecedorItem(fornecedorIdTrocar) {

            $scope.fornecedorTrocarId = fornecedorIdTrocar;

            $modal.open({
                templateUrl: 'scripts/SPACliente/pedidos/trocaFornecedorItem.html',
                controller: 'trocaFornecedorItemCtrl',
                scope: $scope,
                size: 'lg'

            }).result.then(function (data) {

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        };

        //------------------------------------------------------------

        //------------Recarrega Pedido da troca de fornecedor-------------------------
        $scope.recarregaPedido = function recarregaPedido() {

            reloadPedido($scope.pedido.PedidoId);


        };

        //------------------------------------------------------------

        //11-----Carrega Pedidos Membro ----------------------------
        function reloadPedido(pedidoId) {

            close();

            var config = {
                params: {
                    statusId: 23,
                    page: 0,
                    pageSize: 8,
                    dtDe: $filter('date')($scope.DtDe, $scope.format),
                    dtAte: $filter('date')($scope.DtAte, $scope.format),
                    idPedido: pedidoId
                }
            };

            apiService.get('/api/pedido/pedidosMembro', config,
                loadPedidosMembroSucesso,
                loadPedidosMembroFalha);
        }

        function loadPedidosMembroSucesso(response) {

            var pedidos = response.data.pedidos.Items;
            $scope.FornecedoresId = response.data.fornecedores;
            var contador = 0;

            $scope.page = response.data.pedidos.Page;
            $scope.pagesCount = response.data.pedidos.TotalPages;
            $scope.totalCount = response.data.pedidos.TotalCount;

            //Cria o TimeLine de acordo com o Status do Pedido
            for (var i = 0; i < pedidos.length; i++) {
                var cont = 0;
                for (var l = 0; l < pedidos[i].Itens.length; l++) {
                    pedidos[i].Itens[l].primeiraQuantidade = pedidos[i].Itens[l].quantity;
                }
                for (var j = 0; j < $scope.StatusPedido.length; j++) {
                    if (j == 0) {
                        pedidos[i].StatusPedido = [];
                        pedidos[i].StatusPedido.push($scope.StatusPedido[j]);
                    }

                    if (pedidos[i].StatusId <= 30 && cont == 0) {
                        pedidos[i].StatusPedido = [];
                        for (var k = 0; k < $scope.StatusPedido.length; k++) {
                            if ($scope.StatusPedido[k].Id <= 30) {
                                pedidos[i].StatusPedido.push($scope.StatusPedido[k]);
                            }
                        }
                        cont++;
                    }
                    else if (pedidos[i].StatusId == $scope.StatusPedido[j].Id && cont == 0) {
                        pedidos[i].StatusPedido.push($scope.StatusPedido[j]);
                        cont++;
                    }
                    else if (pedidos[i].StatusId == $scope.StatusPedido[j].Id && cont == 0) {
                        pedidos[i].StatusPedido.push($scope.StatusPedido[j]);
                        cont++;
                    }
                }
            }

            //Verifica quais os nomes de fornecedores dos pedidos cancelados
            //for (var m = 0; m < pedidos.length; m++) {
            //    for (var o = 0; o < pedidos[m].ListaHistStatusPedido.length; o++) {
            //        if (pedidos[m].PedidoId == pedidos[m].ListaHistStatusPedido[o].PedidoId) {
            //            for (var n = 0; n < pedidos[m].Itens.length; n++) {
            //                if (pedidos[m].Itens[n].Fornecedor.Usuario.Id == pedidos[m].ListaHistStatusPedido[o].UsuarioId) {
            //                    pedidos[m].ListaHistStatusPedido[o].NomeFantasia = pedidos[m].Itens[n].Fornecedor.NomeFantasia;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}


            close();

            $scope.openDetPedDialog(pedidos[0]);
        }

        function loadPedidosMembroFalha(response) {
            notificationService.displayError(response.data);
        }
        //11------------------------fim-----------------------------


        //12-----Carrega todos Status de Pedido -------------------
        $scope.FornecedorRespondeu = function FornecedorRespondeu(FornecedorItensId) {
            var pedidoId = angular.copy($scope.pedido.PedidoId);
            var fornecedorItensId = FornecedorItensId;



            $scope.IdFornecedorQuePassouMouse = fornecedorItensId;
            if ($scope.idFornePassouMouse.indexOf(FornecedorItensId) == -1) {

                apiService.post('/api/TrocaItemFornecedor/totalItensRespondidos/' + pedidoId + '/' + fornecedorItensId, null,
                    loadItensFornecedorPedidoSucesso,
                    loadItensFornecedorPedidoFalha);


                $scope.idFornePassouMouse.push(FornecedorItensId);
            }
        };

        function loadItensFornecedorPedidoSucesso(response) {
            $scope["itensRespondido_" + $scope.IdFornecedorQuePassouMouse] = response.data.totalItensRespondidoFornecedor + "/" + response.data.totalItens;
        }

        function loadItensFornecedorPedidoFalha(response) {
            close();
            notificationService.displayError(response.data);
        }
        //12------------------------fim-----------------------------

        function openModalTrocaFornecedor(itemProdutoId, flgOutraMarca, nomeItem, idDoItem) {

            $scope.ItemIdTroca = idDoItem;
            $scope.ItemProdutoId = itemProdutoId;
            $scope.flgOutraMarcaTrocaItem = flgOutraMarca;

            $scope.statusPedidoId = angular.copy($scope.pedido.StatusId);


            $scope.nomeItemParaTroca = nomeItem;

            $modal.open({

                templateUrl: 'scripts/SPACliente/pedidos/trocaFornecedor.html',
                controller: 'trocaFornecedorCtrl',
                scope: $scope,
                size: 'sm'

            }).result.then(function (data) {

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        };

        //14----- Aprovação Reprovação Itens Pedido -------------------

        function aprovacaoReprovacaoItensPedido(pedido) {

            $scope.subtotal = 0;
            $scope.quantidadeItens = 0;

            for (var i = 0; i < pedido.Itens.length; i++) {

                if ($scope.aprovacaoTodosItensPedido) {

                    pedido.Itens[i].AprovacaoMembro = true;

                    for (var x = 0; x < $scope.somaSubTotalFornecedores.length; x++) {
                        if ($scope.somaSubTotalFornecedores[x].FornecedorId == pedido.Itens[i].FornecedorId) {
                            $scope.somaSubTotalFornecedores[x].subtotalFornecedor += pedido.Itens[i].SubTotal;
                        }
                    }

                    $scope.subtotal += pedido.Itens[i].SubTotal;
                    $scope.quantidadeItens += pedido.Itens[i].quantity;
                    pedido.QtdItem = $scope.quantidadeItens;

                } else {

                    pedido.Itens[i].AprovacaoMembro = false;

                    for (var t = 0; t < $scope.somaSubTotalFornecedores.length; t++) {                      
                            $scope.somaSubTotalFornecedores[t].subtotalFornecedor = 0;
                    }
                }

                pedido.Itens[i].FormaPagtoId = null;
            }


            for (var p = 0; p < $scope.somaSubTotalFornecedores.length; p++) {
                for (var w = 0; w < $scope.somaSubTotalFornecedores[p].Fornecedor.FormasPagamento.length; w++) {
                    $scope.somaSubTotalFornecedores[p].Fornecedor.FormasPagamento[w].confirmed = false;
                }
            }


            apiService.post('/api/pedido/AtualizaItensPedido/' + $scope.aprovacaoTodosItensPedido, pedido.Itens,
                AtualizaItensPedidoPedidoLoadCompleted,
                AtualizaItensPedidoPedidoLoadFailed
            );
        }

        function AtualizaItensPedidoPedidoLoadCompleted(result) {

            if (result.data.ItensAprovados > 0) {
                $scope.totalItensPedido = result.data.ItensAprovados;
                notificationService.displaySuccess('Itens do pedido aprovados!');
            }

            if (result.data.ItensReprovados > 0) {
                $scope.totalItensPedido = 0;
                notificationService.displaySuccess('Itens do pedido reprovados!');
            }

        }

        function AtualizaItensPedidoPedidoLoadFailed(result) {
            notificationService.displayError('Erro ao atualizar os itens do pedido!');
        }

        //14-----------------------------------------------------------

        //15----- Verifica Todos itens Aprovados Membro --------------

        function verificaTodosItensAprovadosMembro() {

            var contador = 0;

            for (var i = 0; i < $scope.pedido.Itens.length; i++) {

                if ($scope.pedido.Itens[i].AprovacaoMembro)
                    contador++;
            }

            $scope.aprovacaoTodosItensPedido = $scope.pedido.Itens.length == contador;

        }

        //15-----------------------------------------------------------

        carregaPrazoSemanalCalculaSubtotalFornecedor();
        limpaFormaPagamentoFornecedor();

    }

})(angular.module('ECCCliente'));