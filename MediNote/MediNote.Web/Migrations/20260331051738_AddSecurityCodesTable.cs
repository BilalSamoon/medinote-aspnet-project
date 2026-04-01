using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediNote.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityCodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsClaimed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityCodes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SecurityCodes",
                columns: new[] { "Id", "Code", "IsClaimed", "Role" },
                values: new object[,]
                {
                    { 1, "DOC123", true, "Doctor" },
                    { 2, "ADM123", true, "Admin" },
                    { 3, "DOC456", false, "Doctor" },
                    { 4, "DOC789", false, "Doctor" },
                    { 5, "ADM456", false, "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityCodes");
        }
    }
}
