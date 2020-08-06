
(function (app) {
    'use strict';

    app.controller('configuracaoCtrl', configuracaoCtrl);

    configuracaoCtrl.$inject = ['$scope', '$timeout', 'apiService'];

    function configuracaoCtrl($scope, $timeout, apiService) {

        $scope.TiposNotificacao = {};
        $scope.notificacaoUsuario = notificacaoUsuario;

        function tipoNotificacao() {
            apiService.get("/api/notificacao/tipoNotificacao", null,
            tipoNotificacaoCompleted,
            tipoNotificacaoFailed);
        }

        function tipoNotificacaoCompleted(response) {
            $scope.TiposNotificacao = response.data;
        }

        function tipoNotificacaoFailed(response) {
        }

        function notificacaoUsuario(notificacao) {
            apiService.post("/api/notificacao/atualizarNotificacaoUsuario", notificacao,
            notificacaoUsuarioCompleted,
            notificacaoUsuarioFailed);
        }

        function notificacaoUsuarioCompleted(response) {
        }

        function notificacaoUsuarioFailed(response) {
        }

        tipoNotificacao();
    }

})(angular.module('ECCFranquia'));