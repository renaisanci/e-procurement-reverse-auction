using System;
using System.Collections.Generic;
using ECC.EntidadePedido;
using ECC.EntidadeUsuario;
using ECC.EntidadePessoa;
using ECC.EntidadeAvisos;

namespace ECC.Servicos.Abstrato
{
    public interface INotificacoesAlertasService
    {
        List<Pedido> ListarPedidosPendentes(Enum pCliente);
        List<RetornoAvisos> TrataPedidoMembro(Pedido MembroPedido, int TipoAvisosId);
        TipoAvisos GetTipoAviso(int avisoId);
        bool PodeEnviarNotificacao(int usuarioId, int tipoAvisosId, TipoAlerta tipoAlerta);
        List<Usuario> EnviarEmailSmsFornecedorAceitarMembro();
        Usuario UsuarioRobo();
    }
}
