using System;
using System.Collections.Generic;
using ECC.EntidadeProduto;
using ECC.Entidades;

namespace ECC.EntidadePessoa
{
    public class Franquia : EntidadeBase
    {
        public Franquia()
        {
            this.Membros = new List<Membro>();
            this.Fornecedores = new List<FranquiaFornecedor>();
            this.Produtos = new List<FranquiaProduto>();
        }

        public int PessoaId { get; set; }

        public string Responsavel { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public string Descricao { get; set; }
        public bool? DataCotacao { get; set; }

        public virtual ICollection<Membro> Membros { get; set; }

        public virtual ICollection<FranquiaFornecedor> Fornecedores { get; set; }

        public virtual ICollection<FranquiaProduto> Produtos { get; set; }
    }
}
