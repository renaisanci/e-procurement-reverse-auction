
(function (app) {
	'use strict';

	app.controller('fornecedorProdutoCtrl', fornecedorProdutoCtrl);

	fornecedorProdutoCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', 'fornecedorUtilService', 'SweetAlert', '$modal', '$filter'];

	function fornecedorProdutoCtrl($scope, $timeout, apiService, notificationService, fornecedorUtilService, SweetAlert, $modal, $filter) {

		$scope.pageClass = 'page-disponibilidade';
		$scope.pesquisarSubCategoria = pesquisarSubCategoria;
		$scope.pesquisarProduto = pesquisarProduto;
		$scope.InserirDeletarFornecedorProdutoQuantidade = InserirDeletarFornecedorProdutoQuantidade;
		$scope.openModelFornecedorProdutoQuantidade = openModelFornecedorProdutoQuantidade;


		$scope.inserirDeletarDisponibilidadeProd = inserirDeletarDisponibilidadeProd;

		//Campos necessário para funcionar a ordenação do grid
		$scope.predicate = 'DescProduto';
		$scope.reverse = true;


		//1-----Carrega Produtos aba Pesquisar-----------------------
		function pesquisarProduto(page) {
			page = page || 0;

			var config = {
				params: {
					page: page,
					pageSize: 10,
					categoria: $scope.CategoriaId,
					subcategoria: $scope.SubCategoriaId,
					filter: $scope.filtroProduto
				}
			};

			apiService.get('/api/produto/pesquisar', config,
				produtoLoadCompleted,
				produtoLoadFailed);
		}

		function produtoLoadCompleted(result) {

			$scope.produtos = result.data.Items;

			pesquisarDisponibilidadeProduto();

			$scope.page = result.data.Page;
			$scope.pagesCount = result.data.TotalPages;
			$scope.totalCount = result.data.TotalCount;
		}

		function produtoLoadFailed(response) {
			notificationService.displayError(response.data);
		}

		//1----------------------------------------------------------


		//1-----Carrega Produtos aba Pesquisar-----------------------
		function pesquisarDisponibilidadeProduto() {

			apiService.get('/api/produto/getfornecedorproduto', null,
				fornecedorProdutoLoadCompleted,
				fornecedorProdutoLoadFailed);
		}

		function fornecedorProdutoLoadCompleted(result) {

			$scope.fornecedorProduto = result.data;

			for (var i = 0; i < $scope.produtos.length; i++) {

				for (var j = 0; j < $scope.fornecedorProduto.length; j++) {

					if ($scope.produtos[i].Id == $scope.fornecedorProduto[j].ProdutoId) {
						$scope.produtos[i].Relacionado = true;
						$scope.produtos[i].selected = true;
						$scope.produtos[i].Valor = $scope.fornecedorProduto[j].Valor;
						$scope.produtos[i].fornecedorProdutoId = $scope.fornecedorProduto[j].Id;
						$scope.produtos[i].CodigoProdutoFornecedor = $scope.fornecedorProduto[j].CodigoProdutoFornecedor;
						$scope.produtos[i].ListaQuantidadeDesconto = $scope.fornecedorProduto[j].ListaQuantidadeDesconto;
						if ($scope.fornecedorProduto[j].PercentMin == $scope.fornecedorProduto[j].PercentMax && $scope.fornecedorProduto[j].PercentMin == 0) {
							$scope.produtos[i].descontoDisponivel = "";
						}
						else if ($scope.fornecedorProduto[j].PercentMin == $scope.fornecedorProduto[j].PercentMax) {
							$scope.produtos[i].descontoDisponivel = $scope.fornecedorProduto[j].PercentMin + "%";
						}
						else {
							$scope.produtos[i].descontoDisponivel = $scope.fornecedorProduto[j].PercentMin + "% até " + $scope.fornecedorProduto[j].PercentMax + "%";
						}

					}
				}
			}
		}

		function fornecedorProdutoLoadFailed(result) {
			notificationService.displayError(result.data);
		}

		//1----------------------------------------------------------


		//3-----Carrega Categorias para DropDown Categoria------------
		function pesquisarCategoria() {
			apiService.get('/api/produtopromocional/fornecedorcategoria', null,
				categoriaLoadCompleted,
				categoriaLoadFailed);
		}

		function categoriaLoadCompleted(response) {


			var newItem = new function () {
				this.Id = undefined;
				this.DescCategoria = "Categoria ...";

			};
			response.data.push(newItem);
			$scope.categorias = response.data;
		}

		function categoriaLoadFailed(response) {
			console.log(response);
			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}
		//3-----------------------------------------------------------


		//5------------Preencher DropDown SubCategoria ---------------
		function pesquisarSubCategoria(id) {

			var config = {
				params: {
					filter: id
				}
			};

			apiService.get('/api/produto/subcategorias', config,
				subCategoriaLoadCompleted,
				subCategoriaLoadFailed);

		}

		function subCategoriaLoadCompleted(response) {
			$scope.DesabilitarSubCategoria = false;
			$scope.subcategorias = response.data;

		}

		function subCategoriaLoadFailed(response) {
			notificationService.displayError(response.data);
		}
		//5----------------------------------------------------------


		//6----------Inserir ou Deletar Disponibilidade Produto------
		function InserirDeletarFornecedorProdutoQuantidade(produto) {

			var objFornecorProduto = {};
			$scope.nomeProduto = produto.DescProduto;

			objFornecorProduto = {
				FornecedorId: 0,
				ProdutoId: produto.Id,
				Valor: 0,
				Selecionado: produto.selected
			};

			$scope.inserirDeletarDisponibilidadeProd(objFornecorProduto);


		}

		//6----------------------------------------------------------


		//7--------Método Inserir e Deletar Disponibilidade-----------
		function inserirDeletarDisponibilidadeProd(objFornecedorProd) {

			apiService.post('/api/produto/inserirDeletarfornecedorProdutoProduto', objFornecedorProd,
				InserirDeletarFornecedorProdutoQuantidadeLoadCompleted,
				InserirDeletarFornecedorProdutoQuantidadeLoadFailed);

			function InserirDeletarFornecedorProdutoQuantidadeLoadCompleted(result) {


			}

			function InserirDeletarFornecedorProdutoQuantidadeLoadFailed(result) {

				notificationService.displayError(result.data);
			}

		}
		//7-----------------------------------------------------------


		//8------Abrir popup ----------
		function openModelFornecedorProdutoQuantidade(item) {

			$scope.objModal = item;
			$scope.nomeProduto = item.DescProduto;

			$modal.open({
				animation: true,
				templateUrl: 'scripts/SPAFornecedor/produtos/modalFornecedorProdutoQuantidade.html',
				controller: 'modalFornecedorProdutoQuantidadeCtrl',
				backdrop: 'static',
				scope: $scope,
				size: 'lg'
			});

		}
		//8-----------------------------------------------------------

		//9------Remove Ascentos do filtro de produtos----------

		function removeAccents(value) {
			return value
				.replace(/á/g, 'a')
				.replace(/à/g, 'a')
				.replace(/ã/g, 'a')
				.replace(/ç/g, 'c')
				.replace(/é/g, 'e')
				.replace(/ê/g, 'e')
				.replace(/í/g, 'i')
				.replace(/ó/g, 'o')
				.replace(/õ/g, 'o')
				.replace(/ú/g, 'u')
				.replace(/ô/g, 'o')
				.replace(/â/g, 'a')
				;
		}

		$scope.ignoreAccents = function (item) {
			if (!$scope.filtroProduto)
				return true;
			var textDescProduto = removeAccents(item.DescProduto.toLowerCase());
			var textDescCategoria = removeAccents(item.DescCategoria.toLowerCase());
			var textDescMarca = removeAccents(item.Marca.toLowerCase());
			var search = removeAccents($scope.filtroProduto.toLowerCase());
			return textDescProduto.indexOf(search) > -1 || textDescCategoria.indexOf(search) > -1 || textDescMarca.indexOf(search) > -1;
		};

		//9-----------------------------------------------------------



		//pesquisarSubCategoria();
		pesquisarCategoria();
		//deletarfornecedorProdutoForaValidade();

	}

})(angular.module('ECCFornecedor'));