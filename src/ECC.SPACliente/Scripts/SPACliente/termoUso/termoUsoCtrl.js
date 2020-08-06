
(function (app) {
    'use strict';

    app.controller('termoUsoCtrl', termoUsoCtrl);

    termoUsoCtrl.$inject = ['$scope', 'apiService', '$sce'];

    function termoUsoCtrl($scope, apiService, $sce) {
        
        function termoUso() {
            apiService.get("/api/usuario/termoUso", null,
            termoUsoCompleted,
            termoUsoFailed);
        }

        function termoUsoCompleted(response) {
            $scope.conteudo = $sce.trustAsHtml(response.data.Documento);
        }

        function termoUsoFailed(response) {
        }

        termoUso();
    }

})(angular.module('ECCCliente'));