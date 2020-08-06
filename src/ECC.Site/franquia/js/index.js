$('.esqueci').on('click', function() {
  $('.conteudo').stop().addClass('active');
  $('.rodape').stop().addClass('active');
});

$('.close').on('click', function() {
  $('.conteudo').stop().removeClass('active');
   $('.rodape').stop().removeClass('active');
});