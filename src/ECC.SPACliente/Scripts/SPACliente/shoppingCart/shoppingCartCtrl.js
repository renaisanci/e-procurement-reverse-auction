(function (app) {
    'use strict';

    app.controller('shoppingCartCtrl', shoppingCartCtrl);

    shoppingCartCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$modal', '$rootScope', '$location', 'apiService', '$stateParams', 'storeService', 'SweetAlert', '$filter'];

    function shoppingCartCtrl($scope, membershipService, notificationService, $modal, $rootScope, $location, apiService, $stateParams, storeService, SweetAlert, $filter) {
        $scope.pageClass = 'page-shoppingCart';

        $scope.fornRemPedCot = [];

        if (localStorage["fornRemPedCot"] != null && localStorage["fornRemPedCot"] != undefined) {

            $scope.fornRemPedCot = JSON.parse(localStorage["fornRemPedCot"]);
        }

        $scope.cart = storeService.cart;
        $scope.carregaCarrinho = carregaCarrinho;
        $scope.cartPromocoes = storeService.cartPromocoes;
        $scope.inserirPedido = inserirPedido;
        $scope.inserirPedidoPromocao = inserirPedidoPromocao;
        $scope.openEndDialog = openEndDialog;
        $scope.formaPagamento = formaPagamento;
        $scope.limparCarrinhoCompras = limparCarrinhoCompras;
        $scope.desabilitaLimparCarrinho = desabilitaLimparCarrinho;
        $scope.desabilitaTrocaEndereco = desabilitaTrocaEndereco;
        $scope.habilitaDesabilitaAbaCotacao = habilitaDesabilitaAbaCotacao;
        $scope.habilitaDesabilitaAbaPromocao = habilitaDesabilitaAbaPromocao;
        $scope.salvaListaCompras = salvaListaCompras;

        $scope.EnderecoPadrao = {};
        $scope.novoArrayPedPromocao = [];
        $scope.arrayQtdPedidosData = [];



        //var date = new Date(Date.now());
        //var hour = date.getHours();
        //if (hour >= 11) {
        //    date.setDate(date.getDate() + 1);
        //}
        //var week = date.getDay();
        //if (week === 6)
        //    date.setDate(date.getDate() + 1);
        //week = date.getDay();
        //if (week === 0)
        //    date.setDate(date.getDate() + 1);
        //var day = date.getDate();
        //var month = date.getMonth();
        //var year = date.getFullYear();
        //var minDate = new Date(year, month, day, 12);
        //$scope.dtMin = minDate;
        //$scope.dtAbertura = minDate;

        //0--------Declaracao de todas as abas tela Carrinho de Compras------
        $scope.tabsCarrinhoCompras = {
            tabCarrinhoCotacao: {
                tabAtivar: "active",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade in active",
                contentHabilitar: true
            },
            tabCarrinhoPromocao: {
                tabAtivar: "",
                tabhabilitar: true,
                contentAtivar: "tab-pane fade",
                contentHabilitar: true
            }
        };

        function habilitaDesabilitaAbaCotacao() {

            $scope.tabsCarrinhoCompras = {
                tabCarrinhoCotacao: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                },
                tabCarrinhoPromocao: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                }
            };
        }

        function habilitaDesabilitaAbaPromocao() {

            $scope.tabsCarrinhoCompras = {
                tabCarrinhoCotacao: {
                    tabAtivar: "",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade",
                    contentHabilitar: true
                },
                tabCarrinhoPromocao: {
                    tabAtivar: "active",
                    tabhabilitar: true,
                    contentAtivar: "tab-pane fade in active",
                    contentHabilitar: true
                }
            };


        }
        //0----------------------------------------------------------------


        //1--------------Insere pedido-----------------------------------------------------
        function inserirPedidoPromocao() {

            $scope.novoArrayPedPromocao = [];
            var contador = 0;
            var itemsPromocao = $scope.cartPromocoes.itemsPromocao;
            var novoItemPromocao = {};

            if (itemsPromocao.length > 0) {

                for (var k = 0; k < itemsPromocao.length; k++) {

                    if (parseInt(itemsPromocao[k].quantity) < itemsPromocao[k].quantidadeMinima) {
                        notificationService.displayInfo('AUMENTE A QUANTIDADE DO PRODUTO "' + itemsPromocao[k].name +
                            '", POIS ESTÁ ABAIXO DA QUANTIDADE MÍNIMA !');
                        contador++;

                    } else if (parseInt(itemsPromocao[k].quantity) > itemsPromocao[k].quantidadeEstoque) {

                        notificationService.displayInfo('QUANTIDADE ACIMA DO DISPONÍVEL PARA O PRODUTO "' +
                            itemsPromocao[k].name + '" !');
                        contador++;

                    } else {

                        novoItemPromocao = {
                            name: itemsPromocao[k].name,
                            sku: itemsPromocao[k].sku,
                            price: isNaN(itemsPromocao[k].price) === true ? 0 : itemsPromocao[k].price,
                            quantity: itemsPromocao[k].quantity
                        };
                        $scope.novoArrayPedPromocao.push(novoItemPromocao);
                    }
                }

                if (contador === 0) {
                    pesquisaFormaPagamentoFornecedor($scope.novoArrayPedPromocao);
                }
            }
        }

        function inserirPedido() {

            var items = $scope.cart.items;
            var novoArrayItens = [];
            var cont = 0;

            if (items.length > 0) {
                for (var j = 0; j < items.length; j++) {
                    if (parseInt(items[j].quantity) === 0 || items[j].quantity === undefined || items[j].quantity === "") {
                        notificationService.displayInfo('Produto ' + items[j].name +
                            ' está com quantidade "0", tire do carrinho ou aumente a mesma!');

                        cont++;
                    }
                }

                if ($rootScope.repository.loggedUser.faturaMensalidade) {

                    SweetAlert.swal({
                        title: "Período Gratuito Expirou :(",
                        text: "Escolha um ''Plano'', ou vá em ''Pagamentos''\n" +
                            "para verificar possíveis pendências financeiras.",
                       // type: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "Planos",
                        cancelButtonText: "Pagamentos",
                        closeOnConfirm: true,
                        closeOnCancel: true
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                $location.path("/perfil/planos");
                            } else {
                                $location.path("/pagamento/pendente");
                            }
                        });

                } else {

                    if (cont === 0) {
                        SweetAlert.swal({
                            title: "VOCÊ TEM CERTEZA?",
                            text: "Confirmou todas as informações do seu pedido, como Endereço, Quantidade e Data? ",
                            type: "warning",
                            showCancelButton: true,
                            confirmButtonColor: "#DD6B55",
                            confirmButtonText: "Ok",
                            cancelButtonText: "Cancelar",
                            closeOnConfirm: false,
                            closeOnCancel: false
                        },
                            function (isConfirm) {
                                if (isConfirm) {

                                    for (var i = 0; i < items.length; i++) {

                                        var novoItem = {
                                            name: items[i].name,
                                            sku: items[i].sku,
                                            price: isNaN(items[i].price) ? 0 : items[i].price,
                                            quantity: items[i].quantity,
                                            flgOutraMarca: isNaN(items[i].flgOutraMarca) ? false : items[i].flgOutraMarca
                                        };

                                        novoArrayItens.push(novoItem);
                                    }

                                    var config = {
                                        EnderecoId: $scope.EnderecoPadrao.Id,
                                        DtCotacao: $filter('date')($scope.dataCotacao, 'dd/MM/yyyy HH:mm:ss'),
                                        Items: novoArrayItens,
                                        RemFornPedCot: $scope.fornRemPedCot
                                    };

                                    apiService.post('/api/pedido/inserirPedido', config,
                                        inserirPedidoSucesso,
                                        inserirPedidoFalha);
                                } else {

                                    SweetAlert.swal("Cancelado", "AÇÃO CANCELADA", "error");
                                }
                            });
                    }
                }
            }
        }

        function inserirPedidoSucesso(response) {



            $scope.cart.clearItems();
            $scope.cartItens = [];
            if ($scope.cartPromocoes.itemsPromocao.length === 0) {
                $scope.EnderecoPadrao = {};
            }
            localStorage.removeItem('fornRemPedCot');
            $scope.fornRemPedCot = [];

            SweetAlert.swal({
                title: "PEDIDO GERADO COM SUCESSO!",
                text: "Este pedido estará disponível para aprovação em " + $filter('date')(response.data, 'dd/MM/yyyy') + ".\n" +
                    "Acompanhe o andamento no menu Gerenciar > Pedidos > Pedidos Cotação.",
                type: "success"
            });

        }

        function inserirPedidoFalha(response) {
            //console.log(response);
            if (response.status == '400') {

                if (response.data.length > 0) {
                    for (var i = 0; i < response.data.length; i++) {
                        notificationService.displayInfo(response.data[i]);
                    }
                } else {

                    notificationService.displayWarning(response.data.ErrorMessage);
                }

            } else {
                notificationService.displayError(response.statusText);

            }


        }
        //----------------------------Fim---------------------------------------

        //2-----Carrega enderecos padrao membro------------
        function enderecoPadraoMembro() {

            var config = {
                params: {
                    usuarioEmail: $rootScope.repository.loggedUser.username
                }
            };

            apiService.get('/api/endereco/enderecoMembroPadrao', config,
                enderecosLoadCompleted,
                enderecosLoadFailed);
        }

        function enderecosLoadCompleted(response) {

            $scope.EnderecoPadrao = response.data;


        }

        function enderecosLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        //----Fim----------------------------------------------------------------

        //3-----Abre popup troca endereço-------------------------------------------------------
        function openEndDialog(endereco) {
            $scope.enderecoP = endereco;
            $modal.open({
                templateUrl: 'scripts/SPACliente/endereco/enderecos.html',
                controller: 'enderecosCtrl',
                size: 'lg',
                scope: $scope
            }).result.then(function ($scope) {

                trocaEnd($scope);

            }, function () {
            });


        }
        //---------------------------------------------------------------------------------------

        //4-----------------Atualiza endereco selecionado no popup de troca de endereço----------
        function trocaEnd(endselect) {
            //endselect.EnderecoPadrao = true;
            $scope.EnderecoPadrao = endselect;

        }
        //---------------------------------------------------------------------------------------

        //5------------------------Limpar carrinho de compras-----------------------------------
        function limparCarrinhoCompras(cotacao, promocao) {

            if (cotacao) {
                $scope.cart.clearItems();
                $scope.cartItens = [];
                localStorage.removeItem('fornRemPedCot');
                $scope.fornRemPedCot = [];
            } else if (promocao) {
                $scope.cartPromocoes.clearItemsPromocao();
            }
        }

        function desabilitaLimparCarrinho(cotacao, promocao) {

            if (cotacao) {
                if ($scope.cart.getTotalCount() < 1) {
                    return true;
                }


            } else if (promocao) {
                if ($scope.cartPromocoes.getTotalCountPromocao() < 1) {
                    return true;
                }

            }
            return false;
        }

        function desabilitaTrocaEndereco(cotacao, promocao) {

            if (cotacao) {
                if ($scope.cart.getTotalCount() < 1) {
                    return true;
                }
            } else if (promocao) {
                if ($scope.cartPromocoes.getTotalCountPromocao() < 1) {
                    return true;
                }

            }
            return false;

        }
        //5------------------------------------------------------------------------------------

        //6-----------Inserir Forma Pagamento Promoção------------------------------------------
        function pesquisaFormaPagamentoFornecedor(itensPromocao) {

            apiService.post('/api/pedido/pesquisaFornecedorProduto', itensPromocao,
                pesquisaFormaPagamentoFornecedorLoadCompleted,
                pesquisaFormaPagamentoFornecedorLoadFailed);

            function pesquisaFormaPagamentoFornecedorLoadCompleted(result) {

                var formaPagto = result.data;
                var desconto = 0;
                for (var k = 0; k < formaPagto.length; k++) {
                    for (var i = 0; i < formaPagto[k].Fornecedor.FormaPagtos.length; i++) {
                        for (var j = 0; j < formaPagto[k].PromocaoFormaPagto.length; j++) {
                            if (formaPagto[k].Fornecedor.FormaPagtos[i].FormaPagtoId === formaPagto[k].PromocaoFormaPagto[j].FormaPagtoId) {
                                desconto = formaPagto[k].Fornecedor.FormaPagtos[i].Desconto;
                                formaPagto[k].PromocaoFormaPagto[j].Desconto = desconto;
                            }

                        }
                    }
                }

                $scope.ItensPromocionais = result.data;

                //abre popup para inserir a forma de pagamento para os pedidos
                openDialogFormaPagamento();
            }

            function pesquisaFormaPagamentoFornecedorLoadFailed(result) {
                notificationService.displayError(result.data);
            }
        }

        function openDialogFormaPagamento() {

            $modal.open({
                templateUrl: 'scripts/SPACliente/promocoes/modalPagtoPromocao.html',
                controller: 'modalPagtoPromocaoCtrl',
                backdrop: 'static',
                size: 'lg',
                scope: $scope
            });
        }

        function formaPagamento(idPagameno, idProduto, itens) {

            itens.FormaPagtoId = idPagameno;
            var desconto = 0;
            var formasPagto = itens.Fornecedor.FormaPagtos;

            //Pega o desconto do Fornecedor caso o mesmo disponha para suas formas de pagamento.
            for (var i = 0; i < formasPagto.length; i++) {
                if (formasPagto[i].FormaPagtoId === idPagameno) {
                    desconto = formasPagto[i].Desconto;
                    break;
                }
            }

            //Monta array para enviar para Api
            for (var l = 0; l < $scope.ItensPromocionais.length; l++) {
                if (itens.Id === $scope.ItensPromocionais[l].Id) {
                    var precoMedio = itens.PrecoMedio.replace(",", ".");
                    var precoPromocao = itens.PrecoPromocao.replace(",", ".");
                    $scope.novoArrayPedPromocao[l].FormaPagtoId = idPagameno;
                    $scope.novoArrayPedPromocao[l].PrecoMedioUnit = precoMedio;
                    $scope.novoArrayPedPromocao[l].PrecoNegociadoUnit = precoPromocao;
                    $scope.novoArrayPedPromocao[l].Desconto = desconto;

                    var valorPed = $scope.novoArrayPedPromocao[l].PrecoNegociadoUnit * $scope.novoArrayPedPromocao[l].quantity;
                    var valorDesconto = valorPed * desconto / 100;
                    $scope.ItensPromocionais[l].ValorPedDesconto = valorPed - valorDesconto;

                }
            }
        }
        //6--------------------------------------------------------------------------------------

        //7--------------------------------------------------------------------------------------
        function carregaAbasCarrinhosProdutos() {
            var sessao = localStorage.UrlMembro;

            if (sessao != undefined) {

                var url = localStorage.UrlMembro.split("/");

                if (url[1] === "promocoes") {
                    $scope.tabsCarrinhoCompras = {
                        tabCarrinhoCotacao: {
                            tabAtivar: "",
                            tabhabilitar: true,
                            contentAtivar: "tab-pane fade",
                            contentHabilitar: true
                        },
                        tabCarrinhoPromocao: {
                            tabAtivar: "active",
                            tabhabilitar: true,
                            contentAtivar: "tab-pane fade in active",
                            contentHabilitar: true
                        }
                    };
                }

                if (url[1] === "produto") {
                    $scope.tabsCarrinhoCompras = {
                        tabCarrinhoCotacao: {
                            tabAtivar: "active",
                            tabhabilitar: true,
                            contentAtivar: "tab-pane fade in active",
                            contentHabilitar: true
                        },
                        tabCarrinhoPromocao: {
                            tabAtivar: "",
                            tabhabilitar: true,
                            contentAtivar: "tab-pane fade",
                            contentHabilitar: true
                        }
                    };
                }

            } else {
                localStorage.UrlMembro = "/produto";

                var url2 = localStorage.UrlMembro.split("/");

                if (url2[1] === "produto") {
                    $scope.tabsCarrinhoCompras = {
                        tabCarrinhoCotacao: {
                            tabAtivar: "active",
                            tabhabilitar: true,
                            contentAtivar: "tab-pane fade in active",
                            contentHabilitar: true
                        },
                        tabCarrinhoPromocao: {
                            tabAtivar: "",
                            tabhabilitar: true,
                            contentAtivar: "tab-pane fade",
                            contentHabilitar: true
                        }
                    };
                }
            }
        }
        //---------------------------------------------------------------------------------------

        //8-----Carrega a data de cotação do pedido configurada pela franquia------------
        function carregaDataCotacaoFranquia() {

            apiService.get('/api/franquia/carregaDataCotacaoFranquia', null,
                carregaDataCotacaoFranquiaCompleted,
                carregaDataCotacaoFranquiaFailed);
        }

        function carregaDataCotacaoFranquiaCompleted(response) {

            $scope.horaCotacao = response.data.HoraCotacao;
            $scope.dataServidor = response.data.DataServidor;
            $scope.dataCotacao = response.data.DataCotacao;
            $scope.dataCotacaoCadastrada = response.data.NaoPodeEscolherData;
            $scope.diasSemanaCotacao = response.data.DiasSemanaCotacao;

        };

        function carregaDataCotacaoFranquiaFailed(response) {

            notificationService.displayError(response.data);
        };
        //----Fim------------------------------------------------------

        //9-----Abre popup Calendario----------------------------------
        $scope.openCalendario = function openCalendario() {

            $scope.close = function () {
                $modalInstance.dismiss('cancel');
            };

            if ($scope.dataCotacaoCadastrada) {

                SweetAlert.swal({
                    title: "ATENÇÃO!",
                    text: "Próxima data para cotação " + $filter('date')($scope.dataCotacao, 'dd/MM/yyyy'),
                    type: "warning",
                    html: true,
                    showCancelButton: false,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Ok",
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            abreModalDataCotacao();
                        }
                    });
            } else {
                abreModalDataCotacao();
            }
        };
        //9------------------------------------------------------------

        //------------Abre Modal Data Cotação-------------------------
        function abreModalDataCotacao() {
            $modal.open({
                templateUrl: 'scripts/SPACliente/calendarioEvento/calendarioEvento.html',
                controller: 'calendarioEventoCtrl',
                scope: $scope

            }).result.then(function (data) {

                $scope.dataCotacao = data;

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        };
        //------------------------------------------------------------

        //------------Abre Moda Remover fornecedor Cotação-------------------------
        $scope.openRemoveFornecedorCotacao = function openRemoveFornecedorCotacao(itemPrd) {
            console.log(itemPrd);

            if ($location.absUrl().indexOf("localhost") > 0) {
                $scope.categoriaId = itemPrd.imagem.split('/')[5];
            }
            else {
                $scope.categoriaId = itemPrd.imagem.split('/')[4];
            }

            $scope.prdId = itemPrd.sku;

            $modal.open({
                templateUrl: 'scripts/SPACliente/shoppingCart/removeFornCotacao.html',
                controller: 'removeFornCotacaoCtrl',
                scope: $scope

            }).result.then(function (data) {

                $scope.dataCotacao = data;

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        };

        //------------------------------------------------------------

        $scope.$on('countPrdFornRemCall', function (event, data) {

            carregaCarrinho();
            //countPrdFornRem(data);
        });

        function carregaCarrinho() {
            var t = 1;
            var auxPrd = 0;

            if (localStorage["fornRemPedCot"] !== null && localStorage["fornRemPedCot"] !== undefined) {

                $scope.fornRemPedCot = JSON.parse(localStorage["fornRemPedCot"]);
            }

            $scope.fornRemPedCot = $filter('orderBy')($scope.fornRemPedCot, 'prd');

            if ($scope.fornRemPedCot != null || $scope.fornRemPedCot != undefined)
                for (var i = 0; i < storeService.cart.items.length; i++) {

                    t = 1;


                    //if ($scope.fornRemPedCot.length == 0)
                    //    storeService.cart.items[i].qtdForn = 0;


                    var fornPrdtem = $scope.fornRemPedCot.filter(function (returnableObjects) {
                        return returnableObjects.prd == storeService.cart.items[i].sku;
                    });

                    if (fornPrdtem.length == 0) {
                        storeService.cart.items[i].qtdForn = 0;
                    }


                    for (var ii = 0; ii < $scope.fornRemPedCot.length; ii++) {

                        if ($scope.fornRemPedCot[ii].prd == storeService.cart.items[i].sku) {

                            storeService.cart.items[i].qtdForn = t++;

                        }
                    }
                }

            $scope.cartItens = storeService.cart.items;
        }

        function salvaListaCompras() {


            $scope.openListasCompras($scope.cartItens);

            //localStorage.removeItem('listaCompras');
            //$scope.ListasCompras.push({ nomelista: "Lista 01", listaCompras: $scope.cartItens });
            //localStorage.setItem("listaCompras", JSON.stringify($scope.ListasCompras));

        };

        $scope.usalista = function usarListaCompras() {
            $scope.ListasCompras = JSON.parse(localStorage.getItem("listaCompras"));
            console.log($scope.ListasCompras);
        };

        //------------Abre Modal Lista de compras-------------------------
        $scope.openListasCompras = function openListasCompras() {

            $scope.itens = $scope.cartItens;

            $modal.open({
                templateUrl: 'scripts/SPACliente/shoppingCart/listasCompras.html',
                controller: 'listasComprasCtrl',
                scope: $scope

            }).result.then(function (data) {

                $scope.dataCotacao = data;

            }, function () {

                //console.log("Modal Dismissed!!!");
            });
        };

        //------------------------------------------------------------

        //05---é chamado pela controller topBarCtrl.js-------------
        $rootScope.$on("reloadCarrinhoPrincipal", function () {
            carregaCarrinho();
            enderecoPadraoMembro();
        });
        //05-----------------------------------------------------------------

        $scope.marcaExtras = function marcaExtras(prd, flgOutraMarcaShopping) {
            for (var i = 0; i < storeService.cart.items.length; i++) {
                if (storeService.cart.items[i].sku == prd) {

                    storeService.cart.items[i].flgOutraMarca = flgOutraMarcaShopping;
                }
            }


            $scope.cartItens = storeService.cart.items;
        };


        carregaCarrinho();
        enderecoPadraoMembro();
        desabilitaLimparCarrinho();
        carregaAbasCarrinhosProdutos();
        carregaDataCotacaoFranquia();
    }

})(angular.module('ECCCliente'));