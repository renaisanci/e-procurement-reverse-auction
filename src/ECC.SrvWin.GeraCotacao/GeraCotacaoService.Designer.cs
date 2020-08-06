namespace ECC.SrvWin.GeraCotacao
{
    partial class GeraCotacaoService
    {

        #region Attributes

        private System.Diagnostics.EventLog eventLog1;
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #endregion



        #region Disposing

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion



        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLog1 = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();

            this.ServiceName = "Economiza Já - Gerar Cotações";
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
        }

        #endregion
    }
}
