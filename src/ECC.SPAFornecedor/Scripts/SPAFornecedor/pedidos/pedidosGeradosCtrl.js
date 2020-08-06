
(function (app) {
    'use strict';

    app.controller('pedidosGeradosCtrl', pedidosGeradosCtrl);

    pedidosGeradosCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$stateParams', '$modal', '$filter', 'SweetAlert'];

    function pedidosGeradosCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $stateParams, $modal, $filter, SweetAlert) {

        $scope.DtAte = Date.now();
        var date = new Date($scope.DtAte);
        //Subtrai 1 mês da data informada
        date.setMonth(date.getMonth() - 2, date.getDate());
        $scope.DtDe = date;
        //console.log(date);
        $scope.pageClass = 'page-pedidosGeradosCtrl';
        $scope.entregaEfetuada = entregaEfetuada;
        $scope.openDialogDetalhesPedidoGerado = openDialogDetalhesPedidoGerado;
        $scope.validarDtDeBuscaProdutos = validarDtDeBuscaProdutos;
        $scope.validarDtAteBuscaProdutos = validarDtAteBuscaProdutos;
        $scope.existeAguardandoEntrega = false;
        $scope.pedidos = [];
        $scope.loadPedidos = loadPedidos;
        $scope.StatusEntrega = [];
        $scope.despachoParaEntrega = despachoParaEntrega;
        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
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

        function openDatePickerDe($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepickerDe.opened = true;
        };

        function openDatePickerAte($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepickerAte.opened = true;
        };

        $scope.openDatePickerAte = openDatePickerAte;
        $scope.openDatePickerDe = openDatePickerDe;

        $scope.datepickerDe = {};
        $scope.datepickerAte = {};
        $scope.format = 'dd/MM/yyyy';


        //1-----Carrega pedidos -----------------------------------
		function loadPedidos(page) {

			var a = moment($scope.DtDe);
			var b = moment($scope.DtAte);
			var days = b.diff(a, 'days');

			if (days > 63) {

				notificationService.displayInfo('Limite máximo de 60 dias!');
				return;
			}

            page = page || 0;
            var config = {
                params: {

                    page: page,
                    pageSize: 8,
                    dtDe: $filter('date')($scope.DtDe, "dd/MM/yyyy"),
                    dtAte: $filter('date')($scope.DtAte, "dd/MM/yyyy"),
                    pedidoId: $scope.pedidoId
                }
            };
            apiService.get('/api/pedido/pedidosFornecedorAndamento', config,
                loadPedidosSucesso,
                loadPedidosFalha);
        }

        function loadPedidosSucesso(response) {

            $scope.pedidos = response.data.Items;
            $scope.page = response.data.Page;
            $scope.pagesCount = response.data.TotalPages;
            $scope.totalCount = response.data.TotalCount;

            //Cria array de Prazos por dia da semana caso o fornecedor trabalhe com o mesmo.
            for (var i = 0; i < $scope.pedidos.length; i++) {

                var contador = 0;
                var contadori = 0;
                $scope.pedidos[i].PedidoEntregue = false;

                if ($scope.pedidos[i].FornecedorPrazoSemanal !== null) {
                    for (var j = 0; j < $scope.pedidos[i].FornecedorPrazoSemanal.length; j++) {
                        for (var k = 0; k < $scope.arrayDiasSemana.length; k++) {
                            if ($scope.pedidos[i].FornecedorPrazoSemanal[j].DiaSemana === $scope.arrayDiasSemana[k].Id) {
                                $scope.pedidos[i].FornecedorPrazoSemanal[j].DescSemana = $scope.arrayDiasSemana[k].DescSemana;
                            }
                        }
                    }
                }

                //Da o desconto para o pedido de acordo com o tipo da forma de pagamento
                for (var l = 0; l < $scope.pedidos[i].Itens.length; l++) {
                    var formaPagtoId = $scope.pedidos[i].Itens[l].FormaPagtoId;
                    for (var m = 0; m < $scope.pedidos[i].Itens[l].Fornecedor.FormaPagtos.length; m++) {
                        if ($scope.pedidos[i].Itens[l].Fornecedor.FormaPagtos[m].FormaPagtoId === formaPagtoId) {
                            var desconto = ($scope.pedidos[i].ValorTotal * $scope.pedidos[i].Itens[l].Fornecedor.FormaPagtos[m].Desconto) / 100;
                            var objQuantidade = {
                                ValorTotalAvista: $scope.pedidos[i].ValorTotal - desconto
                            };
                            $scope.pedidos[i].Quantidade = objQuantidade;
                        }
                    }

                    if ($scope.pedidos[i].Itens[l].EntregaId >= 2 && contador === 0) {
                        $scope.pedidos[i].PedidoEntregue = true;
                        $scope.pedidos[i].PedidoDespachado= true;
                        contador++;
                    }


                    if ($scope.pedidos[i].Itens[l].flgDespacho == true  && contadori === 0) {
                       
                        $scope.pedidos[i].PedidoDespachado = true;
                        contadori++;
                    }
                }
            }

            if ($scope.pedidos == undefined || $scope.pedidos == null || $scope.pedidos.length < 1) {
                SweetAlert.swal({
                    title: "NÃO EXISTEM PEDIDOS PARA CONSULTA!",
                    text: "Por favor aguarde, até que existam novos pedidos.", //VALIDAR FRASE
                    type: "success",
                    html: true
                });
            }

        }

        function loadPedidosFalha(response) {
            notificationService.displayError(response.data);
        }
        //1------------------------fim-----------------------------

        //2-----Carrega Todos Status de Pedido --------------------
        function loadStatusEntrega() {

            apiService.get('/api/entrega/entregaStatus', null,
                loadStatusEntregaSucesso,
                loadStatusEntregaFalha);
        }

        function loadStatusEntregaSucesso(response) {
            $scope.StatusEntrega = response.data;
        }

        function loadStatusEntregaFalha(response) {
            notificationService.displayError(response.data);
        }
        //2------------------Fim-----------------------------------

        //3 Passamos os ítens de pedido, para alterar o campo Entregue para True.
        // Não podemos realizar update no Pedido para entregue pois o pedido pode ter outros fornecedores
        // que ainda não entregaram os pedidos.
        function entregaEfetuada(itensPedido) {
            apiService.post('/api/pedido/itensPedidoEntregue/', itensPedido,
                entregaEfetuadaSucesso,
                entregaEfetuadaFalha);
        }

        function entregaEfetuadaSucesso(result) {

            if (result.data.success) {

                SweetAlert.swal({
                    title: "PEDIDO ENTREGUE COM SUCESSO!",
                    text: "Processo finalizado, continue fazendo novos negócios",
                    type: "success",
                    html: true
                },
                    function () {
                        loadPedidos();
                        //avaliarEntrega(result.data.pedidoId);
                    });
            }

        }

        function entregaEfetuadaFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        function openDialogDetalhesPedidoGerado(pedido) {

            $scope.detahePedidoGerado = pedido;

            if ($scope.detahePedidoGerado.FornecedorPrazoSemanal !== null) {
                for (var j = 0; j < $scope.detahePedidoGerado.FornecedorPrazoSemanal.length; j++) {
                    for (var k = 0; k < $scope.arrayDiasSemana.length; k++) {
                        if ($scope.detahePedidoGerado.FornecedorPrazoSemanal[j].DiaSemana === $scope.arrayDiasSemana[k].Id) {
                            $scope.detahePedidoGerado.FornecedorPrazoSemanal[j].DescSemana = $scope.arrayDiasSemana[k].DescSemana;
                        }
                    }
                }
            }

            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/pedidos/modalDetPedidoGerado.html',
                controller: 'modalDetPedidoGeradoCtrl',
                scope: $scope,
                size: 'lg'
            });
        }
        //3---------------------------------------------------------

        //4-----------Validar Datas Pesquisa Produto---------------
        function validarDtDeBuscaProdutos() {
            if ($scope.DtDe < date) {
                notificationService.displayInfo('Limite máximo de 60 dias!');
                $scope.DtDe = date;
            } else if ($scope.DtDe > $scope.DtAte) {
                notificationService.displayInfo('Data não pode ser maior que data final!');
                $scope.DtDe = date;
            }

        }
        
        function validarDtAteBuscaProdutos() {
            if ($scope.DtAte < $scope.DtDe) {
                notificationService.displayInfo('Data não pode ser menor que data inicial!');
                $scope.DtAte = Date.now();
            } else if ($scope.DtAte > Date.now()) {
                notificationService.displayInfo('Data não pode ser maior que hoje!');
                $scope.DtAte = Date.now();
            }
        }
        //4-----------------------Fim------------------------------
               
        //005 Informamos ao membro comprador que itens do seus pedido foram despachados para entrega.
        // Não podemos informa que o pedido foi despachados, pois o pedido pode ter outros fornecedores
        // que ainda não despacharam para entrega.
       function despachoParaEntrega(itensPedido) {
            apiService.post('/api/pedidoDespacho/itensPedidoDespachado/', itensPedido,
                despachoParaEntregaSucesso,
                despachoParaEntregaFalha);
        }

        function despachoParaEntregaSucesso(result) {

            if (result.data.success) {

                SweetAlert.swal({
                    title: "O MEMBRO ACABOU DE SER INFORMADO QUE O PEDIDO FOI DESPACHADO!",
                    text: "OBRIGADO POR INFORMA-LÔ!",
                    type: "success",
                    html: true
                },
                    function () {
                        loadPedidos();
                        //avaliarEntrega(result.data.pedidoId);
                    });
            }

        }

        function despachoParaEntregaFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
                
        loadStatusEntrega();
        loadPedidos();

    }

})(angular.module('ECCFornecedor'));