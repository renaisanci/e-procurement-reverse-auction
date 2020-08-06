using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ECC.EntidadeEmail;
using ECC.Servicos.Util;


namespace ECC.Servicos
{
    public class EmailService : IEmailService
    {

        #region Variaveis
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        #endregion

        #region Construtor

        public EmailService() : this(new DbFactory()) { }

        public EmailService(DbFactory dbFactory)
            : this(new EntidadeBaseRep<Emails>(dbFactory),
                    new EntidadeBaseRep<TemplateEmail>(dbFactory),
                    new EntidadeBaseRep<Usuario>(dbFactory),
                    new EncryptionService(),
                    new UnitOfWork(dbFactory))
        {
        }

        public EmailService(IEntidadeBaseRep<Emails> emailsRep,
                            IEntidadeBaseRep<TemplateEmail> templateEmailRep,
                            IEntidadeBaseRep<Usuario> usuarioRep,
                            IEncryptionService encryptionService,
                            IUnitOfWork unitOfWork)
        {
            _emailsRep = emailsRep;
            _templateEmailRep = templateEmailRep;
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
        public void EnviaEmail(string to, string cc, string body, string subject)
        {
            try
            {
                //obtem os valores smtp do arquivo de configuração . Não vou usar estes valores estou apenas mostrando como obtê-los         
                SmtpSection mailSettings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                if (mailSettings != null)
                {

                    SmtpClient smtp = new SmtpClient();
                    MailMessage mail = new MailMessage();
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.To.Add(new MailAddress(to));

                    if (!String.IsNullOrEmpty(cc))
                        mail.CC.Add(new MailAddress(cc));

                    mail.From = new MailAddress(mailSettings.Network.UserName, "Economiza Já", Encoding.UTF8);
                    mail.Subject = subject;
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.Priority = MailPriority.Normal;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(mailSettings.Network.UserName,
                        mailSettings.Network.Password);
                    smtp.Host = mailSettings.Network.Host;
                    smtp.Port = mailSettings.Network.Port;
                    // smtp.EnableSsl = false;
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }
        }


        /// <summary>
        /// Enviar Email SMS
        /// </summary>
        /// <param name="to">Message to address</param>
        /// /// <param name="cc">Message to address</param>
        /// <param name="body">Text of message to send</param>
        /// <param name="subject">Subject line of message</param>        
        public void EnviaEmail(string to, string msg, string subjectNumeroCel)
        {
            try
            {
                //obtem os valores smtp do arquivo de configuração . Não vou usar estes valores estou apenas mostrando como obtê-los         
                SmtpSection mailSettings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                if (mailSettings != null)
                {
                    SmtpClient smtp = new SmtpClient();
                    MailMessage mail = new MailMessage();
                    mail.Body = msg;
                    mail.IsBodyHtml = false;
                    mail.To.Add(new MailAddress(to));
                    mail.From = new MailAddress(mailSettings.Network.UserName, "Economiza Já", Encoding.UTF8);
                    mail.Subject = subjectNumeroCel;
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.Priority = MailPriority.Normal;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(mailSettings.Network.UserName, mailSettings.Network.Password);
                    smtp.Host = mailSettings.Network.Host;
                    smtp.Port = mailSettings.Network.Port;
                    smtp.EnableSsl = false;
                    smtp.Send(mail);
                }

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
        public string MontaEmail<T>(T entidade, string templateHtml)
        {

            Type tipoEntidade = typeof(T);
            //Pegamos as propriedades da entidade.
            PropertyInfo[] propriedades = tipoEntidade.GetProperties();
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo propriedade in propriedades)
            {
                templateHtml = templateHtml.Replace("#" + propriedade.Name + "#",
                    "" + propriedade.GetValue(entidade, null) + "");
            }
            return templateHtml;

        }

        public void Salvar(Emails email)
        {
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);

            email.UsuarioAlteracao = usuario;
            email.DtAlteracao = DateTime.Now;

            _emailsRep.Edit(email);
            _unitOfWork.Commit();
        }

        public Emails BuscarPor(int id)
        {
            return _emailsRep.GetSingle(id);
        }

        public String BuscaTemplate(int id)
        {
            return _templateEmailRep.FirstOrDefault(x => x.Id == id).Template;
        }

        public List<Emails> BuscarPendentes(int limite)
        {
            return _emailsRep.FindBy(x => x.Status == Status.Pendente).OrderBy(x => x.DtCriacao)
                          .Take(limite)
                          .ToList();
        }

        public void LimpaEmailEnviado()
        {

            //comentado temporariamente para monitorar os envios de sms
            //var emailEnviado = _emailsRep.FindBy(x =>   x.Status == Status.Enviado).ToList();

            //_emailsRep.DeleteAll(emailEnviado);
            //_unitOfWork.Commit();

        }

        public List<Emails> BuscarNaoEnviados(int limite)
        {
            return _emailsRep.FindBy(x => ((x.Status == Status.NaoEnviado || x.Status == Status.TentarNovamente) ||
                                           (x.DataInicioProcesso.HasValue &&
                                            x.DataInicioProcesso.Value <= DbFunctions.AddMinutes(DateTime.Now, -5) &&
                                            x.Status == Status.NaoEnviado))).OrderBy(x => x.DtCriacao)

                .Take(limite)
                .ToList();

        }

        public void EnviarEmailViaRobo(Usuario pUsu, string pAssuntoEmail, string pEmailDestino, string pCorpoEmail, Origem pOrigrem)
        {

            Emails emails = new Emails
            {
                UsuarioCriacaoId = pUsu.Id,
                DtCriacao = DateTime.Now,
                AssuntoEmail = pAssuntoEmail,
                EmailDestinatario = pEmailDestino,
                CorpoEmail = pCorpoEmail.Trim(),
                Status = Status.NaoEnviado,
                Origem = pOrigrem

            };

            //Envia EMAIL
            _emailsRep.Add(emails);
            _unitOfWork.Commit();
        }


    }
}
