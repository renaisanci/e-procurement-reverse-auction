
(function (app) {
    'use strict';

    app.controller('avaliarEntregaCtrl', avaliarEntregaCtrl);

    avaliarEntregaCtrl.$inject = ['$scope', 'apiService', 'notificationService', 'SweetAlert', '$modal'];

    function avaliarEntregaCtrl($scope, apiService, notificationService, SweetAlert, $modal) {

        $scope.pageClass = 'page-avaliarEntrega';
        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.pesquisarAguardandoEntrega = pesquisarAguardandoEntrega;
        $scope.detalhesPedido = detalhesPedido;
        $scope.finalizarPedido = finalizarPedido;
        $scope.atualizaStatusItensPedido = atualizaStatusItensPedido;
        $scope.limparCampos = limparCampos;
        $scope.avaliarFornecedor = avaliarFornecedor;

        $scope.fornecedor = {};
        $scope.Pedidos = [];
        $scope.PedidoFornecedor = [];
        $scope.PedidosNaoAvaliados = [];
        $scope.novoDetalhesPedido = {};
        $scope.ped = {};
        $scope.somaTotalPrecoNegociado = 0;
        $scope.somaTotalPrecoAvista = 0;
        $scope.novaAvaliacaoFornecedor = {};
        $scope.painelObsQualidade = false;
        $scope.painelObsEntrega = false;
        $scope.painelObsAtendimento = false;
        $scope.avaliacaoPendente = false;
        $scope.comboNomeFornecedorModal = false;

        $scope.arrayDiasSemana = [
        { Id: 1, DescSemana: 'Seg' },
        { Id: 2, DescSemana: 'Ter' },
        { Id: 3, DescSemana: 'Qua' },
        { Id: 4, DescSemana: 'Qui' },
        { Id: 5, DescSemana: 'Sex' },
        { Id: 6, DescSemana: 'Sáb' },
        { Id: 7, DescSemana: 'Dom' }
        ];
        
        //0--------Declaracao de todas as abas de tela de novo fornecedor--------
        $scope.tabsAvlEntrega = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadAvlEntrega: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            }
        };
        //0------------------------Fim--------------------------------------------------

        function habilitaDesabilitaAbaPesquisa() {

            $scope.tabsAvlEntrega = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadAvlEntrega: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };

            pesquisarAguardandoEntrega();

        }
        
        //1---------------------Carregar Pedidods Aguardando Entrega--------------------
        function pesquisarAguardandoEntrega(page) {

            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 10,
                    filter: $scope.filtroPedido
                }
            };

            apiService.get('/api/pedido/pesquisaAguardandoEntregaPedido', config,
             pesquisarAguardandoEntregaLoadCompleted,
             pesquisarAguardandoEntregaCompletedLoadFailed);

        }

        function pesquisarAguardandoEntregaLoadCompleted(result) {


            var ped = result.data.pedidos.Items;
            $scope.PedidosNaoAvaliados = result.data.pedidosNavaliados;
            $scope.fornecedoresPendentesAvaliacao = result.data.fornecedores;

            if ($scope.PedidosNaoAvaliados.length > 0) {

                $scope.avaliacaoPendente = true;

                for (var i = 0; i < ped.length; i++) {

                    for (var j = 0; j < $scope.PedidosNaoAvaliados.length; j++) {
                        if (ped[i].PedidoId === $scope.PedidosNaoAvaliados[j]) {

                            ped[i].NaoAvaliado = true;
                        }
                    }

                }
            } else {
                $scope.avaliacaoPendente = false;
            }

            $scope.Pedidos = ped;

            $scope.page = result.data.pedidos.Page;
            $scope.pagesCount = result.data.pedidos.TotalPages;
            $scope.totalCount = result.data.pedidos.TotalCount;
        }

        function pesquisarAguardandoEntregaCompletedLoadFailed(result) {

            notificationService.displayError(result.data + result.data.statusMessage);
        }
        //1-----------------------------------------------------------------------------


        //2---------------------Detalhes Pedido-----------------------------------------
        function detalhesPedido(pedido) {

            var uniqueItems = function (data, key) {
                var result = [];

                for (var i = 0; i < data.length; i++) {
                    var value = data[i][key];
                    var resultado = result.indexOf(value);
                    if (resultado === -1) {
                        if (value != null)
                            result.push(value);
                    }

                }
                return result;
            };

            var precoNegociado = 0;
            var somaPrecoNegociado = 0;


            var novoPedido = pedido;

            angular.forEach(pedido.Itens, function (value, key) {

                if (value.Ativo) {

                    precoNegociado = value.PrecoNegociadoUnit * value.quantity;

                    somaPrecoNegociado = precoNegociado + somaPrecoNegociado;

                    if (value.EntregaId === 3) {
                        novoPedido.Itens[key].entregaMembro = true;
                    } else {
                        novoPedido.Itens[key].entregaMembro = false;
                    }

                    if (value.FornecedorId == null) {
                        var objeto = value;

                        var indice = novoPedido.Itens.indexOf(objeto);

                        novoPedido.Itens.splice(indice, 1);

                        if (novoPedido.Itens[key].EntregaId === 3) {
                            novoPedido.Itens[key].entregaMembro = true;
                        }
                    }
                }
                
            });

            $scope.novoDetalhesPedido = novoPedido;

            $scope.somaTotalPrecoNegociado = somaPrecoNegociado;


            //#region Separa Fornecedores e Soma o valor de Itens dos mesmos

            $scope.somaSubTotalFornecedores = [];
            var fornecedores = uniqueItems(pedido.Itens, "FornecedorId");

            //Soma todos os Itens do Pedido por Fornecedor
            for (var j = 0; j < fornecedores.length; j++) {
                var cont = 0;
                for (var k = 0; k < pedido.Itens.length; k++) {

                    if (fornecedores[j] === pedido.Itens[k].FornecedorId && pedido.Itens[k].Ativo) {

                        if (cont === 0) {
                            var objFornecedor = {
                                Fornecedor: pedido.Itens[k].Fornecedor,
                                somaTotalItens: pedido.Itens[k].PrecoNegociadoUnit * pedido.Itens[k].quantity,
                                FormaPagtoId: pedido.Itens[k].FormaPagtoId,
                                Desconto: pedido.Itens[k].Desconto
                            };

                            $scope.somaSubTotalFornecedores.push(objFornecedor);
                            cont++;
                            continue;
                        }


                        for (var l = 0; l < $scope.somaSubTotalFornecedores.length; l++) {
                            if ($scope.somaSubTotalFornecedores[l].Fornecedor.Id === fornecedores[j]) {
                                $scope.somaSubTotalFornecedores[l].somaTotalItens += (pedido.Itens[k].PrecoNegociadoUnit * pedido.Itens[k].quantity);
                            }
                        }
                    }
                }
            }

            //Pega os dias da Semana de cada Fornecedor, caso exista
            for (var r = 0; r < $scope.somaSubTotalFornecedores.length; r++) {
                if ($scope.somaSubTotalFornecedores[r].Fornecedor.FornecedorPrazoSemanal.length > 0) {
                    for (var y = 0; y < $scope.somaSubTotalFornecedores[r].Fornecedor.FornecedorPrazoSemanal.length; y++) {
                        for (var x = 0; x < $scope.arrayDiasSemana.length; x++) {
                            if ($scope.somaSubTotalFornecedores[r].Fornecedor.FornecedorPrazoSemanal[y].DiaSemana === $scope.arrayDiasSemana[x].Id) {
                                $scope.somaSubTotalFornecedores[r].Fornecedor.FornecedorPrazoSemanal[y].DescDiaSemana = $scope.arrayDiasSemana[x].DescSemana;
                                break;
                            }
                        }
                    }
                }
            }


            //Calcula Desconto a Vista
            $scope.somaTotalPrecoAvista = 0;
            for (var m = 0; m < $scope.somaSubTotalFornecedores.length; m++) {

                var formaPagto = $scope.somaSubTotalFornecedores[m].Fornecedor.FormaPagtos;
                var descontoItens = $scope.somaSubTotalFornecedores[m].Desconto;

                for (var n = 0; n < formaPagto.length; n++) {
                    if (formaPagto[n].FormaPagtoId === $scope.somaSubTotalFornecedores[m].FormaPagtoId) {
                        var valorDesconto = ($scope.somaSubTotalFornecedores[m].somaTotalItens * descontoItens) / 100;
                        $scope.somaSubTotalFornecedores[m].somaTotalItensAvista = $scope.somaSubTotalFornecedores[m].somaTotalItens - valorDesconto;
                    }
                }

                $scope.somaTotalPrecoAvista += $scope.somaSubTotalFornecedores[m].somaTotalItensAvista;
            }

            //#endregion


            $scope.tabsAvlEntrega = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadAvlEntrega: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };


        }
        //2-----------------------------------------------------------------------------


        //3-----------------------------------------------------------------------------
        function atualizaStatusItensPedido(Item) {

            $scope.ItensPedido = Item.itens;

            if (Item.itens.entregaMembro) {

                //chama método para falar que foi entregue pelo fornecedor
                apiService.post('/api/pedido/itensPedidoEntregueMembro/' + true, Item.itens,
                    atualizaStatusItensPedidoLoadCompleted,
                    atualizaStatusItensPedidoLoadFailed);         

            }
            else {

                //chama método para falar que não foi entregue pelo fornecedor
                apiService.post('/api/pedido/itensPedidoEntregueMembro/' + false, Item.itens,
                atualizaStatusItensPedidoNaoEntregueLoadCompleted,
                atualizaStatusItensPedidoNaoEntregueLoadFailed);

            }
		}

		function atualizaStatusItensPedidoNaoEntregueLoadCompleted(result) {

			var fornecedor = Item.itens.Fornecedor;

			for (var i = 0; i < $scope.PedidoFornecedor.length; i++) {
				if ($scope.PedidoFornecedor[i].Id === fornecedor.Id) {

					var index = $scope.PedidoFornecedor.indexOf(fornecedor);
					$scope.PedidoFornecedor.splice(index, 1);
				}

			}

			//no retorno verifico se todos os itens do fonecedor foi checado, caso sim abro um modal para avaliar o fornecedor.
			notificationService.displaySuccess('Status alterado para não entregue!')
		};


		function atualizaStatusItensPedidoNaoEntregueLoadFailed(result) {

			notificationService.displayError(result.data);
		};

		function atualizaStatusItensPedidoLoadCompleted(result) {


			notificationService.displaySuccess('Status alterado para entregue!');

			//no retorno verifico se todos os itens do fonecedor foi checado, caso sim abro um modal para avaliar o fornecedor.
			if (result.data.itens == 0 && result.data.avaliacao == 0) {

				$scope.PedidoFornecedor.push($scope.ItensPedido.Fornecedor);

				$scope.fornecedor = $scope.PedidoFornecedor[0];

				if ($scope.PedidoFornecedor.length > 1) {
					$scope.comboNomeFornecedorModal = true;
				} else {
					$scope.comboNomeFornecedorModal = false;
				}

				$modal.open({
					animation: true,
					templateUrl: 'scripts/SPACliente/pedidos/modalAvaliacaoFornecedor.html',
					controller: 'modalAvaliacaoFornecedorCtrl',
					backdrop: 'static',
					scope: $scope,
					size: 'lg'
				});

			}

		}

		function atualizaStatusItensPedidoLoadFailed(result) {

			notificationService.displayError(result.data);
		}
        //3-----------------------------------------------------------------------------


        //4------------------------Inserir avaliação para o fornecedor-----------------------
        function avaliarFornecedor(pedido) {

            $scope.PedidoFornecedor = [];

            $scope.ped = pedido;

            var pedNaoAvaliados = angular.copy($scope.PedidosNaoAvaliados);

            var fornecedoresPendentes = angular.copy($scope.fornecedoresPendentesAvaliacao);

            for (var i = 0; i < pedNaoAvaliados.length; i++) {
                if ($scope.ped.PedidoId === pedNaoAvaliados[i]) {

                    var cont = 0;
                    for (var j = 0; j < $scope.ped.Itens.length; j++) {
                        if ($scope.ped.Itens[j].FornecedorId === fornecedoresPendentes[cont]) {
                            $scope.PedidoFornecedor.push($scope.ped.Itens[j].Fornecedor);
                            cont++;
                        }
                    }

                    $scope.fornecedor = $scope.PedidoFornecedor[0];

                    //Mostra ou esconde select no modal
                    if ($scope.PedidoFornecedor.length > 1) {
                        $scope.comboNomeFornecedorModal = true;
                    } else {
                        $scope.comboNomeFornecedorModal = false;
                    }

                    $modal.open({
                        animation: true,
                        templateUrl: 'scripts/SPACliente/pedidos/modalAvaliacaoFornecedor.html',
                        controller: 'modalAvaliacaoFornecedorCtrl',
                        backdrop: 'static',
                        scope: $scope,
                        size: 'lg'
                    });
                }
            }

        }
        //4------------------------------------------------------------------------------


        //5-------------------------Limpa Campos-------------------------------------------
        function limparCampos() {
            $scope.novaAvaliacaoFornecedor = {};

            $scope.PedidoFornecedor = [];

            $scope.painelObsQualidade = false;

            $scope.painelObsEntrega = false;

            $scope.painelObsAtendimento = false;

            $scope.comboNomeFornecedorModal = false;

        }
        //5--------------------------------------------------------------------------------


        //6-----------------------Finalizar Pedido----------------------------------------
        function finalizarPedido() {

            var idpedido = angular.copy($scope.novoDetalhesPedido.PedidoId);

            apiService.get('/api/pedido/pesquisaItensPedidosEntregues/' + idpedido, null,
                pesquisaItensPedidosEntreguesLoadCompleted,
                pesquisaItensPedidosEntreguesLoadFailed);

            function pesquisaItensPedidosEntreguesLoadCompleted(result) {

                var qtdItens = result.data.qtdItens;
                var avaliacao = result.data.fornecedoresAvaliados;

                if (qtdItens > 0) {
                    SweetAlert.swal({
                        title: "ATENÇÃO!",
                        text: "Confirme a entrega de todos os itens para finalizar este pedido.",
                        type: "warning",
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "Ok"
                    });

                } else if (avaliacao === false) {

                    SweetAlert.swal({
                        title: "Existe pedido pendente de avaliação!",
                        text: "Na aba de pesquisa clique no ícone em vermelho ''Avaliar Fornecedor(s)''",
                        type: "warning",
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "Ok"
                    }, function (isConfirm) {
                        if (isConfirm) {
                            habilitaDesabilitaAbaPesquisa();
                        }
                    });

                } else {
                    //Atualizo campo StatusPedido para Finalizado na tabela de pedidos
                    atualizaPedidoFinalizado(idpedido);
                }
            }

            function pesquisaItensPedidosEntreguesLoadFailed(result) {

                notificationService.displayError(result.data);
            }

        }
        //7--------------------------------------------------------------------------------


        //8--------------------Finalizando Pedido-----------------------------------------
        function atualizaPedidoFinalizado(id) {

            apiService.post('/api/pedido/atualizaStatusPedidoFinalizado/' + id, null,
                atualizaStatusPedidoFinalizadoLoadCompleted,
                atualizaStatusPedidoFinalizadoLoadFailed);

            function atualizaStatusPedidoFinalizadoLoadCompleted(result) {

                notificationService.displaySuccess('Sucesso ao finalizar pedido!');

                habilitaDesabilitaAbaPesquisa();
            }

            function atualizaStatusPedidoFinalizadoLoadFailed(result) {

                notificationService.displayError(result.data);

            }

        }
        //8--------------------Finalizando Pedido-----------------------------------------
        

        pesquisarAguardandoEntrega();
    }

})(angular.module('ECCCliente'));