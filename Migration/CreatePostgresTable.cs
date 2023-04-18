using FluentMigrator;

namespace Migration;

[Migration(20180430121800)]
public class CreatePostgresTable : FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("Log")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Text").AsString();
    }

    public override void Down()
    {
        Delete.Table("Log");
    }
}