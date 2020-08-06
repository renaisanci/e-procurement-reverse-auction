using System;
using System.Collections.Generic;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.EntidadeUsuario;

namespace ECC.EntidadeAvisos
{
    public class Avisos : EntidadeBase
    {
        public Avisos()
        {

        }

        public int IdReferencia { get; set; }
        //public virtual Pedido Pedido { get; set; }
        public DateTime DataUltimoAviso { get; set; }
        public Boolean ExibeNaTelaAvisos { get; set; }

        /// <summary>
        /// // Tipo Aviso
        /// // 3 - Aviso de Pedido Pendente de Aceito Membro/Fornecedor
        /// </summary>
        public int TipoAvisosId { get; set; }
        public virtual TipoAvisos TipoAviso { get; set; }
        public string TituloAviso { get; set; }
        public string DescricaoAviso { get; set; }
        public string ToolTip { get; set; }
        public string URLPaginaDestino { get; set; }
        public int ModuloId { get; set; }

        //acho q tem q alterar isso aqui 1 para N no caso de ter q notificar todos os usuários do fornecedor
        //desse jeito tenho q gerar varios registro no banco só por causa do ID do usuário
        public int? UsuarioNotificadoId { get; set; }
        public virtual Usuario UsuarioNotificado { get; set; }
    }


    public class RetornoAvisos
    {
        public RetornoAvisos()
        {

        }

        public bool EnviarEmail { get; set; }
        public bool EnviarSMS { get; set; }
        public bool NovoAviso { get; set; }

        public Usuario Usuario { get; set; }

        public Avisos Aviso { get; set; }
    }

    public enum TipoAviso
    {
        PedidoPendentedeAceiteMembro = 1,
        PedidoPendentedeAceiteFornecedor = 8,
        PendentedeAceiteFornecedorMembro = 3,
        PedidoCancelado = 4,
        PedidoPromocionalCancelado = 11,
        PedidoCotacaoItensCancelados = 12,
        NovaCotacao = 7,
        PedidoItensAprovados = 15,
        FornecedorAceitaTrabalharMembro = 6,
        AprovarPedidoPromocional = 18,
        NovoFornecedorAvisoMembro = 19,
        FranquiaRetiraFornecedorCotacoes = 21,
        PedidoEntregueFornecedor = 23
    }
}
