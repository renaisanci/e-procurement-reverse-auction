(function (app) {
    'use strict';

    app.factory('utilService', utilService);

    utilService.$inject = ['$http', '$location', 'notificationService', '$rootScope'];

    function utilService($http, $location, notificationService, $rootScope) {

        var service = {
            moedaDecimal: moedaDecimal
        };

        //Converte Moeda Real para Decimal
        function moedaDecimal(moeda) {

            moeda = moeda.replace(".", "");

            moeda = moeda.replace(",", ".");

            return parseFloat(moeda);
        }

        return service;
    }

})(angular.module('common.core'));