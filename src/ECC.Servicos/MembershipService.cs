using ECC.Servicos.Abstrato;
using System;
using System.Linq;
using System.Security.Principal;
using ECC.Dados.Extensions;
using ECC.Dados.Repositorio;
using ECC.Dados.Infra;
using ECC.Servicos.Util;
using System.Security.Claims;
using ECC.EntidadeUsuario;
using ECC.EntidadePessoa;
using ECC.EntidadeRecebimento;

namespace ECC.Servicos
{
    public class MembershipService : IMembershipService
    {


        #region Variaveis
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<RecuperaSenha> _recuperaSehnaRep;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntidadeBaseRep<Telefone> _telefoneRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IPagamentoService _pagamentoService;

        #endregion

        #region Construtor
        public MembershipService(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Perfil> perfilRep,
            IEntidadeBaseRep<RecuperaSenha> recuperaSenhaRep,
            IEncryptionService encryptionService,
            IEntidadeBaseRep<Telefone> telefoneRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Membro> membroRep,
            IPagamentoService pagamentoService,
            IUnitOfWork unitOfWork)
        {
            _recuperaSehnaRep = recuperaSenhaRep;
            _usuarioRep = usuarioRep;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            _telefoneRep = telefoneRep;
            _fornecedorRep = fornecedorRep;
            _membroRep = membroRep;
            _pagamentoService = pagamentoService;
        }
        #endregion

        #region IMembershipService Implementacao

        public MembershipContext ValidateUser(string email, string senha, int perfilModulo)
        {
            var membershipCtx = new MembershipContext();

            var usuario = _usuarioRep.GetSingleByEmail(email, perfilModulo);

            if (usuario != null && isUserValid(usuario, senha))
            {

                membershipCtx.Usuario = usuario;

                string[] perfil = { usuario.Perfil.DescPerfil };

                var identity = new GenericIdentity(usuario.UsuarioEmail);

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString(), ClaimValueTypes.String));

                membershipCtx.Principal = new GenericPrincipal(identity, perfil);
            }

            return membershipCtx;
        }
        public Usuario CreateUser(string nome,
            string email,
            string senha,
            int perfilId,
            int pessoaId,
            int usuarioCriacaoId,
            bool flgMaster,
             string DddTelComl = null,
              string TelefoneComl = null,
              string DddCel = null,
               string Celular = null,
               string Contato = null)
        {
            var existingUser = _usuarioRep.GetSingleByEmail(email, perfilId);

            if (existingUser != null)
            {
                throw new Exception("E-mail já cadastrado!");
            }

            var senhaChave = _encryptionService.CriaChave();


            var usuario = new Usuario()
            {
                UsuarioNome = nome,
                Chave = senhaChave,
                UsuarioEmail = email,
                Ativo = true,
                Senha = _encryptionService.EncryptSenha(senha, senhaChave),
                DtCriacao = DateTime.Now,
                UsuarioCriacaoId = usuarioCriacaoId,
                PessoaId = pessoaId,
                PerfilId = perfilId,
                FlgMaster = flgMaster

            };

            _usuarioRep.Add(usuario);
            _unitOfWork.Commit();

            //usuario.Senha = senha;

            var telefone = new Telefone
            {
                UsuarioCriacao = usuario,
                DtCriacao = DateTime.Now,
                DddTelComl = DddTelComl,
                TelefoneComl = TelefoneComl,
                DddCel = DddCel,
                Celular = Celular,
                Ativo = true,
                Contato = Contato,
                UsuarioTel = usuario,
                PessoaId = usuario.PessoaId

            };

            _telefoneRep.Add(telefone);
            _unitOfWork.Commit();


            return usuario;
        }

        public Usuario GetUser(int usuarioId)
        {
            return _usuarioRep.GetSingle(usuarioId);
        }

        public Usuario GetUserPorEmail(string UserEmail, int UserPerfilModulo)
        {
            //return _usuarioRep.GetAll().Where(x => x.UsuarioEmail == UserEmail).FirstOrDefault();
            Usuario retUsu = _usuarioRep.GetSingleByEmail(UserEmail, UserPerfilModulo);
            return retUsu;
        }

        public void CreateRecuperaSenha(RecuperaSenha objRecuperaSenha)
        {
            _recuperaSehnaRep.Add(objRecuperaSenha);
            _unitOfWork.Commit();
        }

        public RecuperaSenha getRecuperaSenha(RecuperaSenha objRecuperaSenha)
        {
            return _recuperaSehnaRep.GetAll().Where(x => x.Chave == objRecuperaSenha.Chave && x.DtExpira > DateTime.Now).FirstOrDefault();
        }

        public void AlterarSenhaUsuario(Usuario objUser)
        {
            Usuario user = _usuarioRep.GetSingle(objUser.Id);
            if (objUser.Chave == user.Chave)
            {
                user.Senha = _encryptionService.EncryptSenha(objUser.Senha, objUser.Chave);
                user.FlgTrocaSenha = true;
                _unitOfWork.Commit();
            }

        }

        #endregion

        #region Metodos auxiliaares


        private bool isPasswordValid(Usuario usuario, string senha)
        {
            return string.Equals(_encryptionService.EncryptSenha(senha, usuario.Chave), usuario.Senha);
        }

        private bool isUserValid(Usuario usuario, string senha)
        {
            if (isPasswordValid(usuario, senha))
            {
                return usuario.Ativo;
            }

            return false;
        }

        public bool PagamentosFaturasMensalidadeVencidos(int perfil, string email)
        {
            var usuario = GetUserPorEmail(email, perfil);
            bool flag = false;

            if (usuario != null)
            {
                switch (perfil)
                {
                    case 1:
                        flag = false;
                        break;

                    case 2:
                        flag = _pagamentoService.VerificaPlanoMembro(usuario.PessoaId);
                        break;

                    case 3:
                        flag = _pagamentoService.VerificaFaturasFornecedor(usuario.PessoaId);
                        break;

                    case 4:
                        flag = false;
                        break;
                }
            }

            return flag;
        }

        #endregion
    }
}
