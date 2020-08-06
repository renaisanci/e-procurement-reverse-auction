
(function (app) {
	'use strict';

	app.controller('infoBoaCompraCtrl', infoBoaCompraCtrl);

	infoBoaCompraCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', '$sce', 'SweetAlert'];

	function infoBoaCompraCtrl($scope, $modalInstance, $timeout, apiService, notificationService, $sce, SweetAlert) {


		$scope.close = close;
		 

		//1---------Fecha Modal---------------------
		function close() {
			$modalInstance.dismiss();
		}
		//------------------------------------------

		$scope.renderHtml = function (html_code) {
			return $sce.trustAsHtml(html_code);
		};

	}

})(angular.module('ECCCliente'));