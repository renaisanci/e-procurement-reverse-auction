//----------------------------------------------------------------
// shopping cart
//
function shoppingCartPromocoes(cartNamePromocao) {
    this.cartNamePromocao = cartNamePromocao;
    this.clearCartPromocao = false;
    this.checkoutParametersPromocao = {};
    this.itemsPromocao = [];

    // load itemsPromocao from local storage when initializing
    this.loadItemsPromocao();

    // save itemsPromocao to local storage when unloading
    var self = this;
    $(window).unload(function () {
        if (self.clearCartPromocao) {
            self.clearItems();
        }
        self.saveItemsPromocao();
        self.clearCartPromocao = false;
    });
}

// load itemsPromocao from local storage
shoppingCartPromocoes.prototype.loadItemsPromocao = function () {
    var itemsPromo = localStorage != null ? localStorage[this.cartNamePromocao + "_items"] : null;
    if (itemsPromo != null && JSON != null) {
        try {
            var itemsPromocao = JSON.parse(itemsPromo);
            for (var i = 0; i < itemsPromocao.length; i++) {
                var item = itemsPromocao[i];
                if (item.sku != null && item.name != null && item.quantity != null && item.imagem != null) {
                    item = new cartItemPromo(item.sku, item.name, item.price, item.quantity,item.imagem,item.quantidadeMinima,item.quantidadeEstoque, item.obsFrete);
                    this.itemsPromocao.push(item);
                }
            }
        }
        catch (err) {
            // ignore errors while loading...
        }
    }
}

// save itemsPromocao to local storage
shoppingCartPromocoes.prototype.saveItemsPromocao = function () {
    if (localStorage != null && JSON != null) {
        localStorage[this.cartNamePromocao + "_items"] = JSON.stringify(this.itemsPromocao);
    }
}

// adds an item to the cart
shoppingCartPromocoes.prototype.addItemPromocao = function (sku, name, price, quantity, imagem, obsFrete, quantidadeMinima, quantidadeEstoque) {
    quantity = this.toNumberPromocao(quantity);
    if (quantity !== 0) {
        // update quantity for existing item
        var found = false;
        for (var i = 0; i < this.itemsPromocao.length && !found; i++) {
            var item = this.itemsPromocao[i];
            if (item.sku === sku) {
                found = true;
                var quantidade = item.quantity === undefined || item.quantity === "" ? 0 : parseInt(item.quantity);
                item.quantity = this.toNumberPromocao(quantidade + quantity);

                //Aqui verificamos as quantidades se estão dentro das disponibilizadas
                if (item.quantidadeMinima > item.quantity) {
                    item.quantity++;
                }if (item.quantity > item.quantidadeEstoque) {
                    item.quantity = item.quantidadeEstoque;
                }else {
                    if (item.quantity <= 0) {
                        this.itemsPromocao.splice(i, 1);
                    }
                }
                //-----------Fim da verificação---------------------------------------
            }
        }

        // new item, add now
        if (!found) {
            var itemm = new cartItemPromo(sku, name, price, quantity, imagem, quantidadeMinima, quantidadeEstoque,obsFrete);
            this.itemsPromocao.push(itemm);
        }

        // save changes
        this.saveItemsPromocao();
    }
}

// remove an item to the cart
shoppingCartPromocoes.prototype.removeItemPromocao = function(sku) {

    for (var i = 0; i < this.itemsPromocao.length; i++) {
        var item = this.itemsPromocao[i];
        if (item.sku === sku) {
            this.itemsPromocao.splice(i, 1);
        }
    }

    // save changes
    this.saveItemsPromocao();
}

// return true or false case items currently in the cart
shoppingCartPromocoes.prototype.getItemsPromocao = function () {

    var itens = this.itemsPromocao.length;
    if (itens > 0) {
        return true;
    }
    return false;
}

// get the total price for all itemsPromocao currently in the cart
shoppingCartPromocoes.prototype.getTotalPricePromocao = function (sku) {
    var total = 0;
    for (var i = 0; i < this.itemsPromocao.length; i++) {
        var item = this.itemsPromocao[i];
        if (sku == null || item.sku == sku) {
            total += this.toNumberPromocao(item.quantity * item.price);
        }
    }
    return total;
}

// get the total price for all itemsPromocao currently in the cart
shoppingCartPromocoes.prototype.getTotalCountPromocao = function (sku) {
    var count = 0;
    for (var i = 0; i < this.itemsPromocao.length; i++) {
        var item = this.itemsPromocao[i];
        if (sku == null || item.sku == sku) {
            count += this.toNumberPromocao(item.quantity);
        }
    }
    return count;
}

// clear the cart
shoppingCartPromocoes.prototype.clearItemsPromocao = function () {
    this.itemsPromocao = [];
    this.saveItemsPromocao();
}

// define checkout parameters
shoppingCartPromocoes.prototype.addCheckoutParametersPromocao = function (serviceName, merchantID, options) {

    // check parameters
    if (serviceName != "PayPal" && serviceName != "Google") {
        throw "serviceName must be 'PayPal' or 'Google'.";
    }
    if (merchantID == null) {
        throw "A merchantID is required in order to checkout.";
    }

    // save parameters
    this.checkoutParametersPromocao[serviceName] = new checkoutParametersPromocao(serviceName, merchantID, options);
}

// check out
shoppingCartPromocoes.prototype.checkoutPromocao = function (serviceName, clearCart) {

    // select serviceName if we have to
    if (serviceName == null) {
        var p = this.checkoutParametersPromocao[Object.keys(this.checkoutParametersPromocao)[0]];
        serviceName = p.serviceName;
    }

    // sanity
    if (serviceName == null) {
        throw "Use the 'addCheckoutParameters' method to define at least one checkout service.";
    }

    // go to work
    var parms = this.checkoutParametersPromocao[serviceName];
    if (parms == null) {
        throw "Cannot get checkout parameters for '" + serviceName + "'.";
    }
    switch (parms.serviceName) {
        case "PayPal":
            this.checkoutPayPal(parms, clearCart);
            break;
        case "Google":
            this.checkoutGoogle(parms, clearCart);
            break;
        default:
            throw "Unknown checkout service: " + parms.serviceName;
    }
}

// check out using PayPal
// for details see:
// www.paypal.com/cgi-bin/webscr?cmd=p/pdn/howto_checkout-outside
shoppingCartPromocoes.prototype.checkoutPayPalPromocao = function (parms, clearCart) {

    // global data
    var data = {
        cmd: "_cart",
        business: parms.merchantID,
        upload: "1",
        rm: "2",
        charset: "utf-8"
    };

    // item data
    for (var i = 0; i < this.itemsPromocao.length; i++) {
        var item = this.itemsPromocao[i];
        var ctr = i + 1;
        data["item_number_" + ctr] = item.sku;
        data["item_name_" + ctr] = item.name;
        data["quantity_" + ctr] = item.quantity;
        data["amount_" + ctr] = item.price.toFixed(2);
    }

    // build form
    var form = $('<form/></form>');
    form.attr("action", "https://www.paypal.com/cgi-bin/webscr");
    form.attr("method", "POST");
    form.attr("style", "display:none;");
    this.addFormFields(form, data);
    this.addFormFields(form, parms.options);
    $("body").append(form);

    // submit form
    this.clearCart = clearCart == null || clearCart;
    form.submit();
    form.remove();
}

// check out using Google Wallet
// for details see:
// developers.google.com/checkout/developer/Google_Checkout_Custom_Cart_How_To_HTML
// developers.google.com/checkout/developer/interactive_demo
shoppingCartPromocoes.prototype.checkoutGooglePromocao = function (parms, clearCart) {

    // global data
    var data = {};

    // item data
    for (var i = 0; i < this.itemsPromocao.length; i++) {
        var item = this.itemsPromocao[i];
        var ctr = i + 1;
        data["item_name_" + ctr] = item.sku;
        data["item_description_" + ctr] = item.name;
        data["item_price_" + ctr] = item.price.toFixed(2);
        data["item_quantity_" + ctr] = item.quantity;
        data["item_merchant_id_" + ctr] = parms.merchantID;
    }

    // build form
    var form = $('<form/></form>');
    // NOTE: in production projects, use the checkout.google url below;
    // for debugging/testing, use the sandbox.google url instead.
    //form.attr("action", "https://checkout.google.com/api/checkout/v2/merchantCheckoutForm/Merchant/" + parms.merchantID);
    form.attr("action", "https://sandbox.google.com/checkout/api/checkout/v2/checkoutForm/Merchant/" + parms.merchantID);
    form.attr("method", "POST");
    form.attr("style", "display:none;");
    this.addFormFields(form, data);
    this.addFormFields(form, parms.options);
    $("body").append(form);

    // submit form
    this.clearCart = clearCart == null || clearCart;
    form.submit();
    form.remove();
}

// utility methods
shoppingCartPromocoes.prototype.addFormFieldsPromocao = function (form, data) {
    if (data != null) {
        $.each(data, function (name, value) {
            if (value != null) {
                var input = $("<input></input>").attr("type", "hidden").attr("name", name).val(value);
                form.append(input);
            }
        });
    }
}


shoppingCartPromocoes.prototype.toNumberPromocao = function (value) {
    value = value * 1;
    return isNaN(value) ? 0 : value;
}

//----------------------------------------------------------------
// checkout parameters (one per supported payment service)
//
function checkoutParametersPromocao(serviceName, merchantID, options) {
    this.serviceName = serviceName;
    this.merchantID = merchantID;
    this.options = options;
}

//----------------------------------------------------------------
// itemsPromocao in the cart
//
function cartItemPromo(sku, name, price, quantity, imagem, quantidadeMinima, quantidadeEstoque, obsFrete) {
    this.sku = sku;
    this.name = name;
    this.price = price * 1;
    this.quantity = quantity * 1;
    this.imagem = imagem;
    this.quantidadeMinima = quantidadeMinima;
    this.quantidadeEstoque = quantidadeEstoque;
    this.obsFrete = obsFrete;
}

