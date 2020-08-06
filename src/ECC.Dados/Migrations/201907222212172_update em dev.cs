namespace ECC.Dados.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class updateemdev : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemPedido", "FlgOutraMarca", c => c.Boolean(nullable: false,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "False")
                    },
                }));
            AlterColumn("dbo.Fornecedor", "Observacao", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("dbo.Fornecedor", "ObservacaoEntrega", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("dbo.Produto", "Especificacao", c => c.String(maxLength: 6500, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Produto", "Especificacao", c => c.String(maxLength: 2000, unicode: false));
            AlterColumn("dbo.Fornecedor", "ObservacaoEntrega", c => c.String(maxLength: 250, unicode: false));
            AlterColumn("dbo.Fornecedor", "Observacao", c => c.String(maxLength: 250, unicode: false));
            DropColumn("dbo.ItemPedido", "FlgOutraMarca",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DefaultValue", "False" },
                });
        }
    }
}
