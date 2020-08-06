var s = document.createElement('script');
s.type = 'text/javascript';
var v = parseInt(Math.random() * 1000000);
s.src = 'https://sandbox.gerencianet.com.br/v1/cdn/2498b7a8f0d593b9659acd3e12369878/' + v;
s.async = false;
s.id = '2498b7a8f0d593b9659acd3e12369878';
if (!document.getElementById('2498b7a8f0d593b9659acd3e12369878')) {
    document.getElementsByTagName('head')[0].appendChild(s);
}
$gn = {
    validForm: true,
    processed: false,
    done: {},
    ready: function (fn) {
        $gn.done = fn;
    }
};
