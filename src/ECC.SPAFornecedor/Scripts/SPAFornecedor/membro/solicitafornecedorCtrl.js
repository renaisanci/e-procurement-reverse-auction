(function (app) {
	'use strict';

	app.controller('solicitafornecedorCtrl', solicitafornecedorCtrl);

	solicitafornecedorCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'SweetAlert', '$modal'];

	function solicitafornecedorCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, SweetAlert, $modal) {


		$scope.pageClass = 'page-membrosolicitafornecedor';
		$scope.close = close;
		$scope.pesquisarNovoMembroSolicitaFornecedor = pesquisarNovoMembroSolicitaFornecedor;
		$scope.detalhesMembroSolicitaFornecedor = detalhesMembroSolicitaFornecedor;
		$scope.habilitaDesabilitaAbas = habilitaDesabilitaAbas;
		$scope.outroMembro = outroMembro;
		$scope.fornecedorAceitaMembro = fornecedorAceitaMembro;
		$scope.modalMotivoRecusaMembro = modalMotivoRecusaMembro;
		$scope.pesquisarNovoMembroSolicitaFornecedorPF = pesquisarNovoMembroSolicitaFornecedorPF;
		$scope.habilitaDesabilitaAbasPF = habilitaDesabilitaAbasPF;
		$scope.detalhesMembroSolicitaFornecedorPF = detalhesMembroSolicitaFornecedorPF;

		$scope.novoFornecedor = [];
		$scope.showtr = false;
        $scope.tipoPessoa = 'pj';
        $scope.FormaPagtos = [];


		//0--------Declaracao de todas as abas de tela de novo membro--------
		$scope.tabsFornecedorAceitaMembro = {

			tabPesquisar: {
				tabAtivar: "active",
				tabhabilitar: true,
				contentAtivar: "tab-pane fade in active",
				contentHabilitar: true
			},

			tabPesquisarPF: {
				tabAtivar: "",
				tabhabilitar: true,
				contentAtivar: "tab-pane fade",
				contentHabilitar: true
			},


			tabCadFornecedorAceitaMembro: {
				tabAtivar: "",
				tabhabilitar: false,
				contentAtivar: "tab-pane fade",
				contentHabilitar: false
			}
		};
		//0------------------------Fim--------------------------------



		function habilitaDesabilitaAbas() {

			$scope.tabsFornecedorAceitaMembro = {
				tabPesquisar: {
					tabAtivar: "active",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade in active",
					contentHabilitar: true
				},

				tabPesquisarPF: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade",
					contentHabilitar: true
				},
				tabCadFornecedorAceitaMembro: {
					tabAtivar: "",
					tabhabilitar: false,
					contentAtivar: "tab-pane fade",
					contentHabilitar: false
				}
			};
		}


		function habilitaDesabilitaAbasPF() {


			pesquisarNovoMembroSolicitaFornecedorPF();
			$scope.tabsFornecedorAceitaMembro = {
				tabPesquisar: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane",
					contentHabilitar: true
				},

				tabPesquisarPF: {
					tabAtivar: "active",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade in active",
					contentHabilitar: true
				},


				tabCadFornecedorAceitaMembro: {
					tabAtivar: "",
					tabhabilitar: false,
					contentAtivar: "tab-pane fade",
					contentHabilitar: false
				}
			};
		}





		function outroMembro() {


			$scope.novoAceitaMembro = [];

			$scope.tabsFornecedorAceitaMembro = {

				tabPesquisar: {
					tabAtivar: "active",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade in active",
					contentHabilitar: true
				},


				tabPesquisarPF: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade",
					contentHabilitar: true
				},

				tabCadFornecedorAceitaMembro: {
					tabAtivar: "",
					tabhabilitar: false,
					contentAtivar: "tab-pane fade",
					contentHabilitar: false
				}
			};
		}


		//1------Carrega todos membros que querem trabalhar com o fornecedor----------------
		function pesquisarNovoMembroSolicitaFornecedor(page) {

			page = page || 0;

			var config = {
				params: {
					page: page,
					pageSize: 300,
					filter: $scope.filtronovomembrosolicita
				}
			};

			apiService.get('/api/fornecedor/pesquisarmembrofornecedor', config,
				membroSolicitaLoadCompleted,
				membroSolicitaLoadFailed);
		}

		function membroSolicitaLoadCompleted(result) {

			$scope.Membro = result.data.Items;



		}

		function membroSolicitaLoadFailed(result) {

			notificationService.displayError('Erro ao carregar fornecedores');
		}
		//1--------------------------------------------------------------------------------



		//1------Carrega todos membros que querem trabalhar com o fornecedor----------------
		function pesquisarNovoMembroSolicitaFornecedorPF(page) {

			page = page || 0;

			var config = {
				params: {
					page: page,
					pageSize: 300,
					filter: $scope.filtronovomembrosolicita
				}
			};

			apiService.get('/api/fornecedor/pesquisarmembrofornecedorPF', config,
				membroPFSolicitaLoadCompleted,
				membroPFSolicitaLoadFailed);
		}

		function membroPFSolicitaLoadCompleted(result) {

			$scope.MembroPF = result.data.Items;



		}

		function membroPFSolicitaLoadFailed(result) {

			notificationService.displayError('Erro ao carregar fornecedores');
		}
		//1--------------------------------------------------------------------------------





		//2------------------Detalhes do Membro-------------------------------------------
		function detalhesMembroSolicitaFornecedor(membro) {

			$scope.tipoPessoa = 'pj';
			$scope.novoAceitaMembro = membro;

			$scope.tabsFornecedorAceitaMembro = {

				tabPesquisar: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade",
					contentHabilitar: true
				},

				tabPesquisarPF: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade",
					contentHabilitar: true
				},
				tabCadFornecedorAceitaMembro: {
					tabAtivar: "active",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade in active",
					contentHabilitar: true
				}
			};

		}
		//2------------------------------------------------------------------------------

		//2------------------Detalhes do Membro-------------------------------------------
		function detalhesMembroSolicitaFornecedorPF(membroPF) {

			$scope.tipoPessoa = 'pf';
			$scope.novoAceitaMembro = membroPF;

			$scope.tabsFornecedorAceitaMembro = {

				tabPesquisar: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade",
					contentHabilitar: true
				},

				tabPesquisarPF: {
					tabAtivar: "",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade",
					contentHabilitar: true
				},
				tabCadFornecedorAceitaMembro: {
					tabAtivar: "active",
					tabhabilitar: true,
					contentAtivar: "tab-pane fade in active",
					contentHabilitar: true
				}
			};

		}
		//2------------------------------------------------------------------------------







		//3---------------Fornecedor Aceita Membro-----------------------------------------
        function fornecedorAceitaMembro(novoAceitaMembro) {




            angular.forEach($scope.formaPagtosAvista, function (formaPagto) {
                if (formaPagto.selected) {                  
               

                    $scope.FormaPagtos.push(formaPagto.Id     );
                } 
            });

            angular.forEach($scope.formaPagtosParcelado, function (formaPagto) {
                if (formaPagto.selected) {
                  
                    $scope.FormaPagtos.push(formaPagto.Id);
                }
            });

            var idMembro = novoAceitaMembro.Id;
 

            apiService.post('/api/fornecedor/fornecedorAceitaMembro/' + idMembro, $scope.FormaPagtos,
				fornecedorAceitaMembroSucesso,
				fornecedorAceitaMembroFalha);
		}

		function fornecedorAceitaMembroSucesso(result) {

			var tipoPessoa = result.data.tipoPessoa == 1;

			SweetAlert.swal({
				title: "SOLICITAÇÃO ACEITA COM SUCESSO!",
				text: "Agora você poderá dar seus preços nas cotações que este membro estiver envolvido. " +
				"O membro receberá um e-mail alertando que agora ele é seu cliente",
				type: "success",
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "Ok"
			},
				function () {

					if (tipoPessoa) {

						$scope.tabsFornecedorAceitaMembro = {

							tabPesquisar: {
								tabAtivar: "active",
								tabhabilitar: true,
								contentAtivar: "tab-pane fade in active",
								contentHabilitar: true
							},

							tabPesquisarPF: {
								tabAtivar: "",
								tabhabilitar: true,
								contentAtivar: "tab-pane fade",
								contentHabilitar: true
							},

							tabCadFornecedorAceitaMembro: {
								tabAtivar: "",
								tabhabilitar: false,
								contentAtivar: "tab-pane fade",
								contentHabilitar: false
							}
						};

						pesquisarNovoMembroSolicitaFornecedor();

					} else {

						$scope.tabsFornecedorAceitaMembro = {

							tabPesquisar: {
								tabAtivar: "",
								tabhabilitar: true,
								contentAtivar: "tab-pane fade",
								contentHabilitar: true
							},

							tabPesquisarPF: {
								tabAtivar: "active",
								tabhabilitar: true,
								contentAtivar: "tab-pane fade in active",
								contentHabilitar: true
							},

							tabCadFornecedorAceitaMembro: {
								tabAtivar: "",
								tabhabilitar: false,
								contentAtivar: "tab-pane fade",
								contentHabilitar: false
							}
						};

						pesquisarNovoMembroSolicitaFornecedorPF();						
					}

				});
		}

		function fornecedorAceitaMembroFalha(result) {

			notificationService.displayError(result.data);
		}
		//3----------------------------------------------------------------------------------


		// #region Fornecedor Recusa Membro

		function modalMotivoRecusaMembro() {


			$modal.open({
				animation: true,
				templateUrl: 'scripts/SPAFornecedor/membro/modalMotivoRecusaMembro.html',
				controller: 'modalMotivoRecusaMembroCtrl',
				scope: $scope,
				size: 'lg'
			}).result.then(function ($scope) {

				console.log("Modal Closed!!!");

			}, function () {

				console.log("Modal Dismissed!!!");
			});

		}

		// #endregion

        //8-----Carrega Formas de pagamentos ---------------------
        function loadFormaPagto() {
            apiService.get('/api/fornecedor/formaPagtoCadastradaFornecedor', null,
                loadFormaPagtoSucesso,
                loadFormaPagtoFailed);
        }

        function loadFormaPagtoSucesso(response) {
            $scope.formaPagtosAvista = response.data.formaPagtosAvistaVM;
            $scope.formaPagtosParcelado = response.data.formaPagtosParceladoVM;
        }

        function loadFormaPagtoFailed(response) {
            notificationService.displayError(response.data);
        }
        //8------------------------fim-----------------------------



        loadFormaPagto();
		pesquisarNovoMembroSolicitaFornecedor();


	}

})(angular.module('ECCFornecedor'));