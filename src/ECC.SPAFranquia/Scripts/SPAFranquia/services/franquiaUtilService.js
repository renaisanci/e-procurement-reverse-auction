(function (app) {
    'use strict';

    app.factory('franquiaUtilService', franquiaUtilService);

    franquiaUtilService.$inject = ['$http', '$location', 'notificationService', '$rootScope', 'apiService'];

    function franquiaUtilService($http, $location, notificationService, $rootScope, apiService) {


        var service = {
            checkBoxAll: checkBoxAll
        };

        function checkBoxAll(itens, chk) {

            if (chk) {
                chk = true;
            } else {
                chk = false;
            }
            angular.forEach(itens, function (item) {
                item.selected = chk;
            });
        }

        return service;
    }

})(angular.module('common.core'));