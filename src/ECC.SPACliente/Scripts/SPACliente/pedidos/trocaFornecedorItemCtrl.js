
(function (app) {
    'use strict';

	app.controller('trocaFornecedorItemCtrl', trocaFornecedorItemCtrl);

	trocaFornecedorItemCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'SweetAlert', 'utilService', '$rootScope', '$filter'];

	function trocaFornecedorItemCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, SweetAlert, utilService, $rootScope, $filter) {

		$scope.closeModalTroca = closeModalTroca;
		$scope.salvaTrocarFornecedor = salvaTrocarFornecedor;
		$scope.pedidoTrocaItem = {};
		$scope.addItemFornecedorTroca = addItemFornecedorTroca;
		$scope.trocaFornecedorItemSelecionado = [];
        $scope.TrocaItem = false;
       
 

		// console.log($scope);
 

        //1---------Fecha Modal-----------------------------------------
        function closeModalTroca() {
            $modalInstance.dismiss();
        }
        //1---------Fim------------------------------------------------




		//2---------salvar troca fornecedor do item-----------------------------------------
		function salvaTrocarFornecedor() {

			SweetAlert.swal({
				title: "ATENÇÃO, A MUDANÇA DOS ITENS SELECIONADOS PARA ESTE FORNECEDOR SERÁ IRREVERSÍVEL",
				text: "Tem certeza que deseja fechar os itens selecionado com esse fornecedor !!! ???",
				type: "warning",
				showCancelButton: true,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "SIM",
				cancelButtonText: "NÃO",
				closeOnConfirm: true,
				closeOnCancel: true
			},
				function (isConfirm) {
					if (isConfirm) {
						apiService.post('/api/TrocaItemFornecedor/salvaTrocaItemFornecedor/', $scope.trocaFornecedorItemSelecionado,
							salvaTrocaFornecedorItemSucesso,
							salvaTrocaFornecedorItemFalha);
					}
				});




		

		}

		function salvaTrocaFornecedorItemSucesso(result) {

			if (result.data.success) {
				notificationService.displaySuccess("Mudanças dos itens selecionados realizada com sucesso !.");
				closeModalTroca();
				$scope.recarregaPedido();
			}
		}

		function salvaTrocaFornecedorItemFalha(result) {
			notificationService.displayError(result.data);
		}
        //2---------Fim------------------------------------------------


		//3-----Carrega todos Status de Pedido -------------------
		function loadItensFornecedorPedido() {
			var pedidoId = angular.copy($scope.pedido.PedidoId);
			var fornecedorTrocarId = angular.copy($scope.fornecedorTrocarId);
 

			apiService.post('/api/TrocaItemFornecedor/trocaItemFornecedor/' + pedidoId + '/'+ fornecedorTrocarId, null,
				loadItensFornecedorPedidoSucesso,
				loadItensFornecedorPedidoFalha);
		}

		function loadItensFornecedorPedidoSucesso(response) {

			$scope.Pedido = response.data.lancesFornecedorPedido;

            $scope.pedidoTrocaItem = angular.copy($scope.Pedido);

         
		
		}

		function loadItensFornecedorPedidoFalha(response) {
			close();
			notificationService.displayError(response.data);
		}
        //3------------------------fim-----------------------------


		


		function addItemFornecedorTroca(itemId,  valorItemFornecedor, trocaItem, obs) {

			if (trocaItem) {
				$scope.trocaFornecedorItemSelecionado.push(
                    {
                        Observacao:obs,
						ItemId: itemId,
						FornecedorId: $scope.fornecedorTrocarId,
						ValorItemFornecedor: valorItemFornecedor

					});
			} 
			console.log($scope.trocaFornecedorItemSelecionado);
		}


       
		loadItensFornecedorPedido();

    }

})(angular.module('ECCCliente'));