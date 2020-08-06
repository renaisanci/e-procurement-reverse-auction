(function (app) {
    'use strict';

    app.controller('alteraSenhaCtrl', alteraSenhaCtrl);

    alteraSenhaCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService', 'membershipService'];

    function alteraSenhaCtrl($scope, $modalInstance, $timeout, apiService, notificationService, membershipService) {

        $scope.cancel = cancel;
        $scope.salvarNovaSenha = salvarNovaSenha;
        $scope.usuarioNovaSenha = {
            perfilModulo: 1
        };
        $scope.usuarioAcesso = {
            perfilModulo: 1
        };


        //1---------Fecha Modal---------------------
        function cancel() {
            $modalInstance.close();
        }
        //------------------------------------------
      

        //2---------------- Alterar Senha ----------------------------
        function salvarNovaSenha() {
            if ($scope.novaSenha0 != '' && $scope.novaSenha0 != undefined)
            {
                
                if ($scope.novaSenha1 == $scope.novaSenha2  && $scope.novaSenha1 != undefined)
                {
                    //Alterar Senha
                    
                    $scope.usuarioNovaSenha.Senha = $scope.novaSenha0;
                    $scope.usuarioNovaSenha.NovaSenha = $scope.novaSenha1;
                    $scope.usuarioNovaSenha.ConfirmSenha = $scope.novaSenha2;

                    apiService.post('/api/usuario/salvarnovasenha', $scope.usuarioNovaSenha, // $scope.usuario,
                    salvarNovaSenhaSucesso,
                    salvarNovaSenhaFalha);

//                    notificationService.displaySuccess("Olá " + $scope.usuario.Nome);
 //                   $scope.userData.displayUserInfo();

                }
                else {
                    notificationService.displayError('Senhas incompatíveis.');
                }

            }
        }

        function salvarNovaSenhaSucesso(result)
        {
            
            if (result.status == 200)
            {
                notificationService.displaySuccess('Senha alterada com sucesso.');

                $scope.usuarioAcesso = result.data
                $scope.usuarioAcesso.perfilModulo = result.data.PerfilId;
                $scope.usuarioAcesso.Senha = $scope.novaSenha1

                membershipService.saveCredentials($scope.usuarioAcesso, result.data.UsuarioNome);
                $modalInstance.close();
            }

        }

        function salvarNovaSenhaFalha(response) {
            //console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);

        }
        //------------------------------------------------------------


    }

})(angular.module('ECCCliente'));