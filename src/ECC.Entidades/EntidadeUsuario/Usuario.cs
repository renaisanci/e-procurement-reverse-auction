
using ECC.Entidades;
using System;
using System.Collections.Generic;
using ECC.EntidadeArquivo;
using ECC.EntidadeAvisos;
using ECC.EntidadePessoa;

namespace ECC.EntidadeUsuario
{
    public class Usuario : EntidadeBase
    {

        public Usuario()
        {
            this.Telefones = new List<Telefone>();
            this.Notificacoes = new List<UsuarioNotificacao>();
        }

        public int PerfilId { get; set; }
        public int PessoaId { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioEmail { get; set; }
        public string Senha { get; set; }
        public string Chave { get; set; }
        public bool FlgMaster { get; set; }
        public bool FlgTrocaSenha { get; set; }
        public string TokenSignalR { get; set; }
        public virtual Perfil Perfil { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public int? TermoUsoId { get; set; }
        public virtual TermoUso TermoUso { get; set; }
        public bool AceitoTermoUso { get; set; }
        public DateTime? DtLeituraTermoUso { get; set; }
        public DateTime? DtEnvioTermoUso { get; set; }
        public DateTime? DtUsuarioEntrou { get; set; }
        public DateTime? DtUsuarioSaiu { get; set; }
        public bool Logado { get; set; }
        public int OrigemLogin { get; set; }
        public virtual ICollection<UsuarioNotificacao> Notificacoes { get; set; }
        public virtual ICollection<Telefone> Telefones { get; set; }
    }
}
