using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace bytme.Data.Migrations
{
    public partial class test_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "streetnumber",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "surname",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zipcode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ItemCategories",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(nullable: true),
                    dt_created = table.Column<DateTime>(nullable: false),
                    dt_modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    gender = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    long_description = table.Column<string>(nullable: true),
                    price = table.Column<float>(nullable: false),
                    size = table.Column<string>(nullable: true),
                    photo_url = table.Column<string>(nullable: true),
                    category_id = table.Column<int>(nullable: false),
                    quantity = table.Column<int>(nullable: false),
                    issales = table.Column<int>(nullable: false),
                    dt_created = table.Column<DateTime>(nullable: false),
                    dt_modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrderHistories",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    itm_description = table.Column<string>(nullable: true),
                    qty_bought = table.Column<int>(nullable: false),
                    ord_id = table.Column<int>(nullable: false),
                    price_payed = table.Column<float>(nullable: false),
                    dt_created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrderMains",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(nullable: false),
                    dt_ordered = table.Column<DateTime>(nullable: false),
                    dt_delivery = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    dt_created = table.Column<DateTime>(nullable: false),
                    dt_modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMains", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(nullable: true),
                    dt_created = table.Column<DateTime>(nullable: false),
                    dt_modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartModels",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    item_id = table.Column<int>(nullable: false),
                    price = table.Column<float>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    long_description = table.Column<string>(nullable: true),
                    size = table.Column<string>(nullable: true),
                    quantity = table.Column<int>(nullable: false),
                    photo_url = table.Column<string>(nullable: true),
                    qty = table.Column<int>(nullable: false),
                    total = table.Column<float>(nullable: false),
                    subtotal = table.Column<float>(nullable: false),
                    stock = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartModels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WishlistModels",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    item_id = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    long_description = table.Column<string>(nullable: true),
                    price = table.Column<float>(nullable: false),
                    photo_url = table.Column<string>(nullable: true),
                    stock = table.Column<int>(nullable: false),
                    dt_created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistModels", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCategories");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "OrderHistories");

            migrationBuilder.DropTable(
                name: "OrderMains");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropTable(
                name: "ShoppingCartModels");

            migrationBuilder.DropTable(
                name: "WishlistModels");

            migrationBuilder.DropColumn(
                name: "city",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "street",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "streetnumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "surname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "zipcode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
