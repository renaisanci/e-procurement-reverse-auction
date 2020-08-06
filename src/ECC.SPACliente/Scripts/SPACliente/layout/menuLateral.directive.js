(function (app) {
    'use strict';

    app.directive('menuLateral', menuLateral);

    function menuLateral() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/scripts/SPACliente/layout/menuLateral.html',
            controller: 'menuLateralCtrl'

        }
    }

})(angular.module('common.ui'));