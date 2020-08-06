(function (app) {
    'use strict';

    app.controller('modalDetalhesFornecedorCtrl', modalDetalhesFornecedorCtrl);

    modalDetalhesFornecedorCtrl.$inject = ['$scope', '$modalInstance'];

    function modalDetalhesFornecedorCtrl($scope, $uibModalInstance) {


        $scope.cancel = cancel;

        //----------------Fechar Modal------------------------------
        function cancel() {

            $uibModalInstance.dismiss();
        };
        //----------------Fim Modal----------------------------------



    }

})(angular.module('ECCCliente'));