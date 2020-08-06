using System;
using System.Data.Entity;
using ECC.Entidades;
using System.Data.Entity.ModelConfiguration.Conventions;
using ECC.Dados.EFMapConfig;
using ECC.Dados.EFMapConfig.MapArquivo;
using ECC.Dados.EFMapConfig.MapCotacao;
using ECC.Dados.EFMapConfig.MapEmail;
using ECC.Dados.EFMapConfig.MapEndereco;
using ECC.Dados.EFMapConfig.MapFormaPagto;
using ECC.Dados.EFMapConfig.MapMenu;
using ECC.Dados.EFMapConfig.MapPedido;
using ECC.Dados.EFMapConfig.MapPessoa;
using ECC.Dados.EFMapConfig.MapProduto;
using ECC.Dados.EFMapConfig.MapStatus;
using ECC.Dados.EFMapConfig.MapUsuario;
using ECC.Dados.EFMapConfig.MapEstoque;
using ECC.Dados.EFMapConfig.MapParametroSistema;
using ECC.EntidadeEndereco;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.Dados.EFMapConfig.MapSms;
using ECC.Dados.EFMapConfig.MapComum;
using ECC.Dados.EFMapConfig.MapEntrega;
using ECC.Dados.EFMapConfig.MapAvisos;
using ECC.EntidadeAvisos;
using ECC.EntidadeCotacao;
using ECC.EntidadeEmail;
using ECC.EntidadeEntrega;
using ECC.EntidadeEstoque;
using ECC.EntidadeFormaPagto;
using ECC.EntidadeMenu;
using ECC.EntidadePedido;
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeSms;
using ECC.EntidadeStatus;
using System.Diagnostics;
using ECC.Dados.EFMapConfig.MapRecebimento;
using ECC.EntidadeRecebimento;
using ECC.Entidades.EntidadeComum;
using ECC.Entidades.EntidadePessoa;
using ECC.EntidadeFranquia;
using ECC.Dados.EFMapConfig.MapFranquia;
using ECC.Dados.EFMapConfig.MapFrete;
using ECC.Dados.EFMapConfig.MapRobos;
using ECC.EntidadeListaCompras;
using ECC.EntidadeFornecedor;

namespace ECC.Dados
{
    public class EconomizajaContext : DbContext
    {
        public EconomizajaContext()

        //pega o ambiente de acordo com a variavel de sistema no servidor qdo tiver 2 server DEV E PRD
        //Assim que tiver o outro server deixa essa linha assim não precisa se preocupar com a publicação
        : base($"ECC_{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}")
        //: base($"ECC_HOM")

        {
            Database.SetInitializer<EconomizajaContext>(null);

            //Lazyload desabilitado assim irá trazer os dados relacionados das entidades
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
#if (DEBUG)
            base.Database.Log = fs;
#endif
        }

        #region Entity Sets

        public IDbSet<Pessoa> Pessoa { get; set; }
        public IDbSet<Usuario> Usuario { get; set; }
        public IDbSet<Perfil> Perfil { get; set; }
        public IDbSet<PessoaJuridica> PessoaJuridica { get; set; }
        public IDbSet<PessoaFisica> PessoaFisica { get; set; }
        public IDbSet<Endereco> Endereco { get; set; }
        public IDbSet<Estado> Estado { get; set; }
        public IDbSet<Cidade> Cidade { get; set; }
        public IDbSet<Bairro> Bairro { get; set; }
        public IDbSet<CepEndereco> CepEndereco { get; set; }
        public IDbSet<Regiao> Regiao { get; set; }
        public IDbSet<Logradouro> Logradouro { get; set; }
        public IDbSet<Fornecedor> Fornecedor { get; set; }
        public IDbSet<Membro> Membro { get; set; }
        public IDbSet<Franquia> Franquia { get; set; }
        public IDbSet<FranquiaFornecedor> FranquiaFornecedor { get; set; }
        public IDbSet<FornecedorProduto> FornecedorProduto { get; set; }
        public IDbSet<FranquiaProduto> FranquiaProduto { get; set; }
        public IDbSet<Erro> Erro { get; set; }
        public IDbSet<Telefone> Telefone { get; set; }
        public IDbSet<Produto> Produto { get; set; }
        public IDbSet<Categoria> Categoria { get; set; }
        public IDbSet<SubCategoria> SubCategoria { get; set; }
        public IDbSet<MembroCategoria> MembroCategoria { get; set; }
        public IDbSet<MembroDemanda> MembroDemanda { get; set; }
        public IDbSet<Periodicidade> Periodicidade { get; set; }
        public IDbSet<FormaPagto> FormaPagto { get; set; }
        public IDbSet<FornecedorFormaPagto> FornecedorFormaPagto { get; set; }
        public IDbSet<WorkflowStatus> WorkflowStatus { get; set; }
        public IDbSet<Entrega> Entrega { get; set; }
        public IDbSet<StatusSistema> StatusSistema { get; set; }
        public IDbSet<Imagem> Imagem { get; set; }
        public IDbSet<TemplateEmail> TemplateEmail { get; set; }
        public IDbSet<TemplateSms> TemplateSms { get; set; }
        public IDbSet<Sms> Sms { get; set; }
        public IDbSet<FornecedorRegiao> FornecedorRegiao { get; set; }
        public IDbSet<Grupo> Grupo { get; set; }
        public IDbSet<PermissaoGrupo> PermissaoGrupo { get; set; }
        public IDbSet<UsuarioGrupo> UsuarioGrupo { get; set; }
        public IDbSet<Menu> Menu { get; set; }
        public IDbSet<Modulo> Modulo { get; set; }
        public IDbSet<RecuperaSenha> RecuperaSenha { get; set; }
        public IDbSet<Marca> Marca { get; set; }
        public IDbSet<Pedido> Pedido { get; set; }
        public IDbSet<ItemPedido> ItemPedido { get; set; }
        public IDbSet<Ranking> Ranking { get; set; }
        public IDbSet<Estoque> Estoque { get; set; }
        public IDbSet<Cotacao> Cotacao { get; set; }
        public IDbSet<CotacaoPedidos> CotacaoPedidos { get; set; }
        public IDbSet<HistStatusCotacao> HistStatusCotacao { get; set; }
        public IDbSet<HistStatusPedido> HistStatusPedido { get; set; }
        public IDbSet<ResultadoCotacao> ResultadoCotacao { get; set; }
        public IDbSet<SolicitacaoMembroFornecedor> SolicitacaoMembroFornecedor { get; set; }
        public IDbSet<Emails> Emails { get; set; }
        public IDbSet<AvaliacaoFornecedor> AvaliacaoFornecedor { get; set; }
        public IDbSet<Avisos> Avisos { get; set; }
        public IDbSet<PeriodoEntrega> PeriodoEntrega { get; set; }
        public IDbSet<HorasEntregaMembro> HorasEntregaMembro { get; set; }
        public IDbSet<ProdutoPromocional> ProdutoPromocional { get; set; }
        public IDbSet<HistoricoPromocao> HistoricoPromocao { get; set; }
        public IDbSet<PromocaoFormaPagto> PromocaoFormaPagto { get; set; }
        public IDbSet<FornecedorPrazoSemanal> FornecedorPrazoSemanal { get; set; }
        public IDbSet<IndisponibilidadeProduto> Disponibilidade { get; set; }
        public IDbSet<FornecedorProdutoQuantidade> FornecedorProdutoQuantidade { get; set; }
        public IDbSet<UsuarioCancelado> UsuarioCancelado { get; set; }
        public IDbSet<FornecedorFormaPagtoMembro> FornecedorFormaPagtoMembro { get; set; }



        #region Lista de Compras

        public IDbSet<ListaCompras> ListaCompras { get; set; }
        public IDbSet<ListaComprasItem> ListaComprasItem { get; set; }
        public IDbSet<ListaComprasRemoveForn> ListaComprasRemoveForn { get; set; }

        #endregion

        #region Tabela que tem todos os feriados dos proximos anos
        public IDbSet<CalendarioFeriado> CalendarioFeriado { get; set; }
        #endregion

        #region Tabela que grava os fornecedores que serão excluido da cotação do pedido

        public IDbSet<RemoveFornPedido> RemoveFornPedido { get; set; }

        #endregion

        #region Franquia

        public IDbSet<DataCotacaoFranquia> DataCotacaoFranquia { get; set; }

        #endregion

        #region Recebimento

        public IDbSet<Comissao> Comissao { get; set; }
        public IDbSet<Fatura> Fatura { get; set; }
        public IDbSet<Mensalidade> Mensalidade { get; set; }
        public IDbSet<MensalidadeDetalhe> MensalidadeDetalhes { get; set; }
        public IDbSet<ParametrosRecebimento> ParametrosRecebimento { get; set; }
        public IDbSet<PlanoMensalidade> PlanoMensalidade { get; set; }
        public IDbSet<CartaoBandeira> CartaoBandeira { get; set; }
        public IDbSet<CartaoCredito> CartaoCredito { get; set; }

        #endregion

        #endregion

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        //função que irá imprimir na tela (console) tudo o que 
        //aconteceu nas instruções de conexão e SQLs geradas
        private static void fs(string strRestult)
        {
            Debug.Print(strRestult);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Aqui vamos remover a pluralização padrão do Entity Framework que é em inglês
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            /*Desabilitamos o delete em cascata em relacionamentos 1:N evitando
             ter registros filhos     sem registros pai*/
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //Basicamente a mesma configuração, porém em relacionamenos N:N
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            /*Toda propriedade do tipo string na entidade POCO
             seja configurado como VARCHAR no SQL Server*/
            modelBuilder.Properties<string>().Configure(p => p.HasColumnType("varchar"));

            /*Toda propriedade do tipo string na entidade POCO seja configurado como VARCHAR (150) no banco de dados */
            modelBuilder.Properties<string>().Configure(p => p.HasMaxLength(100));

            /*Definimos usando reflexão que toda propriedade que contenha
           "Nome da classe" + Id como "CursoId" por exemplo, seja dada como
           chave primária, caso não tenha sido especificado*/
            modelBuilder.Properties().Where(p => p.Name == p.ReflectedType.Name + "Id").Configure(p => p.IsKey());

            modelBuilder.Configurations.Add(new UsuarioConfig());
            modelBuilder.Configurations.Add(new PerfilConfig());
            modelBuilder.Configurations.Add(new PessoaConfig());
            modelBuilder.Configurations.Add(new PessoaJuridicaConfig());
            modelBuilder.Configurations.Add(new PessoaFisicaConfig());
            modelBuilder.Configurations.Add(new MembroConfig());
            modelBuilder.Configurations.Add(new FornecedorConfig());
            modelBuilder.Configurations.Add(new EstadoConfig());
            modelBuilder.Configurations.Add(new CidadeConfig());
            modelBuilder.Configurations.Add(new BairroConfig());
            modelBuilder.Configurations.Add(new RegiaoConfig());
            modelBuilder.Configurations.Add(new LogradouroConfig());
            modelBuilder.Configurations.Add(new EnderecoConfig());
            modelBuilder.Configurations.Add(new CepEnderecoConfig());
            modelBuilder.Configurations.Add(new ErroConfig());
            modelBuilder.Configurations.Add(new TelefoneConfig());
            modelBuilder.Configurations.Add(new ProdutoConfig());
            modelBuilder.Configurations.Add(new CategoriaConfig());
            modelBuilder.Configurations.Add(new SubCategoriaConfig());
            modelBuilder.Configurations.Add(new UnidadeMedidaConfig());
            modelBuilder.Configurations.Add(new PeriodicidadeConfig());
            modelBuilder.Configurations.Add(new MembroCategoriaConfig());
            modelBuilder.Configurations.Add(new FormaPagtoConfig());
            modelBuilder.Configurations.Add(new FornecedorFormaPagtoConfig());
            modelBuilder.Configurations.Add(new WorkflowStatusConfig());
            modelBuilder.Configurations.Add(new EntregaConfig());
            modelBuilder.Configurations.Add(new StatusSistemaConfig());
            modelBuilder.Configurations.Add(new FabricanteConfig());
            modelBuilder.Configurations.Add(new ImagemConfig());
            modelBuilder.Configurations.Add(new FornecedorRegiaoConfig());
            modelBuilder.Configurations.Add(new TemplateEmailConfig());
            modelBuilder.Configurations.Add(new TemplateSmsConfig());
            modelBuilder.Configurations.Add(new SmsConfig());
            modelBuilder.Configurations.Add(new GrupoConfig());
            modelBuilder.Configurations.Add(new PermissaoGrupoConfig());
            modelBuilder.Configurations.Add(new UsuarioGrupoConfig());
            modelBuilder.Configurations.Add(new MenuConfig());
            modelBuilder.Configurations.Add(new ModuloConfig());
            modelBuilder.Configurations.Add(new RecuperaSenhaConfig());
            modelBuilder.Configurations.Add(new MarcaConfig());
            modelBuilder.Configurations.Add(new PedidoConfig());
            modelBuilder.Configurations.Add(new ItemPedidoConfig());
            modelBuilder.Configurations.Add(new RankingConfig());
            modelBuilder.Configurations.Add(new EstoqueConfig());
            modelBuilder.Configurations.Add(new CotacaoConfig());
            modelBuilder.Configurations.Add(new CotacaoPedidoConfig());
            modelBuilder.Configurations.Add(new HistStatusCotacaoConfig());
            modelBuilder.Configurations.Add(new HistStatusPedidoConfig());
            modelBuilder.Configurations.Add(new ResultadoCotacaoConfig());
            modelBuilder.Configurations.Add(new MembroDemandaConfig());
            modelBuilder.Configurations.Add(new SolicitacaoMembroFornecedorConfig());
            modelBuilder.Configurations.Add(new EmailsConfig());
            modelBuilder.Configurations.Add(new SegmentoConfig());
            modelBuilder.Configurations.Add(new SegmentoCategoriaConfig());
            modelBuilder.Configurations.Add(new AvaliacaoFornecedorConfig());
            modelBuilder.Configurations.Add(new AvisosConfig());
            modelBuilder.Configurations.Add(new HorasEntregaMembroConfig());
            modelBuilder.Configurations.Add(new PeriodoEntregaConfig());
            modelBuilder.Configurations.Add(new TermoUsoConfig());
            modelBuilder.Configurations.Add(new ProdutoPromocionalConfig());
            modelBuilder.Configurations.Add(new HistoricoPromocaoConfig());
            modelBuilder.Configurations.Add(new PromocaoFormaPagtoConfig());
            modelBuilder.Configurations.Add(new NotificacaoConfig());
            modelBuilder.Configurations.Add(new TipoAvisosConfig());
            modelBuilder.Configurations.Add(new UsuarioNotificacaoConfig());
            modelBuilder.Configurations.Add(new ParametroSistemaConfig());
            modelBuilder.Configurations.Add(new IndisponibilidadeProdutoConfig());
            modelBuilder.Configurations.Add(new FornecedorPrazoSemanalConfig());
            modelBuilder.Configurations.Add(new FranquiaConfig());
            modelBuilder.Configurations.Add(new FranquiaFornecedorConfig());
            modelBuilder.Configurations.Add(new FranquiaProdutoConfig());
            modelBuilder.Configurations.Add(new FornecedorProdutoConfig());
            modelBuilder.Configurations.Add(new TransportadoraConfig());
            modelBuilder.Configurations.Add(new FretePedidoFornecedorConfig());
            modelBuilder.Configurations.Add(new FornecedorProdutoQuantidadeConfig());
            modelBuilder.Configurations.Add(new ExecucaoRoboConfig());
            modelBuilder.Configurations.Add(new UsuarioCanceladoConfig());
            modelBuilder.Configurations.Add(new ListaComprasConfig());


            #region Tabela que tem todos os feriados dos proximos anos
            modelBuilder.Configurations.Add(new CalendarioFeriadoConfig());
            #endregion 

            #region Tabela que grava os fornecedores que serão excluido da cotação do pedido

            modelBuilder.Configurations.Add(new RemoveFornPedidoConfig());

            #endregion

            #region Recebimento

            modelBuilder.Configurations.Add(new ComissaoConfig());
            modelBuilder.Configurations.Add(new FaturaConfig());
            modelBuilder.Configurations.Add(new MensalidadeConfig());
            modelBuilder.Configurations.Add(new ParametrosRecebimentoConfig());
            modelBuilder.Configurations.Add(new PlanoMensalidadeConfig());
            modelBuilder.Configurations.Add(new CartaoBandeiraConfig());
            modelBuilder.Configurations.Add(new CartaoCreditoConfig());

            #endregion

            #region Franquia

            modelBuilder.Configurations.Add(new DataCotacaoFranquiaConfig());

            #endregion

        }
    }
}
