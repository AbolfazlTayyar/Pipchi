using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pipchi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContractSizeToPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractSize",
                table: "Positions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractSize",
                table: "Positions");
        }
    }
}
