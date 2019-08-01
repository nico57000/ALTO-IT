namespace Alto_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finalisation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exigences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdExigence = c.Int(nullable: false),
                        Name = c.String(),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        ForeignKey = c.Int(nullable: false),
                        FKToProjet = c.Int(nullable: false),
                        ForeignKey_TO_Norme = c.Int(nullable: false),
                        DocumentPath = c.String(),
                        DocumentName = c.String(),
                        Exigence_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exigences", t => t.Exigence_Id)
                .Index(t => t.Exigence_Id);
            
            CreateTable(
                "dbo.Mesures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Description = c.String(),
                        DocumentName = c.String(),
                        DocumentPath = c.String(),
                        FKToProjets = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        FKToMesure = c.Int(nullable: false),
                        Mesures_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mesures", t => t.Mesures_Id)
                .Index(t => t.Mesures_Id);
            
            CreateTable(
                "dbo.Normes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom_Norme = c.String(),
                        IDNorme = c.Int(nullable: false),
                        DocumentPath = c.String(),
                        FKToProjet = c.Int(nullable: false),
                        DocumentName = c.String(),
                        Norme_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Normes", t => t.Norme_Id)
                .Index(t => t.Norme_Id);
            
            CreateTable(
                "dbo.RelationsMesuresExigences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdExigence = c.Int(nullable: false),
                        IdMesures = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Normes", "Norme_Id", "dbo.Normes");
            DropForeignKey("dbo.Mesures", "Mesures_Id", "dbo.Mesures");
            DropForeignKey("dbo.Exigences", "Exigence_Id", "dbo.Exigences");
            DropIndex("dbo.Normes", new[] { "Norme_Id" });
            DropIndex("dbo.Mesures", new[] { "Mesures_Id" });
            DropIndex("dbo.Exigences", new[] { "Exigence_Id" });
            DropTable("dbo.RelationsMesuresExigences");
            DropTable("dbo.Normes");
            DropTable("dbo.Mesures");
            DropTable("dbo.Exigences");
        }
    }
}
