(function (app) {
    'use strict';

    app.factory('clienteUtilService', clienteUtilService);

    clienteUtilService.$inject = ['$http', '$location', 'notificationService', '$rootScope', 'apiService'];

    function clienteUtilService($http, $location, notificationService, $rootScope, apiService) {

        var service = {
            checkBoxAll: checkBoxAll,
            verificaPendenciaFinanceira: verificaPendenciaFinaneceira,
            limpaAvisos: limpaAvisos
        };
        
        //Função para marcar todos os CheckBox
        function checkBoxAll(itens, chk) {
            if (chk) {
                chk = true;
            } else {
                chk = false;
            }
            angular.forEach(itens, function(item) {
                item.selected = chk;
            });
        }

        //Verifica se usuário tem pendências financeiras
        function verificaPendenciaFinaneceira() {

            var pendente = $rootScope.repository.loggedUser.faturaMensalidade;

            return pendente;
        }

        //Limpa Avisos do TopBar
        function limpaAvisos(pedidoId, tipoAvisoId) {
            
            var config = {
                params: {
                    ReferenciaId: pedidoId,
                    TipoAvisosId: tipoAvisoId
                }
            };

            apiService.get('/api/avisos/limpaAvisoPorReferenciaTipo', config,
                    limpaAvisoPedidoSucesso,
                    limpaAvisoPedidoFalha);

            function limpaAvisoPedidoSucesso(response) {
                $rootScope.$emit("reloadAvisosMem", {});
            }

            function limpaAvisoPedidoFalha(response) {
                notificationService.displayError(response.data);
            }
        }
        
        return service;
    }

})(angular.module('common.core'));