using System.Collections.Generic;
using ECC.EntidadeAvisos;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;

namespace ECC.Servicos.Abstrato
{
    public interface IUtilService
    {
        void MembroInserirUsuarioEnviarEmail(int MembroId, int UsuarioId);

        void FornecedorInserirUsuarioEnviarEmail(int FornecedorId, int UsuarioId);

        Franquia FranquiaInserirUsuarioEnviarEmail(int franquiaId, Usuario usuario, Telefone telefone); 
        
        void EnviaEmailPedido(int pedidoId, int situacao, Usuario Usuario);

        void EnviaEmailPedidoPromocao(int pedidoId, Usuario Usuario);

        void EnviarEmailPrecificacaoAutomatica(int pedidoId, Usuario Usuario);

        string MontaGridItensPedido(List<ItemPedido> itensPedido);

        /// <summary>
        /// Inserir Avisos para notificar Membro ou Fornecedor.
        /// </summary>
        /// <param name="usuario">Usuário a ser notificado.</param>
        /// <param name="tipoAviso">Aviso da Tabela TipoAviso.</param>
        /// <param name="tituloAviso">Título para o aviso.</param>
        /// <param name="descricaoAviso">Descrição para o aviso.</param>
        /// <param name="tooltip">Tooltip ao passar o mouse.</param>
        /// <param name="urlDestino">Url que vai quandoo usuário clicar.</param>
        /// <param name="modulo">Módulo Adm = 1, Cliente = 3, Fornecedor = 4, Franquia = 5.</param>
        /// <param name="idReferencia">Id que irá referenciar o Aviso.</param>
        void InserirAvisos(Usuario usuario, TipoAviso tipoAviso, string tituloAviso, string descricaoAviso, 
            string tooltip, string urlDestino, int modulo, int idReferencia);

        object CotacaoProdsGroup(int usuarioId, int cotacaoId);

        string RemoverAcentos(string texto);

        void EnviaEmailSmsNovoPedidoItemfornecedor(int pedidoId, int idItem, Usuario usuario, bool existePedParaAprovar);

    }
}

