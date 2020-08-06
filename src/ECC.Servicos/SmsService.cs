using System;
using System.Text;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using System.Collections.Generic;
using System.Linq;
using ECC.Servicos.Util;
using ECC.EntidadeSms;

namespace ECC.Servicos
{
    public class SmsService : ISmsService
    {
        #region Variaveis

        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Construtor

        public SmsService() : this(new DbFactory()) { }

        public SmsService(DbFactory dbFactory) : this(new EntidadeBaseRep<Sms>(dbFactory), new EntidadeBaseRep<Usuario>(dbFactory), new EncryptionService(), new UnitOfWork(dbFactory))
        {
        }

        public SmsService(IEntidadeBaseRep<Sms> smsRep, 
            IEntidadeBaseRep<Usuario> usuarioRep, 
            IEncryptionService encryptionService, 
            IUnitOfWork unitOfWork)
        {
            _smsRep = smsRep;
            _usuarioRep = usuarioRep;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        /// <summary>
        /// Enviar Email
        /// </summary>
        /// <param name="to">Message to address</param>
        /// /// <param name="cc">Message to address</param>
        /// <param name="body">Text of message to send</param>
        /// <param name="subject">Subject line of message</param>        
        public void EnviaSms(string numero, string mensagem, TipoOrigemSms? origemSms = null, int? idEntidadeOrigemSms = null)
        {
            try
            {
                var usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
                var sms = new Sms
                {
                    Numero = numero,
                    Mensagem = mensagem,
                    OrigemSms = origemSms,
                    IdEntidadeOrigem = idEntidadeOrigemSms,
                    UsuarioCriacao = usuario,
                    DtCriacao = DateTime.Now,
                    Ativo = true,
                    Status = StatusSms.NaoEnviado
                };

                _smsRep.Add(sms);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Monta email, basta passar a entidade e o template
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidade"></param>
        /// <param name="templateHtml"></param>
        /// <returns>Retorna template com os valores preenchido em cada tag #Propriedade#</returns>
        public string MontaSms<T>(T entidade, string template)
        {
            var tipoEntidade = typeof(T);
            //Pegamos as propriedades da entidade.
            var propriedades = tipoEntidade.GetProperties();
            var sb = new StringBuilder();

            return propriedades.Aggregate(template, (current, propriedade) => current.Replace("#" + propriedade.Name + "#", "" + propriedade.GetValue(entidade, null) + ""));
        }

        public int Salvar(Sms sms)
        {
            var usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);

            sms.UsuarioAlteracao = usuario;
            sms.DtAlteracao = DateTime.Now;

            _smsRep.Edit(sms);
            _unitOfWork.Commit();
            return sms.Id;
        }

        public void LimpaSmsEnviado()
        {

            //comentamos por tempo para monitorar possiveis email enviados errados
            //var smsEnviado= _smsRep.FindBy(x =>  x.Status == StatusSms.Enviado).ToList();
 
            //_smsRep.DeleteAll(smsEnviado);
            //_unitOfWork.Commit();
            
        }

        public Sms BuscarPor(int id)
        {
            return _smsRep.GetSingle(id);
        }

        public List<Sms> BuscarPendentes()
        {
            return _smsRep.FindBy(x => x.Ativo && (x.Status == StatusSms.Pendente || x.Status == StatusSms.Erro)).OrderBy(x => x.DtCriacao).ToList();
        }

        public List<Sms> BuscarNaoEnviados()
        {
            return _smsRep.FindBy(x => x.Ativo && (x.Status == StatusSms.NaoEnviado || x.Status == StatusSms.TentarNovamente)).ToList();
        }
    }
}
