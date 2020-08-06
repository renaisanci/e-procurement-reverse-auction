using System;
using System.Collections.Generic;
using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class Mensalidade : EntidadeBase
    {
        public Mensalidade()
        {
            this.Detalhes = new List<MensalidadeDetalhe>();
        }

        public StatusMensalidade Status { get; set; }

        public string Descricao { get; set; }

        public int MembroId { get; set; }

        public virtual Membro Membro { get; set; }

        public DateTime DtVencimento { get; set; }

        public DateTime? DtRecebimento { get; set; }

        public int ChargerId { get; set; }

        public string UrlPdf { get; set; }

        public string Token { get; set; }
        
        public int PlanoMensalidadeId { get; set; }

        public DateTime? DtEfetivarPlano { get; set; }

        public virtual PlanoMensalidade PlanoMensalidade { get; set; }

        public virtual ICollection<MensalidadeDetalhe> Detalhes { get; set; }
    }
}
