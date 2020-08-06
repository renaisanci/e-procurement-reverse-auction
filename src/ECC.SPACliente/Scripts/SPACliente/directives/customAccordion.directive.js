(function (app) {
    'use strict';

    app.directive('customAccordion', customAccordion);

    app.$inject = ['$scope', 'apiService'];

    function customAccordion(apiService) {
        return {
            restrict: 'C',
            link: function (scope, element, attrs) {


                $('#accordion-style').on('click', function (ev) {
                    var target = $('input', ev.target);
                    var which = parseInt(target.val());
                    if (which == 2) $('#accordion').addClass('accordion-style1');
                    else $('#accordion').removeClass('accordion-style1');
                });
            }
        }
    }
})(angular.module('common.ui'));