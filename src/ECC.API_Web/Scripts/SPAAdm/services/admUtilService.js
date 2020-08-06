(function (app) {
    'use strict';

    app.factory('admUtilService', admUtilService);

    admUtilService.$inject = ['$http', '$location', 'notificationService', '$rootScope'];

    function admUtilService($http, $location, notificationService, $rootScope) {
        var service = {
            checkBoxAll: checkBoxAll,
            moedaDecimal: moedaDecimal
        };


        function checkBoxAll(itens, chk) {
            if (chk) {
                chk = true;
            } else {
                chk = false;
            }
            angular.forEach(itens, function(item) {
                item.selected = chk;
            });
        }

        //Converte Moeda Real para Decimal
        function moedaDecimal(moeda) {

            moeda = moeda.replace(".", "");

            moeda = moeda.replace(",", "");

            return parseFloat(moeda);
        }
 

        return service;
    }

})(angular.module('common.core'));