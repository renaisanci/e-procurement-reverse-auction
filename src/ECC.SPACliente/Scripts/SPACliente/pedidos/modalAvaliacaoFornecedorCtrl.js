(function (app) {
    'use strict';

    app.controller('modalAvaliacaoFornecedorCtrl', modalAvaliacaoFornecedorCtrl);

    modalAvaliacaoFornecedorCtrl.$inject = ['$scope', '$modalInstance', 'notificationService', 'apiService'];

    function modalAvaliacaoFornecedorCtrl($scope, $uibModalInstance, notificationService, apiService) {
        
        $scope.cancel = cancel;
        $scope.salvarAvaliacao = salvarAvaliacao;

        //1----------------Fechar Modal-------------------------------
        function cancel() {

            $uibModalInstance.dismiss();
        };

        function salvarAvaliacao() {

            if (validaCampos()) {

                $scope.novaAvaliacaoFornecedor.FornecedorId = $scope.fornecedor.Id;
               
                apiService.post('/api/pedido/inserirAvaliacaoMembroFornecedorPedido', $scope.novaAvaliacaoFornecedor,
                inserirAvaliacaoFornecedorLoadCompleted,
                inserirAvaliacaoMembroFornecedorLoadFailed);      
            }
		}


		function inserirAvaliacaoFornecedorLoadCompleted(result) {
			notificationService.displaySuccess('Sucesso ao avaliar fornecedor');
			$scope.limparCampos();
			var index = $scope.PedidoFornecedor.indexOf($scope.fornecedor);
			$scope.PedidoFornecedor.splice(index, 1);
			$scope.fornecedor = $scope.PedidoFornecedor[0];

			if ($scope.PedidoFornecedor.length === 0) {

				cancel();
				$scope.pesquisarAguardandoEntrega();
			}
		}

		function inserirAvaliacaoMembroFornecedorLoadFailed(result) {

			notificationService.displayError(result.data);
		}
        //1----------------Fim Modal----------------------------------
        
        //2----------------Validar Campos para salvar avaliação-------
        function validaCampos() {

            if ($scope.novaAvaliacaoFornecedor.QualidadeProdutos === 1 && $scope.novaAvaliacaoFornecedor.ObsQualidade == undefined) {
            notificationService.displayError('Digite o motivo da "Qualidade dos Produtos" ser ruim!');
                return false;
            }

            if ($scope.novaAvaliacaoFornecedor.TempoEntrega === 1 && $scope.novaAvaliacaoFornecedor.ObsEntrega == undefined) {
                notificationService.displayError('Digite o motivo da nota "Tempo de Entrega" ser ruim!');
                return false;
            }

            if ($scope.novaAvaliacaoFornecedor.Atendimento === 1 && $scope.novaAvaliacaoFornecedor.ObsAtendimento == undefined) {

                notificationService.displayError('Digite o motivo da nota "Atendimento" ser ruim!');
                return false;
            }

            if ($scope.novaAvaliacaoFornecedor.TempoEntrega == undefined) {
                
                notificationService.displayInfo('Escolha uma nota para o tempo de entrega!');
                return false;
            }
            if ($scope.novaAvaliacaoFornecedor.QualidadeProdutos == undefined) {
                notificationService.displayInfo('Escolha uma nota para a qualidade dos produtos!');
                return false;

            }
            if ($scope.novaAvaliacaoFornecedor.Atendimento == undefined) {
                notificationService.displayInfo('Escolha uma nota para o atendimento do fornecedor!');
                return false;
            }

            return true;
        }
        //2-----------------------------------------------------------

    }

})(angular.module('ECCCliente'));