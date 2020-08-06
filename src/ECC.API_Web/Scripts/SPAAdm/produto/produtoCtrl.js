(function (app) {
    'use strict';

    app.controller('produtoCtrl', produtoCtrl);

    produtoCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'fileUploadService', '$upload', 'SweetAlert'];

    function produtoCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, fileUploadService, $upload, SweetAlert) {

        $scope.pageClass = 'page-produto';

        $scope.habilitaDesabilitaAbaPesquisa = habilitaDesabilitaAbaPesquisa;
        $scope.habilitaDesabilitaAbaImg = habilitaDesabilitaAbaImg;
        $scope.habilitaDesabilitaAbaCad = habilitaDesabilitaAbaCad;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.inserirProduto = inserirProduto;
        $scope.editarProduto = editarProduto;
        $scope.limpaDados = limpaDados;
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.prepareFiles = prepareFiles;
        $scope.prepareFilesImageGrande = prepareFilesImageGrande;
        $scope.inserirImagem = inserirImagem;
        $scope.inserirImagemGrande = inserirImagemGrande;
        $scope.loadCategoriaPorSegmento = loadCategoriaPorSegmento;
        $scope.filtroCategoriaSegmento = 0;

        $scope.selectCustomer = selectCustomer;


        $scope.novoProduto = {};

        $scope.produtos = [];
        $scope.subcategorias = [];
        $scope.unidademedida = [];
        $scope.exibirImagemPequena = false;
        $scope.exibirImageGrande = false;
      

        //Variavel referente a imagem
        var nomeImage = null;
        var nomeImageGrande = null;


        $scope.novaImagem = {};

        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescProduto';
        $scope.reverse = true;

        //0--------Declaracao de todas abas de cadastro de produto-------
        $scope.tabsProduto = {
            tabPesquisar: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCadProduto: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            },
            tabProdutoImg: {
                tabAtivar: "",
                tabhabilitar: false,
                contentAtivar: "tab-pane fade",
                contentHabilitar: false
            }


        };


        //1-----Carrega Produtos aba Pesquisar--------------------------
        function pesquisarProduto(page) {
            page = page || 0;

            $scope.loadingProdutos = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 20,                   
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

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingProdutos = false;


            var msg = result.data.Items.length > 1 ? " Produtos Encontrados" : "Produto Encontrado";
            if ($scope.page == 0 && $scope.novoProduto.Id == undefined)
                notificationService.displayInfo("( " + $scope.totalCount + " ) " + msg);

        }

        function produtoLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //1-----Fim-----------------------------------------


        //2-----Editar Produtos---------------------------
        function editarProduto(pesqProduto) {

            $scope.novoProduto = pesqProduto;

            pesquisarSubCategoria($scope.novoProduto.CategoriaId);

            pesquisaImagen($scope.novoProduto.Id);

            $scope.filtroCategoriaSegmento = 0;

            loadVisivelCatetoriaPorSegmento();

            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },

                tabProdutoImg: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }

            };

        }

        //2-----Fim-----------------------------------------


        //2-----Atualizar Produtos---------------------------
        function atualizarProduto() {
            apiService.post('/api/produto/atualizar', $scope.novoProduto,
                atualizarProdutoSucesso,
                atualizarProdutoFalha);
        }

        function atualizarProdutoSucesso(response) {

            notificationService.displaySuccess('Produto atualizado com sucesso!');


            $scope.novoProduto = response.data;
            pesquisarProduto();

            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabProdutoImg: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };


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

        //3---------------------Fim----------------------------


        //4-----Inserir novo Produto -----------------------------------
        function inserirProduto() {


            if ($scope.novoProduto.Id > 0) {

                atualizarProduto();

            } else {

                inserirProdutoModel();
            }

        }

        function inserirProdutoModel() {

            if (validaCampos()) {


                apiService.post('/api/produto/inserir', $scope.novoProduto,
                    inserirProdutoSucesso,
                    inserirProdutoFalha);

            }
        }

        function inserirProdutoSucesso(response) {

            $scope.novoProduto = response.data;


            notificationService.displaySuccess('Produto incluído com sucesso.');


            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabProdutoImg: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };


            pesquisarProduto();

            SweetAlert.swal({
                title: "URGENTE",
                text: "Faça o cadastro da Imagem agora, clicando na Aba Imagens.",
                type: "warning"
            });

            $scope.novaImagem.CaminhoImagem = "../../../Content/images/unknown.jpg";
            $scope.exibirImagemPequena = true;
            $scope.exibirImageGrande = false;
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

        //4----------------------------Fim------------------------------


        //------------Imagem--------------------------------------------

        function prepareFiles($files) {
            nomeImage = $files;
        }

        function prepareFilesImageGrande($files) {
            nomeImageGrande = $files;
        }

        function inserirImagem() {

            inserirImagemModel();

        }

        function inserirImagemGrande() {

            inserirImagemModelGrande();
        }

        function inserirImagemModel() {

            if (nomeImage) {

                //$files: uma array de arquivos selecionados
                for (var i = 0; i < nomeImage.length; i++) {

                    var $file = nomeImage[i];

                    (function (index) {
                        $rootScope.upload[index] = $upload.upload({

                            url: "/api/produto/images/upload?produtoId=" + $scope.novoProduto.Id + "&imageGr=" + false, // webapi url

                            method: "POST",

                            file: $file                            

                        }).progress(function (evt) {

                        }).success(function (data, status, headers, config) {

                            notificationService.displaySuccess('Imagem inserida com sucesso!');

                            $scope.novaImagem = data;

                            
                            angular.forEach(

                                angular.element("input[type='file']"),

                                function (inputElem) {

                                    angular.element(inputElem).val(null);
                                }

                            );

                            $scope.exibir = true;

                            var teste = data.CaminhoImagemGrande.length;
                            
                            var invalida = data.CaminhoImagemGrande.substring(teste - 11);


                            if (invalida == "unknown.jpg") {
                                
                                $scope.exibirImageGrande = true;
                                $scope.novaImagem.CaminhoImagemGrande = "../../Content/images/unknown.jpg";

                            }

                        }).error(function (data, status, headers, config) {
                            notificationService.displayError(data.Message);
                        });
                    })(i);
                }
            }

        }

        function inserirImagemModelGrande() {

            if (nomeImageGrande) {

                //$files: uma array de arquivos selecionados
                for (var i = 0; i < nomeImageGrande.length; i++) {

                    var $file = nomeImageGrande[i];


                    (function (index) {
                        $rootScope.upload[index] = $upload.upload({

                            url: "/api/produto/images/upload?produtoId=" + $scope.novoProduto.Id + "&imageGr=" + true, // webapi url

                            method: "POST",

                            file: $file

                        }).progress(function (evt) {

                        }).success(function (data, status, headers, config) {

                            notificationService.displaySuccess('Imagem grande inserida com sucesso!');

                            $scope.novaImagem = data;

                            angular.forEach(

                                angular.element("input[type='file']"),

                                function (inputElem) {

                                    angular.element(inputElem).val(null);
                                }

                            );

                            $scope.exibir = true;

                        }).error(function (data, status, headers, config) {
                            notificationService.displayError(data.Message);
                        });
                    })(i);
                }
            }





        }

        //--------------Fim Imagem----------------------------------------------


        //5------------Preencher DropDown SubCategoria ------------------------
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

        //5----------------------------------------------------------------------


        //6------------Preencher DropDown Unidade Medida ------------------------
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

        //6----------------------------------------------------------------------



        //6------------Preencher DropDown Marca ------------------------

        function selectCustomer($item) {
            if ($item) {
                $scope.novoProduto.MarcaId = $item.originalObject.Id;
            }
        }

        //6----------------------------------------------------------------------


        //7-----Carrega Categorias para DropDown Categoria------------
        function pesquisarCategoria() {


            apiService.get('/api/produto/categoria', null,
                categoriaLoadCompleted,
                categoriaLoadFailed);
        }

        function categoriaLoadCompleted(response) {


            var newItem = new function () {
                this.Id = undefined;
                this.DescCategoria = "Categoria ...";
                this.visivel = true;

            };
            response.data.push(newItem);
            $scope.categorias = response.data;
            loadVisivelCatetoriaPorSegmento()
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

        //7-----Fim-----------------------------------------



        //-----Habilitar de Desabilitar Abas------------
        function habilitaDesabilitaAbaCad() {
            var imgAba = false;

            if ($scope.novoProduto.Id > 0)
                imgAba = true;

            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },


                tabProdutoImg: {
                    tabAtivar: "",
                    tabhabilitar: imgAba,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: imgAba
                }
            };
        }



        //-----Habilitar de Desabilitar Abas------------
        function habilitaDesabilitaAbaPesquisa() {

            $scope.novoProduto = {};
            $scope.filtroProduto = '';
            $scope.novaImagem = {};
            $scope.exibir = false;


            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabProdutoImg: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            };
        }

        //-----Fim-----------------------------------------



        //-----Habilitar de Desabilitar Aba Imagem------------
        function habilitaDesabilitaAbaImg() {




            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabProdutoImg: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };
        }

        //-----Fim-----------------------------------------



        //-----Limpa dados--------------------------------
        function limpaDados() {

            $scope.novoProduto = {};
            $scope.filtroProduto = '';
            $scope.novaImagem = {};
            $scope.exibir = false;


            $scope.tabsProduto = {
                tabPesquisar: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCadProduto: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabProdutoImg: {
                    tabAtivar: "",
                    tabhabilitar: false,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: false
                }
            }
        }

        //-----Limpa dados--------------------------------

        //-------Valida os campos do formulário----------
        function validaCampos() {

            if ($scope.novoProduto.SubCategoriaId === undefined) {

                notificationService.displayError('Favor selecionar Sub-Categoria');
                return false;
            }
            if ($scope.novoProduto.DescProduto === undefined) {

                notificationService.displayError('Favor Preencher campo Produto.');
                return false;
            }
            if ($scope.novoProduto.UnidadeMedidaId === undefined) {

                notificationService.displayError('Favor selecionar Unidade de Medida.');
                return false;
            }
            if ($scope.novoProduto.MarcaId === undefined) {

                notificationService.displayError('Favor selecionar o Marca.');
                return false;
            }


            return true;
        }

        //-----------------------------------------------




        //------------Busca imagem do produto------------------------
        function pesquisaImagen(id) {

            var config = {
                params: {
                    filter: id
                }
            };

            apiService.get('/api/produto/imagens', config,
                pesquisaImagenCompleted,
                pesquisaImagenFailed);

        }

        function pesquisaImagenCompleted(response) {

            var imagens = response.data;

            $scope.novaImagem = imagens;

            //if ($scope.novaImagem.CaminhoImagem == "../../../Content/images/unknown.jpg") {

            //    $scope.exibirImagemPequena = true;
            //    $scope.exibirImageGrande = false;

            //} else {

            //    $scope.exibirImagemPequena = true;
            //    $scope.exibirImageGrande = true;
            //}

            $scope.exibirImagemPequena = true;

            $scope.exibirImageGrande = true;


            var separando = imagens.CaminhoImagem.split(".");
            var separandoGrande = imagens.CaminhoImagemGrande.split(".");
            var semImagem = "/Content/images/unknown";


            if (separando[separando.length - 2] == semImagem) {

                $scope.exibirImagemPequena = true;
                $scope.novaImagem = response.data;
              
                $scope.exibirImageGrande = false;
                $scope.novaImagem.CaminhoImagem = "../../../Content/images/unknown.jpg";

            }

            if (separandoGrande[separandoGrande.length - 2] == semImagem && separando[separando.length - 2] != semImagem) {

                $scope.exibirImagemPequena = true;
                $scope.exibirImageGrande = true;
                $scope.novaImagem = response.data;
                $scope.novaImagem.CaminhoImagemGrande = "../../../Content/images/unknown.jpg";

            }


        }

        function pesquisaImagenFailed(response) {
            console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //----------------------------------------------------------------------


        //------------- Carrega categorias ----------------
        function loadCategoriaPorSegmento(idSeg) {
            if (idSeg != 0) {
                var config = {
                    params: {
                        segmentoId: idSeg
                    }
                };

                apiService.get('/api/produto/categoriaporsegmento', config,
                        loadCategoriaPorSegmentoSucesso,
                        loadCategoriaPorSegmentoFailed);
            }
            else {
                $scope.categoriasSeg = [];
                loadVisivelCatetoriaPorSegmento();
            }
        }

        function loadCategoriaPorSegmentoSucesso(response) {
            $scope.categoriasSeg = response.data;
            loadVisivelCatetoriaPorSegmento();
        }

        function loadCategoriaPorSegmentoFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadVisivelCatetoriaPorSegmento() {

            for (var i = 0; i < $scope.categorias.length; i++) {
                
                    if ($scope.categorias[i].Id == undefined) {
                        $scope.categorias[i].visivel = true;
                        break;
                    }
                    else {
                        $scope.categorias[i].visivel = false;
                    }
                
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
        //------------------------fim-----------------------------

        //-----Carrega Segmentos --
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
        //------------------------fim-----------------------------

        
        pesquisarSubCategoria();
        pesquisarCategoria();
        pesquiarUnidadeMedida();
       // pesquisarProduto();
        loadSegmentos();
    }


})(angular.module('ECCAdm'));