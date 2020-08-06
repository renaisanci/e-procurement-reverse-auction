namespace ECC.Dados.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campopalavrachafrom100to1000 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Fornecedor", "PalavrasChaves", c => c.String(maxLength: 1000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Fornecedor", "PalavrasChaves", c => c.String(maxLength: 100, unicode: false));
        }
    }
}
