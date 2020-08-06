
(function (app) {
    'use strict';

    app.controller('pedidoMapaCtrl', pedidoMapaCtrl);

    pedidoMapaCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'uiGmapGoogleMapApi', 'uiGmapIsReady', 'mapaService'];

    function pedidoMapaCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, uiGmapGoogleMapApi, uiGmapIsReady, mapaService) {

        $scope.close = close;

        //---------Mapa---------------------
        $scope.map = {
            center: { latitude: -15, longitude: -50 },
            zoom: 4,
            fit: true,
            bounds: {}
        };
        $scope.readyForMap = true;
        $scope.control = {};
        $scope.options = {
            scrollwheel: true,
            panControl: true,
            zoomControl: true,
            scaleControl: true,
            mapTypeControl: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        $scope.markersEvents = {
            click: openGWindow,
            mouseover: function (marker, eventName, model) {
            },
            mouseout: function (marker, eventName, model) {
                /*console.log(marker);
                console.log(eventName);
                console.log(model);*/
            }
        };

        var markers = [];
        $scope.markers = markers;

        var map;

        uiGmapIsReady.promise().then(function (maps) {
            map = maps[0].map;
            map.setCenter(new google.maps.LatLng(-15, -50));
            map.setZoom(4);
            $scope.control.refresh();
            loadMapa();
        });

        //---------Fornecedor e Clientes do Mapa---------------------
        function loadMapa() {

            var config = {
                params: {
                    pedidoId: $scope.idPedido
                }
            };

            apiService.get('/api/pedido/mapa', config,
                    loadMapaSucesso,
                    loadMapaFalha);
        }

        function loadMapaSucesso(response) {
            console.log(response);
            var fornecedor = response.data.Fornecedor;
            var pedidoSelecionado = response.data.PedidoSelecionado;
            var pedidos = response.data.PedidosCotacao;

            markers.push(mapaService.createMaker(fornecedor, 'EU', 'blue', 'fornecedor', 0));

            markers.push(mapaService.createMaker(pedidoSelecionado, pedidoSelecionado.Nome, 'yellow', 'pedido', 1));
            
            $.each(pedidos, function (i, item) {
                markers.push(mapaService.createMaker(item, item.Nome, 'green', 'cliente', i + 2));
            });

            $scope.markers = markers;

            $timeout(function () {
                mapaService.centerMap(map, $scope.markers);
            }, 1000);
        }

        function loadMapaFalha(response) {
            notificationService.displayError(response.data);
        }

        //---------Fornecedor e Clientes do Mapa---------------------

        //---------Informações Pin---------------------

        $scope.map.window = {
            coords: { latitude: 0, longitude: 0 },
            options: { },
            source: {}
        };

        function openGWindow(marker, eventName, model) {
            if (model.type == 'fornecedor') {
                $scope.show = false;
                return;
            }

            
            $scope.map.window.coords.latitude = model.latitude;
            $scope.map.window.coords.longitude = model.longitude;
            $scope.map.window.source.PedidoId = 'Pedido Id: ' + model.source.PedidoId;
            $scope.map.window.source.Nome = model.source.Nome;
            $scope.map.window.source.Distancia = 'Distância: ' + model.source.Distancia;
            $scope.map.window.source.Valor = 'Valor do Pedido: ' + model.source.Total;
            $scope.show = true;
        };

        $scope.closeClick = function () {
            $scope.show = false;
        };

        $scope.show = false;

        //---------Informações Pin---------------------
        
        //---------Mapa---------------------

        //---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------
    }

})(angular.module('ECCFornecedor'));