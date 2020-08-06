using System;
using System.Collections.Generic;
using ECC.EntidadeProduto;
using ECC.EntidadeRecebimento;
using ECC.Entidades;

namespace ECC.EntidadePessoa
{
    public class Membro : EntidadeBase
    {
        public Membro()
        {
            MembroCategorias = new List<MembroCategoria>();
            MembroFornecedores = new List<MembroFornecedor>();
            Mensalidades = new List<Mensalidade>();
        }

        public string Comprador { get; set; }

        public bool Vip { get; set; }

        public int PessoaId { get; set; }

        public int? FranquiaId { get; set; }

        public int? PlanoMensalidadeId { get; set; }
        
        public string DddTel{ get; set; }
        public string Telefone { get; set; }

        public string DddCel { get; set; }
        public string Celular { get; set; }

        public string Contato { get; set; }

        public DateTime? DataFimPeriodoGratuito { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public virtual ICollection<MembroCategoria> MembroCategorias { get; set; }

        public virtual ICollection<MembroFornecedor> MembroFornecedores { get; set; }

        public virtual ICollection<Mensalidade> Mensalidades { get; set; }

        public virtual PlanoMensalidade PlanoMensalidade { get; set; }

        public virtual Franquia Franquia { get; set; }


    }
}
