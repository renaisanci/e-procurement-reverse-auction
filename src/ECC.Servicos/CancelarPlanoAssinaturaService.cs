using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using System;
using System.Collections.Generic;
using System.Linq;
using Gerencianet.SDK;

namespace ECC.Servicos
{
    public class CancelarPlanoAssinaturaService : ICancelarPlanoAssinaturaService
    {

        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Fornecedor> _forneceRep;
        private readonly IEntidadeBaseRep<UsuarioCancelado> _usuarioCanceladoRep;

        private readonly IUnitOfWork _unitOfWork;

        #region Construtor
        public CancelarPlanoAssinaturaService() : this(new DbFactory()) { }

        public CancelarPlanoAssinaturaService(DbFactory dbFactory)

            : this(new EntidadeBaseRep<Usuario>(dbFactory),
                   new EntidadeBaseRep<Membro>(dbFactory),
                   new EntidadeBaseRep<Fornecedor>(dbFactory),                   
                   new EntidadeBaseRep<UsuarioCancelado>(dbFactory),
                   new UnitOfWork(dbFactory))
        { }

        public CancelarPlanoAssinaturaService(
                                IEntidadeBaseRep<Usuario> usuarioRep,
                                IEntidadeBaseRep<Membro> membroRep,
                                IEntidadeBaseRep<Fornecedor> fornecedorRep,
                                IEntidadeBaseRep<UsuarioCancelado> usuarioCanceladoRep,
                                IUnitOfWork unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _membroRep = membroRep;
            _forneceRep = fornecedorRep;
            _usuarioCanceladoRep = usuarioCanceladoRep;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos

        public void CancelarPlanoAssinatura()
        {

            var dataHoje = DateTime.Now.Date;
            var usuariosCancelados = _usuarioCanceladoRep.FindBy(x => x.DataCancelamento < dataHoje && x.Ativo).ToList();
            var pessoa = new List<int>();


            usuariosCancelados.ForEach(x =>
            {

                var usuario = _usuarioRep.FirstOrDefault(u => u.Id == x.UsuarioId);
                usuario.Ativo = false;
                _usuarioRep.Edit(usuario);

                var usuarioCancelado = _usuarioCanceladoRep.FirstOrDefault(c => c.UsuarioId == x.UsuarioId);
                usuarioCancelado.Ativo = false;
                _usuarioCanceladoRep.Edit(usuarioCancelado);


                _unitOfWork.Commit();

                if (!pessoa.Contains(usuario.PessoaId))
                    pessoa.Add(usuario.PessoaId);
            });

            if (pessoa.Count > 0)
            {
                pessoa.ForEach(p =>
                {
                    var membro = _membroRep.FirstOrDefault(m => m.PessoaId == p);
                    if (membro != null)
                    {
                        membro.Ativo = false;
                        _membroRep.Edit(membro);
                    }

                    var fornecedor = _forneceRep.FirstOrDefault(m => m.PessoaId == p);
                    if (fornecedor != null)
                    {
                        fornecedor.Ativo = false;
                        _forneceRep.Edit(fornecedor);
                    }

                    _unitOfWork.Commit();
                });
            }
        }

        public void CancelarTransacaoPagamento(int chargerId)
        {
            try
            {
                if (chargerId > 0)
                {
                    var gerenciaNet = new GerencianetAPI();

                    gerenciaNet.CancelTransaction(chargerId);
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao cancelar transação", ex);
            }
        }
               

        #endregion


    }
}
