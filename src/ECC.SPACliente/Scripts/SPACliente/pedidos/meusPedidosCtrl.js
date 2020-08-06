(function (app) {
    'use strict';

    app.controller('meusPedidosCtrl', meusPedidosCtrl);

    meusPedidosCtrl.$inject = ['$scope', 'membershipService', 'notificationService', 'SweetAlert', '$rootScope', '$location', 'apiService', '$stateParams', 'storeService', '$modal', '$filter', 'clienteUtilService', '$templateCache'];

    function meusPedidosCtrl($scope, membershipService, notificationService, SweetAlert, $rootScope, $location, apiService, $stateParams, storeService, $modal, $filter, clienteUtilService, $templateCache) {
        $scope.pageClass = 'page-meusPedidos';
        $scope.StatusId = 23;
        $scope.loadPedidosMembro = loadPedidosMembro;
        $scope.DtDe = new Date();
        $scope.DtDe.setDate($scope.DtDe.getDate() - 60);
        $scope.DtAte = new Date();
        $scope.limpaStatus = limpaStatus;
        $scope.openDetPedDialog = openDetPedDialog;
        $scope.aprovacaoItem = null;
        $scope.openModalLancesPorFornecedor = openModalLancesPorFornecedor;
        $scope.somaSubTotalFornecedores = [];
        $scope.ItensFornecedores = [];
        $scope.habilitaAprovacaoPedido = habilitaAprovacaoPedido;
        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };





        $scope.openDatePickerDe = openDatePickerDe;
        function openDatePickerDe($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepickerDe.opened = true;
        };

        $scope.openDatePickerAte = openDatePickerAte;
        function openDatePickerAte($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepickerAte.opened = true;
        };
        $scope.datepickerDe = {};
        $scope.datepickerAte = {};
        $scope.format = 'dd/MM/yyyy';


        //1-----Carrega Pedidos Membro ----------------------------
        function loadPedidosMembro(page) {

            page = page || 0;

            if ($scope.IdPedido !== undefined) {
                $scope.StatusId = undefined;
            }

            var config = {
                params: {
                    statusId: $scope.StatusId,
                    page: page,
                    pageSize: 8,
                    dtDe: $filter('date')($scope.DtDe, $scope.format),
                    dtAte: $filter('date')($scope.DtAte, $scope.format),
                    idPedido: $scope.IdPedido
                }
            };

            apiService.get('/api/pedido/pedidosMembro', config,
                loadPedidosMembroSucesso,
                loadPedidosMembroFalha);
        }

        function loadPedidosMembroSucesso(response) {

            var pedidos = response.data.pedidos.Items;
            $scope.FornecedoresId = response.data.fornecedores;

            $scope.fornecedorFormaPagtoRemoveMembro = response.data.fornecedorFormaPagtoRemove;

            $scope.ListForneQtdItensRespondido = response.data.forneQtdItensRespondido;

            $scope.page = response.data.pedidos.Page;
            $scope.pagesCount = response.data.pedidos.TotalPages;
            $scope.totalCount = response.data.pedidos.TotalCount;

            //Cria o TimeLine de acordo com o Status do Pedido
            for (var i = 0; i < pedidos.length; i++) {

                var cont = 0;
                var existeItensObservacao = false;
                pedidos[i].existeItensObservacao = false;

                for (var l = 0; l < pedidos[i].Itens.length; l++) {

                    pedidos[i].Itens[l].primeiraQuantidade = pedidos[i].Itens[l].quantity;

                    if (!existeItensObservacao && pedidos[i].Itens[l].Observacao !== null && pedidos[i].Itens[l].Observacao.length > 0)
                        pedidos[i].existeItensObservacao = true;

                }

                for (var j = 0; j < $scope.StatusPedido.length; j++) {
                    if (j === 0) {
                        pedidos[i].StatusPedido = [];
                        pedidos[i].StatusPedido.push($scope.StatusPedido[j]);
                    }

                    if (pedidos[i].StatusId <= 30 && cont === 0) {
                        pedidos[i].StatusPedido = [];
                        for (var k = 0; k < $scope.StatusPedido.length; k++) {
                            if ($scope.StatusPedido[k].Id <= 30) {
                                pedidos[i].StatusPedido.push($scope.StatusPedido[k]);
                            }
                        }
                        cont++;
                    }
                    else if (pedidos[i].StatusId === $scope.StatusPedido[j].Id && cont === 0) {
                        pedidos[i].StatusPedido.push($scope.StatusPedido[j]);
                        cont++;
                    }
                    else if (pedidos[i].StatusId === $scope.StatusPedido[j].Id && cont === 0) {
                        pedidos[i].StatusPedido.push($scope.StatusPedido[j]);
                        cont++;
                    }
                }
            }

            //Verifica quais os nomes de fornecedores dos pedidos cancelados
            for (var m = 0; m < pedidos.length; m++) {
                for (var o = 0; o < pedidos[m].ListaHistStatusPedido.length; o++) {
                    if (pedidos[m].PedidoId === pedidos[m].ListaHistStatusPedido[o].PedidoId) {
                        for (var n = 0; n < pedidos[m].Itens.length; n++) {
                            if (pedidos[m].Itens[n].Fornecedor.Usuario.Id === pedidos[m].ListaHistStatusPedido[o].UsuarioId) {
                                pedidos[m].ListaHistStatusPedido[o].NomeFantasia = pedidos[m].Itens[n].Fornecedor.NomeFantasia;
                                break;
                            }
                        }
                    }
                }
            }

            $scope.Pedidos = pedidos;

            $scope.IdPedido = undefined;
        }

        function loadPedidosMembroFalha(response) {
            notificationService.displayError(response.data);
        }
        //1------------------------fim-----------------------------


        //2-----Carrega todos Status de Pedido -------------------
        function loadStatusPedido() {

            var config = {
                params: {
                    workflowStatusId: 12
                }
            };
            apiService.get('/api/statusSistema/statusPorWorkflow', config,
                loadStatusPedidoSucesso,
                loadStatusPedidoFalha);
        }

        function loadStatusPedidoSucesso(response) {

            $scope.StatusPedido = response.data;

            if ($scope.PesquisaPedidoId != undefined) {
                $scope.IdPedido = $scope.PesquisaPedidoId;
            }

            loadPedidosMembro();

        }

        function loadStatusPedidoFalha(response) {
            notificationService.displayError(response.data);
        }
        //2------------------------fim-----------------------------


        //3----------Limpa Status----------------------------------
        function limpaStatus() {
            $scope.StatusId = "";
        }
        //3------------------------fim-----------------------------


        //4----------Abre Modal------------------------------------
        function openDetPedDialog(pedido) {
            $scope.ItensFornecedores = [];
            $scope.pedido = pedido;
            $scope.qtdItens = 0;
            $scope.totalItensPedido = 0;
            $scope.somaSubTotalFornecedores = [];


            $scope.loadTodosPedido = loadPedidosMembro();

            var fornecedores = uniqueItems(pedido.Itens, "FornecedorId");

            //Pega todos os fornecedores do pedido para o membro escolher a forma de pagamento 
            for (var l = 0; l < fornecedores.length; l++) {
                for (var m = 0; m < pedido.Itens.length; m++) {
                    if (fornecedores[l] === pedido.Itens[m].FornecedorId) {
                        if ($scope.ItensFornecedores.length === 0) {
                            var objFormaPagto = {
                                Fornecedor: pedido.Itens[m].Fornecedor,
                                FormaPagtoId: uniqueItems(pedido.Itens, "FormaPagtoId")[0]
                            };

                            $scope.ItensFornecedores.push(objFormaPagto);
                        } else {
                            var cont = 0;
                            for (var n = 0; n < $scope.ItensFornecedores.length; n++) {
                                if ($scope.ItensFornecedores[n].Fornecedor.Id === fornecedores[l]) {
                                    cont++;
                                }
                            }
                            if (cont === 0) {
                                var objFormaPagto2 = {
                                    Fornecedor: pedido.Itens[m].Fornecedor,
                                    FormaPagtoId: pedido.Itens[m].FormaPagtoId
                                };
                                $scope.ItensFornecedores.push(objFormaPagto2);
                            }
                        }
                    }
                }
            }


            //Verifica se existe Itens Aprovados pelo Membro
            for (var k = 0; k < pedido.Itens.length; k++) {
                if (pedido.Itens[k].AprovacaoMembro) {
                    $scope.qtdItens++;
                    $scope.totalItensPedido++;
                }
            }


            getTotalCount(pedido.Itens, pedido.StatusId);

        }
        //4------------------------fim-----------------------------


        //5---------Monta modal com itens do pedido----------------
        var uniqueItems = function (data, key) {
            var result = [];

            for (var i = 0; i < data.length; i++) {
                var value = data[i][key];

                if (result.indexOf(value) == -1) {
                    if (value != null)
                        result.push(value);
                }

            }
            return result;
        };

        function getTotalCount(itens, pedStatusId) {

            $scope.fornecedorgroup = uniqueItems(itens, 'FornecedorId');
            $scope.quantidadeItens = 0;
            $scope.subtotal = 0;
            var objSubTotalFornecedor = {};
            var forn = 0;

            //Subtotal de cada item do pedido
            for (var i = 0; i < itens.length; i++) {
                if (itens[i].AprovacaoMembro === true && itens[i].Ativo) {
                    $scope.quantidadeItens += itens[i].quantity;
                    $scope.subtotal += itens[i].SubTotal;
                    $scope.pedido.QtdItem = $scope.quantidadeItens;
                }
            }

            //Separa os fornecedores do pedido para mostrar as formas de pagamento
            for (var k = 0; k < $scope.fornecedorgroup.length; k++) {

                for (var b = 0; b < itens.length; b++) {
                    if (itens[b].FornecedorId === $scope.fornecedorgroup[k] && (forn !== itens[b].FornecedorId)) {
                        forn = itens[b].FornecedorId;
                        objSubTotalFornecedor = itens[b];
                        objSubTotalFornecedor.subtotalFornecedor = 0;
                        objSubTotalFornecedor.primeiroSubtotalFornecedor = 0;
                        $scope.somaSubTotalFornecedores[k] = objSubTotalFornecedor;
                        //Aqui verificamos se o Fornecedor tem a flag 'PrimeiraAvista' como 'true', se sim carregamos somente
                        //a forma de pagamento 'Á Vista'.
                        for (var x = 0; x < $scope.FornecedoresId.length; x++) {
                            if ($scope.FornecedoresId[x] === forn) {
                                if ($scope.somaSubTotalFornecedores[k].Fornecedor.PrimeiraAvista === true) {
                                    var fomasPag = $scope.somaSubTotalFornecedores[k].Fornecedor.FormasPagamento;
                                    fomasPag.forEach(function (item, index) {

                                        if (item.Avista) {
                                            item.DescMotivo = "SOMENTE A VÍSTA, pois é sua primeira compra com este fornecedor.\n" +
                                                "Após a segunda compra poderá escolher outras formas de pagamento.";

                                            $scope.somaSubTotalFornecedores[k].Fornecedor.FormasPagamento = [item];
                                        }
                                    });
                                }
                            }
                        }
                        break;
                    }
                }


                //Soma todos o subtotais por fornecedor
                for (var m = 0; m < itens.length; m++) {
                    if (itens[m].AprovacaoMembro) {
                        if ($scope.fornecedorgroup[k] === itens[m].FornecedorId && itens[m].Ativo) {
                            $scope.somaSubTotalFornecedores[k].subtotalFornecedor += itens[m].SubTotal;
                            $scope.somaSubTotalFornecedores[k].primeiroSubtotalFornecedor += itens[m].SubTotal;
                        }
                    }
                }

            }

            //Verifica se o subtotal de algum fornecedor está abaixo do valor de pedido mínimo
            for (var j = 0; j < $scope.somaSubTotalFornecedores.length; j++) {

                var pedValorMin = $scope.somaSubTotalFornecedores[j].Fornecedor.VlPedidoMin.replace(",", ".");


                //#region Verifica se a região que este Fornecedor atende é CIF ou FOB
                var taxaEntrega = 0;
                if ($scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorRegiao.length > 0) {
                    for (var l = 0; l < $scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorRegiao.length; l++) {
                        if ($scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorRegiao[l].Cif) {
                            $scope.somaSubTotalFornecedores[j].Fornecedor.Cif = true;
                            taxaEntrega = $scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorRegiao[l].TaxaEntrega;
                            $scope.somaSubTotalFornecedores[j].Fornecedor.TaxaEntrega = taxaEntrega;

                            break;
                        } else {
                            $scope.somaSubTotalFornecedores[j].Fornecedor.Cif = false;
                            taxaEntrega = $scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorRegiao[l].TaxaEntrega;
                            $scope.somaSubTotalFornecedores[j].Fornecedor.TaxaEntrega = taxaEntrega;

                        }
                    }
                }
                if ($scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorPrazoSemanal.length > 0) {
                    for (var n = 0; n < $scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorPrazoSemanal.length; n++) {
                        if ($scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorPrazoSemanal[n].Cif) {
                            $scope.somaSubTotalFornecedores[j].Fornecedor.Cif = true;
                            taxaEntrega = $scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorPrazoSemanal[n].TaxaEntrega;
                            $scope.somaSubTotalFornecedores[j].Fornecedor.TaxaEntrega = taxaEntrega;
                            break;
                        } else {
                            $scope.somaSubTotalFornecedores[j].Fornecedor.Cif = false;
                            taxaEntrega = $scope.somaSubTotalFornecedores[j].Fornecedor.FornecedorPrazoSemanal[n].TaxaEntrega;
                            $scope.somaSubTotalFornecedores[j].Fornecedor.TaxaEntrega = taxaEntrega;
                        }
                    }
                }
                //#endregion

                var exibirMensagemValPedMin = parseFloat($scope.somaSubTotalFornecedores[j].Fornecedor.VlPedidoMin) >
                    $scope.somaSubTotalFornecedores[j].subtotalFornecedor &&
                    $scope.somaSubTotalFornecedores[j].Fornecedor.TaxaEntrega > 0;

                var mensagemValorPedMinNaoAtingiu = exibirMensagemValPedMin
                    ?
                    "Os produtos comprados com o fornecedor ''" +
                    $scope.somaSubTotalFornecedores[j].Fornecedor.NomeFantasia +
                    "'' não atingiu o ''valor de pedido mínimo''\n" +
                    "Será cobrado a taxa de entrega no valor de " +
                    $filter('currency')($scope.somaSubTotalFornecedores[j].Fornecedor.TaxaEntrega) +
                    "\nOu adicione mais itens para chegar ao valor de pedido mínimo."
                    :
                    "Os produtos comprados com o fornecedor ''" +
                    $scope.somaSubTotalFornecedores[j].Fornecedor.NomeFantasia +
                    "'' não atingiu o ''valor de pedido mínimo''\n" +
                    " Adicione mais itens para chegar ao valor de pedido mínimo.\n" +
                    " Caso contrário ficará ao critério do fornecedor aprovar ou não.\n" +
                    " Fique atento aos seus e-mails e sms para ter mais informações sobre o andamento deste pedido.";


                if ($scope.somaSubTotalFornecedores[j].subtotalFornecedor < pedValorMin &&
                    pedStatusId === 23) {

                    SweetAlert.swal({
                        title: "ATENÇÃO!",
                        text: mensagemValorPedMinNaoAtingiu,
                        type: "warning"
                    });
                }
            }

            $templateCache.remove("scripts/SPACliente/pedidos/detPedido.html");

            $modal.open({
                templateUrl: 'scripts/SPACliente/pedidos/detPedido.html',
                controller: 'detPedidoCtrl',
                //backdrop: 'static',
                scope: $scope,
                size: 'lg'
            });
        }
        //5------------------------fim-----------------------------


        //6------Só habilita no status Aguardando Aprovação--------
        function habilitaAprovacaoPedido(statusPedido) {
            if (statusPedido == 23) {
                return true;
            }
            return false;

        }
        //6------------------------fim-----------------------------


        //7------Abre Modal com os melhores lances por fornecedor--
        function openModalLancesPorFornecedor(pPedido) {
            $scope.pPedido = pPedido;
            $modal.open({
                templateUrl: 'scripts/SPACliente/relatorios/modalLanceFornecedor.html',
                controller: 'modalLanceFornecedorCtrl',
                backdrop: 'static',
                scope: $scope,
                size: 'lg'
            }).result.then(function ($scope) {

                //console.log("Modal Closed!!!");

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        }
        //7------------------------fim-----------------------------


        //--Trata abertura da tela quando vindo do menu de Avisos--
        if ($rootScope.Referencia != undefined) {
            $scope.PesquisaPedidoId = angular.copy($rootScope.Referencia.Id);
            if ($rootScope.Referencia.TipoAvisosId === 12) {
                //Pedido Excluido
                clienteUtilService.limpaAvisos($scope.PesquisaPedidoId, $rootScope.Referencia.TipoAvisosId);
            }
            $rootScope.Referencia = undefined;
        }
        //---------------------------------------------------------

        loadStatusPedido();
    }

})(angular.module('ECCCliente'));