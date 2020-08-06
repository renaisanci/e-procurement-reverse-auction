
(function (app) {
    'use strict';

    app.controller('modalFornecedorProdutoQuantidadeCtrl', modalFornecedorProdutoQuantidadeCtrl);

    modalFornecedorProdutoQuantidadeCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'notificationService', 'apiService'];

    function modalFornecedorProdutoQuantidadeCtrl($scope, $modalInstance, $timeout, notificationService, apiService) {

        $scope.close = close
        $scope.inserirQtdPercent = inserirQtdPercent;
        $scope.internoObjModal = angular.copy($scope.objModal);
        $scope.removeItem = removeItem;
		$scope.salvaFornecedorProdutoQuantidade = salvaFornecedorProdutoQuantidade;
		$scope.openDatePickerValidadeQtdValor = openDatePickerValidadeQtdValor;
		$scope.format = 'dd/MM/yyyy';
		$scope.validarDataValidadeQtdDesconto = validarDataValidadeQtdDesconto;
		$scope.datepicker = {};

		$scope.dateOptions = {
			formatYear: 'yyyy',
			startingDay: 0
		};


        if ($scope.internoObjModal.ListaQuantidadeDesconto == undefined) {
            $scope.internoObjModal.ListaQuantidadeDesconto = [];
        }

        for (var i = 0; i < $scope.internoObjModal.ListaQuantidadeDesconto.length; i++) {
            $scope.internoObjModal.ListaQuantidadeDesconto[i].ativo = true;
        }



		function openDatePickerValidadeQtdValor($event) {
			$event.preventDefault();
			$event.stopPropagation();

			$scope.datepicker.opened = true;

		};

        //0---------Fecha Modal--------------------------------
        function close() {
            $modalInstance.dismiss('cancel');
        }
        //0----------------------------------------------------

        //1---------Inserir novo item na lista--------------------------------
		function inserirQtdPercent() {

            if (validarDataValidadeQtdDesconto()) {
                notificationService.displayInfo('Não pode ser menor que a data de hoje');
                return;
            } else if ($scope.validadeQtdDesconto === undefined) {
                notificationService.displayInfo('Escolha a data de validade deste preço e quantidade para este produto !');
                return;
            }


            var bPodeInserir = true;
            var bErrQuantidade = false;
			var bErrPercentual = false;

			

            if ($scope.modalQuantidade > 0) {

                for (var i = 0; i < $scope.internoObjModal.ListaQuantidadeDesconto.length; i++) {
                    if ($scope.internoObjModal.ListaQuantidadeDesconto[i].ativo == true) {
                        if ($scope.internoObjModal.ListaQuantidadeDesconto[i].QuantidadeMinima == $scope.modalQuantidade
                            || $scope.internoObjModal.ListaQuantidadeDesconto[i].PercentualDesconto == $scope.modalPercentual) {
                            bPodeInserir = false;
                            if ($scope.internoObjModal.ListaQuantidadeDesconto[i].QuantidadeMinima == $scope.modalQuantidade) {
                                bErrQuantidade = true;
                            }
                            if ($scope.internoObjModal.ListaQuantidadeDesconto[i].PercentualDesconto == $scope.modalPercentual) {
                                bErrPercentual = true;
							}

							if ($scope.internoObjModal.ListaQuantidadeDesconto[i].ValidadeQtdDesconto == $scope.validadeQtdDesconto) {
								bErrValidadeQtdDesconto = true;
							}
                        }
                    }
                }

                if (bPodeInserir) {
                    var item = {
                        QuantidadeMinima: $scope.modalQuantidade,
                        PercentualDesconto: $scope.modalPercentual,
                        FornecedorProdutoId: 0,
						ativo: true,
						ValidadeQtdDesconto: $scope.validadeQtdDesconto
                    };

                    $scope.internoObjModal.ListaQuantidadeDesconto.push(item);

                    $scope.modalQuantidade = "";
					$scope.modalPercentual = "";
					$scope.validadeQtdDesconto = "";
                }
                else {
                    if (bErrQuantidade && bErrPercentual) {
                        notificationService.displayWarning("Dados repetidos na lista de valores.");
                    }
                    else {
                        if (bErrQuantidade)
                            notificationService.displayWarning("Quantidade repetida na lista de valores.");

                        if (bErrPercentual)
							notificationService.displayWarning("Percentual repetido na lista de valores.");

						if (bErrValidadeQtdDesconto)
							notificationService.displayWarning("Data repetido na lista de valores.");
                    }
                }

            }
        }
        //1-----------------------------------------

        //2---------Remover item da Lista--------------------------------
        function removeItem(item) {

            item.ativo = false;

            $scope.modalQuantidade = "";
            $scope.modalPercentual = "";
        }
        //2----------------------------------------



		function validarDataValidadeQtdDesconto() {

			var dataHoje = new Date();
			var dia = dataHoje.getDate();
			var mes = dataHoje.getMonth();
			var ano = dataHoje.getFullYear();

			var novadata = new Date(ano, mes, dia);

			if ($scope.validadeQtdDesconto < novadata) {
				$scope.validadeQtdDesconto = undefined;
				return true;
			} else {

				return false;
			}
		}

        //3------------Salvar dados Modal ----------------
        function salvaFornecedorProdutoQuantidade() {
            var objEnvio = {
                FornecedorProdutoId: $scope.internoObjModal.fornecedorProdutoId,
                FornecedorId: 0,
                ProdutoId: $scope.internoObjModal.Id,
                CodigoProdutoFornecedor: $scope.internoObjModal.CodigoProdutoFornecedor,
                Valor: $scope.internoObjModal.Valor,

                ListaQuantidadeDesconto: []
            };

            $scope.itemFPQ = {
                PercentualDesconto: 0,
                QuantidadeMinima: 0,
				FornecedorProdutoId: $scope.internoObjModal.fornecedorProdutoId,
				ValidadeQtdDesconto: undefined
            };

            for (var i = 0; i < $scope.internoObjModal.ListaQuantidadeDesconto.length; i++) {
                if ($scope.internoObjModal.ListaQuantidadeDesconto[i].ativo == true) {
                    $scope.itemFPQ.PercentualDesconto = $scope.internoObjModal.ListaQuantidadeDesconto[i].PercentualDesconto;
					$scope.itemFPQ.QuantidadeMinima = $scope.internoObjModal.ListaQuantidadeDesconto[i].QuantidadeMinima;

					$scope.itemFPQ.ValidadeQtdDesconto = $scope.internoObjModal.ListaQuantidadeDesconto[i].ValidadeQtdDesconto;

                    objEnvio.ListaQuantidadeDesconto.push(angular.copy($scope.itemFPQ))
                }
            }

            salvaDados(objEnvio);

        }

        //3-----------------------------------------------

        //4-------------- Salva Dados --------------------
        function salvaDados(obj) {

            apiService.post('/api/produto/salvarProdutoQtd', obj,
                salvaDadosCompleted,
                salvaDadosFailed);

        }

        function salvaDadosCompleted(response) {
            notificationService.displaySuccess('Valores atualizados com sucesso.');
            $scope.objModal.Valor = parseFloat($scope.internoObjModal.Valor);
            $scope.objModal.CodigoProdutoFornecedor = $scope.internoObjModal.CodigoProdutoFornecedor;

            if (response.data.PercentMin == response.data.percMax && response.data.PercentMin == 0) {
                $scope.objModal.descontoDisponivel = "";
            }
            else if (response.data.PercentMin == response.data.PercentMax) {
                $scope.objModal.descontoDisponivel = response.data.PercentMin + "%";
            }
            else {
                $scope.objModal.descontoDisponivel = response.data.PercentMin + "% até " + response.data.PercentMax + "%";
            }
            //$scope.objModal.ListaQuantidadeDesconto = $scope.internoObjModal.ListaQuantidadeDesconto;
            $scope.objModal.ListaQuantidadeDesconto = [];
            for (var i = 0; i < $scope.internoObjModal.ListaQuantidadeDesconto.length; i++) {
                if ($scope.internoObjModal.ListaQuantidadeDesconto[i].ativo == true) {
                    $scope.objModal.ListaQuantidadeDesconto.push(angular.copy($scope.internoObjModal.ListaQuantidadeDesconto[i]))
                }
            }
            close();
        }

        function salvaDadosFailed(response) {
            notificationService.displayError(response.data);
        }
        //4----------------------------------------------


    }

})(angular.module('ECCFornecedor'));