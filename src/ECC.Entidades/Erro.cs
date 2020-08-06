namespace ECC.Entidades
{
    public class Erro : EntidadeBase
    {       
        public string Mensagem { get; set; }
        public string StackTrace { get; set; }
    }
}
