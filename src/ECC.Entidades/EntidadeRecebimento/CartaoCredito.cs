using ECC.EntidadePessoa;
using ECC.Entidades;
using System;

namespace ECC.EntidadeRecebimento
{
    public class CartaoCredito : EntidadeBase
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Numero { get; set; }
        public DateTime DataVencimento { get; set; }
        public string Cvc { get; set; }
        public string TokenCartaoGerenciaNet { get; set; }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }
        public int CartaoBandeiraId { get; set; }
        public virtual CartaoBandeira CartaoBandeira { get; set; }
        public bool Padrao { get; set; }
        public bool Ativo { get; set; }

    }
}
