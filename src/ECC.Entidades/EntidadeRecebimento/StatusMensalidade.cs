namespace ECC.EntidadeRecebimento
{
    public enum StatusMensalidade
    {
        Gerado = 1, 
        Recebido = 2,
        AguardandoPagamento = 3,
        NaoPago = 4,
        Devolvido = 5,
        Contestado = 6,
        Link = 7,
        Cancelado = 8,
        AssinaturaExpirada = 9
    }
}
