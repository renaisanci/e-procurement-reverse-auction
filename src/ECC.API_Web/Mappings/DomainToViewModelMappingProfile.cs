using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using AutoMapper;
using ECC.API_Web.Models;
using ECC.EntidadeEndereco;
using ECC.EntidadeFormaPagto;
using ECC.EntidadePessoa;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeStatus;
using ECC.EntidadeUsuario;
using ECC.EntidadeMenu;
using ECC.API_Web.InfraWeb;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadeEstoque;
using ECC.EntidadeEntrega;
using ECC.EntidadeArquivo;
using ECC.EntidadeProduto;
using ECC.EntidadeAvisos;
using ECC.EntidadeRecebimento;
using ECC.EntidadeRelatorio;
using ECC.Entidades.EntidadeComum;
using ECC.Entidades.EntidadePessoa;
using ECC.Servicos.Util;

namespace ECC.API_Web.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {

        protected override void Configure()
        {
            #region Mapeamento UnidadeMedida

            Mapper.CreateMap<UnidadeMedida, UnidadeMedidaViewModel>()

                     .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescUnidadeMedida, map => map.MapFrom(m => m.DescUnidadeMedida))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
            .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Periodicidade

            Mapper.CreateMap<Periodicidade, PeriodicidadeViewModel>()
                    .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                    .ForMember(vm => vm.DescPeriodicidade, map => map.MapFrom(m => m.DescPeriodicidade));

            #endregion

            #region Mapeamento Membro PF

            Mapper.CreateMap<Membro, MembroPFViewModel>()


                .ForMember(vm => vm.PessoaId, map => map.MapFrom(m => m.Pessoa.Id))
                //o Perfil é de acordo com o Id q esta no banco tabela perfil
                .ForMember(vm => vm.PerfilId, map => map.MapFrom(m => 2))
                .ForMember(vm => vm.Cpf, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Cpf))
                .ForMember(vm => vm.Nome, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Nome))
                .ForMember(vm => vm.Cro, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Cro))
                .ForMember(vm => vm.Sexo, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Sexo))
                .ForMember(vm => vm.Rg, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Rg))
                .ForMember(vm => vm.DtNascimento, map => map.MapFrom(m => m.Pessoa.PessoaFisica.DtNascimento))
                .ForMember(vm => vm.Email, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Email))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Pessoa.PessoaFisica.Ativo))
                .ForMember(vm => vm.Franquia, map => map.MapFrom(m => m.Franquia))
                .ForMember(vm => vm.Vip, map => map.MapFrom(m => m.Vip))
                .ForMember(vm => vm.DddTelComl, map => map.MapFrom(m => m.DddTel))
                .ForMember(vm => vm.TelefoneComl, map => map.MapFrom(m => m.Telefone))
                .ForMember(vm => vm.DddCel, map => map.MapFrom(m => m.DddCel))
                .ForMember(vm => vm.Celular, map => map.MapFrom(m => m.Celular))
                .ForMember(vm => vm.Contato, map => map.MapFrom(m => m.Contato))
                .ForMember(vm => vm.Cep, map => map.MapFrom(m => m.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao).Cep))
                .ForMember(vm => vm.TipoPessoa, map => map.MapFrom(m => m.Pessoa.TipoPessoa))
                .ForMember(vm => vm.Completo, map => map.MapFrom(m => m.Pessoa.Enderecos.Count == 0 ? "Nao" : m.MembroCategorias.Count == 0 ? "Nao" : "Sim"))
                .ForMember(vm => vm.Usuario, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault()));


            #endregion

            #region Mapeamento Membro PJ

            Mapper.CreateMap<Membro, MembroViewModel>()


                .ForMember(vm => vm.PessoaId, map => map.MapFrom(m => m.Pessoa.Id))
                //o Perfil é de acordo com o Id q esta no banco tabela perfil
                .ForMember(vm => vm.PerfilId, map => map.MapFrom(m => 2))
                .ForMember(vm => vm.Cnpj, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Cnpj))
                .ForMember(vm => vm.Comprador, map => map.MapFrom(m => m.Comprador))
                .ForMember(vm => vm.NomeFantasia, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.NomeFantasia))
                .ForMember(vm => vm.RazaoSocial, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.RazaoSocial))
                .ForMember(vm => vm.DtFundacao, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.DtFundacao))
                .ForMember(vm => vm.Email, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Email))
                .ForMember(vm => vm.InscEstadual, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.InscEstadual))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Ativo))
                .ForMember(vm => vm.DddTelComl, map => map.MapFrom(m => m.DddTel))
                .ForMember(vm => vm.TelefoneComl, map => map.MapFrom(m => m.Telefone))
                .ForMember(vm => vm.DddCel, map => map.MapFrom(m => m.DddCel))
                .ForMember(vm => vm.Celular, map => map.MapFrom(m => m.Celular))
                .ForMember(vm => vm.Contato, map => map.MapFrom(m => m.Contato))
                .ForMember(vm => vm.Cep, map => map.MapFrom(m => m.Pessoa.Enderecos.FirstOrDefault(x=>x.EnderecoPadrao).Cep))
                .ForMember(vm => vm.Franquia, map => map.MapFrom(m => m.Franquia))
                .ForMember(vm => vm.Vip, map => map.MapFrom(m => m.Vip))
                  .ForMember(vm => vm.TipoPessoa, map => map.MapFrom(m => m.Pessoa.TipoPessoa))
                .ForMember(vm => vm.Completo, map => map.MapFrom(m => m.Pessoa.Enderecos.Count == 0 ? "Nao" : m.MembroCategorias.Count == 0 ? "Nao" : "Sim"))
                .ForMember(vm => vm.Usuario, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault()));


            #endregion

            #region Mapeamento Logradouro
            Mapper.CreateMap<Logradouro, LogradouroViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                  .ForMember(vm => vm.DescLogradouro, map => map.MapFrom(m => m.DescLogradouro));
            #endregion

            #region Mapeamento Estado
            Mapper.CreateMap<Estado, EstadoViewModel>()
           .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
             .ForMember(vm => vm.DescEstado, map => map.MapFrom(m => m.DescEstado));

            #endregion

            #region Mapeamento Cidade
            Mapper.CreateMap<Cidade, CidadeViewModel>()
           .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
             .ForMember(vm => vm.DescCidade, map => map.MapFrom(m => m.DescCidade));

            #endregion

            #region Mapeamento Bairro
            Mapper.CreateMap<Bairro, BairroViewModel>()
           .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
             .ForMember(vm => vm.DescBairro, map => map.MapFrom(m => m.DescBairro));

            #endregion

            #region Mapeamento Cep Endereco

            Mapper.CreateMap<CepEndereco, EnderecoViewModel>()

                .ForMember(vm => vm.Id, map => map.MapFrom(m => 0))
                .ForMember(vm => vm.EstadoId, map => map.MapFrom(m => m.EstadoId))
                .ForMember(vm => vm.CidadeId, map => map.MapFrom(m => m.CidadeId))
                .ForMember(vm => vm.BairroId, map => map.MapFrom(m => m.BairroId))
                .ForMember(vm => vm.LogradouroId, map => map.MapFrom(m => m.LogradouroId))
                .ForMember(vm => vm.Cep, map => map.MapFrom(m => m.Cep))
                .ForMember(vm => vm.Complemento, map => map.MapFrom(m => m.Complemento))
                .ForMember(vm => vm.Bairro, map => map.MapFrom(m => m.Bairro.DescBairro))
                .ForMember(vm => vm.Cidade, map => map.MapFrom(m => m.Cidade.DescCidade))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => false))
                .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.DescLogradouro));

            #endregion

            #region Mapeamento Endereco

            Mapper.CreateMap<Endereco, EnderecoViewModel>()

                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.EstadoId, map => map.MapFrom(m => m.EstadoId))
                .ForMember(vm => vm.CidadeId, map => map.MapFrom(m => m.CidadeId))
                .ForMember(vm => vm.BairroId, map => map.MapFrom(m => m.BairroId))
                .ForMember(vm => vm.LogradouroId, map => map.MapFrom(m => m.LogradouroId))
                .ForMember(vm => vm.Cep, map => map.MapFrom(m => m.Cep))
                .ForMember(vm => vm.Complemento, map => map.MapFrom(m => m.Complemento))
                .ForMember(vm => vm.Bairro, map => map.MapFrom(m => m.Bairro.DescBairro))
                .ForMember(vm => vm.Cidade, map => map.MapFrom(m => m.Cidade.DescCidade))
                .ForMember(vm => vm.Estado, map => map.MapFrom(m => m.Estado.DescEstado))
                .ForMember(vm => vm.Logradouro, map => map.MapFrom(m => m.Logradouro.DescLogradouro))
                .ForMember(vm => vm.NumEndereco, map => map.MapFrom(m => m.Numero))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.EnderecoPadrao, map => map.MapFrom(m => m.EnderecoPadrao))
                .ForMember(vm => vm.DescHorarioEntrega, map => map.MapFrom(m => m.HorasEntregaMembro.Select(x => x.DescHorarioEntrega).FirstOrDefault()))
                .ForMember(vm => vm.PeriodoEntrega, map => map.MapFrom(m => m.HorasEntregaMembro.Select(x => x.Periodo)))
                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.DescEndereco));

            #endregion

            #region Mapeamento Telefones

            Mapper.CreateMap<Telefone, TelefoneViewModel>()

                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DddTelComl, map => map.MapFrom(m => m.DddTelComl))
                .ForMember(vm => vm.TelefoneComl, map => map.MapFrom(m => m.TelefoneComl))
                .ForMember(vm => vm.DddCel, map => map.MapFrom(m => m.DddCel))
                .ForMember(vm => vm.Celular, map => map.MapFrom(m => m.Celular))
                .ForMember(vm => vm.Contato, map => map.MapFrom(m => m.Contato))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo));

            #endregion

            #region Mapeamento Categoria
            Mapper.CreateMap<Categoria, CategoriaViewModel>()
           .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
            .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.DescCategoria))
            .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
            .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Membro Categoria

            Mapper.CreateMap<MembroCategoria, CategoriaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CategoriaId))
                .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.Categoria.DescCategoria));



            #endregion

            #region Mapeamento Membro Demanda

            Mapper.CreateMap<MembroDemanda, MembroDemandaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.MembroId, map => map.MapFrom(m => m.MembroId))
                .ForMember(vm => vm.CategoriaId, map => map.MapFrom(m => m.SubCategoria.CategoriaId))
                .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.SubCategoria.Categoria.DescCategoria))
                .ForMember(vm => vm.SubCategoriaId, map => map.MapFrom(m => m.SubCategoriaId))
                .ForMember(vm => vm.DescSubCategoria, map => map.MapFrom(m => m.SubCategoria.DescSubCategoria))
                .ForMember(vm => vm.PeriodicidadeId, map => map.MapFrom(m => m.PeriodicidadeId))
                .ForMember(vm => vm.DescPeriodicidade, map => map.MapFrom(m => m.Periodicidade.Id))
                .ForMember(vm => vm.DescUnidadeMedida, map => map.MapFrom(m => m.UnidadeMedida.DescUnidadeMedida))
                .ForMember(vm => vm.Quantidade, map => map.MapFrom(m => m.Quantidade))
                .ForMember(vm => vm.Observacao, map => map.MapFrom(m => m.Observacao))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Fornecedor

            Mapper.CreateMap<Fornecedor, FornecedorViewModel>()


                .ForMember(vm => vm.PessoaId, map => map.MapFrom(m => m.Pessoa.Id))
                 .ForMember(vm => vm.PerfilId, map => map.MapFrom(m => 3))
                .ForMember(vm => vm.Cnpj, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Cnpj))
                .ForMember(vm => vm.Responsavel, map => map.MapFrom(m => m.Responsavel))
                .ForMember(vm => vm.NomeFantasia, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.NomeFantasia))
                .ForMember(vm => vm.RazaoSocial, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.RazaoSocial))
                .ForMember(vm => vm.DtFundacao, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.DtFundacao))
                .ForMember(vm => vm.Email, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Email))
                .ForMember(vm => vm.PrimeiraAvista, map => map.MapFrom(m => m.PrimeiraAvista))
                .ForMember(vm => vm.Observacao, map => map.MapFrom(m => m.Observacao))
                .ForMember(vm => vm.ObservacaoEntrega, map => map.MapFrom(m => m.ObservacaoEntrega))

                .ForMember(vm => vm.Completo,
                    map => map.MapFrom(f => f.Pessoa.Enderecos.Count == 0 ? "Não" :
                                        f.FornecedorCategorias.Count == 0 ? "Não" :
                                        f.FornecedorRegiao.Count == 0 && f.FornecedorRegiaoSemanal.Count == 0 ? "Não" :
                                        f.FornecedorFormaPagtos.Count == 0 ? "Não" :
                                        "Sim"))

                .ForMember(vm => vm.VlPedidoMin, map => map.MapFrom(m => string.Format("{0:C2}", m.VlPedidoMin).Replace("R$", "").Trim()))

                .ForMember(vm => vm.InscEstadual, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.InscEstadual))
                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))

                .ForMember(vm => vm.DddTelComl, map => map.MapFrom(m => m.DddTel))
                .ForMember(vm => vm.TelefoneComl, map => map.MapFrom(m => m.Telefone))
                .ForMember(vm => vm.DddCel, map => map.MapFrom(m => m.DddCel))
                .ForMember(vm => vm.Celular, map => map.MapFrom(m => m.Celular))
                .ForMember(vm => vm.Contato, map => map.MapFrom(m => m.Contato))

                .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.Pessoa.Enderecos.FirstOrDefault()))

                .ForMember(vm => vm.FormasPagamento, map => map.MapFrom(m => m.FornecedorFormaPagtos.Select(x => x.FormaPagto).ToList()))

                .ForMember(vm => vm.Usuario, map => map.MapFrom(u => u.Pessoa.Usuarios.FirstOrDefault()))

                //.ForMember(vm => vm.PrazoEntrega, map => map.MapFrom(u => u.FornecedorRegiao.Select(x => x.Prazo).FirstOrDefault()))

                .ForMember(vm => vm.FornecedorPrazoSemanal, map => map.MapFrom(u => u.FornecedorRegiaoSemanal))

                .ForMember(vm => vm.FornecedorRegiao, map => map.MapFrom(u => u.FornecedorRegiao))

                .ForMember(vm => vm.FormaPagtos, map => map.MapFrom(m => m.FornecedorFormaPagtos.ToList()));


            #endregion

            #region Mapeamento Forma de Pagamento

            Mapper.CreateMap<FormaPagto, FormaPagtoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.Avista, map => map.MapFrom(m => m.Avista))
                .ForMember(vm => vm.QtdParcelas, map => map.MapFrom(m => m.QtdParcelas))
                .ForMember(vm => vm.DescFormaPagto, map => map.MapFrom(m => m.DescFormaPagto));

            #endregion

            #region Mapeamento Fornecedor Forma de Pagamento

            Mapper.CreateMap<FornecedorFormaPagto, FornecedorFormaPagtoViewModel>()
                .ForMember(vm => vm.FormaPagtoId, map => map.MapFrom(m => m.FormaPagtoId))
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                .ForMember(vm => vm.VlFormaPagto, map => map.MapFrom(m => string.Format("{0:C2}", m.ValorPedido).Replace("R$", "").Trim()))
                .ForMember(vm => vm.ValorMinParcela, map => map.MapFrom(m => string.Format("{0:C2}", m.ValorMinParcela).Replace("R$", "").Trim()))
                .ForMember(vm => vm.ValorMinParcelaPedido, map => map.MapFrom(m => string.Format("{0:C2}", m.ValorMinParcelaPedido).Replace("R$", "").Trim()))
                .ForMember(vm => vm.Desconto, map => map.MapFrom(m => m.Desconto));

            #endregion

            #region Mapeamento Produto

            Mapper.CreateMap<Produto, ProdutoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescProduto, map => map.MapFrom(m => m.DescProduto))
                .ForMember(vm => vm.Fabricante, map => map.MapFrom(m => m.Fabricante.DescFabricante))
                  .ForMember(vm => vm.Marca, map => map.MapFrom(m => m.Marca.DescMarca))
                .ForMember(vm => vm.UnidadeMedida, map => map.MapFrom(m => m.UnidadeMedida.DescUnidadeMedida))
                .ForMember(vm => vm.SubCategoriaId, map => map.MapFrom(m => m.SubCategoriaId))
                .ForMember(vm => vm.UnidadeMedidaId, map => map.MapFrom(m => m.UnidadeMedidaId))
                .ForMember(vm => vm.FabricanteId, map => map.MapFrom(m => m.FabricanteId))

                .ForMember(vm => vm.Ranking, map => map.MapFrom(m => m.Rankings.Where(x => x.Nota > 0).Any() ? m.Rankings.Where(x => x.Nota > 0).Sum(x => x.Nota.TryParseInt()) / m.Rankings.Where(x => x.Nota > 0).Count().TryParseInt() : 0))

                   .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
                     .ForMember(vm => vm.MarcaId, map => map.MapFrom(m => m.MarcaId))
                .ForMember(vm => vm.PrecoMedio, map => map.MapFrom(m => string.Format("{0:C2}", m.PrecoMedio).Replace("R$", "").Trim()))

                 .ForMember(vm => vm.Preco, map => map.MapFrom(m => m.PrecoMedio))
                .ForMember(vm => vm.Especificacao, map => map.MapFrom(m => m.Especificacao))
                .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.SubCategoria.Categoria.DescCategoria))
                .ForMember(vm => vm.DescSubCategoria, map => map.MapFrom(m => m.SubCategoria.DescSubCategoria))

                .ForMember(vm => vm.Imagem, map => map.MapFrom(m => m.Imagens.Count > 0 ? ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] + m.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   m.SubCategoria.Id.ToString() + @"/" + m.Imagens.FirstOrDefault().CaminhoImagem : "../../../Content/images/fotoIndisponivel.png"))

                .ForMember(vm => vm.ImagemGrande, map => map.MapFrom(m => m.Imagens.Count > 0 ? ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] + m.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   m.SubCategoria.Id.ToString() + @"/" + m.Imagens.FirstOrDefault().CaminhoImagemGrande : "../../../Content/images/fotoIndisponivel.png"))


                .ForMember(vm => vm.ProdutoPromocionalId, map => map.MapFrom(m => m.ProdutoPromocionalId))


                .ForMember(vm => vm.CategoriaId, map => map.MapFrom(m => m.SubCategoria.Categoria.Id));


            #endregion

            #region Mapeamento Historico Status Pedido

            Mapper.CreateMap<HistStatusPedido, HistStatusPedidoViewModel>()
                .ForMember(vm => vm.UsuarioId, map => map.MapFrom(m => m.UsuarioCriacaoId))
                .ForMember(vm => vm.PedidoId, map => map.MapFrom(m => m.PedidoId))
                .ForMember(vm => vm.DescMotivoCancelamento, map => map.MapFrom(m => m.DescMotivoCancelamento));

            #endregion

            #region Mapeamento Produto Promocional

            Mapper.CreateMap<Produto, ProdutoPromocionalViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescProduto, map => map.MapFrom(m => m.DescProduto))
                .ForMember(vm => vm.Fabricante, map => map.MapFrom(m => m.Fabricante.DescFabricante))
                .ForMember(vm => vm.Marca, map => map.MapFrom(m => m.Marca.DescMarca))
                .ForMember(vm => vm.UnidadeMedida, map => map.MapFrom(m => m.UnidadeMedida.DescUnidadeMedida))
                .ForMember(vm => vm.SubCategoriaId, map => map.MapFrom(m => m.SubCategoriaId))
                .ForMember(vm => vm.UnidadeMedidaId, map => map.MapFrom(m => m.UnidadeMedidaId))
                .ForMember(vm => vm.FabricanteId, map => map.MapFrom(m => m.FabricanteId))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))

                .ForMember(vm => vm.MarcaId, map => map.MapFrom(m => m.MarcaId))

                .ForMember(vm => vm.Ranking, map => map.MapFrom(m => m.Rankings.Any() ? m.Rankings.Sum(x => x.Nota.TryParseInt()) / m.Rankings.Count.TryParseInt() : 0))

                .ForMember(vm => vm.PrecoMedio, map => map.MapFrom(m => string.Format("{0:C2}", m.PrecoMedio).Replace("R$", "").Trim()))

               .ForMember(vm => vm.Especificacao, map => map.MapFrom(m => m.Especificacao))

               .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.SubCategoria.Categoria.DescCategoria))

               .ForMember(vm => vm.DescSubCategoria, map => map.MapFrom(m => m.SubCategoria.DescSubCategoria))

                .ForMember(vm => vm.Imagem, map => map.MapFrom(m => m.Imagens.Count > 0 ? ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] + m.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   m.SubCategoria.Id.ToString() + @"/" + m.Imagens.FirstOrDefault().CaminhoImagem : "../../../Content/images/fotoIndisponivel.png"))

                .ForMember(vm => vm.ImagemGrande, map => map.MapFrom(m => m.Imagens.Count > 0 ? ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] + m.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   m.SubCategoria.Id.ToString() + @"/" + m.Imagens.FirstOrDefault().CaminhoImagemGrande : "../../../Content/images/fotoIndisponivel.png"))

                .ForMember(vm => vm.CategoriaId, map => map.MapFrom(m => m.SubCategoria.Categoria.Id))

                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.ProdutoPromocional.FornecedorId))

                .ForMember(vm => vm.Fornecedor, map => map.MapFrom(m => m.ProdutoPromocional.Fornecedor))

                .ForMember(vm => vm.QtdProdutos, map => map.MapFrom(m => m.ProdutoPromocional.QuantidadeProduto))

                .ForMember(vm => vm.QtdMinVenda, map => map.MapFrom(m => m.ProdutoPromocional.QtdMinVenda))

                .ForMember(vm => vm.MotivoPromocao, map => map.MapFrom(m => m.ProdutoPromocional.MotivoPromocao))

                .ForMember(vm => vm.ValidadeProd, map => map.MapFrom(m => m.ProdutoPromocional.ValidadeProduto))

                .ForMember(vm => vm.InicioPromocao, map => map.MapFrom(m => m.ProdutoPromocional.InicioPromocao))

                .ForMember(vm => vm.Aprovado, map => map.MapFrom(m => m.ProdutoPromocional.Aprovado))

                .ForMember(vm => vm.FimPromocao, map => map.MapFrom(m => m.ProdutoPromocional.FimPromocao))

                .ForMember(vm => vm.DescMotivoCancelamento, map => map.MapFrom(m => m.ProdutoPromocional.DescMotivoCancelamento))

                .ForMember(vm => vm.PromoAtivo, map => map.MapFrom(m => m.ProdutoPromocional.Ativo))

                .ForMember(vm => vm.PromocaoFormaPagto, map => map.MapFrom(m => m.ProdutoPromocional.PromocaoFormaPagtos.Where(p => p.Ativo).ToList()))

                .ForMember(vm => vm.PrecoPromocao, map => map.MapFrom(m => string.Format("{0:C2}", m.ProdutoPromocional.PrecoPromocao).Replace("R$", "").Trim()))

                .ForMember(vm => vm.FreteGratis, map => map.MapFrom(m => m.ProdutoPromocional.FreteGratis))

                .ForMember(vm => vm.ObsFrete, map => map.MapFrom(m => m.ProdutoPromocional.ObsFrete))

                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.ProdutoPromocional.FimPromocao >= DateTime.Now &&
                                                                    m.ProdutoPromocional.Ativo &&
                                                                    m.ProdutoPromocional.Aprovado));


            #endregion

            #region Promoção Forma de Pagamento

            Mapper.CreateMap<PromocaoFormaPagto, PromocaoFormaPagtoViewModel>()
              .ForMember(vm => vm.ProdutoPromocionalId, map => map.MapFrom(m => m.ProdutoPromocionalId))
              .ForMember(vm => vm.FormaPagtoId, map => map.MapFrom(m => m.FormaPagtoId))
              .ForMember(vm => vm.DescFormaPagamento, map => map.MapFrom(m => m.FormaPagto.DescFormaPagto));

            #endregion

            #region Mapeamento SubCategoria

            Mapper.CreateMap<SubCategoria, SubCategoriaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescSubCategoria, map => map.MapFrom(m => m.DescSubCategoria))
                .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.Categoria.DescCategoria))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.CategoriaId, map => map.MapFrom(m => m.CategoriaId))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Membro Categoria SubCategoria Menu Produto

            Mapper.CreateMap<MembroCategoria, CategoriaSubCatMenuViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CategoriaId))
                .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.Categoria.DescCategoria))
                  .ForMember(vm => vm.PrimeiraLetra, map => map.MapFrom(m => m.Categoria.DescCategoria.Substring(0, 1)))
                .ForMember(vm => vm.MenuSubCategorias, map => map.MapFrom(m =>
                Mapper.Map<IEnumerable<SubCategoria>, IEnumerable<SubCategoriaViewModel>>(m.Categoria.SubCategorias.Where(x => x.Ativo && x.Produtos.Any(y => y.Ativo && y.ProdutoPromocionalId == null) && x.Produtos.Count > 0))));


            #endregion

            #region Mapeamento Workflow Status Sistema

            Mapper.CreateMap<WorkflowStatus, workflowStatusViewModel>()
                .ForMember(vm => vm.DescWorkslowStatus, map => map.MapFrom(m => m.DescWorkslowStatus))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));
            #endregion

            #region Mapeamento Status Sistema

            Mapper.CreateMap<StatusSistema, StatusSistemaViewModel>()
                .ForMember(vm => vm.DescStatus, map => map.MapFrom(m => m.DescStatus))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.WorkflowStatusId, map => map.MapFrom(m => m.WorkflowStatusId))
                .ForMember(vm => vm.WorkflowStatusSistema, map => map.MapFrom(m => m.WorkflowStatus.DescWorkslowStatus));


            #endregion

            #region Mapeamento Fornecedores Membro

            Mapper.CreateMap<MembroFornecedor, MembroFornecedorViewModel>()
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                .ForMember(vm => vm.MembroId, map => map.MapFrom(m => m.MembroId));

            #endregion

            #region Mapeamento Fornecedor Categoria

            Mapper.CreateMap<FornecedorCategoria, CategoriaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CategoriaId))
                .ForMember(vm => vm.DescCategoria, map => map.MapFrom(m => m.Categoria.DescCategoria));

            #endregion

            #region Mapeamento Fabricante

            Mapper.CreateMap<Fabricante, FabricanteViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescFabricante, map => map.MapFrom(m => m.DescFabricante))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Usuario

            Mapper.CreateMap<Usuario, UsuarioViewModel>()
             .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
             .ForMember(vm => vm.ConfirmSenha, map => map.MapFrom(m => string.Empty))
             .ForMember(vm => vm.Senha, map => map.MapFrom(m => string.Empty))
             .ForMember(vm => vm.TelefoneUsuario, map => map.MapFrom(m => m.Telefones.FirstOrDefault()))
             .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
             .ForMember(vm => vm.DescFlgMaster, map => map.MapFrom(m => m.FlgMaster ? "Sim" : "Não"));

            #endregion

            #region Mapeamento Imagem

            Mapper.CreateMap<Imagem, ImagemViewModel>()
                .ForMember(vm => vm.ImagemId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.IdProduto, map => map.MapFrom(m => m.ProdutoId))

                .ForMember(vm => vm.CaminhoImagemGrande,
                    map =>
                        map.MapFrom(m => string.IsNullOrEmpty(m.CaminhoImagemGrande) == true ? "../../Content/images/unknown.jpg" : m.CaminhoImagemGrande))


                .ForMember(vm => vm.CaminhoImagem,
                    map =>
                        map.MapFrom(m => string.IsNullOrEmpty(m.CaminhoImagem) == true ? "../../Content/images/unknown.jpg" : m.CaminhoImagem));




            #endregion

            #region Mapeamento Fornecedor Regiao

            Mapper.CreateMap<FornecedorRegiao, FornecedorRegiaoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                .ForMember(vm => vm.CidadeId, map => map.MapFrom(m => m.CidadeId))
                .ForMember(vm => vm.Prazo, map => map.MapFrom(m => m.Prazo))
                .ForMember(vm => vm.DescCidade, map => map.MapFrom(m => m.Cidade.DescCidade));

            #endregion

            #region Mapeamento Regiao

            Mapper.CreateMap<Regiao, RegiaoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescRegiao, map => map.MapFrom(m => m.DescRegiao));
            #endregion

            #region Mapeamento Menu

            Mapper.CreateMap<Menu, MenuViewModel>()

                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.DescModulo,
                    map => map.MapFrom(m => m.Modulo.DescModulo))
                    ;

            #endregion

            #region Mapeamento Marca

            Mapper.CreateMap<Marca, MarcaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescMarca, map => map.MapFrom(m => m.DescMarca))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Recupera Senha

            Mapper.CreateMap<RecuperaSenha, RecuperaSenhaViewModel>()
                .ForMember(vm => vm.UsuarioNome, map => map.MapFrom(m => m.Usuario.UsuarioNome))
                .ForMember(vm => vm.Usuarioemail, map => map.MapFrom(m => m.Usuario.UsuarioEmail));

            #endregion

            #region Mapeamento Usuario Grupo

            Mapper.CreateMap<Grupo, GrupoViewModel>()
                .ForMember(vm => vm.DescGrupo, map => map.MapFrom(m => m.DescGrupo))
                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Item Pedido

            Mapper.CreateMap<ItemPedido, ItemPedidoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.sku, map => map.MapFrom(m => m.Produto.Id))
                .ForMember(vm => vm.PrecoMedioUnit, map => map.MapFrom(m => m.PrecoMedioUnit))
                .ForMember(vm => vm.PrecoNegociadoUnit, map => map.MapFrom(m => m.PrecoNegociadoUnit))
                .ForMember(vm => vm.name, map => map.MapFrom(m => m.Produto.DescProduto))
                .ForMember(vm => vm.quantity, map => map.MapFrom(m => m.Quantidade))
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                .ForMember(vm => vm.PedidoId, map => map.MapFrom(m => m.PedidoId))
                .ForMember(vm => vm.FormaPagtoId, map => map.MapFrom(m => m.FormaPagtoId))
                .ForMember(vm => vm.Observacao, map => map.MapFrom(m => m.Observacao))
                .ForMember(vm => vm.EntregaId, map => map.MapFrom(m => m.EntregaId))
                .ForMember(vm => vm.Desconto, map => map.MapFrom(m => m.Desconto))
                .ForMember(vm => vm.TaxaEntrega, map => map.MapFrom(m => m.TaxaEntrega))
                .ForMember(vm => vm.AprovacaoMembro, map => map.MapFrom(m => m.AprovacaoMembro))
                .ForMember(vm => vm.AprovacaoFornecedor, map => map.MapFrom(m => m.AprovacaoFornecedor))
                .ForMember(vm => vm.SubTotal, map => map.MapFrom(m => m.Quantidade * m.PrecoNegociadoUnit));

            #endregion



            #region Mapeamento Pedidos PF

            Mapper.CreateMap<Pedido, PedidoPFViewModel>()
                .ForMember(vm => vm.PedidoId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DtPedido, map => map.MapFrom(m => m.DtPedido))
                .ForMember(vm => vm.StatusId, map => map.MapFrom(m => m.StatusSistemaId))
                .ForMember(vm => vm.OrdemStatus, map => map.MapFrom(m => m.StatusSistema.Ordem))
                .ForMember(vm => vm.QtdItem, map => map.MapFrom(m => m.ItemPedidos.Where(x => x.AprovacaoMembro && x.Ativo).Sum(x => x.Quantidade)))
                .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.Endereco))
                .ForMember(vm => vm.Membro, map => map.MapFrom(m => m.Membro))
                .ForMember(vm => vm.FlgCotado, map => map.MapFrom(m => m.FlgCotado))
                .ForMember(vm => vm.DtCotacao, map => map.MapFrom(m => m.DtCotacao))
                .ForMember(vm => vm.PedidoPromocional, map => map.MapFrom(m => m.ItemPedidos.FirstOrDefault().Produto.ProdutoPromocionalId == null ? false : true))
                .ForMember(vm => vm.Itens,
                    map =>
                        map.MapFrom(
                            m => (Mapper.Map<IEnumerable<ItemPedido>, IEnumerable<ItemPedidoViewModel>>(m.ItemPedidos))));

            #endregion


            #region Mapeamento Pedidos

            Mapper.CreateMap<Pedido, PedidoViewModel>()
                .ForMember(vm => vm.PedidoId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DtPedido, map => map.MapFrom(m => m.DtPedido))
                .ForMember(vm => vm.StatusId, map => map.MapFrom(m => m.StatusSistemaId))
                .ForMember(vm => vm.OrdemStatus, map => map.MapFrom(m => m.StatusSistema.Ordem))
                .ForMember(vm => vm.QtdItem, map => map.MapFrom(m => m.ItemPedidos.Where(x => x.AprovacaoMembro && x.Ativo).Sum(x => x.Quantidade)))
                .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.Endereco))
                .ForMember(vm => vm.Membro, map => map.MapFrom(m => m.Membro))
                .ForMember(vm => vm.FlgCotado, map => map.MapFrom(m => m.FlgCotado))
                .ForMember(vm => vm.DtCotacao, map => map.MapFrom(m => m.DtCotacao))
                .ForMember(vm => vm.PedidoPromocional, map => map.MapFrom(m => m.ItemPedidos.FirstOrDefault().Produto.ProdutoPromocionalId == null ? false : true))
                .ForMember(vm => vm.Itens,
                    map =>
                        map.MapFrom(
                            m => (Mapper.Map<IEnumerable<ItemPedido>, IEnumerable<ItemPedidoViewModel>>(m.ItemPedidos))));

            #endregion

            #region Mapeamento Cotação Itens Pedido

            Mapper.CreateMap<CotacaoPedidos, CotacaoItensPedidoViewModel>()
                .ForMember(vm => vm.CotacaoId, map => map.MapFrom(m => m.CotacaoId));

            #endregion

            #region Mapeamento Cotacoes

            Mapper.CreateMap<CotacaoPedidos, CotacaoViewModel>()

                .ForMember(vm => vm.CotacaoId, map => map.MapFrom(m => m.Cotacao.Id))
                .ForMember(vm => vm.DtCotacao, map => map.MapFrom(m => m.Cotacao.DtCriacao))
                 .ForMember(vm => vm.DtFechamento, map => map.MapFrom(m => m.Cotacao.DtFechamento))
                .ForMember(vm => vm.StatusId, map => map.MapFrom(m => m.Cotacao.StatusSistemaId))
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Cotacao.StatusSistema.DescStatus))
                .ForMember(vm => vm.OrdemStatus, map => map.MapFrom(m => m.Cotacao.StatusSistema.Ordem))
                .ForMember(vm => vm.QtdItem, map => map.MapFrom(m => m.Pedido.ItemPedidos.Sum(x => x.Quantidade)));

            #endregion

            #region Mapeamento Modulos

            Mapper.CreateMap<Modulo, ModuloViewModel>();


            #endregion

            #region Mapeamento Estoque

            Mapper.CreateMap<Estoque, EstoqueViewModel>()

                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"))
                .ForMember(vm => vm.DescProduto,
                    map => map.MapFrom(m => m.Produto.DescProduto))
                .ForMember(vm => vm.DescMembro,
                    map => map.MapFrom(m => m.Membro.Pessoa.PessoaJuridica.NomeFantasia))
                .ForMember(vm => vm.DescEndereco,
                    map => map.MapFrom(m => m.Endereco.DescEndereco))
                    ;

            #endregion

            #region Mapeamento Membro Fornecedor

            Mapper.CreateMap<MembroFornecedor, MembroFornecedorViewModel>()

             .ForMember(vm => vm.NomeFantasia, map => map.MapFrom(m => m.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia))
             .ForMember(vm => vm.RazaoSocial, map => map.MapFrom(m => m.Fornecedor.Pessoa.PessoaJuridica.RazaoSocial))
             .ForMember(vm => vm.Cnpj, map => map.MapFrom(m => m.Fornecedor.Pessoa.PessoaJuridica.Cnpj))
             .ForMember(vm => vm.DataCriado, map => map.MapFrom(m => m.DtCriacao))
             .ForMember(vm => vm.FlgSomenteAvista, map => map.MapFrom(m => m.Fornecedor.FornecedorFormaPagtos.Where(x => x.FormaPagto.Avista == false).Count() > 0 ? false : true))
             .ForMember(vm => vm.DataCriado, map => map.MapFrom(m => m.DtCriacao))
             .ForMember(vm => vm.FormaPagtoString, map => map.MapFrom(m => string.Join(" - ", m.Fornecedor.FornecedorFormaPagtos.Select(x => x.FormaPagto.DescFormaPagto + " " + Math.Round(x.Desconto, 0) + "%")))); ;


            #endregion

            #region Mapeamento Membro Solicita Novo Fornecedor

            Mapper.CreateMap<Fornecedor, MembroSolicitaFornecedorViewModel>()

                .ForMember(vm => vm.DescAtivo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.NomeFornecedor, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.NomeFantasia))
                .ForMember(vm => vm.NomeRazaoSocialFornecedor, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.RazaoSocial))
                .ForMember(vm => vm.CnpjFornecedor, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Cnpj))
                .ForMember(vm => vm.PrazoEntegaFornecedor, map => map.MapFrom(m => m.FornecedorRegiao.Select(f => f.Prazo).FirstOrDefault()))

                .ForMember(vm => vm.FormasPagamento, map => map.MapFrom(m => m.FornecedorFormaPagtos.Select(x => new FormaPagtoViewModel
                {
                    Id = x.Id,
                    DescFormaPagto = x.FormaPagto.DescFormaPagto,
                    Avista = x.FormaPagto.Avista,
                    Desconto = x.Desconto,
                    QtdParcelas = x.FormaPagto.QtdParcelas
                })))


                .ForMember(vm => vm.FornecedorPrazoSemanal, map => map.MapFrom(m => m.FornecedorRegiaoSemanal))
                .ForMember(vm => vm.ObservacaoFormPagto, map => map.MapFrom(m => m.Observacao))
                .ForMember(vm => vm.ObservacaoEntrega, map => map.MapFrom(m => m.ObservacaoEntrega))

                .ForMember(vm => vm.VlPedMinimo, map => map.MapFrom(m => m.FornecedorRegiao.Count > 0 ?
                m.FornecedorRegiao.Select(x => x.VlPedMinRegiao).FirstOrDefault() :
                m.FornecedorRegiaoSemanal.Select(x => x.VlPedMinRegiao).FirstOrDefault()))

                //Nota dada ao fornecedor referente as entregas realizadas de cada pedido
                .ForMember(vm => vm.MediaAvaliacaoPedido, map => map.MapFrom(m =>
                m.AvaliacaoFornecedor.Sum(x => x.NotaQualidadeProdutos + x.NotaTempoEntrega + x.NotaAtendimento)))
                .ForMember(vm => vm.QtdNotas, map => map.MapFrom(m => m.AvaliacaoFornecedor.Count * 3))
                .ForMember(vm => vm.FormaPagtos, map => map.MapFrom(m => m.FornecedorFormaPagtos.Select(x => x.FormaPagtoId).ToArray()));

            #endregion

            #region Mapeamento Solicitacao Membro Fornecedor

            Mapper.CreateMap<SolicitacaoMembroFornecedor, SolicitacaoMembroFornecedorViewModel>()

                .ForMember(vm => vm.MembroId, map => map.MapFrom(m => m.MembroId))

                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))

                .ForMember(vm => vm.NomeFantasia, map => map.MapFrom(m => m.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia))

                .ForMember(vm => vm.RazaoSocial, map => map.MapFrom(m => m.Fornecedor.Pessoa.PessoaJuridica.RazaoSocial))

                .ForMember(vm => vm.Cnpj, map => map.MapFrom(m => m.Fornecedor.Pessoa.PessoaJuridica.Cnpj))

               .ForMember(vm => vm.DataCriado, map => map.MapFrom(m => m.DtCriacao))

                .ForMember(vm => vm.DataAlteracao, map => map.MapFrom(m => m.DtAlteracao))

               .ForMember(vm => vm.ObservacaoFormaPagto, map => map.MapFrom(m => m.Fornecedor.Observacao))

               .ForMember(vm => vm.ObservacaoEntrega, map => map.MapFrom(m => m.Fornecedor.ObservacaoEntrega))

                .ForMember(vm => vm.Observacao, map => map.MapFrom(m => m.MotivoRecusa))

                .ForMember(vm => vm.Descricao, map => map.MapFrom(m => m.Fornecedor.Descricao))

                 .ForMember(vm => vm.PalavrasChaves, map => map.MapFrom(m => m.Fornecedor.PalavrasChaves))

                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo));

            #endregion

            #region Mapeamento Segmento

            Mapper.CreateMap<Segmento, SegmentoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescSegmento, map => map.MapFrom(m => m.DescSegmento))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo))
                .ForMember(vm => vm.DescAtivo,
                    map => map.MapFrom(m => m.Ativo ? "Ativo" : "Inativo"));

            #endregion

            #region Mapeamento Segmento Categoria

            Mapper.CreateMap<SegmentoCategoria, SegmentoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.SegmentoId))
                .ForMember(vm => vm.DescSegmento, map => map.MapFrom(m => m.Segmento.DescSegmento));

            #endregion

            #region Mapeamento Entrega

            Mapper.CreateMap<Entrega, EntregaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescEntrega, map => map.MapFrom(m => m.DescEntrega))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo));

            #endregion

            #region Mapeamento Avaliacao Fornecedor

            Mapper.CreateMap<AvaliacaoFornecedor, AvaliacaoFornecedorViewModel>()
                .ForMember(vm => vm.QualidadeProdutos, map => map.MapFrom(m => m.NotaQualidadeProdutos))
                .ForMember(vm => vm.TempoEntrega, map => map.MapFrom(m => m.NotaTempoEntrega))
                .ForMember(vm => vm.Atendimento, map => map.MapFrom(m => m.NotaAtendimento));

            #endregion

            #region Periodo Entrega Membro

            Mapper.CreateMap<PeriodoEntrega, PeriodoEntregaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DescPeriodoEntrega, map => map.MapFrom(m => m.DescPeriodoEntrega));


            Mapper.CreateMap<HorasEntregaMembro, HorasEntregaMembroViewModel>()
                 .ForMember(vm => vm.PeriodoId, map => map.MapFrom(m => m.PeriodoId))
                  .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.Endereco))
                 .ForMember(vm => vm.Periodo, map => map.MapFrom(m => m.Periodo));


            #endregion

            #region Mapeamento Termo de Uso

            Mapper.CreateMap<TermoUso, TermoUsoViewModel>()
                .ForMember(vm => vm.Documento, map => map.MapFrom(m => m.Documento));

            #endregion

            #region Mapeamento Historico Lance Por Fornecedor

            Mapper.CreateMap<FornecedorLance, FornecedorLanceViewModel>()
                .ForMember(vm => vm.FornecedorLance, map => map.MapFrom(m => m.ItemFornecedorLance))
                .ForMember(vm => vm.valorLance, map => map.MapFrom(m => m.valorLance));


            Mapper.CreateMap<ItemPedidosFornecedores, ItemPedidosFornecedoresViewModel>()
                .ForMember(vm => vm.ItemPedidos, map => map.MapFrom(m => m.ItemPedidos))
                .ForMember(vm => vm.FornecedorLance,
                    map =>
                        map.MapFrom(
                            m => (Mapper.Map<IEnumerable<FornecedorLance>, IEnumerable<FornecedorLanceViewModel>>(m.ListFornecedorLance))));


            Mapper.CreateMap<PedidoFornecedoresPorITem, PedidoFornecedoresPorItemViewModel>()
                .ForMember(vm => vm.PedidoId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DtPedido, map => map.MapFrom(m => m.DtPedido))
                .ForMember(vm => vm.StatusId, map => map.MapFrom(m => m.StatusSistemaId))
                .ForMember(vm => vm.Status, map => map.MapFrom(m =>
                    m.StatusSistemaId < 23 ? "Pedido Gerado" :
                    m.StatusSistemaId == 23 ? "Aguardando sua Aprovação" :
                    m.StatusSistemaId > 23 && m.StatusSistemaId < 30 ? "Aguardando Entrega" :
                    m.StatusSistemaId == 30 ? "Finalizado" :
                    "Fora de Status"
                    ))
                .ForMember(vm => vm.ItemPedidosFornecedor,
                    map =>
                        map.MapFrom(
                            m => (Mapper.Map<IEnumerable<ItemPedidosFornecedores>, IEnumerable<ItemPedidosFornecedoresViewModel>>(m.ItemPedidosFornecedor))));

            #endregion

            #region Indisponibilidade Produtos Fornecedor

            Mapper.CreateMap<IndisponibilidadeProduto, IndisponibilidadeProdutoViewModel>()
                .ForMember(vm => vm.DisponibilidadeId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                .ForMember(vm => vm.ProdutoId, map => map.MapFrom(m => m.ProdutoId))
                .ForMember(vm => vm.InicioIndisponibilidade, map => map.MapFrom(m => m.InicioIndisponibilidade))
                .ForMember(vm => vm.FimIndisponibilidade, map => map.MapFrom(m => m.FimIndisponibilidade));


            #endregion

            #region Prazo Semanal Fornecedor

            Mapper.CreateMap<FornecedorPrazoSemanal, FornecedorPrazoSemanalViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                .ForMember(vm => vm.CidadeId, map => map.MapFrom(m => m.CidadeId))
                .ForMember(vm => vm.DescCidade, map => map.MapFrom(m => m.Cidade.DescCidade))
                .ForMember(vm => vm.DiaSemana, map => map.MapFrom(m => m.DiaSemana));

            #endregion

            #region Mapeamento Franquia

            Mapper.CreateMap<Franquia, FranquiaViewModel>()
                .ForMember(vm => vm.FranquiaId, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.Responsavel, map => map.MapFrom(m => m.Responsavel))
                .ForMember(vm => vm.Descricao, map => map.MapFrom(m => m.Descricao))
                .ForMember(vm => vm.DataCotacao, map => map.MapFrom(m => m.DataCotacao))
                .ForMember(vm => vm.Cnpj, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Cnpj))
                .ForMember(vm => vm.NomeFantasia, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.NomeFantasia))
                .ForMember(vm => vm.RazaoSocial, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.RazaoSocial))
                .ForMember(vm => vm.DtFundacao, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.DtFundacao))
                .ForMember(vm => vm.Email, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.Email))
                .ForMember(vm => vm.InscEstadual, map => map.MapFrom(m => m.Pessoa.PessoaJuridica.InscEstadual))
                .ForMember(vm => vm.DddTelComl, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault().Telefones.FirstOrDefault().DddTelComl))
                .ForMember(vm => vm.TelefoneComl, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault().Telefones.FirstOrDefault().TelefoneComl))
                .ForMember(vm => vm.DddCel, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault().Telefones.FirstOrDefault().DddCel))
                .ForMember(vm => vm.Celular, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault().Telefones.FirstOrDefault().Celular))
                .ForMember(vm => vm.Contato, map => map.MapFrom(m => m.Pessoa.Usuarios.FirstOrDefault().Telefones.FirstOrDefault().Contato))
                .ForMember(vm => vm.Endereco, map => map.MapFrom(m => m.Pessoa.Enderecos.FirstOrDefault()))
                .ForMember(vm => vm.Completo, map => map.MapFrom(m => m.Pessoa.Enderecos.Count == 0 ? "Não" :
                m.Pessoa.Usuarios.SelectMany(x => x.Telefones).Count() == 0 ? "Não" : "Sim"))
            .ForMember(vm => vm.FranquiaId, map => map.MapFrom(m => m.Id));

            #endregion

            #region Mapeamento Fornecedor Produtos

            Mapper.CreateMap<FornecedorProduto, FornecedorProdutoViewModel>()
                    .ForMember(vm => vm.FornecedorId, map => map.MapFrom(m => m.FornecedorId))
                    .ForMember(vm => vm.ProdutoId, map => map.MapFrom(m => m.ProdutoId))
                    .ForMember(vm => vm.ListaQuantidadeDesconto,
                    map =>
                         map.MapFrom(
                            m => (Mapper.Map<IEnumerable<FornecedorProdutoQuantidade>, IEnumerable<FornecedorProdutoQuantidadeViewModel>>(m.ListaQuantidadeDesconto))))
                     .ForMember(vm => vm.PercentMin, map => map.MapFrom(m => m.ListaQuantidadeDesconto.Count > 0 ? m.ListaQuantidadeDesconto.Min(lq => lq.PercentualDesconto) : 0))
                     .ForMember(vm => vm.PercentMax, map => map.MapFrom(m => m.ListaQuantidadeDesconto.Count > 0 ? m.ListaQuantidadeDesconto.Max(lq => lq.PercentualDesconto) : 0));

            Mapper.CreateMap<FornecedorProdutoQuantidade, FornecedorProdutoQuantidadeViewModel>()
                    .ForMember(vm => vm.FornecedorProdutoId, map => map.MapFrom(m => m.FornecedorProdutoId));


            #endregion

            #region Calendario Feriado

            Mapper.CreateMap<CalendarioFeriado, CalendarioFeriadoViewModel>()
                .ForMember(vm => vm.NomeCidade, map => map.MapFrom(m => m.Cidade))
                .ForMember(vm => vm.Estado, map => map.MapFrom(m => m.Estado))
                .ForMember(vm => vm.DtEvento, map => map.MapFrom(m => m.DtEvento))
                .ForMember(vm => vm.NomeFeriado, map => map.MapFrom(m => m.NomeFeriado))
                .ForMember(vm => vm.TipoFeriado, map => map.MapFrom(m => m.TipoFeriado));

            #endregion


            this.TermoUso();

            this.Notificacao();

            this.Pagamento();
        }

        #region Mapeamento Termo de Uso

        private void TermoUso()
        {
            Mapper.CreateMap<TermoUso, TermoUsoViewModel>()
                .ForMember(vm => vm.Documento, map => map.MapFrom(m => m.Documento));
        }

        #endregion

        #region Mapeamento Notificação

        private void Notificacao()
        {
            Mapper.CreateMap<Notificacao, NotificacaoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.Alerta, map => map.MapFrom(m => m.TipoAlerta))
                .ForMember(vm => vm.Ativo, map => map.MapFrom(m => m.Ativo));

            Mapper.CreateMap<TipoAvisos, TipoNotificacaoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.Descricao, map => map.MapFrom(m => m.Descricao))
                .ForMember(vm => vm.Notificacoes,
                    map => map.MapFrom(m => Mapper.Map<IEnumerable<Notificacao>, List<NotificacaoViewModel>>(m.Notificacoes.OrderBy(x => x.TipoAlerta))));
        }

        #endregion


        #region Mapeamento de Pagamento

        private void Pagamento()
        {
            Mapper.CreateMap<Comissao, ComissaoViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.PedidoId, map => map.MapFrom(m => m.PedidoId))
                .ForMember(vm => vm.Valor, map => map.MapFrom(m => m.Valor))
                .ForMember(vm => vm.PedidoTotal, map => map.MapFrom(m => m.PedidoTotal))
                .ForMember(vm => vm.DtCriacao, map => map.MapFrom(m => m.DtCriacao));


            Mapper.CreateMap<Fatura, FaturaViewModel>()
                .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
                .ForMember(vm => vm.DtVencimento, map => map.MapFrom(m => m.DtVencimento))
                .ForMember(vm => vm.Status, map => map.MapFrom(m => StatusFaturaToString(m.Status)))
                .ForMember(vm => vm.StatusRecebimento, map => map.MapFrom(m => m.Status))
                .ForMember(vm => vm.UrlBoleto, map => map.MapFrom(m => m.UrlBoleto))
                .ForMember(vm => vm.Comissoes, map => map.MapFrom(m => Mapper.Map<IEnumerable<Comissao>, List<ComissaoViewModel>>(m.Comissoes.OrderBy(x => x.DtCriacao))));

            Mapper.CreateMap<Mensalidade, MensalidadeViewModel>()
               .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
               .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status))
               .ForMember(vm => vm.Descricao, map => map.MapFrom(m => m.Descricao))
               .ForMember(vm => vm.Total, map => map.MapFrom(m => m.Detalhes.Sum(x => x.Valor)))
               .ForMember(vm => vm.DtVencimento, map => map.MapFrom(m => m.DtVencimento));
        }

        private string StatusFaturaToString(StatusFatura status)
        {
            switch (status)
            {
                case StatusFatura.Parcial:
                    return "Parcial";
                default:
                    return "Fechado";
            }
        }

        #endregion
    }
}
