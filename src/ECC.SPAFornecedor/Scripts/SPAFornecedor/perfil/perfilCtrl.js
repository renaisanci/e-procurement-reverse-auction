
(function (app) {
    'use strict';

    app.controller('perfilCtrl', perfilCtrl);

    perfilCtrl.$inject = ['$scope', '$timeout', 'apiService', 'notificationService', '$rootScope', 'membershipService', 'SweetAlert', '$modal', '$location'];

    function perfilCtrl($scope, $timeout, apiService, notificationService, $rootScope, membershipService, SweetAlert, $modal, $location) {
        $scope.novoUsuario = {}
        $scope.atualizarModelUsuario = atualizarModelUsuario;
        $scope.cancelarAssinatura = cancelarAssinatura;

        //3-----Retorna Dados-------------------------------
        function carregarUsuario() {
            apiService.get('/api/usuario/perfil', $scope.novoUsuario,
                carregarUsuarioSucesso,
                carregarUsuarioFalha);

        }

        function carregarUsuarioSucesso(response) {

            $scope.emailAnt = response.data.UsuarioEmail;

            $scope.novoUsuario = response.data;
        }

        function carregarUsuarioFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //3 ----------------------------Fim---------------------------------------//

        //3-----Atualiza Usuario-------------------------------
        function atualizarModelUsuario() {

            if ($scope.emailAnt != $scope.novoUsuario.UsuarioEmail)
                membershipService.logout();


            apiService.post('/api/usuario/atualizar', $scope.novoUsuario,
                atualizarUsuarioSucesso,
                atualizarUsuarioFalha);

        }

        function atualizarUsuarioSucesso(response) {
            notificationService.displaySuccess($scope.novoUsuario.UsuarioNome + ' Atualizado com Sucesso.');
            $scope.novoUsuario = response.data;


            if ($scope.emailAnt != response.data.UsuarioEmail)
                $rootScope.$emit("logoutExterno");

        }

        function atualizarUsuarioFalha(response) {
            //console.log(response);
            if (response.status == '400')
                for (var i = 0; i < response.data.length; i++) {
                    notificationService.displayInfo(response.data[i]);
                }
            else
                notificationService.displayError(response.statusText);
        }

        //3 ----------------------------Fim---------------------------------------//

        //4-----Cancelar Assinatura-------------------------------
        function cancelarAssinatura() {

            //if ($scope.emailAnt != $scope.novoUsuario.UsuarioEmail)
            //    membershipService.logout();

            SweetAlert.swal({
                title: "ATENÇÃO",
                text: "Tem certeza que deseja cancelar sua assinatura?",
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
                        apiService.post('/api/usuario/cancelarAssinatura', null,
                            cancelarAssinaturaSucesso,
                            cancelarAssinaturaFalha);
                    }
                });
        }

        function cancelarAssinaturaSucesso(response) {

            var isArray = Array.isArray(response.data);

            if (isArray) {

                if (response.data.length > 0) {

                    var mensagem = response.data.length > 1 ? "Existem pendências financeiras." : "Existe pendência financeira.";

                    SweetAlert.swal({
                        title: "Não foi possível cancelar sua assinatura.",
                        text: mensagem + " Clique em 'SIM' regularizar.",
                        //type: "info",
                        imageUrl: '../../Content/images/deslikeIcon.png',
                        showCancelButton: true,
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "SIM",
                        cancelButtonText: "NÃO",
                        closeOnConfirm: true,
                        closeOnCancel: true
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                $location.path('/pagamento');
                            }
                        });
                } else {

                    SweetAlert.swal({
                        title: "Assinatura cancelada com sucesso.",
                        text: "Obrigado por ter sido nosso parceiro, você será direcionado para a tela de login.",
                        type: "success",
                        confirmButtonText: "OK",
                        closeOnConfirm: true,
                    },
                        function (isConfirm) {

                            if (isConfirm) {
                                membershipService.logout();
                                $location.path("/login");
                            }
                        });

                }
            } else {

                SweetAlert.swal({
                    title: "Atenção!",
                    text: response.data,
                    type: "warning",
                    confirmButtonText: "OK"
                });
            }
        }

        function cancelarAssinaturaFalha(response) {
            //console.log(response);
            //if (response.status == '400')
            //    for (var i = 0; i < response.data.length; i++) {
            //        notificationService.displayInfo(response.data[i]);
            //    }
            //else
            //    notificationService.displayError(response.statusText);
        }

        //4 ----------------------------Fim---------------------------------------//

        carregarUsuario();
    }

})(angular.module('ECCFornecedor'));