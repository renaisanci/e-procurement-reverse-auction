(function (app) {
	'use strict';

	app.controller('membroCadastroCtrl', membroCadastroCtrl);

	membroCadastroCtrl.$inject = ['$scope', 'membershipService', '$modal', 'notificationService', '$rootScope', '$location', 'apiService', 'admUtilService', 'SweetAlert'];

	function membroCadastroCtrl($scope, membershipService, $modal, notificationService, $rootScope, $location, apiService, admUtilService, SweetAlert) {
		$scope.pageClass = 'page-membro-cadastro';
		$scope.inserirMembro = inserirMembro;
		$scope.limpaDados = limpaDados;


		$scope.novoMembro = { Ativo: true };
		$scope.logradouros = [];
		$scope.estados = [];
		$scope.cidades = [];
		$scope.bairros = [];
		$scope.loadEnderecoCep = loadEnderecoCep;
		$scope.novoEndereco = { Ativo: true };
		$scope.inserirEndereco = inserirEndereco;
		$scope.page = 0;
		$scope.pagesCount = 0;
		$scope.loadCidadeChange = loadCidade;
		$scope.loadBairroChange = loadBairro;
		$scope.habilitaddlCidade = true;
		$scope.habilitaddlBairro = true;
		$scope.limpaDadosEndereco = limpaDadosEndereco;
		$scope.editarEndereco = editarEndereco;
		$scope.categorias = [];
		$scope.cadastrarMembro = cadastrarMembro;
		$scope.inserirMembroCategoriasProduto = inserirMembroCategoriasProduto;
		$scope.modelContainer = [];
		$scope.fornecedoresRegiao = [];
		$scope.openEditDialog = openEditDialog;
		$scope.openDatePicker = openDatePicker;
		$scope.atualizaParaEndPadrao = atualizaParaEndPadrao;
		$scope.subcategorias = [];
		$scope.usuariosMembro = [];
		$scope.loadCategoriaPorSegmento = loadCategoriaPorSegmento;
		$scope.filtroCategoriaSegmento = 0;
		$scope.HorarioEntrega = [];
		$scope.FranquiaMembro = [];
		$scope.entrega = {};
		$scope.mostraBairroDll = true;
		$scope.mostraBairroInput = false;
		$scope.habilitaEscolhaTipoPessoa = false;
		$scope.habilitaCadastroEndereco = false;
		$scope.habilitaRelacionarCategoria = false;
		$scope.openDatePickerNascimento = openDatePickerNascimento;

		$scope.sexos = [
			{ Id: 1, DescSexo: "Masculino" },
			{ Id: 2, DescSexo: "Feminino" },
			{ Id: 3, DescSexo: "Outros" }
		];

		var idEndereco = 0;

		$scope.dateOptions = {
			formatYear: 'yyyy',
			startingDay: 0
		};

		$scope.subCategoriaSelecionada = '';
		$scope.membroCategoria = '';

		$scope.loadMembroCategoriaId = loadMembroCategoriaId;

		$scope.tipoPessoa = "pj";

		function openEditDialog(membroParam) {
			$scope.novoUsuario = membroParam;
			$modal.open({
				templateUrl: 'Scripts/SPAAdm/usuario/usuarioModal.html',
				controller: 'usuarioCtrl',

				scope: $scope
			}).result.then(function ($scope) {
				//clearSearch();
			}, function () {
			});
		}

		function openDatePicker($event) {
			$event.preventDefault();
			$event.stopPropagation();

			$scope.datepicker.opened = true;
		};
		$scope.datepicker = {};
		$scope.format = 'dd/MM/yyyy';

		function openDatePickerNascimento($event) {
			$event.preventDefault();
			$event.stopPropagation();

			$scope.datepickerNascimento.opened = true;
		};
		$scope.datepickerNascimento = {};
		$scope.formatNascimento = 'dd/MM/yyyy';

		//1-----Cadastrar Membro------------
		function cadastrarMembro() {

			inserirMembro();

			if ($scope.novoMembro.Id > 0 && ($scope.Enderecos == undefined || $scope.Enderecos.Id > 0))
				inserirEndereco();

			if ($scope.Enderecos.length > 0)
				inserirMembroCategoriasProduto();

		}
		//1-----Fim---------------------------------------


		function limpaDados() {

			$scope.novoMembro = {
				Ativo: true
			};

			$scope.filtroMembros = '';
			$scope.novoEndereco = {};
			$scope.novoMembro = {};
			$scope.Enderecos = [];
			$scope.categorias = [];
			$scope.habilitaEscolhaTipoPessoa = false;
			$scope.habilitaCadastroEndereco = false;
			$scope.habilitaRelacionarCategoria = false;
		}


		//2-----Insere novo membro aba Membro------------
		function inserirMembro() {



			if ($scope.tipoPessoa == "pj") {

				if ($scope.novoMembro.Cnpj != undefined) {
					$scope.novoMembro.Cnpj = $scope.novoMembro.Cnpj.split(".").join("").split("/").join("").split("-").join("");
				} else {

					notificationService.displayInfo("CNPJ obrigatório !");
					return;
				}


				if ($scope.novoMembro.NomeFantasia == undefined || $scope.novoMembro.NomeFantasia == "" || $scope.novoMembro.NomeFantasia == " ") {
					notificationService.displayInfo("Nome Fantasia obrigatório para !");
					return;

				}



				if ($scope.novoMembro.RazaoSocial == undefined || $scope.novoMembro.RazaoSocial == "" || $scope.novoMembro.RazaoSocial == " ") {
					notificationService.displayInfo("Razão Social Fantasia obrigatório para !");
					return;

				}



			} else {


				if ($scope.novoMembro.Cpf != undefined) {
					$scope.novoMembro.Cpf = $scope.novoMembro.Cpf.split(".").join("").split("/").join("").split("-").join("");
				} else {

					notificationService.displayInfo("CPF obrigatório !");
					return;
				}

				if ($scope.novoMembro.Cro == undefined || $scope.novoMembro.Cro == "" || $scope.novoMembro.Cro == " ") {
					notificationService.displayInfo("CRO obrigatório para Pessoa Física!");
					return;

				}


				if ($scope.novoMembro.Nome == undefined || $scope.novoMembro.Nome == "" || $scope.novoMembro.Nome == " ") {
					notificationService.displayInfo("Nome obrigatório para !");
					return;

				}
			}
			if ($scope.novoMembro.Email == undefined || $scope.novoMembro.Email == "" || $scope.novoMembro.Email == " ") {
				notificationService.displayInfo("E-Mail obrigatório para !");
				return;

			}


			if ($scope.novoMembro.DddTelComl != undefined) {
				$scope.novoMembro.DddTelComl = $scope.novoMembro.DddTelComl.split("(").join("").split(")").join("");
			} else {

				notificationService.displayInfo("DDD Telefone Comercial obrigatório !");
				return;
			}


			if ($scope.novoMembro.TelefoneComl != undefined) {
				$scope.novoMembro.TelefoneComl = $scope.novoMembro.TelefoneComl.split("-").join("");
			} else {

				notificationService.displayInfo("Telefone Comercial obrigatório !");
				return;
			}


			if ($scope.novoMembro.DddCel != undefined) {
				$scope.novoMembro.DddCel = $scope.novoMembro.DddCel.split("(").join("").split(")").join("");
			} else {

				notificationService.displayInfo("DDD Celular obrigatório !");
				return;
			}


			if ($scope.novoMembro.Celular != undefined) {
				$scope.novoMembro.Celular = $scope.novoMembro.Celular.split("-").join("");
			}
			else {

				notificationService.displayInfo("Celular obrigatório !");
				return;
			}
			if ($scope.novoMembro.Id > 0) {
				atualizarMembro();
			} else {
				inserirMembroModel();
			}
		}

		function inserirMembroModel() {

			if ($scope.tipoPessoa == "pj") {

				apiService.post('/api/membroCadastro/inserir', $scope.novoMembro,
					inserirMembroSucesso,
					inserirMembroFalha);
			} else {

				apiService.post('/api/membroCadastro/inserirPf', $scope.novoMembro,
					inserirMembroSucesso,
					inserirMembroFalha);
			}

		}

		function inserirMembroSucesso(response) {


			SweetAlert.swal({
				title: "SUCESSO!!\nAGORA CADASTRE SEU ENDEREÇO ABAIXO.",
				text: "",
				type: "success",
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "OK"
			});

			//if ($scope.tipoPessoa == "pj") {
			//	notificationService.displaySuccess("Cadastrado com sucesso, cadastre seu endereço mais abaixo!");

			//	SweetAlert.swal({
			//		title: "SUCESSO!!\nCadastre seu endereçço mais abaixo.",
			//		text: "",
			//		type: "success",
			//		confirmButtonColor: "#DD6B55",
			//		confirmButtonText: "OK"
			//	});




			//} else {

			//	SweetAlert.swal({
			//		title: "AGORA PRECISAMOS QUE VOCÊ CADASTRE SEU ENDEREÇO",
			//		text: "",
			//		type: "success",
			//		confirmButtonColor: "#DD6B55",
			//		confirmButtonText: "OK"
			//	});				 

			//}

			$scope.habilitaEscolhaTipoPessoa = true;
			$scope.habilitaCadastroDados = true;
			$scope.habilitaCadastroEndereco = true;
			$scope.novoMembro = response.data;

			loadLogradouro();
			loadEstado();
		}

		function inserirMembroFalha(response) {
			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}
		//2---------------------Fim--------------------------


		//4-----Atualiza dados membro aba Membro------------
		function atualizarMembro() {



			if ($scope.tipoPessoa == "pj") {

				$scope.novoMembro.Cnpj = $scope.novoMembro.Cnpj.split(".").join("").split("/").join("").split("-").join("");
				apiService.post('/api/membroCadastro/atualizar', $scope.novoMembro,
					atualizarMembroSucesso,
					atualizarMembroFalha);
			} else {


				$scope.novoMembro.Cpf = $scope.novoMembro.Cpf.split(".").join("").split("/").join("").split("-").join("");
				apiService.post('/api/membroCadastro/atualizarPf', $scope.novoMembro,
					atualizarMembroSucesso,
					atualizarMembroFalha);

			}
		}

		function atualizarMembroSucesso(response) {
			if ($scope.tipoPessoa == "pj") {
				notificationService.displaySuccess($scope.novoMembro.RazaoSocial + ' Atualizado com Sucesso.');

			} else {
				notificationService.displaySuccess($scope.novoMembro.Nome + ' Atualizado com Sucesso.');
			}


			$scope.novoMembro = response.data;
			$scope.filtroMembros = response.data.Cnpj;

		}

		function atualizarMembroFalha(response) {
			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}
		//4---------------------Fim--------------------------


		//7-----Carrega dropdown Logradouro --
		function loadLogradouro() {
			apiService.get('/api/endereco/logradouro', null,
				logradourLoadCompleted,
				logradourLoadFailed);
		}

		function logradourLoadCompleted(response) {


			var newItem = new function () {
				this.Id = undefined;
				this.DescLogradouro = "Logradouro...";

			};
			response.data.push(newItem);
			$scope.logradouros = response.data;
		}

		function logradourLoadFailed(response) {
			notificationService.displayError(response.data);
		}
		//7------------------------fim-----------------------------

		//8-----Carrega dropdown Estado--
		function loadEstado() {
			apiService.get('/api/endereco/estado', null,
				loadEstadoCompleted,
				loadEstadoFailed);
		}

		function loadEstadoCompleted(response) {


			var newItem = new function () {
				this.Id = undefined;
				this.DescEstado = "Estado...";

			};
			response.data.push(newItem);
			$scope.estados = response.data;
		}

		function loadEstadoFailed(response) {
			notificationService.displayError(response.data);
		}
		//8------------------------fim-----------------------------

		//9-----Busca Endereço pelo cep--
		function loadEnderecoCep(cep) {
			if (cep.length === 8) {

				var config = {
					params: {
						cep: cep.trim()
					}
				};
				apiService.get('/api/endereco/enderecoCep', config,
					loadEnderecoCepCompleted,
					loadEnderecoCepFailed);
			}
		}

		function loadEnderecoCepCompleted(response) {
			if (response.data != null) {
				if (response.data.EstadoId > 0) {
					loadCidade(response.data.EstadoId);

				}


				if (response.data.CidadeId > 0) {
					loadBairro(response.data.CidadeId);

				}


				var periodoEntregaEnd = $scope.novoEndereco.PeriodoEntrega;

				var idEndereco = $scope.novoEndereco.Id;
				var enderecoSelecPadrao = $scope.novoEndereco.EnderecoPadrao;
				var endAtivo = $scope.novoEndereco.Ativo;
				var obsHorario = $scope.novoEndereco.DescHorarioEntrega;



				if ($scope.novoEndereco.Id == "" || $scope.novoEndereco.Id == undefined) {




					if (response.data.Endereco == "" || response.data.Endereco == null) {
						notificationService.displayInfo("CEP NÃO ENCONTRADO FAVOR DIGITAR O ENDEREÇO MANUALMENTE !");

					} else {



						var idEnd = $scope.novoEndereco.Id;

						$scope.novoEndereco = response.data;

						$scope.novoEndereco.Id = idEnd;

						$scope.novoEndereco.PeriodoEntrega = periodoEntregaEnd;

						$scope.novoEndereco.DescHorarioEntrega = obsHorario;
					}

				} else {


					if (response.data.Endereco == "" || response.data.Endereco == null) {
						notificationService.displayInfo("CEP NÃO ENCONTRADO FAVOR DIGITAR O ENDEREÇO MANUALMENTE !");

					} else {

						$scope.novoEndereco = response.data;
						$scope.novoEndereco.Id = idEndereco;
						$scope.novoEndereco.Ativo = endAtivo;
						$scope.novoEndereco.EnderecoPadrao = enderecoSelecPadrao;
						$scope.novoEndereco.PeriodoEntrega = periodoEntregaEnd;
						$scope.novoEndereco.DescHorarioEntrega = obsHorario;


					}


				}



			}

		}

		function loadEnderecoCepFailed(response) {
			notificationService.displayError(response.data);
		}
		//9------------------------fim-----------------------------


		//11----------Cadastro de Novo Endereço-----
		function inserirEndereco() {
			$scope.novoEndereco.Id = idEndereco;
			if ($scope.novoEndereco.Id > 0) {
				atualizarEndereco();
			} else {
				inserirEnderecoModel();
			}

			loadSegmentos();
		}

		function inserirEnderecoModel() {

			if (validarEndereco()) {

				$scope.novoEndereco.Cep = $scope.novoEndereco.Cep.replace(/-/g, '').trim();

				$scope.novoEndereco.PessoaId = $scope.novoMembro.PessoaId;

				apiService.post('/api/endereco/inserirCadastro', $scope.novoEndereco,
					inserirEnderecoSucesso,
					inserirEnderecoFalha);
			}
		}

		function inserirEnderecoSucesso(response) {

			SweetAlert.swal({
				title: "ABAIXO, ESCOLHA O SEGMENTO E CATEGORIAS DE PRODUTOS QUE DESEJA COMPRAR",
				text: "",
				type: "success",
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "OK"
			});

			$scope.habilitaRelacionarCategoria = true;

			$scope.novoEndereco = {};
			pesquisarEnderecoMembro();
		}

		function inserirEnderecoFalha(response) {
			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}

		function validarEndereco() {

			if ($scope.novoEndereco.Cep == null) {
				notificationService.displayInfo('Inserir um Cep!');
				return false;
			}

			if ($scope.novoEndereco.LogradouroId == null) {
				notificationService.displayInfo('Selecione um logradouro para o endereço!');
				return false;
			}

			if ($scope.novoEndereco.Endereco == null) {
				notificationService.displayInfo('Inserir um endereço!');
				return false;
			}

			if ($scope.novoEndereco.NumEndereco === "") {
				notificationService.displayInfo('Inserir um número para o endereço!');
				return false;
			}

			if ($scope.novoEndereco.EstadoId == null) {
				notificationService.displayInfo('Selecione um estado para o endereço!');
				return false;
			}

			if ($scope.novoEndereco.CidadeId == null) {
				notificationService.displayInfo('Selecione uma cidade para o endereço!');
				return false;
			}

			if ($scope.novoEndereco.BairroId == null) {
				notificationService.displayInfo('Selecione um bairro para o endereço!');
				return false;
			}



			return true;
		}

		//11----------------------------------------


		//12----Atuallizar endereco membro----------------
		function atualizarEndereco() {

			if (validarEndereco()) {
				$scope.novoEndereco.Cep = $scope.novoEndereco.Cep.replace(/-/g, '').trim();
				apiService.post('/api/endereco/atualizar', $scope.novoEndereco,
					atualizarEnderecoSucesso,
					atualizarEnderecoFalha);
			}
		}

		function atualizarEnderecoSucesso(response) {
			notificationService.displaySuccess('Endereço Atualizado com Sucesso.');
			//$scope.novoEndereco = response.data;
			$scope.novoEndereco = {};
			pesquisarEnderecoMembro();
		}

		function atualizarEnderecoFalha(response) {
			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}
		//12-------------------------------------------


		//13-----Carrega dropdown Cidade --
		function loadCidade(estadoId) {

			var config = {
				params: {
					EstadoId: estadoId
				}
			};
			apiService.get('/api/endereco/cidade', config,
				loadCidadeCompleted,
				loadCidadeFailed);
		}

		function loadCidadeCompleted(response) {


			$scope.cidades = response.data;
			$scope.habilitaddlCidade = false;
		}

		function loadCidadeFailed(response) {
			notificationService.displayError(response.data);
		}
		//13------------------------fim-----------------------------



		//14-----Carrega dropdown Bairro --
		function loadBairro(cidadeId) {

			var config = {
				params: {
					CidadeId: cidadeId
				}
			};
			apiService.get('/api/endereco/bairro', config,
				loadBairroCompleted,
				loadBairroFailed);
		}

		function loadBairroCompleted(response) {


			$scope.bairros = response.data;
			$scope.habilitaddlBairro = false;


			if (response.data === "") {
				$scope.mostraBairroDll = false;
				$scope.mostraBairroInput = true;
			} else {

				$scope.mostraBairroDll = true;
				$scope.mostraBairroInput = false;
			}
		}

		function loadBairroFailed(response) {
			notificationService.displayError(response.data);
		}
		//14------------------------fim-----------------------------

		//15-----Carrega enderecos membro------------
		function pesquisarEnderecoMembro(page) {

			page = page || 0;

			$scope.loadingEnderecos = true;

			var config = {
				params: {
					pessoaId: $scope.novoMembro.PessoaId,
					page: page,
					pageSize: 10,
					filter: $scope.filtroEndereco
				}
			};

			apiService.get('/api/endereco/enderecoMembro', config,
				enderecosLoadCompleted,
				enderecosLoadFailed);
		}

		function enderecosLoadCompleted(response) {

			$scope.Enderecos = response.data;

			$scope.loadingEnderecos = false;
		}

		function enderecosLoadFailed(response) {
			notificationService.displayError(response.data);
		}
		//15----Fim---------------------------------------


		//17----Limpas dados do endereço para inserir um novo------------
		function limpaDadosEndereco() {

			$scope.novoEndereco = '';
			idEndereco = 0;
			$scope.entrega = {};

			$scope.habilitaddlCidade = true;
			$scope.habilitaddlBairro = true;


		}
		//17-------------------Fim----------------------------------------

		//18-----Editar endereco selecionado---------------------
		function editarEndereco(enderecoPesq) {
			$scope.novoEndereco = enderecoPesq;
			idEndereco = $scope.novoEndereco.Id;

			loadCidade(enderecoPesq.EstadoId);

			loadBairro(enderecoPesq.CidadeId);

		}
		//18---------------------------------Fim---------------------


		//21-----Carrega categorias --
		function loadCategoria() {

			apiService.get('/api/produto/categoria', null,
				loadCategoriaSucesso,
				loadCategoriaFailed);
		}

		function loadCategoriaSucesso(response) {

			$scope.categorias = response.data;
			loadMembroCategoria();

		}

		function loadCategoriaFailed(response) {
			notificationService.displayError(response.data);
		}
		//21------------------------fim-----------------------------

		//22-----Carrega categoria Membro --
		function loadMembroCategoria() {

			var config = {
				params: {
					membroId: $scope.novoMembro.Id

				}
			};


			apiService.get('/api/membroCadastro/membroCategorias', config,
				loadMembroCategoriaSucesso,
				loadMembroCategoriaFailed);
		}

		function loadMembroCategoriaSucesso(response) {

			var membroCategoria = response.data;
			for (var i = 0; i < $scope.categorias.length; i++) {
				for (var j = 0; j < membroCategoria.length; j++) {

					if ($scope.categorias[i].Id == membroCategoria[j].Id) {
						$scope.categorias[i].Relacionado = true;
						$scope.categorias[i].selected = true;
						break;
					}
				}
				$scope.categorias[i].visivel = false;
				if ($scope.categoriasSeg == undefined || $scope.categoriasSeg.length == 0 || $scope.filtroCategoriaSegmento == 0) {
					$scope.categorias[i].visivel = true;
				}
				else {
					for (var j = 0; j < $scope.categoriasSeg.length; j++) {

						if ($scope.categorias[i].Id == $scope.categoriasSeg[j].Id) {
							$scope.categorias[i].visivel = true;
							break;
						}
					}
				}

			}
		}

		function loadMembroCategoriaFailed(response) {
			notificationService.displayError(response.data);
		}
		//22------------------------fim-----------------------------

		//23-----Relaciona categoria de produtos ao membro----------
		function inserirMembroCategoriasProduto() {

			$scope.membroCategorias = [];
			angular.forEach($scope.categorias, function (categoriaPesq) {
				if (categoriaPesq.selected)
					$scope.membroCategorias.push(categoriaPesq.Id);

			});

			if ($scope.membroCategorias.length > 0)
				apiService.post('/api/membroCadastro/inserirMembroCategoria/' + $scope.novoMembro.Id, $scope.membroCategorias,
					membroCategoriasProdutoSucesso,
					membroCategoriasProdutoFalha);
			else {
				notificationService.displayInfo('Relacione uma ou mais categorias para finalizar o cadastro!');
				return;
			}
		}

		function membroCategoriasProdutoSucesso(result) {

			if (result.data.success)
				notificationService.displaySuccess('Categoria relacionada com sucesso.');
			loadMembroCategoria();


			SweetAlert.swal({
				title: "SEJA BEM VINDO À ECONOMIZA JÁ!  ",
				text: "AGUARDE O CONTATO DA NOSSA EQUIPE PARA LIBERAÇÃO!!!",
				type: "success",
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "OK"
			},
				function (isConfirm) {
					if (isConfirm) {
						window.location.href = 'https://WWW.ECONOMIZAJA.COM.BR';
					}
				});
		}

		function membroCategoriasProdutoFalha(response) {
			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}
		//23------------------------fim-----------------------------
        

		//25.5------------- Carrega categorias ----------------
		function loadCategoriaPorSegmento(idSeg) {
			if (idSeg != 0) {
				var config = {
					params: {
						segmentoId: $scope.filtroCategoriaSegmento
					}
				};

				apiService.get('/api/produto/categoriaporsegmento', config,
					loadCategoriaPorSegmentoSucesso,
					loadCategoriaPorSegmentoFailed);
			}
			else {
				$scope.categoriasSeg = [];
				//loadFornecedorCategoria();
				loadMembroCategoria();
			}
		}

		function loadCategoriaPorSegmentoSucesso(response) {
			$scope.categorias = response.data;
			$scope.categoriasSeg = response.data;
			loadMembroCategoria();
		}

		function loadCategoriaPorSegmentoFailed(response) {
			notificationService.displayError(response.data);
		}
		//25.5------------------------fim-----------------------------


		//27-----Carrega categoria Membro --
		function loadMembroFornecedor() {

			var config = {
				params: {
					membroId: $scope.novoMembro.Id
				}
			};


			apiService.get('/api/membroCadastro/membroFornecedores', config,
				loadMembroFornecedorSucesso,
				loadMembroFornecedorFalha);
		}

		function loadMembroFornecedorSucesso(response) {

			var membroFornecedor = response.data;
			for (var i = 0; i < $scope.fornecedoresRegiao.length; i++) {
				for (var j = 0; j < membroFornecedor.length; j++) {

					if ($scope.fornecedoresRegiao[i].Id == membroFornecedor[j].FornecedorId) {
						$scope.fornecedoresRegiao[i].Relacionado = true;
						$scope.fornecedoresRegiao[i].selected = true;
						break;
					}
				}

			}
		}

		function loadMembroFornecedorFalha(response) {
			notificationService.displayError(response.data);
		}
		//27------------------------fim-----------------------------


		//32-----Carrega Segmentos --
		function loadSegmentos() {

			apiService.get('/api/segmento/loadsegmentos', null,
				loadSegmentosSucesso,
				loadSegmentosFailed);
		}

		function loadSegmentosSucesso(response) {
			$scope.segmentos = response.data;
			//loadCategoriaSegmento();

		}

		function loadSegmentosFailed(response) {
			notificationService.displayError(response.data);
		}
		//32------------------------fim-----------------------------



		//----------------------------------Altera endereco padrão-------------
		function atualizaParaEndPadrao(endereco) {

			endereco.Cep = endereco.Cep.replace(/-/g, '').trim();
			endereco.EnderecoPadrao = true;
			apiService.post('/api/endereco/atualizarEndPadrao', endereco,
				atualizarEnderecoPSucesso,
				atualizarEnderecoPFalha);

		}

		function atualizarEnderecoPSucesso(response) {

			notificationService.displaySuccess('Endereço Padrão Atualizado com Sucesso.');


			pesquisarEnderecoMembro();

		}

		function atualizarEnderecoPFalha(response) {

			if (response.status == '400')
				for (var i = 0; i < response.data.length; i++) {
					notificationService.displayInfo(response.data[i]);
				}
			else
				notificationService.displayError(response.statusText);
		}

		//checkedall-------------------------------------------------
		$scope.checkAll = admUtilService.checkBoxAll;
		//-----------------------------------------------------------


		//2-----Carrega Categorias para DropDown Categoria------------
		function pesquisarCategoria() {


			apiService.get('/api/subcategoria/categoria', null,
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
			notificationService.displayError(response.data);
		}

		//2-----FIM--------------------------------------------------

		function loadMembroCategoriaId(categoriaId) {
			if (categoriaId != undefined)
				membroCategoriaId.value = categoriaId;
			else
				membroCategoriaId.value = 0;
		}


		//3-------------Carrega Período Entrega-------------------------
		function carregaPeriodoEntrega() {

			apiService.get('/api/endereco/periodoentrega', null,
				periodoentregaLoadCompleted,
				periodoentregaLoadFailed);

			function periodoentregaLoadCompleted(result) {
				$scope.HorarioEntrega = result.data;
			}

			function periodoentregaLoadFailed(result) {

				notificationService.displayError('Erro ao carregar períodos!');
			}
		}

		//3-------------Carrega Período Entrega-------------------------


		//-------------Carrega franquias---------------------------------
		function carregaFranquias() {

			apiService.get('/api/franquia/carregaFranquiasMembro', null,
				carregaFranquiasLoadCompleted,
				loadFranquiasLoadFailed);
		}

		function carregaFranquiasLoadCompleted(result) {
			$scope.FranquiaMembro = result.data;
		}

		function loadFranquiasLoadFailed(result) {

			notificationService.displayError('Erro ao carregar combo de franquias!');
		}

		//--------------------------------------------------------------




	}

})(angular.module('common.core')); (function (app) {
	'use strict';

	app.controller('recuperasenhaCtrl', recuperasenhaCtrl);

	recuperasenhaCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location'];

	function recuperasenhaCtrl($scope, membershipService, notificationService, $rootScope, $location) {
		$scope.pageClass = 'page-login';
		$scope.novaSenha1;
		$scope.novaSenha2;
		$scope.alterarSenha = alterarSenha;
		$scope.desabilitaAlterar = true;
		$scope.usuario = {
			perfilModulo: 1
		};

		function PageLoad() {

			var objGet = $location.search();

			$scope.objRecuperaSenha = {
				Chave: objGet.Q.toString()
			}

			membershipService.getRecuperaSenha($scope.objRecuperaSenha, pageLoadCompleted);

		}

		function alterarSenha() {
			var senha1 = $scope.novaSenha1;
			var senha2 = $scope.novaSenha2;

			if (senha1 == senha2) {
				$scope.usuario.senha = senha1;
				membershipService.alterarSenhaUsuario($scope.usuario, senhaAlteradaCompleted)
			}
			else {
				notificationService.displayError('As senhas digitadas não conferem. Tente Novamente.');
			}

		}

		function senhaAlteradaCompleted(result) {
			if (result.data.success) {

				notificationService.displaySuccess('Senha alterada com sucesso.');
				$location.path('/');

			}
		}

		function pageLoadCompleted(result) {
			if (result.data.success) {
				//membershipService.saveCredentials($scope.usuario, result.data.usuarioNome);
				$scope.usuario.usuarioNome = result.data.usuarioNome;
				$scope.usuario.Id = result.data.usuarioId;
				$scope.usuario.Chave = result.data.usuarioChave;
				$scope.desabilitaAlterar = false;
			}
			else {
				$scope.desabilitaAlterar = true;
				notificationService.displayError('Acesso inválido.');

			}
		}


		PageLoad();
	}

})(angular.module('common.core'));