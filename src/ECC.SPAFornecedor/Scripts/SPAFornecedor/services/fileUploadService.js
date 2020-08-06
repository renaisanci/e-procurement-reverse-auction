(function (app) {
    'use strict';

    app.factory('fileUploadService', fileUploadService);

    fileUploadService.$inject = ['$rootScope', '$http', '$timeout', '$upload', 'notificationService','apiService'];

    function fileUploadService($rootScope, $http, $timeout, $upload, notificationService, apiService) {

        $rootScope.upload = [];
        var identificaUrl = apiService.identificaAmbiente();
        var requestUrl = identificaUrl + "/api/produtopromocional/images/upload?produtoId=";

        var service = {
            uploadImage: uploadImage
        }

        function uploadImage($files, produtoId) {
            //$files: an array of files selected
           
                var $file = $files;
                (function () {
                    $rootScope.uploaded = $upload.upload({

                        url: requestUrl + produtoId + "&imageGr=" + false, // webapi url

                        method: "POST",

                        file: $file

                    }).progress(function (evt) {

                    }).success(function (data, status, headers, config) {

                        notificationService.displaySuccess('Imagem inserida com sucesso !');

                        $scope.novaImagem = data;


                    }).error(function (data, status, headers, config) {


                        notificationService.displayError(data.Message);

                    });
                })();
        }

        return service;
    }

})(angular.module('common.core'));