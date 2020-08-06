using System;
using System.Configuration;
using System.Net;
using ECC.Framework;

namespace ECC.SMS
{
    public class TWW
    {
        #region Attributes

        private readonly TwwService.ReluzCapWebService _client;
        private readonly string _user;
        private readonly string _password;

        #endregion

        #region Constructors

        public TWW()
        {
        
            //this._user = "saicro";
            this._user = ConfigurationManager.AppSettings["TWWUsuario"];
            this._password = ConfigurationManager.AppSettings["TWWSenha"]; ;
            this._client = new TwwService.ReluzCapWebService();

            
        }

        #endregion

        #region Methods

        public int Creditos()
        {


          

            var creditos = this._client.VerCredito(this._user, this._password);
            return creditos;
        }

        public DateTime ValidadeCreditos()
        {
            var validade = this._client.VerValidade(this._user, this._password);
            return validade;
        }

        public string Send(int id, string celular, string mensagem)
        {
            mensagem = mensagem.RemoveAccents();
            var response = this._client.EnviaSMS(this._user, this._password, id.ToString(), celular, mensagem);
            return response;
        }

        public string StatusSMS(int id)
        {
            var status = this._client.StatusSMS(this._user, this._password, id.ToString());
            return status.Tables[0].Columns["op"].ToString();
        }

        #endregion
    }
}
