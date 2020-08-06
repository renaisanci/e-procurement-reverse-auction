 
using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class FornecedorConfig : EntidadeBaseConfig<Fornecedor>
    {
        public FornecedorConfig()
        {
            Property(p => p.PessoaId).IsRequired();
            Property(p => p.Responsavel).IsRequired().HasMaxLength(1000);
            Property(p => p.Descricao).IsOptional();
            Property(p => p.PalavrasChaves).IsOptional().HasMaxLength(1000); ;
            Property(p => p.PrimeiraAvista).IsOptional();
            Property(p => p.VlPedidoMin).IsRequired();
            Property(p => p.Observacao).IsOptional().HasMaxLength(500);
            Property(p => p.ObservacaoEntrega).IsOptional().HasMaxLength(500);
            Property(p => p.DataFimPeriodoGratuito).IsOptional();
            HasMany(p => p.FornecedorFormaPagtos).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
            HasMany(p => p.FornecedorCategorias).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
            HasMany(p => p.FornecedorRegiao).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
            HasMany(p => p.FornecedorRegiaoSemanal).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
            HasMany(p => p.Faturas).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
            HasMany(p => p.Produtos).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
            HasMany(p => p.Franquias).WithRequired(r => r.Fornecedor).HasForeignKey(r => r.FornecedorId);
        }
    }
}
