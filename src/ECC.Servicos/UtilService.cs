using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ECC.Servicos.Abstrato;
using ECC.Dados.Repositorio;
using ECC.Dados.Infra;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.Servicos.ModelService;
using ECC.EntidadeParametroSistema;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeCotacao;
using ECC.EntidadeProduto;
using ECC.EntidadeSms;

namespace ECC.Servicos
{
    public class UtilService : IUtilService
    {

        #region Variaveis
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<CotacaoPedidos> _cotacaoPedidos;
        private readonly IEntidadeBaseRep<FornecedorProduto> _fornecedorProdutoRep;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedor;
        private readonly IEntidadeBaseRep<Franquia> _franquiaRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacao;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IMembershipService _membershipService;
        private readonly IEmailService _emailService;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<IndisponibilidadeProduto> _indisponibilidadeProdutoRep;
        private readonly IEntidadeBaseRep<RemoveFornPedido> _removeFornPedidoRep;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Construtor
        public UtilService(IEntidadeBaseRep<Emails> emailsRep,
            IEntidadeBaseRep<Pedido> pedidoRep,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<CotacaoPedidos> cotacaoPedidos,
            IEntidadeBaseRep<FornecedorProduto> fornecedorProdutoRep,
            IEntidadeBaseRep<MembroFornecedor> membroFornecedor,
            IEntidadeBaseRep<Franquia> franquiaRep,
            IEntidadeBaseRep<ResultadoCotacao> resultadoCotacao,
            IEntidadeBaseRep<TemplateEmail> templateEmailRep,
            IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
            IEntidadeBaseRep<RecuperaSenha> recuperaSenhaRep,
            IEntidadeBaseRep<Avisos> avisosRep,
            IEntidadeBaseRep<IndisponibilidadeProduto> indisponibilidadeProdutoRep,
            IEntidadeBaseRep<RemoveFornPedido> removeFornPedidoRep,
            ISmsService smsService,
            IMembershipService membershipService,
            IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _emailsRep = emailsRep;
            _usuarioRep = usuarioRep;
            _membroRep = membroRep;
            _templateEmailRep = templateEmailRep;
            _membershipService = membershipService;
            _emailService = emailService;
            _parametroSistemaRep = parametroSistemaRep;
            _unitOfWork = unitOfWork;
            _pedidoRep = pedidoRep;
            _fornecedorRep = fornecedorRep;
            _cotacaoPedidos = cotacaoPedidos;
            _membroFornecedor = membroFornecedor;
            _franquiaRep = franquiaRep;
            _resultadoCotacao = resultadoCotacao;
            _avisosRep = avisosRep;
            _indisponibilidadeProdutoRep = indisponibilidadeProdutoRep;
            _removeFornPedidoRep = removeFornPedidoRep;
            _smsService = smsService;
        }



        #endregion

        #region IUtilService Implementacao

        /// <summary>
        /// Criar usuário para ADM Franquia e Enviar Email.
        /// </summary>
        /// <param name="FranquiaId">Passar Id da Franquia</param>
        /// <param name="usu">Passar usuário de Criação.</param>
        /// <param name="telefone">Passar objeto telefone.</param>
        public Franquia FranquiaInserirUsuarioEnviarEmail(int FranquiaId, Usuario usu, Telefone telefone)
        {

            bool booValidaUsuario = false;

            Franquia franquiaAtual = _franquiaRep.GetSingle(FranquiaId);


            //Verifica se já foi completo o cadastro para criar Usuário Master e Enviar Email de Boas vindas.

            //inserir usuario
            int PerfilId = 4;

            Usuario _user = _membershipService.CreateUser(
                franquiaAtual.Pessoa.PessoaJuridica.NomeFantasia,
                franquiaAtual.Pessoa.PessoaJuridica.Email,
                franquiaAtual.Pessoa.PessoaJuridica.Cnpj.Substring(0, 8),
                PerfilId,
                franquiaAtual.PessoaId,
                usu.Id,
                true,
                telefone.DddTelComl,
                telefone.TelefoneComl,
                telefone.DddCel,
                telefone.Celular,
                telefone.Contato);

            //Enviar Email
            var template = _templateEmailRep.GetSingle(29).Template;

            _emailService.EnviaEmail(franquiaAtual.Pessoa.PessoaJuridica.Email, "",
                _emailService.MontaEmail(franquiaAtual.Pessoa.PessoaJuridica, template), "Bem-Vindo à Economiza Já");

            return franquiaAtual;

        }

        public void MembroInserirUsuarioEnviarEmail(int MembroId, int UsuarioId)
        {

            bool booValidaUsuario = false;

            Membro membroAtual = _membroRep.GetSingle(MembroId);

            //Verifica se tem usuario Master
            Usuario u = membroAtual.Pessoa.Usuarios.FirstOrDefault(x => x.FlgMaster);
            if (u == null)
                booValidaUsuario = true;

            //Verifica se já foi completo o cadastro para criar Usuário Master e Enviar Email de Boas vindas.
            if (membroAtual.Pessoa.Enderecos.Any() &&
                // membroAtual.MembroFornecedores.Any() &&
                membroAtual.MembroCategorias.Any() &&
                booValidaUsuario)
            {

                //inserir usuario
                int PerfilId = 2;
                Usuario usuario = _usuarioRep.GetSingle(UsuarioId);




                if (membroAtual.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                {
                    Usuario _user = _membershipService.CreateUser(membroAtual.Pessoa.PessoaJuridica.NomeFantasia, membroAtual.Pessoa.PessoaJuridica.Email, membroAtual.Pessoa.PessoaJuridica.Cnpj.Substring(0, 8),
                          PerfilId, membroAtual.PessoaId, usuario.Id, true, membroAtual.DddTel, membroAtual.Telefone, membroAtual.DddCel, membroAtual.Celular, membroAtual.Contato);


                    //Enviar Email
                    var template = _templateEmailRep.GetSingle(1).Template;


                    Emails email1 = new Emails()
                    {
                        EmailDestinatario = membroAtual.Pessoa.PessoaJuridica.Email,
                        CorpoEmail = _emailService.MontaEmail(membroAtual.Pessoa.PessoaJuridica, template),
                        AssuntoEmail = "Bem-Vindo à Economiza Já",
                        Status = Status.NaoEnviado,
                        Origem = Origem.CriarUsuarioFornecedorMembro,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = usuario,
                        Ativo = true
                    };

                    _emailsRep.Add(email1);
                    _unitOfWork.Commit();


                }
                else
                {
                    Usuario _user = _membershipService.CreateUser(membroAtual.Pessoa.PessoaFisica.Nome, membroAtual.Pessoa.PessoaFisica.Email, membroAtual.Pessoa.PessoaFisica.Cpf.Substring(0, 8),
                         PerfilId, membroAtual.PessoaId, usuario.Id, true, membroAtual.DddTel, membroAtual.Telefone, membroAtual.DddCel, membroAtual.Celular, membroAtual.Contato);





                    //Enviar Email
                    var template = _templateEmailRep.GetSingle(35).Template;


                    Emails email1 = new Emails()
                    {
                        EmailDestinatario = membroAtual.Pessoa.PessoaFisica.Email,
                        CorpoEmail = _emailService.MontaEmail(membroAtual.Pessoa.PessoaFisica, template),
                        AssuntoEmail = "Bem-Vindo à Economiza Já",
                        Status = Status.NaoEnviado,
                        Origem = Origem.CriarUsuarioFornecedorMembro,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = usuario,
                        Ativo = true
                    };

                    _emailsRep.Add(email1);
                    _unitOfWork.Commit();




                }



            }



        }

        public void FornecedorInserirUsuarioEnviarEmail(int FornecedorId, int UsuarioId)
        {
            bool booValidaUsuario = false;

            Fornecedor fornecedorAtual = _fornecedorRep.GetSingle(FornecedorId);

            //Verifica se tem usuario Master
            //Usuario u = fornecedorAtual.Pessoa.Usuarios.FirstOrDefault(x => x.FlgMaster);

            //if (u == null)
            //    booValidaUsuario = true;

            //Verifica se já foi completo o cadastro para criar Usuário Master e Enviar Email de Boas vindas.
            if (fornecedorAtual.Pessoa.Enderecos.Any() &&
                fornecedorAtual.FornecedorCategorias.Any() &&
                fornecedorAtual.FornecedorRegiao.Any() &&
                 fornecedorAtual.FornecedorFormaPagtos.Any()
                 // &&  booValidaUsuario
                 )
            {

                //inserir usuario
                int PerfilId = 3;
                Usuario usuario = _usuarioRep.GetSingle(UsuarioId);

                Usuario _user = _membershipService.CreateUser(
                    fornecedorAtual.Pessoa.PessoaJuridica.NomeFantasia,
                    fornecedorAtual.Pessoa.PessoaJuridica.Email,
                    fornecedorAtual.Pessoa.PessoaJuridica.Cnpj.Substring(0, 8),
                      PerfilId,
                      fornecedorAtual.PessoaId,
                      usuario.Id,
                      true, fornecedorAtual.DddTel, fornecedorAtual.Telefone, fornecedorAtual.DddCel, fornecedorAtual.Celular);

                //Enviar Email
                var template = _templateEmailRep.GetSingle(19).Template;




                Emails email1 = new Emails()
                {
                    EmailDestinatario = fornecedorAtual.Pessoa.PessoaJuridica.Email,
                    CorpoEmail = _emailService.MontaEmail(fornecedorAtual.Pessoa.PessoaJuridica, template),
                    AssuntoEmail = "Bem-Vindo à Economiza Já",
                    Status = Status.NaoEnviado,
                    Origem = Origem.CriarUsuarioFornecedorMembro,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacao = usuario,
                    Ativo = true
                };

                _emailsRep.Add(email1);
                _unitOfWork.Commit();




            }

        }

        /// <summary>
        /// Enviar pedido do cliente Membro
        /// </summary>
        /// <param name="pedidoId"></param>
        /// <param name="situacao">1 envia pedido gerado e 2 Enviar o pedido com os preços cotados </param>
        /// 
        public void EnviaEmailPedido(int pedidoId, int situacao, Usuario Usuario)
        {

            Pedido pedido = _pedidoRep.GetSingle(pedidoId);
            var usuarioMembro = _usuarioRep.FirstOrDefault(x => x.Id == pedido.UsuarioCriacaoId);
            PedidoEmailViewModel pedidoEmailVm = null;

            if (usuarioMembro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
            {
                pedidoEmailVm = new PedidoEmailViewModel()
                {
                    Id = pedido.Id,
                    NomeFantasia = pedido.Membro.Pessoa.PessoaJuridica.NomeFantasia
                };
            }
            else
            {

                pedidoEmailVm = new PedidoEmailViewModel()
                {
                    Id = pedido.Id,
                    NomeFantasia = pedido.Membro.Pessoa.PessoaFisica.Nome
                };
            }

            switch (situacao)
            {
                //Depois, talvez precisamos mandar no email do cadastro do membro e no email do usuário ou mandar de acordo com a configuração decidida pelo mesmo no menu configurações do membro
                //Situacao 1 é para quando gerar o pedido
                case 1:

                    Emails email1 = new Emails()
                    {
                        EmailDestinatario = Usuario.UsuarioEmail,
                        CorpoEmail = _emailService.MontaEmail(pedidoEmailVm, _templateEmailRep.GetSingle(6).Template.Replace("#Grid#", "" + MontaGridItensPedido(pedido.ItemPedidos.ToList()) + "")),
                        AssuntoEmail = "Confirmação - Número do Pedido " + pedidoId,
                        Status = Status.NaoEnviado,
                        Origem = Origem.PedidoMembroGerado,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = Usuario,
                        Ativo = true
                    };

                    _emailsRep.Add(email1);
                    _unitOfWork.Commit();

                    break;

                //Situacao 2 é para quando atualizar o valor do item do pedido do cliente nesse email iremos pedir para ele aprovar
                case 2:
                    Emails email2 = new Emails()
                    {
                        EmailDestinatario = Usuario.UsuarioEmail,
                        CorpoEmail = _emailService.MontaEmail(pedidoEmailVm, _templateEmailRep.GetSingle(6).Template.Replace("#Grid#", "" + MontaGridItensPedido(pedido.ItemPedidos.ToList()) + "")),
                        AssuntoEmail = "Aprovação de Preço - Número do Pedido " + pedidoId,
                        Status = Status.NaoEnviado,
                        Origem = Origem.PedidoMembroAprovado,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = Usuario,
                        Ativo = true
                    };

                    _emailsRep.Add(email2);
                    _unitOfWork.Commit();

                    break;

                //Situacao 3 é quando o fornecedor aprova um pedido da Cotação
                case 3:
                    var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == Usuario.PessoaId);
                    var itensPedido = pedido.ItemPedidos.Where(x => x.FornecedorId == fornecedor.Id &&
                    x.AprovacaoMembro && x.AprovacaoFornecedor).ToList();

                    Emails email3 = new Emails()
                    {
                        EmailDestinatario = usuarioMembro.UsuarioEmail,

                        CorpoEmail = _emailService.MontaEmail(pedidoEmailVm, _templateEmailRep.GetSingle(25).Template
                         .Replace("#Grid#", MontaGridItensPedido(itensPedido))
                         .Replace("#NomeFornecedor#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia)),
                        AssuntoEmail = "Itens do Pedido Aprovados - Número do Pedido " + pedidoId,
                        Status = Status.NaoEnviado,
                        Origem = Origem.PedidoMembroAprovado,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = Usuario,
                        Ativo = true
                    };

                    _emailsRep.Add(email3);
                    _unitOfWork.Commit();

                    break;

                //Situacao 4 é quando o fornecedor aprova o pedido promocional
                case 4:

                    var fornecedorPromocao = _fornecedorRep.FirstOrDefault(x => x.PessoaId == Usuario.PessoaId);

                    Emails email4 = new Emails()
                    {
                        EmailDestinatario = usuarioMembro.UsuarioEmail,

                        CorpoEmail = _emailService.MontaEmail(pedidoEmailVm, _templateEmailRep.GetSingle(26).Template
                        .Replace("#Grid#", MontaGridItensPedidoPromocao(pedido))
                        .Replace("#NomeFornecedor#", fornecedorPromocao.Pessoa.PessoaJuridica.NomeFantasia)),

                        AssuntoEmail = "Pedido promocional aprovado - Número do pedido " + pedidoId,
                        Status = Status.NaoEnviado,
                        Origem = Origem.PedidoMembroAprovado,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = Usuario,
                        Ativo = true
                    };

                    _emailsRep.Add(email4);
                    _unitOfWork.Commit();

                    break;

                //Envia email quando fornecedor confirma entrega dos itens do pedido.
                case 5:

                    var fornecedorEntrega = _fornecedorRep.FirstOrDefault(x => x.PessoaId == Usuario.PessoaId);

                    var itensPedidoEntrega = pedido.ItemPedidos.Where(x => x.FornecedorId == fornecedorEntrega.Id &&
                    x.AprovacaoMembro && x.AprovacaoFornecedor).ToList();

                    var tipoPedido = pedido.ItemPedidos
                        .Any(x => x.FornecedorId == fornecedorEntrega.Id &&
                        x.AprovacaoMembro && x.AprovacaoFornecedor &&
                        x.Produto.ProdutoPromocionalId == null);


                    var nomeMembro = pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                                     pedido.Membro.Pessoa.PessoaJuridica.NomeFantasia :
                                     pedido.Membro.Pessoa.PessoaFisica.Nome;

                    var corpoEmail = _templateEmailRep.GetSingle(32).Template
                        .Replace("#IdPedido#", pedido.Id.ToString())
                        .Replace("#NomeMembro#", nomeMembro)
                        .Replace("#NomeFornecedor#", fornecedorEntrega.Pessoa.PessoaJuridica.NomeFantasia)
                        .Replace("#Grid#", tipoPedido ? MontaGridItensPedido(itensPedidoEntrega)
                                                      : MontaGridItensPedidoPromocao(pedido));

                    var email = new Emails
                    {
                        UsuarioCriacao = Usuario,
                        DtCriacao = DateTime.Now,
                        AssuntoEmail = "Fornecedor confirmou a entrega dos itens do pedido " + pedido.Id + ".",
                        EmailDestinatario = usuarioMembro.UsuarioEmail,
                        CorpoEmail = corpoEmail.Trim(),
                        Status = Status.NaoEnviado,
                        Origem = Origem.FornecedorConfirmaEntregaPedido,
                        Ativo = true
                    };

                    _emailsRep.Add(email);
                    _unitOfWork.Commit();

                    break;


                //Envia email quando fornecedor despachar dos itens do pedido.
                case 6:

                    var fornecedorDespacho = _fornecedorRep.FirstOrDefault(x => x.PessoaId == Usuario.PessoaId);

                    var itensPedidoDespacho = pedido.ItemPedidos.Where(x => x.FornecedorId == fornecedorDespacho.Id &&
                    x.AprovacaoMembro && x.AprovacaoFornecedor).ToList();

                    var tipoPedidoDespacho = pedido.ItemPedidos
                        .Any(x => x.FornecedorId == fornecedorDespacho.Id &&
                        x.AprovacaoMembro && x.AprovacaoFornecedor &&
                        x.Produto.ProdutoPromocionalId == null);


                    var nomeMembroDespacho = pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                                     pedido.Membro.Pessoa.PessoaJuridica.NomeFantasia :
                                     pedido.Membro.Pessoa.PessoaFisica.Nome;

                    var corpoEmailDespacho = _templateEmailRep.GetSingle(40).Template
                        .Replace("#IdPedido#", pedido.Id.ToString())
                        .Replace("#NomeMembro#", nomeMembroDespacho)
                        .Replace("#NomeFornecedor#", fornecedorDespacho.Pessoa.PessoaJuridica.NomeFantasia)
                        .Replace("#Grid#", tipoPedidoDespacho ? MontaGridItensPedido(itensPedidoDespacho)
                                                      : MontaGridItensPedidoPromocao(pedido));

                    var emailDespacho = new Emails
                    {
                        UsuarioCriacao = Usuario,
                        DtCriacao = DateTime.Now,
                        AssuntoEmail = "Fornecedor Despachou para Entrega itens do seu pedido " + pedido.Id + ".",
                        EmailDestinatario = usuarioMembro.UsuarioEmail,
                        CorpoEmail = corpoEmailDespacho.Trim(),
                        Status = Status.NaoEnviado,
                        Origem = Origem.FornecedorDespachouItensPedido,
                        Ativo = true
                    };

                    _emailsRep.Add(emailDespacho);
                    _unitOfWork.Commit();
                    break;
            }
        }

        public void EnviaEmailSmsNovoPedidoItemfornecedor(int pedidoId, int idItem, Usuario usuario, bool existePedParaAprovar)
        {
            Pedido pedido = _pedidoRep.GetSingle(pedidoId);
           
            var itensPedido = pedido.ItemPedidos.Where(x => x.Id == idItem).ToList();
            var fornecedor = itensPedido.FirstOrDefault().Fornecedor;         

            var usuarioFornecedor = fornecedor.Pessoa.Usuarios.FirstOrDefault(x => x.Ativo);

          
            var textoItemOuPedido = existePedParaAprovar ? "item" : "pedido ";
            var linkPessoaFisicajuridica = pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? "pedidos" : "pedidospessoafisica";


            Emails email = new Emails()
            {
                EmailDestinatario = fornecedor.Pessoa.PessoaJuridica.Email,

                CorpoEmail = _templateEmailRep.GetSingle(43).Template
                 .Replace("#Grid#", MontaGridItensPedido(itensPedido))
                 .Replace("#NomeFantasia#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia)
                 .Replace("#ItemOuPedido#", textoItemOuPedido)
                 .Replace("#TipoPessoa#", linkPessoaFisicajuridica),
                AssuntoEmail = existePedParaAprovar ? $"Economiza Ja - Mais itens do pedido {pedido.Id} para aprovar." : "Economiza Ja - Pedido pendente de aprovação." ,
                Status = Status.NaoEnviado,
                Origem = Origem.MembroTrocouFornecedorItemCancelado,
                DtCriacao = DateTime.Now,
                UsuarioCriacao = usuario,
                Ativo = true
            };

            _emailsRep.Add(email);

            var mensagemSms = existePedParaAprovar ? $"Economiza Já - Mais itens do pedido {pedido.Id} para você aprovar." : $"Economiza Já - Pedido {pedido.Id} pendente de aprovação." ;

            _smsService.EnviaSms($"{fornecedor.DddCel}{fornecedor.Celular}",
                                mensagemSms, 
                                TipoOrigemSms.PedidosPendentesAprovacaoFronecedor, 
                                (int)TipoOrigemSms.MembroTrocaFornecedorItemCancelado);


            foreach (var item in fornecedor.Pessoa.Usuarios)
            {
                var descricaoAviso = existePedParaAprovar ? $"Mais itens do pedido {pedido.Id} para aprovar" :  $"Pedido {pedido.Id} pendente de aprovação";

                var linkAviso = pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? "/#/pedidos" : "/#/pedidospessoafisica";

                this.InserirAvisos(item,
                    TipoAviso.PedidoPendentedeAceiteFornecedor,
                    "Aprovar pedido",
                    descricaoAviso,
                    "Aprovar pedido",
                    linkAviso,
                    4,
                    pedido.Id);
            }

            _unitOfWork.Commit();

        }

        public void EnviaEmailPedidoPromocao(int pedidoId, Usuario Usuario)
        {

            Pedido pedido = _pedidoRep.GetSingle(pedidoId);

            PedidoEmailViewModel pedidoEmailVm = new PedidoEmailViewModel()
            {
                Id = pedido.Id,
                NomeFantasia = pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? pedido.Membro.Pessoa.PessoaJuridica.NomeFantasia : pedido.Membro.Pessoa.PessoaFisica.Nome
            };

            Emails email = new Emails()
            {
                EmailDestinatario = Usuario.UsuarioEmail,
                CorpoEmail = _emailService.MontaEmail(pedidoEmailVm, _templateEmailRep.GetSingle(21).Template.Replace("#Grid#", "" + MontaGridItensPedidoPromocao(pedido) + "")),
                AssuntoEmail = "Confirmação - Número do Pedido " + pedidoId,
                Status = Status.NaoEnviado,
                Origem = Origem.PedidoMembroGerado,
                DtCriacao = DateTime.Now,
                UsuarioCriacao = Usuario

            };

            _emailsRep.Add(email);
            _unitOfWork.Commit();
        }

        public void EnviarEmailPrecificacaoAutomatica(int pedidoId, Usuario Usuario)
        {

            Pedido pedido = _pedidoRep.GetSingle(pedidoId);
            var usuarioMembro = _usuarioRep.FirstOrDefault(x => x.Id == pedido.UsuarioCriacaoId);

            PedidoEmailViewModel pedidoEmailVm = new PedidoEmailViewModel()
            {
                Id = pedido.Id,
                NomeFantasia = pedido.Membro.Pessoa.PessoaJuridica.NomeFantasia
            };


            Emails emailPrecificacao = new Emails()
            {
                EmailDestinatario = Usuario.UsuarioEmail,
                CorpoEmail = _emailService.MontaEmail(pedidoEmailVm, _templateEmailRep.GetSingle(34).Template.Replace("#CotacaoId#", "")),
                AssuntoEmail = "Precificação Automática " + pedidoId,
                Status = Status.NaoEnviado,
                Origem = Origem.PrecificacaoAutomaticaCotacaoFornecedor,
                DtCriacao = DateTime.Now,
                UsuarioCriacao = Usuario,
                Ativo = true
            };

            _emailsRep.Add(emailPrecificacao);
            _unitOfWork.Commit();
        }

        public string getParametroSistema(string Codigo)
        {

            ParametroSistema retorno = _parametroSistemaRep.GetAll().FirstOrDefault(x => x.Codigo == Codigo && x.Ativo == true);
            if (retorno != null)
            {
                return retorno.Valor;
            }
            else
            {
                return "";
            }

        }

        public void InserirAvisos(Usuario usuario, TipoAviso tipoAviso,
            string tituloAviso, string descricaoAviso, string tooltip, string urlDestino,
            int modulo, int idReferencia)
        {
            var inserirAvisos = new Avisos
            {
                UsuarioCriacao = usuario,
                DtCriacao = DateTime.Now,
                DataUltimoAviso = DateTime.Now,
                TipoAvisosId = (int)tipoAviso,
                ExibeNaTelaAvisos = true,
                TituloAviso = tituloAviso,
                DescricaoAviso = descricaoAviso,
                ToolTip = tooltip,
                URLPaginaDestino = urlDestino,
                ModuloId = modulo,
                UsuarioNotificadoId = usuario.Id,
                IdReferencia = idReferencia,
                Ativo = true
            };

            _avisosRep.Add(inserirAvisos);
            _unitOfWork.Commit();

        }


        public object CotacaoProdsGroup(int usuarioId, int cotacaoId)
        {
            var usuario = _usuarioRep.GetSingle(usuarioId);
            var fornecedor = _fornecedorRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

            var membrosForn = _membroFornecedor.GetAll()
                .Where(x => x.FornecedorId == fornecedor.Id && x.Ativo)
                .Select(o => o.Membro.Id)
                .ToList();

            var indisponibilidade = _indisponibilidadeProdutoRep.GetAll()
                                      .Where(i => i.FornecedorId == fornecedor.Id
                                            && i.InicioIndisponibilidade <= DateTime.Now
                                            && i.FimIndisponibilidade >= DateTime.Now);
            //.Select(p => p.ProdutoId).ToList();
            var resultCotacao = _resultadoCotacao.GetAll()
                .Where(x => x.CotacaoId == cotacaoId && x.FornecedorId == fornecedor.Id);

            var resultadoFornecedorProdutoValor = _fornecedorProdutoRep.GetAll().Where(fp => fp.Ativo && fp.FornecedorId == fornecedor.Id);

            var ultimaOferta = _resultadoCotacao.FindBy(x => x.CotacaoId == cotacaoId)
                .GroupBy(x => new { x.ProdutoId, x.CotacaoId, x.Qtd, x.PrecoNegociadoUnit })
                .Select(u => new { preco = u.Select(d => d.PrecoNegociadoUnit).Min(), qtdPrecoIgual = u.Count(), u.Key.ProdutoId, u.Key.Qtd });

            //pega todos os pedido da tabela remover fornecedor que estaja na cotação = ProdutoId
            var itemPedidos = _cotacaoPedidos.GetAll().Where(x => x.CotacaoId == cotacaoId)
                .SelectMany(i => i.Pedido.ItemPedidos.Select(p => p.Id)).ToList();

            var removeItemForn = _removeFornPedidoRep
                .FindBy(x => x.FonecedorId == fornecedor.Id && itemPedidos.Contains(x.ItemPedidoId))
                .Select(p => p.ItemPedidoId)
                .ToList();

            var cotacaoProdsGroup = _cotacaoPedidos.GetAll()
                .Where(x => x.CotacaoId == cotacaoId && membrosForn.Contains(x.Pedido.Membro.Id))
                .SelectMany(x => x.Pedido.ItemPedidos.Where(p => !removeItemForn.Contains(p.Id)))
                .GroupBy(x => new { x.Produto.DescProduto, x.ProdutoId, x.Produto.Marca.DescMarca, x.Produto.Especificacao })
                .Select(g => new
                {
                    indProdutoIndisponivel = !indisponibilidade.Any() ? false : indisponibilidade.Where(w => w.ProdutoId == g.Key.ProdutoId).Any(),
                    dtProdutoIndisponivel = !indisponibilidade.Any() ? DateTime.MinValue : indisponibilidade.FirstOrDefault(w => w.ProdutoId == g.Key.ProdutoId).FimIndisponibilidade,
                    indIndisponivelPermanente = !indisponibilidade.Any() ? false : !indisponibilidade.Where(w => w.ProdutoId == g.Key.ProdutoId && w.IndisponivelPermanente).Any() ? false : true,
                    indPrecoIgual = (!resultCotacao.Any() ? 0 : resultCotacao.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && x.FornecedorId == fornecedor.Id &&
                                                             x.Qtd == g.Sum(s => s.Quantidade)).PrecoNegociadoUnit) == (
                                                             !ultimaOferta.Any()
                                        ? 0
                                        : ultimaOferta.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && x.Qtd == g.Sum(s => s.Quantidade))
                                            .preco) ? "1" : "0",

                    indMaisPrecoIgual = (!ultimaOferta.Any()
                                        ? 0
                                        : ultimaOferta.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && x.Qtd == g.Sum(s => s.Quantidade))
                                            .qtdPrecoIgual) > 1 ? "1" : "0",

                    menorPreco = !ultimaOferta.Any()
                                        ? "0"
                                        : ultimaOferta.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && x.Qtd == g.Sum(s => s.Quantidade))
                                            .preco.ToString(),
                    g.Key.DescProduto,
                    g.Key.DescMarca,
                    g.Key.Especificacao,
                    qtd = g.Sum(s => s.Quantidade),
                    g.Key.ProdutoId,
                    resultCotacao.OrderByDescending(o => o.DtCriacao)
                        .FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && !string.IsNullOrEmpty(x.Observacao)).Observacao,
                    PrecoNegociadoUnit = !resultCotacao.Any() ?
                                            (!resultadoFornecedorProdutoValor.Any() ? "0" :
                                                resultadoFornecedorProdutoValor.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && x.FornecedorId == fornecedor.Id)
                                                    .ListaQuantidadeDesconto.Count(c => c.QuantidadeMinima <= g.Sum(s => s.Quantidade) && c.ValidadeQtdDesconto >= DateTime.Now) > 0 ?
                                                    //Calcula Valor com percentual de desconto  //"0" :
                                                    ((Math.Round(resultadoFornecedorProdutoValor.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId
                                                    && x.FornecedorId == fornecedor.Id).Valor, 2) -
                                                   Math.Round((resultadoFornecedorProdutoValor.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId
                                                        && x.FornecedorId == fornecedor.Id).ListaQuantidadeDesconto.Where(w             ////
                                                                => w.QuantidadeMinima <= g.Sum(s => s.Quantidade)).OrderByDescending(ob           //// Calcula fator de percentual 
                                                                => ob.QuantidadeMinima).FirstOrDefault().PercentualDesconto / 100)      //// 
                                                         * resultadoFornecedorProdutoValor.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId   //// Calcula percentual
                                                                && x.FornecedorId == fornecedor.Id).Valor, 2)) / 10000).ToString() :                         ////   do valor do produto
                                                                                                                                                             //exibe valor do produto, não contem desconto para range de quantidade
                                                resultadoFornecedorProdutoValor.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId
                                                    && x.FornecedorId == fornecedor.Id).Valor.ToString()
                                                    ) :
                                            //Pega preço já negociado
                                            resultCotacao.FirstOrDefault(x => x.ProdutoId == g.Key.ProdutoId && x.FornecedorId == fornecedor.Id &&
                                                         x.Qtd == g.Sum(s => s.Quantidade)).PrecoNegociadoUnit.ToString()
                }).ToList();

            //var itemsProdGroup = cotacaoProdsGroup.Where(c => !indisponibilidade.Contains(c.ProdutoId))
            //    .ToList();
            var itemsProdGroup = cotacaoProdsGroup.ToList();

            return itemsProdGroup;
        }




        #endregion

        #region Metodos auxiliares

        public string MontaGridItensPedido(List<ItemPedido> itemPedido)
        {

            string GridItemPedido = string.Empty;
            decimal total = 0;
            int totalItens = 0;

            //Cabelho do grid de itens do pedido
            GridItemPedido = "<table style='background-color: white; border-radius:10px; font-size:13px;'>";
            GridItemPedido = GridItemPedido + "<thead>";
            GridItemPedido = GridItemPedido + "<tr>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Item</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Qtd.</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Preço Médio Unit.</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Preço Unit. Economiza Já</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Subtotal Economiza Já</td>";
            GridItemPedido = GridItemPedido + "</tr>";
            GridItemPedido = GridItemPedido + "</thead>";
            GridItemPedido = GridItemPedido + "<tbody>";

            //itens do pedido
            foreach (var item in itemPedido)
            {
                GridItemPedido = GridItemPedido + "<tr>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + item.Produto.DescProduto + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + item.Quantidade + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + string.Format("{0:C}", item.PrecoMedioUnit) + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + string.Format("{0:C}", item.PrecoNegociadoUnit) + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + string.Format("{0:C}", (item.Quantidade * item.PrecoNegociadoUnit)) + "</td>";
                GridItemPedido = GridItemPedido + "</tr>";
                total += (item.Quantidade * item.PrecoNegociadoUnit).Value;
                totalItens = totalItens + item.Quantidade;
            }

            //Total
            var desconto = itemPedido.FirstOrDefault().Desconto;
            decimal valorDesconto = Convert.ToDecimal((total * desconto) / 100);
            var totalDesconto = total - valorDesconto;
            if (desconto > 0)
                total = totalDesconto;


            GridItemPedido = GridItemPedido + "</tbody>";
            GridItemPedido = GridItemPedido + "<tr>";
            GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'><strong>Total de Itens:</strong></td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold; border-top: 1px solid blue;'>" + totalItens + "</td>";
            GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'></td>";
            if (desconto > 0)
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'><strong>Total á Vista</strong></td>";
            else
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'><strong>Total</strong></td>";

            GridItemPedido = GridItemPedido + "<td style='font-weight:bold; border-top:1px solid blue;'>" + string.Format("{0:C}", total) + "</td>";
            GridItemPedido = GridItemPedido + "</tr>";
            GridItemPedido = GridItemPedido + "</table>";

            return GridItemPedido;
        }

        public string MontaGridItensPedidoPromocao(Pedido pedido)
        {

            string GridItemPedido = string.Empty;
            decimal total = 0;
            int totalItens = 0;

            //Cabelho do grid de itens do pedido
            GridItemPedido = "<table style='background-color: white; border-radius:10px; font-size:13px;'>";
            GridItemPedido = GridItemPedido + "<thead>";
            GridItemPedido = GridItemPedido + "<tr>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Item</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Qtd.</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Preço Real</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Preço Promocional</td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold;'>Subtotal Promoção</td>";
            GridItemPedido = GridItemPedido + "</tr>";
            GridItemPedido = GridItemPedido + "</thead>";
            GridItemPedido = GridItemPedido + "<tbody>";

            //itens do pedido
            foreach (var item in pedido.ItemPedidos)
            {
                GridItemPedido = GridItemPedido + "<tr>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + item.Produto.DescProduto + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + item.Quantidade + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + string.Format("{0:C}", item.PrecoMedioUnit) + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + string.Format("{0:C}", item.PrecoNegociadoUnit) + "</td>";
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'>" + string.Format("{0:C}", (item.Quantidade * item.PrecoNegociadoUnit)) + "</td>";
                GridItemPedido = GridItemPedido + "</tr>";
                total += (item.Quantidade * item.PrecoNegociadoUnit).Value;
                totalItens = totalItens + item.Quantidade;
            }
            //Total
            var desconto = pedido.ItemPedidos.FirstOrDefault().Desconto;
            decimal valorDesconto = Convert.ToDecimal((total * desconto) / 100);
            var totalDesconto = total - valorDesconto;
            if (desconto > 0)
                total = totalDesconto;

            GridItemPedido = GridItemPedido + "</tbody>";
            GridItemPedido = GridItemPedido + "<tr>";
            GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'><strong>Total de Itens:</strong></td>";
            GridItemPedido = GridItemPedido + "<td style='font-weight:bold; border-top: 1px solid blue;'>" + totalItens + "</td>";
            GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'></td>";
            if (desconto > 0)
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'><strong>Total á Vista</strong></td>";
            else
                GridItemPedido = GridItemPedido + "<td style='border-top: 1px solid blue'><strong>Total</strong></td>";

            GridItemPedido = GridItemPedido + "<td style='font-weight:bold; border-top:1px solid blue;'>" + string.Format("{0:C}", total) + "</td>";
            GridItemPedido = GridItemPedido + "</tr>";
            GridItemPedido = GridItemPedido + "</table>";

            return GridItemPedido;
        }

        public string RemoverAcentos(string texto)
        {

            string s = texto.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < s.Length; k++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }
            }
            return sb.ToString();
        }



        #endregion

    }
}
