$(document).ready(function () {

 

 

    $(document).on('ready simularClique', function () {



        $('[data-rel=tooltip]').tooltip();
        $('[data-rel=popover]').popover({ html: true });
        var menuLateral = $('#sidebar-collapse');
        menuLateral.click();

        $(".inputs").keyup(function () {
            if (this.value.length === this.maxLength) {
                $(this).next('.inputs').focus();
            }
        });

    });
    // Chama a função de clique para simular um clique no menu, pois o 
    //mesmo não funcionado sem um primeiro clique no botão 
    //que diminuir o menu na letaral
    //$(document).trigger('simularClique');

});
