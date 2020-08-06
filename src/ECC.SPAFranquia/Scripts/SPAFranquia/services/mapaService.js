(function (app) {
    'use strict';

    app.factory('mapaService', mapaService);

    mapaService.$inject = ['$http', '$location', 'notificationService', '$rootScope', 'uiGmapGoogleMapApi', 'uiGmapIsReady'];

    function mapaService($http, $location, notificationService, $rootScope, uiGmapGoogleMapApi, uiGmapIsReady) {
        var service = {
            pushPin: pushPin(),
            createMaker: createMaker,
            centerMap: centerMap
        };

        function pushPin() {
            return {
                'red': {
                    url: '/Content/images/push_pin_red.png',
                    scaledSize: new google.maps.Size(38, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 32)
                },
                'green': {
                    url: '/Content/images/push_pin_green.png',
                    scaledSize: new google.maps.Size(38, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 32)
                },
                'blue': {
                    url: '/Content/images/push_pin_blue.png',
                    scaledSize: new google.maps.Size(38, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 32)
                },
                'yellow': {
                    url: '/Content/images/push_pin_yellow.png',
                    scaledSize: new google.maps.Size(38, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 32)
                },
                'purple': {
                    url: '/Content/images/push_pin_purple.png',
                    scaledSize: new google.maps.Size(38, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(10, 32)
                }
            };
        }

        function createMaker(source, label, pin, type, index) {
            var ret = {
                latitude: source.Latitude,
                longitude: source.Longitude,
                icon: service.pushPin[pin],
                type: type,
                source: source,
                options: {
                    labelContent: label,
                    labelAnchor: '10 44',
                    labelClass: 'marker-labels',
                    labelVisible: true
                }
            };

            ret["id"] = index;

            return ret;
        }

        function centerMap(map, markers) {
            var bounds = new google.maps.LatLngBounds();

            for (var i = 0; i < markers.length; i++) {
                var geoCode = new google.maps.LatLng(markers[i].latitude, markers[i].longitude);
                bounds.extend(geoCode);
            }

            map.setCenter(bounds.getCenter());
            map.fitBounds(bounds);
        };

        return service;
    }

})(angular.module('common.core'));