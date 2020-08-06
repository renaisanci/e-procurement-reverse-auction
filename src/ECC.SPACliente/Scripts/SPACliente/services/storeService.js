(function (app) {
    'use strict';

    app.factory('storeService', storeService);

    storeService.$inject = ['apiService', 'notificationService', '$http', '$base64', '$cookieStore', '$rootScope'];

    function storeService(apiService, notificationService, $http, $base64, $cookieStore, $rootScope) {

        // create shopping cart
        var myCart = new shoppingCart("economizaStore");
        var myCartPromocoes = new shoppingCartPromocoes("economizaStorePromocao");

        // enable PayPal checkout
        // note: the second parameter identifies the merchant; in order to use the 
        // shopping cart with PayPal, you have to create a merchant account with 
        // PayPal. You can do that here:
        // https://www.paypal.com/webapps/mpp/merchant
        myCart.addCheckoutParameters("PayPal", "renaisanci@gmail.com");
        myCartPromocoes.addCheckoutParametersPromocao("PayPal", "renaisanci@gmail.com");

        // enable Google Wallet checkout
        // note: the second parameter identifies the merchant; in order to use the 
        // shopping cart with Google Wallet, you have to create a merchant account with 
        // Google. You can do that here:
        // https://developers.google.com/commerce/wallet/digital/training/getting-started/merchant-setup
        myCart.addCheckoutParameters("Google", "500640663394527",
            {
                ship_method_name_1: "UPS Next Day Air",
                ship_method_price_1: "20.00",
                ship_method_currency_1: "USD",
                ship_method_name_2: "UPS Ground",
                ship_method_price_2: "15.00",
                ship_method_currency_2: "USD"
            }
        );

        myCartPromocoes.addCheckoutParametersPromocao("Google", "500640663394527",
           {
               ship_method_name_1: "UPS Next Day Air",
               ship_method_price_1: "20.00",
               ship_method_currency_1: "USD",
               ship_method_name_2: "UPS Ground",
               ship_method_price_2: "15.00",
               ship_method_currency_2: "USD"
           }
       );

        // return data object with store and cart
        return {

            cart: myCart,
            cartPromocoes: myCartPromocoes
        };
    }



})(angular.module('common.core'));