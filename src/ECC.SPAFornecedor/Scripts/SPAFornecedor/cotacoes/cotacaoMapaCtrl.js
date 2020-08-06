
(function (app) {
    'use strict';

    app.controller('cotacaoMapaCtrl', pedidoMapaCtrl);

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
                    cotacaoId: $scope.idCotacao
                }
            };

            apiService.get('/api/cotacao/mapa', config,
                    loadMapaSucesso,
                    loadMapaFalha);
        }

        function loadMapaSucesso(response) {
            var fornecedor = response.data.Fornecedor;
            var pedidos = response.data.Pedidos;

            markers.push(mapaService.createMaker(fornecedor, 'Meu Endereço', 'blue', 'fornecedor', 0));

            $.each(pedidos, function (i, item) {
                markers.push(mapaService.createMaker(item, item.Nome, 'green', 'cliente', i + 1));
            });

            $scope.markers = markers;

            $timeout(function () {
                centerMap();
            }, 1000);
        }

        function loadMapaFalha(response) {
            notificationService.displayError(response.data);
        }

        function centerMap() {
            var bounds = new google.maps.LatLngBounds();

            for (var i = 0; i < markers.length; i++) {
                var geoCode = new google.maps.LatLng(markers[i].latitude, markers[i].longitude);
                bounds.extend(geoCode);
            }

            map.setCenter(bounds.getCenter());
            map.fitBounds(bounds);
        };

        //---------Fornecedor e Clientes do Mapa---------------------

        //---------Informações Pin---------------------

        $scope.map.window = {
            coords: { latitude: 0, longitude: 0 },
            options: { },
            source: {}
        };

        function openGWindow(marker, eventName, model) {
            if (model.type != 'cliente') {
                $scope.show = false;
                return;
            }
            $scope.map.window.coords.latitude = model.latitude;
            $scope.map.window.coords.longitude = model.longitude;
            $scope.map.window.source.Nome = model.source.Nome;
            $scope.map.window.source.Distancia = 'Distância: ' + model.source.Distancia;
            $scope.show = true;
        };

        $scope.closeClick = function () {
            $scope.show = false;
        };

        $scope.show = false;

        //---------Informações Pin---------------------

        //---------Mapa---------------------

        //---------Fecha Modal---------------------
        $scope.close = function () {
            $modalInstance.dismiss('cancel');
        };
        //------------------------------------------
    }

})(angular.module('ECCFornecedor'));