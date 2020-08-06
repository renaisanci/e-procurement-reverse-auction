
(function (app) {
    'use strict';

    app.controller('cadpromocoesCtrl', cadpromocoesCtrl);

    cadpromocoesCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', 'SweetAlert', '$modal', '$rootScope', '$upload', '$filter', '$location'];

    function cadpromocoesCtrl($scope, $timeout, apiService, notificationService, SweetAlert, $modal, $rootScope, $upload, $filter, $location) {

        $scope.pageClass = 'page-cadpromocoes';
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.habilitaDesabilitaAbas = habilitaDesabilitaAbas;
        $scope.habilitaDesabilitaAbaCadProduto = habilitaDesabilitaAbaCadProduto;
        $scope.editarProduto = editarProduto;
        $scope.limparCampos = limparCampos;
        $scope.selectCustomer = selectCustomer;
        $scope.openDatePickerValidadeProduto = openDatePickerValidadeProduto;
        $scope.openDatePickerValidadeInicioPromocao = openDatePickerValidadeInicioPromocao;
        $scope.openDatePickerValidadeFimPromocao = openDatePickerValidadeFimPromocao;
        $scope.inserirProduto = inserirProduto;
        $scope.inserirNovoProduto = inserirNovoProduto;
        $scope.prepareFiles = prepareFiles;
        $scope.inseriFormaPagto = inseriFormaPagto;
        $scope.openDialogDetalhePromocao = openDialogDetalhePromocao;
        $scope.validarDataInicioPromocao = validarDataInicioPromocao;
        $scope.validarDataFimPromocao = validarDataFimPromocao;
        $scope.validarDataValidadeProduro = validarDataValidadeProduro;

        $scope.novoProduto = {};
        $scope.datepicker = {};
        var objFormaPagViewModel = {};
        var arrayNovoFormapagamento = [];
        $scope.FormasPagamentoFornecedor = [];
        $scope.format = 'dd/MM/yyyy';
        $scope.arrayFormaPagto = [];
        $scope.novoProduto = { Ativo: true };


        //Variável referente a imagem
        var nomeImage = null;
        $scope.novaImagem = {};

        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };

        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescProduto';
        $scope.reverse = true;


        //Identifica o Ambiente para as requisições do AutoComplet de marcas
        $scope.identificaAmbienteNet = apiService.identificaAmbiente();


        //0--------Declaracao de todas as abas de tela de novo membro--------
        $scope.tabsPromocoes = {

            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadPromocoes: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }
        };

        function habilitaDesabilitaAbaCadProduto() {

            $scope.tabsPromocoes = {

                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadPromocoes: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };

            $scope.novaImagem.CaminhoImagemGrande = "../../Content/images/unknown.jpg";
            limparCampos();
            $scope.novoProduto = { Ativo: true };

            apiService.get('/api/fornecedor/carregafornecedor', null,
           loadCarregaFornecedorCompleted,
           loadCarregaFornecedorFailed);

            function loadCarregaFornecedorCompleted(result) {
                $scope.prazoMaximoEntrega = result.data;
            }

            function loadCarregaFornecedorFailed(result) {
                notificationService.displayError(result.data);
            }

        }

        function habilitaDesabilitaAbas() {

            $scope.tabsPromocoes = {

                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadPromocoes: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };

            limparCampos();


        }

        //0------------------------Fim--------------------------------


        //-------------------------Tratamento DatePicker--------------

        function openDatePickerValidadeProduto($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.opened = true;

        };

        function openDatePickerValidadeInicioPromocao($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.openedIniPromocao = true;
        };

        function openDatePickerValidadeFimPromocao($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.openedFimPromocao = true;
        };

        function validarDataInicioPromocao() {

            var dataHoje = new Date();
            var dia = dataHoje.getDate();
            var mes = dataHoje.getMonth();
            var ano = dataHoje.getFullYear();

            var novadata = new Date(ano, mes, dia);

            if ($scope.novoProduto.InicioPromocao < novadata) {
                notificationService.displayInfo('Início da promoção não pode ser menor que a data de hoje');
                $scope.novoProduto.InicioPromocao = undefined;
            } else if ($scope.novoProduto.InicioPromocao > $scope.novoProduto.ValidadeProd) {
                notificationService.displayInfo('Início da promoção não pode ser maior que a data de validade do produto');
                $scope.novoProduto.InicioPromocao = undefined;
            }
        }

        function validarDataFimPromocao() {

            var dataHoje = new Date();
            var dia = dataHoje.getDate();
            var mes = dataHoje.getMonth();
            var ano = dataHoje.getFullYear();

            var novadata = new Date(ano, mes, dia);

            if ($scope.novoProduto.FimPromocao > $scope.novoProduto.ValidadeProd) {
                notificationService.displayInfo('Data final da promoção não pode ser maior que data de validade do produto');
                $scope.novoProduto.FimPromocao = undefined;
            } else if ($scope.novoProduto.ValidadeProd == undefined) {
                notificationService.displayInfo('Selecione a data de validade do produto');
                $scope.novoProduto.FimPromocao = undefined;
            } else if ($scope.novoProduto.FimPromocao < novadata) {
                notificationService.displayInfo('Fim da Promoção não pode ser menor que a data de hoje');
                $scope.novoProduto.FimPromocao = undefined;
            } else {

                var dataFinalPromo = $filter('date')($scope.novoProduto.FimPromocao, 'dd/MM/yyyy').split('/');
                var diaDataFinalPromo = parseInt(dataFinalPromo[0]) + $scope.prazoMaximoEntrega;
                var mesDataFinalPromo = parseInt(dataFinalPromo[1] - 1);
                var anoDataFinalPromo = parseInt(dataFinalPromo[2]);

                var dataFinal = new Date(anoDataFinalPromo, mesDataFinalPromo, diaDataFinalPromo);

                if (dataFinal > $scope.novoProduto.ValidadeProd) {

                    var texto = "Aumentar a data final da promoção, " +
                        "seu prazo de entrega somado com está data o produto chegará vencido para nosso membro";

                    notificationService.displayInfo(texto);

                    $scope.novoProduto.FimPromocao = undefined;

                }
            }


        }

        function validarDataValidadeProduro() {

            var dataHoje = new Date();
            var dia = dataHoje.getDate();
            var mes = dataHoje.getMonth();
            var ano = dataHoje.getFullYear();

            var novadata = new Date(ano, mes, dia);

            if (!($scope.novoProduto.ValidadeProd >= novadata)) {

                notificationService.displayInfo('Data de validade não pode ser menor que a data de hoje.');
                $scope.novoProduto.ValidadeProd = undefined;
            }
        }
        //-----------------------------------------------------------


        //1-----Carrega Produtos aba Pesquisar-----------------------
        function pesquisarProduto(page) {
            page = page || 0;

            var config = {
                params: {
                    page: page,
                    pageSize: 30,
                    categoria: $scope.CategoriaId,
                    subcategoria: $scope.SubCategoriaId,
                    filter: $scope.filtroProduto
                }
            };

            apiService.get('/api/produtopromocional/pesquisar', config,
                produtoLoadCompleted,
                produtoLoadFailed);
        }

        function produtoLoadCompleted(result) {

            var prod = result.data.Items;
            $scope.FaturaPendente = $rootScope.repository.loggedUser.faturaMensalidade;
            var data = new Date();

            for (var i = 0; i < prod.length; i++) {

                var fimPromocao = $filter('date')(prod[i].FimPromocao, "MM/dd/yyyy");

                var novadataFinal = new Date(fimPromocao);

                if (data > novadataFinal) {

                    prod[i].PromocaoAtiva = false;

                } else {

                    prod[i].PromocaoAtiva = true;
                }
            }

            $scope.produtos = prod;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;

            var msg = result.data.Items.length > 1 ? " Produtos Encontrados" : "Produto Encontrado";
            if ($scope.page == 0 && $scope.novoProduto.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function produtoLoadFailed(response) {
            notificationService.displayError(response.data);
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



        //6------------Preencher DropDown Unidade Medida ------------
        function pesquiarUnidadeMedida() {
            apiService.get('/api/produto/unidademedida', null,
                unidadeMedidaLoadCompleted,
                unidadeMedidaLoadFailed);
        }

        function unidadeMedidaLoadCompleted(response) {


            var newItem = new function () {
                this.Id = undefined;
                this.DescUnidadeMedida = "Unid. Med ...";
            };

            response.data.push(newItem);

            $scope.unidademedida = response.data;

        }

        function unidadeMedidaLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //6----------------------------------------------------------



        //7------------Editar Produto--------------------------------
        function editarProduto(produto) {

            $scope.tabsPromocoes = {

                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadPromocoes: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };


            for (var j = 0; j < $scope.FormasPagamentoFornecedor.length; j++) {

                for (var k = 0; k < produto.PromocaoFormaPagto.length; k++) {

                    if ($scope.FormasPagamentoFornecedor[j].Id == produto.PromocaoFormaPagto[k].FormaPagtoId) {

                        $scope.FormasPagamentoFornecedor[j].selected = true;
                        $scope.FormasPagamentoFornecedor[j].Relacionado = true;
                        $scope.arrayFormaPagto.push($scope.FormasPagamentoFornecedor[j]);
                        break;
                    }
                }
            }


            $scope.novoProduto = produto;

            pesquisarSubCategoria($scope.novoProduto.CategoriaId);


            if (!$scope.novoProduto.Aprovado && !$scope.novoProduto.PromoAtivo) {

                $modal.open({
                    templateUrl: 'scripts/SPAFornecedor/promocoes/promocaoRecusada.html',
                    controller: 'promocaoRecusadaCtrl',
                    //backdrop: 'static',
                    scope: $scope,
                    size: ''
                });
            }


            $scope.novaImagem.CaminhoImagemGrande = $scope.novoProduto.ImagemGrande;

        }
        //7----------------------------------------------------------


        //8------------Limpar Campos de Cadastro---------------------
        function limparCampos() {
            $scope.novoProduto = {};
            $scope.novaImagem = {};
            $scope.novaImagem.CaminhoImagemGrande = "../../Content/images/unknown.jpg";
            $scope.arrayFormaPagto = [];
            carregaNovoArrayFormaPagamento();
        }

        function carregaNovoArrayFormaPagamento() {
            $scope.FormasPagamentoFornecedor = [];
            var novo = angular.copy(arrayNovoFormapagamento);
            $scope.FormasPagamentoFornecedor = novo;
        }
        //8----------------------------------------------------------


        //9------------Preencher DropDown Marca ---------------------
        function selectCustomer($item) {
            if ($item) {
                $scope.novoProduto.MarcaId = $item.originalObject.Id;
            }
        }
        //9----------------------------------------------------------


        //10-----Inserir novo Produto -------------------------------
        function inserirProduto() {

            if ($scope.FaturaPendente == false) {
                if ($scope.novoProduto.Id > 0) {
                    atualizarProduto();
                } else {
                    inserirProdutoModel();
                }
            } else {
                SweetAlert.swal({
                    title: "ATENÇÃO!",
                    text: "Existe fatura pendente de pagamento!\n" +
                           "Clique em ''OK'' para imprimir o boleto.",
                    type: "warning",
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "OK"
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $location.path('/pagamento');
                        }
                    });
            }
        }

        function inserirProdutoModel() {
            if (validaCamposSalvar()) {

                apiService.post('/api/produtopromocional/inserir', $scope.novoProduto,
                    inserirProdutoSucesso,
                    inserirProdutoFalha);
            }
        }

        function inserirProdutoSucesso(response) {

            $scope.novoProduto = response.data;

            inserirImagem();

            pesquisarProduto();

            SweetAlert.swal({
                title: "Promoção cadastrada com sucesso!",
                text: "Agora aguarde a sua promoção ser aprovada pela equipe da Economiza Já.\n" +
                      "Sendo aprovada sua promoção estará disponível para nossos membros.",
                type: "success",
                html: true
            });

        }

        function inserirProdutoFalha(response) {
            console.log(response);
            if (response.status === '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }
        //10---------------------------------------------------------



        //11-----Atualizar Produtos----------------------------------
        function atualizarProduto() {

            if (validaCamposEditar()) {

                $scope.novoProduto.Fornecedor = null;

                apiService.post('/api/produtopromocional/atualizar', $scope.novoProduto,
                atualizarProdutoSucesso,
                atualizarProdutoFalha);
            }
        }

        function atualizarProdutoSucesso(response) {

            notificationService.displaySuccess('Produto atualizado com sucesso!');

            inserirImagem();

            $scope.novoProduto = response.data;
            pesquisarProduto();

        }

        function atualizarProdutoFalha(response) {
            console.log(response);
            if (response.status == '400') {
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            } else {
                notificationService.displayError(response.statusText);
            }
        }
        //11--------------------Fim----------------------------------



        //12-------Valida os campos do formulário--------------------
        function validaCamposSalvar() {

            $scope.novoProduto.PromocaoFormaPagto = criarObjetoFormaPagamento($scope.arrayFormaPagto);

            if ($scope.novoProduto.InicioPromocao > $scope.novoProduto.FimPromocao) {

                notificationService.displayInfo('Data de "Início da Promoção" não pode ser maior que Data "Final da Promoção"');
                return false;
            }


            if ($scope.novoProduto.CategoriaId === undefined) {

                notificationService.displayInfo('Favor selecionar uma categoria');
                return false;
            }

            if ($scope.novoProduto.SubCategoriaId === undefined) {

                notificationService.displayInfo('Favor selecionar sub-categoria');
                return false;
            }
            if ($scope.novoProduto.DescProduto === undefined) {

                notificationService.displayInfo('Favor preencher campo descrição do produto.');
                return false;
            }
            if ($scope.novoProduto.UnidadeMedidaId === undefined) {

                notificationService.displayInfo('Favor selecionar unidade de medida.');
                return false;
            }
            if ($scope.novoProduto.MarcaId === undefined) {

                notificationService.displayInfo('Favor selecionar a marca.');
                return false;
            }
            if ($scope.novoProduto.QtdProdutos === undefined) {

                notificationService.displayInfo('Favor adicionar quantidade de produtos para está promoção.');
                return false;
            }
            if ($scope.novoProduto.PrecoMedio === undefined) {

                notificationService.displayInfo('Favor adicionar o valor deste produto.');
                return false;
            }
            if ($scope.novoProduto.PrecoPromocao === undefined) {

                notificationService.displayInfo('Favor adicionar o preço de promoção.');
                return false;
            }
            if ($scope.novoProduto.Especificacao === undefined) {

                notificationService.displayInfo('Favor descrever uma especificação para este produto.');
                return false;
            }

            if ($scope.novoProduto.MotivoPromocao === undefined) {

                notificationService.displayInfo('Favor descrever um motivo para adicionar este produto como promoção.');
                return false;
            }

            if ($scope.novoProduto.ValidadeProd === undefined) {

                notificationService.displayInfo('Favor adicionar a validade do produto.');
                return false;
            }

            if ($scope.novoProduto.InicioPromocao === undefined) {

                notificationService.displayInfo('Favor adicionar a data de início para está promoção.');
                return false;
            }
            if ($scope.novoProduto.FimPromocao === undefined) {

                notificationService.displayInfo('Favor adicionar a data final para está promoção.');
                return false;
            }

            if (nomeImage == null) {

                notificationService.displayInfo('Adicione uma imagem para este produto!');
                return false;
            }

            if ($scope.novoProduto.PromocaoFormaPagto == undefined) {
                notificationService.displayInfo('Selecione pelo menos uma forma de pagamento!');
                return false;
            }




            return true;
        }

        function validaCamposEditar() {


            $scope.novoProduto.PromocaoFormaPagto = criarObjetoFormaPagamento($scope.arrayFormaPagto);

            if ($scope.novoProduto.InicioPromocao > $scope.novoProduto.FimPromocao) {

                notificationService.displayInfo('Data de "Início da Promoção" não pode ser maior que Data "Final da Promoção"');
                return false;
            }


            if ($scope.novoProduto.CategoriaId === undefined) {

                notificationService.displayInfo('Favor selecionar uma Categoria');
                return false;
            }

            if ($scope.novoProduto.SubCategoriaId === undefined) {

                notificationService.displayInfo('Favor selecionar Sub-Categoria');
                return false;
            }
            if ($scope.novoProduto.DescProduto === undefined) {

                notificationService.displayInfo('Favor Preencher campo Produto.');
                return false;
            }
            if ($scope.novoProduto.UnidadeMedidaId === undefined) {

                notificationService.displayInfo('Favor selecionar Unidade de Medida.');
                return false;
            }
            if ($scope.novoProduto.MarcaId === undefined) {

                notificationService.displayInfo('Favor selecionar o Marca.');
                return false;
            }
            if ($scope.novoProduto.QtdProdutos === undefined) {

                notificationService.displayInfo('Favor adicionar a quantidade de produtos para está promoção.');
                return false;
            }
            if ($scope.novoProduto.PrecoMedio === undefined) {

                notificationService.displayInfo('Favor adicionar o preço real deste produto.');
                return false;
            }
            if ($scope.novoProduto.PrecoPromocao === undefined) {

                notificationService.displayInfo('Favor adicionar o preço de promoção.');
                return false;
            }
            if ($scope.novoProduto.Especificacao === undefined) {

                notificationService.displayInfo('Favor descrever uma especificação para este produto.');
                return false;
            }

            if ($scope.novoProduto.MotivoPromocao === undefined) {

                notificationService.displayInfo('Favor descrever um motivo para adicionar este produto como promoção.');
                return false;
            }

            if ($scope.novoProduto.ValidadeProd === undefined) {

                notificationService.displayInfo('Favor adicionar a validade do produto.');
                return false;
            }

            if ($scope.novoProduto.InicioPromocao === undefined) {

                notificationService.displayInfo('Favor adicionar a data de início para está promoção.');
                return false;
            }
            if ($scope.novoProduto.FimPromocao === undefined) {

                notificationService.displayInfo('Favor adicionar a data final para está promoção.');
                return false;
            }
            if ($scope.novoProduto.PromocaoFormaPagto.length === 0) {
                notificationService.displayInfo('Selecione pelo menos uma forma de pagamento!');
                return false;
            }


            return true;
        }
        //12---------------------------------------------------------



        //13-----------------Inserir Imagem--------------------------
        function prepareFiles($files) {
            nomeImage = $files;
        }

        function inserirImagem() {
            inserirImagemModel();
        }

        function inserirImagemModel() {

            if (nomeImage) {

                //$files: uma array de arquivos selecionados
                for (var i = 0; i < nomeImage.length; i++) {

                    var $file = nomeImage[i];

                    var identificaUrl = apiService.identificaAmbiente();
                    var requestUrl = identificaUrl + "/api/produtopromocional/images/upload?produtoId=";

                    (function () {

                        $upload.upload({

                            url: requestUrl + $scope.novoProduto.Id + "&imageGr=" + false, // webapi url

                            method: "POST",

                            file: $file

                        }).progress(function (evt) {

                        }).success(function (data, status, headers, config) {

                            $scope.novaImagem = data;


                        }).error(function (data, status, headers, config) {

                            notificationService.displayError('Erro ao salvar imagem!\n\n' + data.Message);

                        });
                    })();

                }
            }

        }
        //13---------------------------------------------------------


        //14-----------------Novo Produto----------------------------
        function inserirNovoProduto() {

            $scope.novoProduto = {};
            $scope.novaImagem = {};
            $scope.arrayFormaPagto = [];
        }
        //14---------------------------------------------------------


        //15-----------------Novo Produto----------------------------
        function inseriFormaPagto(pagamento) {

            if (pagamento.selected) {
                $scope.arrayFormaPagto.push(pagamento);
                objFormaPagViewModel = {};
            } else {
                for (var i = 0; i < $scope.arrayFormaPagto.length; i++) {
                    if ($scope.arrayFormaPagto[i].Id == pagamento.Id) {
                        var index = $scope.arrayFormaPagto.indexOf(pagamento);
                        $scope.arrayFormaPagto.splice(index, 1);
                    }
                }
            }

        }
        //15---------------------------------------------------------


        //16---------------Pesquisar Formas de Pagamento-------------
        function pesquisarFormasPagamento(parameters) {

            apiService.get('/api/produtopromocional/formaspagamentofornecedor', null,
                pesquisarFormasPagamentoLoadCompleted,
                pesquisarFormasPagamentoLoadFailed);
        }

        function pesquisarFormasPagamentoLoadCompleted(result) {


            $scope.FormasPagamentoFornecedor = result.data;
            arrayNovoFormapagamento = result.data;

        }

        function pesquisarFormasPagamentoLoadFailed(result) {

            notificationService.displayError('Erro ao carregar formas de pagamento para cadastro da promoção!');
        }
        //16---------------------------------------------------------


        //17------------Open Dialog Detalhe Promoção----------------
        function openDialogDetalhePromocao(promocao) {

            $scope.detalhesPromocao = promocao;

            $modal.open({
                templateUrl: 'scripts/SPAFornecedor/promocoes/modalDetProdPromocao.html',
                controller: 'modalDetProdPromocaoCtrl',
                //backdrop: 'static',
                scope: $scope,
                size: ''
            });
        }
        //17--------------------------------------------------------

        function criarObjetoFormaPagamento(array) {

            var array2 = [];

            for (var i = 0; i < array.length; i++) {
                objFormaPagViewModel.IdProdutoPromocionalId = null;
                objFormaPagViewModel.FormaPagtoId = array[i].Id;
                objFormaPagViewModel.DescFormaPagamento = array[i].DescFormaPagto;
                array2.push(objFormaPagViewModel);
                objFormaPagViewModel = {};
            }

            return array2;
        }
        
        pesquisarSubCategoria();
        pesquisarCategoria();
        pesquiarUnidadeMedida();
        pesquisarFormasPagamento();
        pesquisarProduto();
    }

})(angular.module('ECCFornecedor'));