using ECC.API_Web.Models;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using ECC.EntidadeEndereco;
using ECC.EntidadeEstoque;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeStatus;
using ECC.EntidadeAvisos;
using ECC.Entidades.EntidadeComum;

namespace ECC.API_Web.InfraWeb.ExtensionsWeb
{
    public static class EntidadeExtensions
    {

        #region Atualiza dados do membro PF
        public static void AtualizarMembroPf(this Membro membro, MembroPFViewModel membroVm, Usuario usuario)
        {
            membro.UsuarioAlteracao = usuario;
            membro.Pessoa.UsuarioAlteracao = usuario;
            membro.Pessoa.PessoaFisica.UsuarioAlteracao = usuario;
            membro.Ativo = membroVm.Ativo;
            membro.Pessoa.Ativo = membroVm.Ativo;
            membro.Pessoa.PessoaFisica.Ativo = membroVm.Ativo;
            membro.Comprador = membroVm.Comprador;
            membro.Pessoa.PessoaFisica.Nome = membroVm.Nome;
            membro.Pessoa.PessoaFisica.Sexo =(Sexo)membroVm.Sexo;
            membro.Pessoa.PessoaFisica.Cpf = membroVm.Cpf;
            membro.Pessoa.PessoaFisica.Rg = membroVm.Rg;
            membro.Pessoa.PessoaFisica.Cro = membroVm.Cro;
            membro.Pessoa.PessoaFisica.Email = membroVm.Email;
 
            membro.Vip = membroVm.Vip;
            membro.DddTel = membroVm.DddTelComl;
            membro.Telefone = membroVm.TelefoneComl;
            membro.DddCel = membroVm.DddCel;
            membro.Celular = membroVm.Celular;
            membro.Contato = membroVm.Contato;
            membro.DataFimPeriodoGratuito = membroVm.DataFimPeriodoGratuito;
          
        }
        #endregion

        #region Atualiza dados do membro
        public static void AtualizarMembro(this Membro membro, MembroViewModel membroVm, Usuario usuario)
        {
            membro.UsuarioAlteracao = usuario;
            membro.Pessoa.UsuarioAlteracao = usuario;
            membro.Pessoa.PessoaJuridica.UsuarioAlteracao = usuario;
            membro.Ativo = membroVm.Ativo;
            membro.Pessoa.Ativo = membroVm.Ativo;
            membro.Pessoa.PessoaJuridica.Ativo = membroVm.Ativo;
            membro.Comprador = membroVm.Comprador;
            membro.Pessoa.PessoaJuridica.NomeFantasia = membroVm.NomeFantasia;
            membro.Pessoa.PessoaJuridica.Cnpj = membroVm.Cnpj;
            membro.Pessoa.PessoaJuridica.RazaoSocial = membroVm.RazaoSocial;
            membro.Pessoa.PessoaJuridica.DtFundacao = membroVm.DtFundacao;
            membro.Pessoa.PessoaJuridica.Email = membroVm.Email;
            membro.Pessoa.PessoaJuridica.InscEstadual = membroVm.InscEstadual;
            membro.Vip = membroVm.Vip;
            membro.DddTel = membroVm.DddTelComl;
            membro.Telefone = membroVm.TelefoneComl;
            membro.DddCel = membroVm.DddCel;
            membro.Celular = membroVm.Celular;
            membro.Contato = membroVm.Contato;
            membro.FranquiaId = membroVm.FranquiaId;
            membro.DataFimPeriodoGratuito = membroVm.DataFimPeriodoGratuito;
        }
        #endregion

        #region Atualiza dados do membro demanda
        public static void AtualizarMembroDemanda(this MembroDemanda membroDemanda, MembroDemandaViewModel membroDemandaVm, Usuario usuario)
        {
            membroDemanda.UsuarioAlteracao = usuario;
            membroDemanda.SubCategoriaId = membroDemandaVm.SubCategoriaId;
            membroDemanda.UnidadeMedidaId = membroDemandaVm.UnidadeMedidaId;
            membroDemanda.PeriodicidadeId = membroDemandaVm.PeriodicidadeId;
            membroDemanda.Quantidade = membroDemandaVm.Quantidade;
            membroDemanda.Observacao = membroDemandaVm.Observacao;
            membroDemanda.Ativo = membroDemandaVm.Ativo;

        }
        #endregion

        #region Atualiza Telefone Pessoa
        public static void AtualizarTelefoneMembro(this Telefone telefone, MembroViewModel membroViewModel, Usuario usuario)
        {
            telefone.UsuarioAlteracao = usuario;
            telefone.DtCriacao = DateTime.Now;
            telefone.DddTelComl = membroViewModel.DddTelComl;
            telefone.TelefoneComl = membroViewModel.TelefoneComl;
            telefone.DddCel = membroViewModel.DddCel;
            telefone.Celular = membroViewModel.Celular;
            telefone.Ativo = membroViewModel.Ativo;
            telefone.Contato = membroViewModel.Contato;
            telefone.Ativo = membroViewModel.Ativo;


        }
        #endregion

        #region Atualiza endereco pessoa
        public static void AtualizarEndereco(this Endereco endereco, EnderecoViewModel enderecoViewModel, Usuario usuario)
        {
            endereco.UsuarioAlteracao = usuario;
            endereco.DtAlteracao = DateTime.Now;
            endereco.EstadoId = enderecoViewModel.EstadoId;
            endereco.CidadeId = enderecoViewModel.CidadeId;
            endereco.BairroId = enderecoViewModel.BairroId;
            endereco.LogradouroId = enderecoViewModel.LogradouroId;
            endereco.Numero = enderecoViewModel.NumEndereco;
            endereco.Complemento = enderecoViewModel.Complemento;
            endereco.DescEndereco = enderecoViewModel.Endereco.Trim();
            endereco.Cep = enderecoViewModel.Cep;
            endereco.Ativo = enderecoViewModel.Ativo;
            endereco.EnderecoPadrao = enderecoViewModel.EnderecoPadrao;
            endereco.Referencia = enderecoViewModel.Referencia;
            endereco.LocalizacaoGoogle();
        }

        #endregion

        #region Atualiza unidade de medida
        public static void AtualizarUnidadeMedida(this UnidadeMedida unidadeMedida, UnidadeMedidaViewModel unidadeVm, Usuario usuario)
        {
            unidadeMedida.UsuarioAlteracao = usuario;
            unidadeMedida.Ativo = unidadeVm.Ativo;
            unidadeMedida.DescUnidadeMedida = unidadeVm.DescUnidadeMedida;
            unidadeMedida.DtAlteracao = DateTime.Now;
        }
        #endregion

        #region Atualiza Periodicidade
        public static void AtualizarPeriodicidade(this Periodicidade periodicidade, PeriodicidadeViewModel periodicidadeVm, Usuario usuario)
        {
            periodicidade.UsuarioAlteracao = usuario;
            periodicidade.Ativo = periodicidadeVm.Ativo;
            periodicidade.DescPeriodicidade = periodicidadeVm.DescPeriodicidade;
            periodicidade.DtAlteracao = DateTime.Now;
        }
        #endregion

        #region Atualiza Fornecedor

        public static void AtualizarForecedor(this Fornecedor fornecedor, FornecedorViewModel fornecedorVm, Usuario usuario)
        {

            fornecedor.UsuarioAlteracao = usuario;
            fornecedor.Pessoa.UsuarioAlteracao = usuario;
            fornecedor.Pessoa.PessoaJuridica.UsuarioAlteracao = usuario;
            fornecedor.Ativo = fornecedorVm.Ativo;
            fornecedor.Pessoa.Ativo = fornecedorVm.Ativo;
            fornecedor.VlPedidoMin = fornecedorVm.VlPedidoMin.TryParseDecimal();
            fornecedor.Pessoa.PessoaJuridica.Ativo = fornecedorVm.Ativo;
            fornecedor.Responsavel = fornecedorVm.Responsavel;
            fornecedor.Pessoa.PessoaJuridica.NomeFantasia = fornecedorVm.NomeFantasia;
            fornecedor.Pessoa.PessoaJuridica.Cnpj = fornecedorVm.Cnpj;
            fornecedor.Pessoa.PessoaJuridica.RazaoSocial = fornecedorVm.RazaoSocial;
            fornecedor.Pessoa.PessoaJuridica.DtFundacao = fornecedorVm.DtFundacao;
            fornecedor.Pessoa.PessoaJuridica.Email = fornecedorVm.Email;
            fornecedor.Pessoa.PessoaJuridica.InscEstadual = fornecedorVm.InscEstadual;
            fornecedor.Descricao = fornecedorVm.Descricao;
            fornecedor.PalavrasChaves = fornecedorVm.PalavrasChaves;
            fornecedor.Observacao = fornecedorVm.Observacao;
            fornecedor.ObservacaoEntrega = fornecedorVm.ObservacaoEntrega;
            fornecedor.PrimeiraAvista = fornecedorVm.PrimeiraAvista;         
            fornecedor.DddTel = fornecedorVm.DddTelComl;
            fornecedor.Telefone = fornecedorVm.TelefoneComl;
            fornecedor.DddCel = fornecedorVm.DddCel;
            fornecedor.Celular = fornecedorVm.Celular;
            fornecedor.Contato = fornecedorVm.Contato;




        }

        #endregion

        #region Atualiza Telefone Fornecedor
        public static void AtualizarTelefoneFornecedor(this Telefone telefone, FornecedorViewModel fornecedorViewModel, Usuario usuario)
        {
            telefone.UsuarioAlteracao = usuario;
            telefone.DtCriacao = DateTime.Now;
            telefone.DddTelComl = fornecedorViewModel.DddTelComl;
            telefone.TelefoneComl = fornecedorViewModel.TelefoneComl;
            telefone.DddCel = fornecedorViewModel.DddCel;
            telefone.Celular = fornecedorViewModel.Celular;
            telefone.Ativo = fornecedorViewModel.Ativo;
            telefone.Contato = fornecedorViewModel.Contato;
            telefone.Ativo = fornecedorViewModel.Ativo;


        }
        #endregion

        #region Atualiza Workflow Status Sistema
        public static void AtualizarWorkflowStatus(this WorkflowStatus workflowStatus, workflowStatusViewModel workflowStatusVM, Usuario usuario)
        {
            workflowStatus.UsuarioAlteracao = usuario;
            workflowStatus.Ativo = workflowStatusVM.Ativo;
            workflowStatus.DescWorkslowStatus = workflowStatusVM.DescWorkslowStatus;
            workflowStatus.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Status Sistema
        public static void AtualizarStatusSistema(this StatusSistema Status, StatusSistemaViewModel StatusVM, Usuario usuario)
        {
            Status.UsuarioAlteracao = usuario;
            Status.Ativo = StatusVM.Ativo;
            Status.DescStatus = StatusVM.DescStatus;
            Status.Ordem = StatusVM.Ordem;
            Status.WorkflowStatusId = StatusVM.WorkflowStatusId;
            Status.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Categoria
        public static void AtualizarCategoria(this Categoria categoria, CategoriaViewModel categoriaVM, Usuario usuario)
        {
            categoria.UsuarioAlteracao = usuario;
            categoria.Ativo = categoriaVM.Ativo;
            categoria.DescCategoria = categoriaVM.DescCategoria;
            categoria.Id = categoriaVM.Id;
            categoria.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Sub-Categoria
        public static void AtualizarSubCategoria(this SubCategoria subcategoria, SubCategoriaViewModel SubCategoriaVM, Usuario usuario)
        {
            subcategoria.UsuarioAlteracao = usuario;
            subcategoria.Ativo = SubCategoriaVM.Ativo;
            subcategoria.DescSubCategoria = SubCategoriaVM.DescSubCategoria;
            subcategoria.CategoriaId = SubCategoriaVM.CategoriaId;
            subcategoria.Id = SubCategoriaVM.Id;
            subcategoria.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Produto
        public static void AtualizarProduto(this Produto produto, ProdutoViewModel produtoVM, Usuario usuario)
        {
            produto.UsuarioAlteracao = usuario;
            produto.DtAlteracao = DateTime.Now;
            produto.DescProduto = produtoVM.DescProduto;
            produto.SubCategoriaId = produtoVM.SubCategoriaId;
            produto.UnidadeMedidaId = produtoVM.UnidadeMedidaId;
            produto.MarcaId = produtoVM.MarcaId;
            produto.Especificacao = produtoVM.Especificacao;
            produto.PrecoMedio = produtoVM.PrecoMedio.TryParseDecimal();
            produto.CodigoCNP = produtoVM.CodigoCNP;
            produto.Ativo = produtoVM.Ativo;

        }
        #endregion

        #region Atualiza Fabricante
        public static void AtualizarFabricante(this Fabricante fabricante, FabricanteViewModel fabricanteVM, Usuario usuario)
        {
            fabricante.UsuarioAlteracao = usuario;
            fabricante.Ativo = fabricanteVM.Ativo;
            fabricante.DescFabricante = fabricanteVM.DescFabricante;
            fabricante.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Imagem
        public static void AtualizarImagem(this Imagem imagem, ImagemViewModel imagemVM, Usuario usuario)
        {
            imagem.UsuarioAlteracao = usuario;
            imagem.Id = imagemVM.ImagemId;
            imagem.ProdutoId = imagemVM.IdProduto;
            imagem.CaminhoImagem = imagemVM.CaminhoImagem;
            imagem.CaminhoImagemGrande = imagemVM.CaminhoImagemGrande;

        }
        #endregion

        #region Atualiza Usuarios
        public static void AtualizarUsuario(this Usuario usuario, UsuarioViewModel usuarioVM, Usuario usuarioAlt, IEncryptionService _encryptionService)
        {
            usuario.UsuarioAlteracao = usuarioAlt;
            usuario.UsuarioNome = usuarioVM.UsuarioNome;
            usuario.UsuarioEmail = usuarioVM.UsuarioEmail;
            var senhaChave = _encryptionService.CriaChave();

            if (usuarioVM.TelefoneUsuario != null)
            {
                var telefoneUsuario = usuario.Telefones.FirstOrDefault(x => x.Id == usuarioVM.TelefoneUsuario.Id);
                usuario.Telefones.Remove(telefoneUsuario);
                telefoneUsuario.DddCel = usuarioVM.TelefoneUsuario.DddCel;
                telefoneUsuario.Celular = usuarioVM.TelefoneUsuario.Celular;
                telefoneUsuario.DddTelComl = usuarioVM.TelefoneUsuario.DddTelComl;
                telefoneUsuario.TelefoneComl = usuarioVM.TelefoneUsuario.TelefoneComl;
                telefoneUsuario.Contato = usuarioVM.TelefoneUsuario.Contato;
                usuario.Telefones.Add(telefoneUsuario);
            }
            
            //var usuarioTelefone = usuario.Telefones.Where(x=>x.Id == )

            if (!String.IsNullOrEmpty(usuarioVM.Senha))
            {
                if (!String.IsNullOrEmpty(usuarioVM.NovaSenha))
                {
                    usuario.Senha = _encryptionService.EncryptSenha(usuarioVM.NovaSenha, senhaChave);
                }
                else
                {
                    usuario.Senha = _encryptionService.EncryptSenha(usuarioVM.Senha, senhaChave);
                }
                usuario.Chave = senhaChave;
            }

            usuario.Ativo = usuarioVM.Ativo;
            usuario.FlgMaster = usuarioVM.FlgMaster;

        }
        #endregion

        #region Atualiza Prazo
        public class dadosPrazoRegiao
        {
            public int fornecedorId { get; set; }
            public int regiaoId { get; set; }
            public int prazo { get; set; }
        }

        public static void atualizaPrazo(this FornecedorRegiao[] fornecedorRegiao, dadosPrazoRegiao fornecedorPrazo,
            Usuario usuario)
        {
            for (int i = 0; fornecedorRegiao.Count() < i; i++)
            {
                fornecedorRegiao[i].Prazo = fornecedorPrazo.prazo;
            }
        }

        #endregion

        #region Atualiza Marca
        public static void AtualizarMarca(this Marca marca, MarcaViewModel marcaVM, Usuario usuario)
        {
            marca.UsuarioAlteracao = usuario;
            marca.Ativo = marcaVM.Ativo;
            marca.DescMarca = marcaVM.DescMarca;
            marca.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Segmento
        public static void AtualizarSegmento(this Segmento marca, SegmentoViewModel marcaVM, Usuario usuario)
        {
            marca.UsuarioAlteracao = usuario;
            marca.Ativo = marcaVM.Ativo;
            marca.DescSegmento = marcaVM.DescSegmento;
            marca.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Grupo
        public static void AtualizarGrupo(this Grupo grupo, GrupoViewModel fabricanteVM, Usuario usuario)
        {
            grupo.UsuarioAlteracao = usuario;
            grupo.Ativo = fabricanteVM.Ativo;
            grupo.DescGrupo = fabricanteVM.DescGrupo;
            grupo.DtAlteracao = DateTime.Now;

        }
        #endregion

        #region Atualiza Menu
        public static void AtualizarMenu(this EntidadeMenu.Menu menu, MenuViewModel menuVM, Usuario usuario)
        {
            menu.UsuarioAlteracao = usuario;
            menu.DtAlteracao = DateTime.Now;
            menu.Ativo = menuVM.Ativo;
            menu.ModuloId = menuVM.ModuloId;
            menu.DescMenu = menuVM.DescMenu;
            menu.MenuPaiId = menuVM.MenuPaiId;
            menu.Nivel = menuVM.Nivel;
            menu.Ordem = menuVM.Ordem;
            menu.FontIcon = menuVM.FontIcon;
            menu.Feature1 = menuVM.Feature1;
            menu.Feature2 = menuVM.Feature2;

        }
        #endregion

        #region Atualiza Membro Fornecedor

        public static void AtualizarMembroFornecedor(this MembroFornecedor mf, MembroFornecedorViewModel mfVM, Usuario usuario)
        {
            mf.UsuarioAlteracao = usuario;
            mf.DtAlteracao = DateTime.Now;
            mf.Ativo = mfVM.Ativo;
            mf.MembroId = mfVM.MembroId;
            mf.FornecedorId = mfVM.FornecedorId;
        }


        #endregion


        #region Atualiza Estoque
        public static void AtualizarEstoque(this Estoque estoque, EstoqueViewModel estoqueVM, Usuario usuario)
        {
            estoque.UsuarioAlteracao = usuario;
            estoque.DtAlteracao = DateTime.Now;
            estoque.Ativo = estoqueVM.Ativo;
            estoque.ProdutoId = estoqueVM.ProdutoId;
            estoque.MembroId = estoqueVM.MembroId;
            estoque.EnderecoId = estoqueVM.EnderecoId;
            estoque.MinimoEstoque = estoqueVM.MinimoEstoque;
            estoque.MaximoEstoque = estoqueVM.MaximoEstoque;
            estoque.QtdEstoque = estoqueVM.QtdEstoque;
            estoque.QtdEstoqueReceber = estoqueVM.QtdEstoqueReceber;

        }
        #endregion


        #region Atualiza Solicitação Membro Fornecedor

        public static void AtualizarSolicitacaoMembroFornecedor(this SolicitacaoMembroFornecedor mf, SolicitacaoMembroFornecedorViewModel mfVM, Usuario usuario)
        {
            mf.UsuarioAlteracao = usuario;
            mf.DtAlteracao = DateTime.Now;
            mf.Ativo = mfVM.Ativo;
            mf.MembroId = mfVM.MembroId;
            mf.FornecedorId = mfVM.FornecedorId;
            mf.MotivoRecusa = mfVM.Observacao;
        }


        #endregion

        #region Atualiza Itens Pedido Membro

        public static void AtualizarItensPedidoMembro(this ItemPedido ip, ItemPedidoViewModel itemVM, Usuario usuario)
        {
            ip.UsuarioAlteracao = usuario;
            ip.DtAlteracao = DateTime.Now;
            ip.DataEntregaMembro = itemVM.DataEntregaMembro;
            ip.EntregaId = itemVM.EntregaId;
        }


        #endregion


        #region Atualiza Status Pedido Finalizado

        public static void AtualizarStatusPedidoFinalizado(this Pedido ped, PedidoViewModel pedidoVM, Usuario usuario)
        {
            ped.UsuarioAlteracao = usuario;
            ped.DtAlteracao = DateTime.Now;
            ped.StatusSistemaId = pedidoVM.StatusId;
        }


        #endregion


        #region Recupera Coordenadas

        public static void LocalizacaoGoogle(this Endereco endereco)
        {
            try
            {
                Dictionary<string, object> jsonResult;

                var url = new[]
                {
                    string.Format(
                        "https://maps.googleapis.com/maps/api/geocode/json?address={0}+{1},+{2},+{3},+{4},+BR&sensor=false&key=AIzaSyC7bpLjf8oFauFPwuUNYxNHVmkyn22UyEI",
                        endereco.Numero, endereco.DescEndereco, endereco.Bairro.DescBairro,
                        endereco.Cidade.DescCidade, endereco.Estado.DescEstado),
                    string.Format(
                        "https://maps.googleapis.com/maps/api/geocode/json?address={0}+{1},+{2},+{3},+BR&sensor=false&key=AIzaSyC7bpLjf8oFauFPwuUNYxNHVmkyn22UyEI",
                        endereco.Numero, endereco.DescEndereco,
                        endereco.Cidade.DescCidade, endereco.Estado.DescEstado),
                    string.Format(
                        "https://maps.googleapis.com/maps/api/geocode/json?address=+{0},+{1},+{2},+BR&sensor=false&key=AIzaSyC7bpLjf8oFauFPwuUNYxNHVmkyn22UyEI",
                        endereco.Bairro.DescBairro,endereco.Cidade.DescCidade, endereco.Estado.DescEstado),
                    string.Format(
                        "https://maps.googleapis.com/maps/api/geocode/json?address=+{0},+{1},+BR&sensor=false&key=AIzaSyC7bpLjf8oFauFPwuUNYxNHVmkyn22UyEI",
                        endereco.Cidade.DescCidade, endereco.Estado.DescEstado)
                };

                var i = 0;

                do
                {
                    var request = WebRequest.Create(url[i].Replace(" ", "+"));

                    var response = string.Empty;

                    using (var resp = (HttpWebResponse)request.GetResponse())
                    using (var dataStream = resp.GetResponseStream())
                        if (dataStream != null)
                            using (var reader = new StreamReader(dataStream))
                            {
                                response = reader.ReadToEnd();
                            }

                    var json = new JavaScriptSerializer();

                    jsonResult = (Dictionary<string, object>)json.DeserializeObject(response);
                    i++;
                } while (i < url.Length && (jsonResult == null || (string)jsonResult["status"] != "OK"));

                foreach (Dictionary<string, object> results in (object[])jsonResult["results"])
                {
                    var location =
                        (Dictionary<string, object>)((Dictionary<string, object>)results["geometry"])["location"];

                    if (location == null) continue;

                    var point = string.Format("POINT({0} {1})", location["lng"], location["lat"]);
                    endereco.Localizacao = DbGeography.FromText(point.Replace(",", "."), 4326);
                    break;
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        #endregion




        public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T> callback)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            IterateHelper(enumerable, (x, i) => callback(x));
        }

        public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T, int> callback)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            IterateHelper(enumerable, callback);
        }

        private static void IterateHelper<T>(this IEnumerable<T> enumerable, Action<T, int> callback)
        {
            int count = 0;
            foreach (var cur in enumerable)
            {
                callback(cur, count);
                count++;
            }
        }



    }
}