using ECC.EntidadePessoa;
using System.Collections.Generic;
using ECC.Entidades;
using System.Data.Entity.Spatial;

namespace ECC.EntidadeEndereco
{
    public class Endereco : EntidadeBase
    {

        public Endereco()
        {
            HorasEntregaMembro = new List<HorasEntregaMembro>();
        }

        public int PessoaId { get; set; }
        public int EstadoId { get; set; }
        public int CidadeId { get; set; }
        public int BairroId { get; set; }
        public int LogradouroId { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string DescEndereco { get; set; }
        public string Cep { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual Bairro Bairro { get; set; }
        public virtual Logradouro Logradouro { get; set; }
        public bool EnderecoPadrao { get; set; }
        public string Referencia { get; set; }
        public DbGeography Localizacao { get; set; }
      
        public virtual ICollection<HorasEntregaMembro> HorasEntregaMembro { get; set; }

    }
}

