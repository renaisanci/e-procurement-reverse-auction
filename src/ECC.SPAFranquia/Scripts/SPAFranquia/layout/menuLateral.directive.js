(function (app) {
    'use strict';

    app.directive('menuLateral', menuLateral);

    function menuLateral() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/scripts/SPAFranquia/layout/menuLateral.html',
            controller: 'menuLateralFraCtrl'

        }
    }

})(angular.module('common.ui'));