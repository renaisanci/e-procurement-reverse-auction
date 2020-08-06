
(function (app) {
    'use strict';

	app.controller('listasComprasCtrl', listasComprasCtrl);

    listasComprasCtrl.$inject = ['$scope', '$modalInstance', 'notificationService', '$rootScope', 'apiService'];

    function listasComprasCtrl($scope, $modalInstance, notificationService, $rootScope, apiService) {

		$scope.close = close;

		$scope.NomeListaCompras = "";
		$scope.ListasCompras = [];

	

        //1---------Fecha Modal---------------------
        function close() {
            $scope.$parent.carregaCarrinho();
            $modalInstance.dismiss('cancel');
        }
        //------------------------------------------
 

		$scope.salvaListaCompras = function salvaListaCompras() {

			//console.log(localStorage.getItem("listaCompras"));

			//if (localStorage.getItem("listaCompras") != null) {

			//	$scope.ListasCompras = JSON.parse(localStorage.getItem("listaCompras"));

			//	if ($scope.ListasCompras.length == 4) {
			//		notificationService.displayWarning("Limite de listas de compras excedido, o máximo são 4.");
			//		return false;
			//	}
			//}


			//var duplicado = true;
			//if ($scope.NomeListaCompras == "") {
			//	notificationService.displayWarning("Infome o nome da lista de compras.");
			//} else {

 
			//	for (var i = 0; i < $scope.ListasCompras.length; i++) {

			//		if ($scope.ListasCompras[i].nomelista == $scope.NomeListaCompras) {
			//			notificationService.displayInfo("Já existe este nome de lista.");
			//			duplicado = false;
			//			break;
			//		}				 
			//	}


			//	if (duplicado) {
            $scope.ListasCompras = { Nomelista: $scope.NomeListaCompras, listaCompras: $scope.cartItens, RemFornPedCot: $scope.fornRemPedCot };
			//		localStorage.setItem("listaCompras", JSON.stringify($scope.ListasCompras));
			//		$scope.ListasCompras = JSON.parse(localStorage.getItem("listaCompras"));
			//		notificationService.displaySuccess("Lista de compras salva com sucesso.");
			//		console.log($scope.ListasCompras);

			//		$rootScope.$emit("reloadListaCompras", {});
			//	}
   //         }


            apiService.post('/api/listaCompras/inserirLista', $scope.ListasCompras,
                inserirListaSucesso,
                inserirListaFailed);



		};


        function inserirListaSucesso(response) {
            $rootScope.$emit("reloadListaCompras", {});
            notificationService.displaySuccess("Lista de compras salva com sucesso.");
            $modalInstance.dismiss('cancel');
        }

        function inserirListaFailed(response) {
            notificationService.displayWarning(response.data);
        }

 
 
    }

})(angular.module('ECCCliente'));