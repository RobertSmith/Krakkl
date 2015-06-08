using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace Krakkl.Migrations
{
    public partial class NewProfileProperties : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.AddColumn(
                name: "AboutMe",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
            migration.AddColumn(
                name: "AuthorProfile",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
            migration.AddColumn(
                name: "EditorLanguage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropColumn(name: "AboutMe", table: "AspNetUsers");
            migration.DropColumn(name: "AuthorProfile", table: "AspNetUsers");
            migration.DropColumn(name: "EditorLanguage", table: "AspNetUsers");
        }
    }
}
