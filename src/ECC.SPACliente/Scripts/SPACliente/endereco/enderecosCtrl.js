(function (app) {
    'use strict';

    app.controller('enderecosCtrl', enderecosCtrl);

    enderecosCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService'];

    function enderecosCtrl($scope, $modalInstance, $timeout, apiService, notificationService) {
    
        $scope.endSelec = {};
        $scope.close = close;

        //1---------Fecha Modal---------------------
        function close() {
            $modalInstance.dismiss();
        }
        //------------------------------------------
 

        //2-----Carrega enderecos padrao membro------------
        function enderecoPadraoMembro() {

            var config = {
                params: {
                    pessoaId: $scope.EnderecoPadrao.PessoaId
                }
            };

            apiService.get('/api/endereco/enderecoMembroAtivo', config,
                        enderecosLoadCompleted,
                        enderecosLoadFailed);
        }

        function enderecosLoadCompleted(response) {

            $scope.enderecos = response.data;


        }

        function enderecosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //----Fim---------------------------------------
        
        $scope.selecionaEndereco = function (enderecoSelected) {
            $scope.endSelec = enderecoSelected;
            $modalInstance.close($scope.endSelec);
        };


        enderecoPadraoMembro();

    }

})(angular.module('ECCCliente'));