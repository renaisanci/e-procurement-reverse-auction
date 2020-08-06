(function (app) {
	'use strict';

	app.controller('cotacoesCtrl', cotacoesCtrl);

    cotacoesCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', '$stateParams', '$modal', '$filter', 'SweetAlert', 'Hub'];

    function cotacoesCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, $stateParams, $modal, $filter, SweetAlert, Hub) {
		$scope.pageClass = 'page-cotacoes';
		$scope.showDados = false; // Caso existam registros para mostrar colocar como true.
		$scope.StatusId = 25; // Cotação em andamento - WorkFlow: 14 Ordem: 1
		$scope.loadCotacoes = loadCotacoes;
		$scope.DtDe = '';
		$scope.DtAte = '';
		$scope.limpaStatus = limpaStatus;
		$scope.openCotacaoMapa = openCotacaoMapa;

		$scope.dateOptions = {
			formatYear: 'yyyy',
			startingDay: 0
		};

		$scope.openDatePickerDe = openDatePickerDe;
		function openDatePickerDe($event) {
			$event.preventDefault();
			$event.stopPropagation();

			$scope.datepickerDe.opened = true;
		};

		$scope.openDatePickerAte = openDatePickerAte;
		function openDatePickerAte($event) {
			$event.preventDefault();
			$event.stopPropagation();

			$scope.datepickerAte.opened = true;
		};
		$scope.datepickerDe = {};
		$scope.datepickerAte = {};
		$scope.format = 'dd/MM/yyyy';

		//1-----Carrega Pedidos Membro ----------------------------
		function loadCotacoes(page) {

			page = page || 0;

			var config = {
				params: {
					dtDe: $filter('date')($scope.DtDe, "dd/MM/yyyy"),
					dtAte: $filter('date')($scope.DtAte, "dd/MM/yyyy"),
					statusId: $scope.StatusId,
					page: page,
					pageSize: 10
				}
			};

			apiService.get('/api/cotacao/cotacaoFornecedor', config,
                    loadCotacaoSucesso,
                    loadCotacaoFalha);
		}

		function loadCotacaoSucesso(response) {

		    $scope.Cotacoes = response.data.cotacoesFornecedor;


			if ($scope.Cotacoes == undefined || $scope.Cotacoes == null || $scope.Cotacoes.length < 1) {
				//$scope.showDados = false;

				SweetAlert.swal({
					title: "NÃO ENCONTRAMOS COTAÇÕES!",
					text: "No momento, não temos cotações em andamento.",
					type: "success",
					html: true
				});
			} else {
				$scope.showDados = true;
				$scope.page = response.data.pagSet.Page;
				$scope.pagesCount = response.data.pagSet.TotalPages;
				$scope.totalCount = response.data.pagSet.TotalCount;


				$scope.dateCotaIni = response.data.dateCotaNow;
			}
		}

		function loadCotacaoFalha(response) {
			notificationService.displayError(response.data);
		}
		//1------------------------fim-----------------------------

		//2-----Carrega Todos Status de Pedido --
		function loadStatusCotacao() {
			var config = {
				params: {
					workflowStatusId: 14
				}
			};
			apiService.get('/api/statusSistema/statusPorWorkflow', config,
                    loadStatusPedidoSucesso,
                    loadStatusPedidoFalha);
		}

		function loadStatusPedidoSucesso(response) {
			$scope.StatusCotacao = response.data;
			loadCotacoes();
		}

		function loadStatusPedidoFalha(response) {
			notificationService.displayError(response.data);
		}
		//2------------------------fim-----------------------------
        
		function limpaStatus() {
			$scope.StatusId = "";
		}

	    //-----Abre popup do mapa----------------------------------
        function openCotacaoMapa(idCotacao) {
		    $scope.idCotacao = idCotacao;
		    $modal.open({
                templateUrl: 'scripts/SPAFornecedor/cotacoes/cotacaoMapa.html',
                controller: 'cotacaoMapaCtrl',
                scope: $scope,
                size: 'lg-mapa'
		    }).result.then(function ($scope) {
		        //console.log("Modal Closed!!!");
		    }, function () {
                //console.log("Modal Dismissed!!!");
		    });
		}
        //-----Abre popup do mapa----------------------------------

	    //------------------Atualização em tempo Real---------------
        $rootScope.$on("cotacoesAtualizar", function (event) {
            loadCotacoes();
        });
	    //------------------Atualização em tempo Real---------------

        loadStatusCotacao();

	}

})(angular.module('ECCFornecedor'));