namespace BELibrary.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class New_Article_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Article",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(),
                    Image = c.String(),
                    Description = c.String(),
                    Content = c.String(),
                    IsDelete = c.Boolean(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Article");
        }
    }
}