(function (app) {
    'use strict';

    app.directive('menuLateral', menuLateral);

    function menuLateral() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/scripts/SPAAdm/layout/menuLateral.html',
            controller: 'menuLateralAdmCtrl'
        }
    }

})(angular.module('common.ui'));