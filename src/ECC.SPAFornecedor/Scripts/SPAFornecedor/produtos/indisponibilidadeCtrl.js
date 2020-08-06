
(function (app) {
    'use strict';

    app.controller('indisponibilidadeCtrl', indisponibilidadeCtrl);

    indisponibilidadeCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', 'fornecedorUtilService', 'SweetAlert', '$modal', '$filter'];

    function indisponibilidadeCtrl($scope, $timeout, apiService, notificationService, fornecedorUtilService, SweetAlert, $modal, $filter) {

        $scope.pageClass = 'page-disponibilidade';
        $scope.pesquisarSubCategoria = pesquisarSubCategoria;
        $scope.pesquisarProduto = pesquisarProduto;
        $scope.inserirDeletarDisponibilidadeProduto = inserirDeletarDisponibilidadeProduto;
        $scope.validarInicioIndisponibilidadeProd = validarInicioIndisponibilidadeProd;
        $scope.validarFimIndisponibilidadeProd = validarFimIndisponibilidadeProd;

        $scope.openDatePickerInicioIndisponibilidadeProd = openDatePickerInicioIndisponibilidadeProd;
        $scope.openDatePickerFimIndisponibilidadeProd = openDatePickerFimIndisponibilidadeProd;

        $scope.inserirDeletarDisponibilidadeProd = inserirDeletarDisponibilidadeProd;

        $scope.format = 'dd/MM/yyyy';
        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 0
        };
        $scope.datepicker = {};
        $scope.disponibilidadeProd = {};

        //Campos necessário para funcionar a ordenação do grid
        $scope.predicate = 'DescProduto';
        $scope.reverse = true;



        //checkedall-------------------------------------------------
        function validarInicioIndisponibilidadeProd() {

            var dataHoje = new Date();
            var dia = dataHoje.getDate();
            var mes = dataHoje.getMonth();
            var ano = dataHoje.getFullYear();

            var novadata = new Date(ano, mes, dia);

            if ($scope.disponibilidadeProd.InicioIndisponibilidade < novadata) {

                notificationService.displayInfo('Data não pode ser menor que a data de hoje.');

                $scope.disponibilidadeProd.InicioIndisponibilidade = undefined;
            }


        }

        function validarFimIndisponibilidadeProd() {

            var dataHoje = new Date();
            var dia = dataHoje.getDate();
            var mes = dataHoje.getMonth();
            var ano = dataHoje.getFullYear();

            var novadata = new Date(ano, mes, dia);

            if ($scope.disponibilidadeProd.FimIndisponibilidade <= $scope.disponibilidadeProd.InicioIndisponibilidade) {

                notificationService.displayInfo('Data Fim não pode ser menor ou igual a Data Início');
                $scope.disponibilidadeProd.FimIndisponibilidade = undefined;
            }


        }

        function openDatePickerInicioIndisponibilidadeProd($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.openedInicio = true;

        };


        function openDatePickerFimIndisponibilidadeProd($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datepicker.openedFim = true;

        };


        //-----------------------------------------------------------


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

            apiService.get('/api/produto/indisponibilidadeProduto', null,
                disponibilidadeProdutoLoadCompleted,
                disponibilidadeProdutoLoadFailed);
        }

        function disponibilidadeProdutoLoadCompleted(result) {

            $scope.disponibilidadeProdutos = result.data;

            for (var i = 0; i < $scope.produtos.length; i++) {

                for (var j = 0; j < $scope.disponibilidadeProdutos.length; j++) {

                    if ($scope.produtos[i].Id == $scope.disponibilidadeProdutos[j].ProdutoId) {
                        $scope.produtos[i].Relacionado = true;
                        $scope.produtos[i].selected = true;
                        $scope.produtos[i].PeriodoIndisponivel = $scope.disponibilidadeProdutos[j].IndisponivelPermanente == true ? "Permanente" : $filter('date')(new Date($scope.disponibilidadeProdutos[j].InicioIndisponibilidade), 'dd/MM/yyyy') + " até " + $filter('date')(new Date($scope.disponibilidadeProdutos[j].FimIndisponibilidade), 'dd/MM/yyyy');
                    }
                }
            }

        }

        function disponibilidadeProdutoLoadFailed(result) {
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
        function inserirDeletarDisponibilidadeProduto(produto) {

            var objDisponibilidadeProduto = {};
            $scope.nomeProduto = produto.DescProduto;

            objDisponibilidadeProduto = {
                DisponibilidadeId: 0,
                FornecedorId: 0,
                ProdutoId: produto.Id
            };


            if (produto.selected) {

                SweetAlert.swal({
                    title: "Deseja realmente indisponibilizar este produto?",
                    text: "Clique em 'SIM' e adicione uma data de INÍCIO e FIM, caso seja temporariamente.",
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
                        $modal.open({
                            templateUrl: 'scripts/SPAFornecedor/produtos/modalDataIndisponibilidade.html',
                            controller: 'modalDataIndisponibilidadeCtrl',
                            backdrop: 'static',
                            scope: $scope,
                            resolve: {
                                items: function() {
                                    return objDisponibilidadeProduto;
                                } 
                            },
                            size: ''
                        });
                    } else {
                        $scope.inserirDeletarDisponibilidadeProd(true, objDisponibilidadeProduto);

                    }
                });

            } else {
                $scope.inserirDeletarDisponibilidadeProd(false, objDisponibilidadeProduto);
            }

        }

        //6----------------------------------------------------------


        //7--------Método Inserir e Deletar Disponibilidade-----------
        function inserirDeletarDisponibilidadeProd(premissa, objDisponibildadeProd) {

            apiService.post('/api/produto/inserirDeletarIndisponibilidadeProduto/' + premissa, objDisponibildadeProd,
            inserirDeletarDisponibilidadeProdutoLoadCompleted,
            inserirDeletarDisponibilidadeProdutoLoadFailed);

            function inserirDeletarDisponibilidadeProdutoLoadCompleted(result) {

                if (result.status == 201) {
                    //notificationService.displaySuccess('Você não receberá cotação para o produto (' + $scope.nomeProduto + ').');
                }

                if (result.status == 200) {
                    //notificationService.displaySuccess('Agora você receberá cotação para o produto (' + $scope.nomeProduto + ').');

                }
            }

            function inserirDeletarDisponibilidadeProdutoLoadFailed(result) {

                notificationService.displayError(result.data);
            }

        }
        //7-----------------------------------------------------------




        //8------Deletar produtos indisponiveis fora da data----------
        function deletarIndisponibilidadeForaValidade() {
            apiService.get('/api/produto/deletarIndisponibilidadeForaValidade', null,
                deletarIndisponibilidadeForaValidadeLoadCompleted,
                deletarIndisponibilidadeForaValidadeLoadFailed);
        }

        function deletarIndisponibilidadeForaValidadeLoadCompleted(result) {
            
        }

        function deletarIndisponibilidadeForaValidadeLoadFailed(result) {
            notificationService.displayError(result.data);
        }
        //8-----------------------------------------------------------

        pesquisarSubCategoria();
        pesquisarCategoria();
        deletarIndisponibilidadeForaValidade();

    }

})(angular.module('ECCFornecedor'));