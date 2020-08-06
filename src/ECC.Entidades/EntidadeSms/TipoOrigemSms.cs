namespace ECC.EntidadeSms
{
    public enum TipoOrigemSms
    {
        RecuperarSenha = 1,
        NovaCotacao = 2,
        MembroSolicitaFornecedor = 3,
        FornecedorAceitaMembro = 4,
        FornecedorRecusaMembro = 5,
        PedidosPendentesAprovacaoFronecedor = 6,
        PedidosPendentesAprovacaoMembro = 7,
        PedidoPromocionalPendenteAprovacao = 8,
        FornecedorCancelaPedidoPromocional = 9,
        FornecedorAprovaItensPedido = 10,
        MembroAprovaPedido = 11,
        BoletoGerado = 12,
        LembreteFornecedorAceiteMembro = 13,
        AprovaCadastroProdutoPromocional = 14,
        FornecedorDespachoItensPedido = 15,
        MembroTrocaFornecedorItemCancelado = 16,
        CancelamentoPedidoNaoAprovadoPeloMembro = 17
    }
}
