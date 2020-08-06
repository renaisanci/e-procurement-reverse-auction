using System;
using System.Collections.Generic;
using ECC.EntidadeAvisos;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;

namespace ECC.Servicos.Abstrato
{
    public interface ICalendarioFeriadoService
    {
        /// <summary>
        /// Verifica se é feriado de acordo com a data passada e o nome da cidade.
        /// </summary>
        /// <param name="data">Data a ser verificada</param>
        /// <param name="membro">Membro a solicitar a verificação da data</param>
        /// <param name="descEstado">Nome do estado do usuário</param>
        /// <returns></returns>
        RetornoFeriadoMembro VerificaFeriadoMembro(DateTime data, Membro membro, string descEstado = null);

    }
}

