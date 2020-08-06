(function (app) {
    'use strict';

	app.controller('cotacaoHistCtrl', cotacaoHistCtrl);

	cotacaoHistCtrl.$inject = ['$scope', 'membershipService', 'notificationService', 'SweetAlert', '$rootScope', '$location', 'apiService', '$stateParams', '$filter', '$templateCache'];

	function cotacaoHistCtrl($scope, membershipService, notificationService, SweetAlert, $rootScope, $location, apiService, $stateParams, $filter,  $templateCache) {
        $scope.pageClass = 'page-cotacaohist';

        
        $scope.loadCotacaoFornecedor = loadCotacaoFornecedor;
        $scope.CotacaoItens = [];

     
        //1-----Carrega cotacao -----
        function loadCotacaoFornecedor() {

            if ($scope.CotacaoId !== undefined && $scope.CotacaoId > 0) {

                apiService.get('/api/cotacao/cotacaoItensFornecedor/' + $scope.CotacaoId, null,
                    loadCotacaoFornecedorSucesso,
                    loadCotacaoFornecedorFalha);
            } else {
                notificationService.displayInfo("Digite o número da cotação!");
            }

           
        }

        function loadCotacaoFornecedorSucesso(response) {
            $scope.CotacaoItens = response.data.itensCotacao;
        }

        function loadCotacaoFornecedorFalha(response) {
            notificationService.displayError(response.data);
        }
        //1------------------------fim-----------------------------

 
    }

})(angular.module('ECCFornecedor'));