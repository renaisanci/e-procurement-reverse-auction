(function (app) {
    'use strict';

    app.directive('ngCardValidator', ngCardValidator);

    var ngFor;
    var cards = {
        Visa: /^4[0-9]{12}(?:[0-9]{3})/,
        Mastercard: /^5[1-5][0-9]{14}/,
        Amex: /^3[47][0-9]{13}/,
        DinersClub: /^3(?:0[0-5]|[68][0-9])[0-9]{11}/,
        Discover: /^6(?:011|5[0-9]{2})[0-9]{12}/,
        JCB: /^(?:2131|1800|35\d{3})\d{11}/
    };

    function returnCard(value) {
        if (!value) return null;

        while (value.indexOf(" ") !== -1)
            value = value.replace(" ", "");

        if (value.length < 16)
            return null;

        for (var card in cards)
            if (cards.hasOwnProperty(card))
                if (value.match(cards[card]))
                    return card;

        return null;
    }


    function ngCardValidator() {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, $element, $attrs, ngModel) {

                if (!ngModel) return;

                ngFor = $attrs.ngFor;

                /*$scope.$watch($attrs.ngModel, function (value) {
                    var card = returnCard(value);
                    if (ngFor)
                        $('#' + ngFor).val(card);
                    return card != null;
                });*/

                ngModel.$validators.ngCardValidator = function (value) {
                    var card = returnCard(value);
                    if (ngFor)
                        $('#' + ngFor).val(card);
                    return card != null;
                };
            }
        }
    }


})(angular.module('common.ui'));