(function (app) {
    'use strict';

    app.factory('fornecedorUtilService', fornecedorUtilService);

    fornecedorUtilService.$inject = ['$http', '$location', 'notificationService', '$rootScope'];

    function fornecedorUtilService($http, $location, notificationService, $rootScope) {
        var service = {
            checkBoxAll: checkBoxAll,
			verificaPendenciaFinanceira: verificaPendenciaFinaneceira,
			moedaDecimal: moedaDecimal
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

        function verificaPendenciaFinaneceira() {

            var pendente = $rootScope.repository.loggedUser.faturaMensalidade;

            return pendente;
		}

		function moedaDecimal(moeda) {

			moeda = moeda.replace(".", "");

			moeda = moeda.replace(",", ".");

			return parseFloat(moeda);
		}

        return service;
    }

})(angular.module('common.core'));