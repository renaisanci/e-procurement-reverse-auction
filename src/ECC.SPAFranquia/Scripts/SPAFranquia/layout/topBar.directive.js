(function (app) {
    'use strict';

    app.directive('topBar', topBar);

    function topBar() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/scripts/SPAFranquia/layout/topBar.html',
            controller: 'topBarCtrl'
        }
    }

})(angular.module('common.ui'));