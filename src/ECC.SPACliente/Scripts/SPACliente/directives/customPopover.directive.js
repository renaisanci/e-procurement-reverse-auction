(function (app) {
    'use strict';

    app.directive('customPopover', customPopover);

    app.$inject = ['$scope'];

    function customPopover() {
        return {
            restrict: 'C',
            link: function (scope, element, attrs) {

                $('[data-rel=popover]').popover({
                    html: true
                });
            }
        }
    }
})(angular.module('common.ui'));