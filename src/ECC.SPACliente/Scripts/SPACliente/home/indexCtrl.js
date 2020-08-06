/// <reference path="../termoUso/aceiteTermoUso.html" />
(function (app) {
    'use strict';

    app.controller('indexCtrl', indexCtrl);

    indexCtrl.$inject = ['$scope', 'apiService', 'notificationService', 'Hub', '$rootScope', '$modal', 'storeService', '$filter', '$location', 'SweetAlert'];

    function indexCtrl($scope, apiService, notificationService, Hub, $rootScope, $modal, storeService, $filter, $location, SweetAlert) {


        $scope.pageClass = 'page-home';
        $scope.prodAgrupados = {};
        $scope.openModalBoaCompra = openModalBoaCompra;
        $scope.loadPedAgrupados = loadPedAgrupados;
        $scope.cart = storeService.cart;
        $scope.openDetProdDialog = openDetProdDialog;


        //02-------------Carrega grid de produtos agrupados dos pedidos que nao foram cotados---------
        function loadPedAgrupados() {

            //var data = dataPedido !== undefined ? dataPedido : new Date();
            var data = $scope.pedidos !== undefined ? $scope.pedidos : new Date();

            var config = {
                params: {
                    dataCotacao: $filter('date')(data, 'dd/MM/yyyy')
                }
            };

            apiService.get("/api/pedido/totalPedidoGroup", config,
                loadPedAgrupadosCompleted,
                loadPedAgrupadosFailed);
        }

        function loadPedAgrupadosFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadPedAgrupadosCompleted(response) {

            $scope.prodAgrupados = response.data.prodsGroup;
            $scope.caminhoImg = response.data.cam;

            $scope.loadingProdG = false;
        }
        //02------------------------------------------------------------------------------------------


        $rootScope.$on("recebeProdGroup", function (event, data, cam) {
            //$scope.prodAgrupados = data;
            //$scope.caminhoImg = cam;

            //necessario carregar o monitor novamente pq se mandar depois q um membro qualquer gerar pedido o membro q gerou o pedido pode ter uma categoria q outros q vao receber nao tem
            //Author: Renato Fonseca
            loadPedAgrupados();
        });

        function openModalBoaCompra(pedido) {
            $scope.pedido = pedido;
            $scope.StatusPedido = $scope.StatusPedido;

            $modal.open({
                templateUrl: 'scripts/SPACliente/home/infoBoaCompra.html',
                controller: 'infoBoaCompraCtrl',
                scope: $scope,
                size: 'md'
            }).result.then(function ($scope) {
                //console.log("Modal Closed!!!");
            }, function () {
                //console.log("Modal Dismissed!!!");
            });
        }

        function openDetProdDialog(prod) {
            $scope.produtoPesq = prod;

            var caminhoImagem = $scope.caminhoImg;

            $scope.produtoPesq.ImagemGrande = caminhoImagem + $scope.produtoPesq.categoria + '/' + $scope.produtoPesq.subcategoria + '/' + $scope.produtoPesq.ImagemG;

            //$scope.produtoPesq.Imagem = caminhoImagem + $scope.produtoPesq.categoria + '/' + $scope.produtoPesq.subcategoria + '/' + prod.Imagem;

            $scope.produtoPesq.Ranking = ($scope.produtoPesq.notas / $scope.produtoPesq.contNota);

            $scope.isMonitor = true;

            $modal.open({
                templateUrl: 'scripts/SPACliente/produto/detProduto.html',
                controller: 'detProdutoCtrl',
                scope: $scope
            }).result.then(function ($scope) {
                //console.log("Modal Closed!!!");
                loadPedAgrupados();
            }, function () {
                //console.log("Modal Dismissed!!!");
                loadPedAgrupados();
            });
        }

        //--------------------------------------------------------------------------------------------


        //03----------------Aceite do Termo de Uso----------------------------------------------------
        function aceiteTermoUso() {
            apiService.get("/api/usuario/verificaTermoUso", null,
                aceiteTermoUsoCompleted,
                aceiteTermoUsoFailed);
        }

        function aceiteTermoUsoCompleted(response) {
            //console.log(response.data);

            if (!response.data.ExibirAceite) return;

            $scope.TermoUso = response.data.TermoUso;

            $modal.open({
                backdrop: 'static',
                keyboard: false,
                templateUrl: 'scripts/SPACliente/termoUso/aceiteTermoUso.html',
                controller: 'aceiteTermoUsoCtrl',
                scope: $scope,
                size: 'lg-ecc'
            }).result.then(function ($scope) {
            }, function () {
            });
        }

        function aceiteTermoUsoFailed(response) {
            notificationService.displayError(response.data);
        }
        //03------------------------------------------------------------------------------------------

        //04----------------Aceite do Termo de Uso----------------------------------------------------
        function carregaTotalPedidosPorData() {
            apiService.get('/api/pedido/totalPedidosPorData', null,
                loadcarregaQtdPedidosPorDataCompleted,
                loadcarregaQtdPedidosPorDataFailed);
        }

        function loadcarregaQtdPedidosPorDataCompleted(result) {

            $scope.arrayQtdPedidosData = [];
            var objetoPedidosData = {};
            var totalPedidosPorData = result.data.listaQtdPedData;

            var dataHojeServidor = result.data.DataHojeServidor;
            var data = $filter('date')(dataHojeServidor, 'dd/MM/yyyy');
            var hora = $filter('date')(dataHojeServidor, 'HH:mm:ss');

            var horaCotacao = result.data.HoraCotacao;

            if (totalPedidosPorData.length > 0) {

                for (var i = 0; i < totalPedidosPorData.length; i++) {

                    if (data === totalPedidosPorData[i].data && hora < horaCotacao) {

                        objetoPedidosData = {
                            title: 'Hoje',
                            data: totalPedidosPorData[i].data
                        }

                    } else {
                        objetoPedidosData = {
                            title: totalPedidosPorData[i].qtd > 1 ?
                                totalPedidosPorData[i].data + " - " + totalPedidosPorData[i].qtd + ' Pedidos' :
                                totalPedidosPorData[i].data + " - " + totalPedidosPorData[i].qtd + ' Pedido',
                            data: totalPedidosPorData[i].data
                        }
                    }

                    $scope.arrayQtdPedidosData.push(objetoPedidosData);
                }
                $scope.pedidos = totalPedidosPorData[0].data;
                loadPedAgrupados();
            }
        }

        function loadcarregaQtdPedidosPorDataFailed(result) {

            notificationService.displayError(result.data);
        }
        //04------------------------------------------------------------------------------------------

        function verificaPeriodoGratuitoMembro() {

            if ($rootScope.repository.loggedUser.faturaMensalidade) {

                SweetAlert.swal({
                    title: "Período Gratuito Expirou :(",
                    text: "Escolha um ''Plano'', ou vá em ''Pagamentos''\n" +
                        "para verificar possíveis pendências financeiras.",
                   // type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Planos",
                    cancelButtonText: "Pagamentos",
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $location.path("/perfil/planos");
                        } else {
                            $location.path("/pagamento/pendente");
                        }
                    });
            }
        }


        aceiteTermoUso();
        carregaTotalPedidosPorData();
        verificaPeriodoGratuitoMembro();
    }

})(angular.module('ECCCliente'));