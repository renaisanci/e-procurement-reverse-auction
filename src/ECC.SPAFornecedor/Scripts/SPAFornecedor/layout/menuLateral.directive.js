(function (app) {
    'use strict';

    app.directive('menuLateral', menuLateral);

    function menuLateral() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/scripts/SPAFornecedor/layout/menuLateral.html',
            controller: 'menuLateralFornCtrl'

        }
    }

})(angular.module('common.ui'));